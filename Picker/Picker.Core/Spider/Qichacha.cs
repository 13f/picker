using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Picker.Core.Helpers;
using Picker.Core.Storage;

namespace Picker.Core.Spider {
  public class Qichacha {
    IStorage store = null;
    WebClient client = null;

    string defaultCookie = null;
    string cookie = null;

    int countPerPage = 10;

    public const string Website = "http://www.qichacha.com";
    public const string Website0 = "http://qichacha.com";
    public const string Url_Search_Company = "http://qichacha.com/search?key={0}&index=2";

    /// <summary>
    /// 公司详情
    /// </summary>
    public const string Api_Company_Base = Website + "/company_base?unique={0}&companyname={1}";
    /// <summary>
    /// 分页查询：对外投资
    /// </summary>
    public const string Api_Paging_InvestList = Website + "/company_touzilist?unique={0}&companyname={1}&p={2}";
    /// <summary>
    /// 公司的无形资产
    /// </summary>
    public const string Api_Company_Assets = Website + "/company_assets?unique={0}&companyname={1}";
    /// <summary>
    /// 分页查询：资产：商标
    /// </summary>
    public const string Api_Paging_Trademark = Website + "/company_shangbiao?unique={0}&companyname={1}&p={2}";
    /// <summary>
    /// 分页查询：资产：专利
    /// </summary>
    public const string Api_Paging_Patent = Website + "/company_zhuanli?unique={0}&companyname={1}&p={2}";
    /// <summary>
    /// 分页查询：资产：著作权
    /// </summary>
    public const string Api_Paging_Copyright = Website + "/company_zzq?unique={0}&companyname={1}&p={2}";
    /// <summary>
    /// 分页查询：资产：软件著作权
    /// </summary>
    public const string Api_Paging_SoftwareCopyright = Website + "/company_rjzzq?unique={0}&companyname={1}&p={2}";
    /// <summary>
    /// 分页查询：资产：证书
    /// </summary>
    public const string Api_Paging_Certificate = Website + "/company_zhengshu?unique={0}&companyname={1}&p={2}";

    /// <summary>
    /// 查询细节：资产：商标
    /// </summary>
    public const string Api_Detail_Trademark = Website + "/company_shangbiaoView";
    /// <summary>
    /// 查询细节：资产：专利
    /// </summary>
    public const string Api_Detail_Patent = Website + "/company_zhuanliView";
    /// <summary>
    /// 查询细节：资产：证书
    /// </summary>
    public const string Api_Detail_Certificate = Website + "/company_zhengshuView";


    const string Regex_Trademark1 = "sbview\\(\"\\w+\"\\)";
    const string Regex_Trademark2 = "(?<=sbview\\(\")\\w+(?=\"\\))";

    const string PlaceHolder_DataHasError = "----";

    string regNum = null,
        orgCode = null;

    public Qichacha( IStorage _store ) {
      store = _store;
      client = NetHelper.GetWebClient_UTF8();

      byte[] buff = client.DownloadData( "http://www.qichacha.com/" );
      defaultCookie = client.ResponseHeaders.Get( "Set-Cookie" );
    }

    public void SetAuthCookie(string cookieStringOfAuth ) {
      cookie = defaultCookie + cookieStringOfAuth;
      if ( client.Headers["Cookie"] == null )
        client.Headers.Add( "Cookie", cookie );
      else
        client.Headers["Cookie"] = cookie;
    }

    public async Task<string> HttpGet( string url, NameValueCollection query ) {
      client.QueryString = query;
      string result = await client.DownloadStringTaskAsync( new Uri( url ) );
      return result;
    }

    public void SaveAlbum( string uri, string title, DateTime? modified, bool updateIfExists, bool saveChanges ) {
      store.Qichacha_SaveAlbum( uri, title, modified, updateIfExists, saveChanges );
    }

    public async Task PickAlbumList( TimeSpan? interval, string albumPage = "http://qichacha.com/album" ) {
      // http://qichacha.com/album?&p=5
      int albumCountPerPage = 15;
      int page = 1;
      HtmlDocument doc = new HtmlDocument();
      while ( true ) {
        string content = await client.DownloadStringTaskAsync( new Uri( albumPage + "?&p=" + page.ToString() ) );
        doc.LoadHtml( content );
        var divlist = doc.DocumentNode.SelectNodes( "//div[@class='caption']" );
        if ( divlist == null || divlist.Count == 0 )
          break;
        int count = 0;
        foreach ( var div in divlist ) {
          count++;
          var p1 = div.SelectNodes( "./p[@class='text-ellipsis m-b-none font-15']" ).FirstOrDefault();
          if ( p1 == null )
            continue;
          var aOfP1 = p1.SelectNodes( "./a" ).FirstOrDefault();
          if ( aOfP1 == null )
            continue;
          // url
          string url = aOfP1.Attributes["href"].Value;
          if ( url.StartsWith( "/" ) )
            url = Website0 + url;
          string title = aOfP1.InnerText.Trim();
          // modified
          var p2 = div.SelectSingleNode( "./p[@class='text-muted m-t']" );
          DateTime? modified = null;
          if(p2 != null ) {
            string dtString = p2.InnerText.Trim();
            int start = dtString.IndexOf( " " );
            dtString = dtString.Substring( 0, start );
            modified = TimeHelper.GetDateTime( dtString );
          }
          // save album
          store.Qichacha_SaveAlbum( url, title, modified, true, false );
        }
        // save changes
        await store.Qichacha_SaveChanges();

        // over?
        if ( count < albumCountPerPage )
          break;
        // wait and continue
        if ( interval != null )
          await Task.Delay( interval.Value );
        page++;
      }
    }

    public async Task PickCompanyListByAlbum( TimeSpan? interval, string albumUrl ) {
      if ( string.IsNullOrWhiteSpace( albumUrl ) )
        return;
     
      SaveAlbum( albumUrl, null, null, false, true ); // save album

      int countPerPageByAlbum = 20;
      HtmlDocument doc = new HtmlDocument();
      int page = 1;
      while ( true ) {
        string content = await client.DownloadStringTaskAsync( new Uri( albumUrl + "?p=" + page.ToString() ) );
        // HtmlAgilityPack
        doc.LoadHtml( content );
        var ulList = doc.DocumentNode.SelectNodes( "//ul[@class='list-group list-group-lg no-bg auto']" );
        if ( ulList == null )
          break;
        var hnUl = ulList.FirstOrDefault();
        if ( hnUl == null )
          break;
        int count = 0;
        var alist = hnUl.SelectNodes( "./a" );
        foreach(var aItem in alist ) {
          count++;
          string url = aItem.Attributes["href"].Value;
          string id = getIdFromCompanyLink( url );
          var span = aItem.SelectNodes( "./span[@class='clear']" ).FirstOrDefault();
          if ( span == null )
            continue;
          var spanChild = span.SelectNodes( "./span" ).FirstOrDefault();
          if ( spanChild == null )
            continue;
          string name = spanChild.InnerText.Trim();
          // save/update
          await store.Qichacha_SaveCompanyKey( id, name, false, false );
        }
        // save changes
        await store.Qichacha_SaveChanges();

        // over?
        if ( count < countPerPageByAlbum )
          break;
        // wait and continue
        if ( interval != null )
          await Task.Delay( interval.Value );
        page++;
      }

      // update album tag
      store.Qichacha_UpdateTag_Album( albumUrl, true );
    }

    public async Task PickCompanyListBySearch( TimeSpan? interval, string keyword, int defaultPage = 1 ) {
      string urlSearch = string.Format( Url_Search_Company, keyword );
      int page = defaultPage > 0 ? defaultPage : 1;
      HtmlDocument doc = new HtmlDocument();
      while ( true ) {
        string content = await client.DownloadStringTaskAsync( new Uri( urlSearch + "&p=" + page.ToString() ) );
        doc.LoadHtml( content );
        var list = doc.DocumentNode.SelectNodes( "//section[@id='searchlist']" );
        if ( list == null || list.Count == 0 )
          break;
        int count = 0;
        foreach(var section in list ) {
          count++;
          var a = section.SelectSingleNode( "./a" );
          if ( a == null )
            continue;
          // id
          string url = a.Attributes["href"].Value;
          string id = getIdFromCompanyLink( url );
          // name
          var span1 = a.SelectSingleNode( "./span[@class='clear']" );
          if ( span1 == null )
            continue;
          var span2 = span1.SelectSingleNode( "./span[@class='name']" );
          if ( span2 == null )
            continue;
          string name = span2.InnerText.Trim();
          // save/update
          await store.Qichacha_SaveCompanyKey( id, name, false, false );
        }

        // save search page
        store.Qichacha_UpdateCompanySearch( keyword, page, false );
        // save changes
        await store.Qichacha_SaveChanges();

        // over?
        if ( count < countPerPage )
          break;
        // wait and continue
        if ( interval != null )
          await Task.Delay( interval.Value );
        page++;
      }
      // save search page
      store.Qichacha_UpdateCompanySearch( keyword, -1, true );
    }

    JObject createConfig_Company() {
      JObject jo = new JObject();
      jo.Add( "basicInfo", "基本信息" );
      jo.Add( "shareholders", "股东信息" );
      jo.Add( "members", "主要人员" );
      jo.Add( "branches", "分支机构" );
      jo.Add( "changesHistory", "变更记录" );
      jo.Add( "summary", "公司简介" );

      jo.Add( "universalCreditCode", "统一社会信用代码" );
      jo.Add( "registrationId", "注册号" );
      jo.Add( "organizationCode", "组织机构代码" );
      jo.Add( "status", "经营状态" );
      jo.Add( "type", "公司类型" );
      jo.Add( "establishedAt", "成立日期" );
      jo.Add( "legalRepresentative", "法定代表" );
      jo.Add( "registeredCapital", "注册资本" );
      jo.Add( "interval", "营业期限" );
      jo.Add( "registrationAuthority", "登记机关" );
      jo.Add( "distributedAt", "发照日期" );
      jo.Add( "size", "公司规模" );
      jo.Add( "trade", "所属行业" );
      jo.Add( "address", "企业地址" );
      jo.Add( "scope", "经营范围" );
      return jo;
    }

    /// <summary>
    /// 基本信息
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    JObject getBasicInfo_Company( HtmlNodeCollection items ) {
      JObject jo = new JObject();
      foreach ( var li in items ) {
        var label = li.SelectSingleNode( "./label" ).InnerText;
        string lic = li.InnerText;
        string v = lic.Substring( label.Length );
        if ( label.StartsWith( "统一社会信用代码" ) )
          jo.Add( "universalCreditCode", v.Trim() );
        else if ( label.StartsWith( "注册号" ) ) {
          regNum = v.Trim();
          jo.Add( "registrationId", regNum );
        }
        else if ( label.StartsWith( "组织机构代码" ) ) {
          orgCode = v.Trim();
          jo.Add( "organizationCode", orgCode );
        }
        else if ( label.StartsWith( "经营状态" ) )
          jo.Add( "status", v.Trim() );
        else if ( label.StartsWith( "公司类型" ) )
          jo.Add( "type", v.Trim() );
        else if ( label.StartsWith( "成立日期" ) )
          jo.Add( "establishedAt", v.Trim() );
        else if ( label.StartsWith( "法定代表" ) )
          jo.Add( "legalRepresentative", v.Trim() );
        else if ( label.StartsWith( "注册资本" ) )
          jo.Add( "registeredCapital", v.Trim() );
        else if ( label.StartsWith( "营业期限" ) )
          jo.Add( "interval", v.Trim() );
        else if ( label.StartsWith( "登记机关" ) )
          jo.Add( "registrationAuthority", v.Trim() );
        else if ( label.StartsWith( "发照日期" ) )
          jo.Add( "distributedAt", v.Trim() );
        else if ( label.StartsWith( "公司规模" ) )
          jo.Add( "size", v.Trim() );
        else if ( label.StartsWith( "所属行业" ) )
          jo.Add( "trade", v.Trim() );
        else if ( label.StartsWith( "企业地址" ) )
          jo.Add( "address", v.Trim() );
        else if ( label.StartsWith( "经营范围" ) )
          jo.Add( "scope", v.Trim() );
      }
      return jo;
    }

    /// <summary>
    /// 股东信息
    /// </summary>
    /// <param name="divItems"></param>
    /// <returns></returns>
    JArray getShareholders_Company( HtmlNodeCollection divItems ) {
      JArray items = new JArray();
      foreach(var div in divItems ) {
        var tmp = div.SelectSingleNode( "./section[@class='panel panel-default']" )
          .SelectSingleNode( "./div[@class='panel-body']" )
          .SelectSingleNode( "./div[@class='clear']" );
        string name = tmp.SelectSingleNode( "./a" ).InnerText;
        string type = tmp.SelectSingleNode( "./small[@class='text-muted clear text-ellipsis m-t-xs m-b-xs']" ).InnerText;
        JObject jo = new JObject();
        jo.Add( "name", name.Trim() );
        jo.Add( "type", type.Trim() );
        var investment = tmp.SelectSingleNode( "./small[@class='block text-muted text-md']" );
        if( investment != null ) {
          var txt = investment.InnerText;
          txt = txt.Replace( "认缴出资额：", "" )
            .Trim();
          jo.Add( "investment", txt );
        }
        items.Add( jo );
      }
      return items;
    }

    /// <summary>
    /// 主要人员
    /// </summary>
    /// <param name="divItems"></param>
    /// <returns></returns>
    JArray getMembers_Company( HtmlNodeCollection divItems ) {
      JArray items = new JArray();
      foreach ( var div in divItems ) {
        var tmp = div.SelectSingleNode( "./section[@class='panel panel-default']" )
          .SelectSingleNode( "./div[@class='panel-body']" )
          .SelectSingleNode( "./div[@class='clear']" );
        string name = tmp.SelectSingleNode( "./a" ).InnerText;
        string job = tmp.SelectSingleNode( "./small" ).InnerText;
        JObject jo = new JObject();
        jo.Add( "name", name.Trim() );
        jo.Add( "job", job.Trim() );
        items.Add( jo );
      }
      return items;
    }

    /// <summary>
    /// 分支机构
    /// </summary>
    /// <param name="divItems"></param>
    /// <returns></returns>
    JArray getBranches_Company( HtmlNodeCollection divItems ) {
      JArray items = new JArray();
      foreach ( var div in divItems ) {
        var tmp = div.SelectSingleNode( "./section[@class='panel panel-default']" )
          .SelectSingleNode( "./div[@class='panel-body']" )
          .SelectSingleNode( "./div[@class='clear']" );
        string name = tmp.InnerText;

        JObject jo = new JObject();
        jo.Add( "name", name.Trim() );
        items.Add( jo );
      }
      return items;
    }

    /// <summary>
    /// 变更记录
    /// </summary>
    JArray getChangeLogs_Company( HtmlNodeCollection divItems ) {
      JArray items = new JArray();
      foreach ( var div in divItems ) {
        var tmpList = div.SelectSingleNode( "./section[@class='panel panel-default']" )
          .SelectSingleNode( "./div[@class='panel-body']" )
          .SelectSingleNode( "./div[@class='clear']" )
          .SelectNodes( "./div[@class='col-md-6']" );
        JObject jo = new JObject();
        foreach(var tmp in tmpList ) {
          var small = tmp.SelectSingleNode( "./small" );
          string all = small.InnerText;
          var span = small.SelectSingleNode( "./span" ).InnerText;
          if ( all.StartsWith( "变更项目" ) )
            jo.Add( "eventType", span.Trim() );
          else if ( all.StartsWith( "变更日期" ) )
            jo.Add( "date", span.Trim() );
          else if ( all.StartsWith( "变更前" ) )
            jo.Add( "beforeEvent", span.Trim() );
          else if ( all.StartsWith( "变更后" ) )
            jo.Add( "afterEvent", span.Trim() );
        }
        items.Add( jo );
      }
      return items;
    }

    /// <summary>
    /// 公司简介
    /// </summary>
    /// <param name="div"></param>
    /// <returns></returns>
    JObject getSummary_Company(HtmlNode div) {
      JObject jo = new JObject();
      string title = "简称或主要产品：";

      string all = div.InnerText;
      string description = all.Replace( title, "" );

      var p = div.SelectSingleNode( "./p" );
      if(p != null ) {
        var span = p.SelectSingleNode( "./span" );
        if(span != null ) {
          string spanValue = span.InnerText;
          jo.Add( "shortNameAndProduct", spanValue.Trim() );

          description = all.Replace( spanValue, "" );
        }
      }
      jo.Add( "description", description.Trim() );
      return jo;
    }

    public async Task PickCompanyBase( string id, string name, bool saveChanges ) {
      string uriQuery = string.Format( Api_Company_Base, id, name );
      string content = await client.DownloadStringTaskAsync( new Uri( uriQuery ) );

      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( content );

      JObject joRoot = new JObject();
      //// config
      //var joConfig = createConfig_Company();
      //joRoot.Add( "config", joConfig );
      // 基本信息
      //var sectionBasic = doc.DocumentNode.SelectSingleNode( "//section[@class='panel b-a base_info']" );
      var ulBase = doc.DocumentNode
        .SelectSingleNode( "//ul[@class='company-base']" );
      if(ulBase == null ) {
        store.Qichacha_SaveCompany_BasicInfo( id, name, PlaceHolder_DataHasError, PlaceHolder_DataHasError, PlaceHolder_DataHasError, saveChanges );
        return;
      }
      var liItems = ulBase.SelectNodes( "./li" );
      if ( liItems == null || liItems.Count == 0 ) {
        store.Qichacha_SaveCompany_BasicInfo( id, name, PlaceHolder_DataHasError, PlaceHolder_DataHasError, PlaceHolder_DataHasError, saveChanges );
        return;
      }
      JObject joBasic = getBasicInfo_Company( liItems );
      joRoot.Add( "basicInfo", joBasic );
      // 股东信息, 主要人员, 分支机构, 变更记录
      var sections = doc.DocumentNode.SelectNodes( "//section[@class='panel b-a clear']" );
      if( sections != null ) {
        foreach ( var section in sections ) {
          var divPanelHeading = section.SelectSingleNode( "./div[@class='panel-heading b-b']" );
          if ( divPanelHeading == null )
            divPanelHeading = section.SelectSingleNode( "./div[@class='panel-heading b-b m-b']" );
          var spanTitle = divPanelHeading.SelectSingleNode( "./span[@class='font-bold font-15 text-dark']" );
          var title = spanTitle.InnerText.Trim();
          if ( title == "股东信息" ) {
            var divlist = section.SelectNodes( "./div[@class='col-md-6']" );
            var array = getShareholders_Company( divlist );
            joRoot.Add( "shareholders", array );
          }
          else if ( title == "主要人员" ) {
            var divlist = section.SelectNodes( "./div[@class='col-md-3']" );
            var array = getMembers_Company( divlist );
            joRoot.Add( "members", array );
          }
          else if ( title == "分支机构" ) {
            var divlist = section.SelectNodes( "./div[@class='col-md-6']" );
            var array = getBranches_Company( divlist );
            joRoot.Add( "branches", array );
          }
          else if ( title == "变更记录" ) {
            var divlist = section.SelectNodes( "./div[@class='col-md-12']" );
            var array = getChangeLogs_Company( divlist );
            joRoot.Add( "changesHistory", array );
          }
        }
      }

      // 公司简介
      var divSummary = doc.DocumentNode.SelectSingleNode( "//div[@class='panel-body base-black m-b']" );
      if( divSummary != null ) {
        JObject joSummary = getSummary_Company( divSummary );
        joRoot.Add( "summary", joSummary );
      }

      // save
      string json = joRoot.ToString( Newtonsoft.Json.Formatting.Indented );
      store.Qichacha_SaveCompany_BasicInfo( id, name, regNum, orgCode, json, saveChanges );
    }

    public void PickList_Invest( string id, string name ) {
      
    }

    public async Task PickAssets( string id, string name ) {
      string uriQuery = string.Format( Api_Company_Assets, id, name );
      string content = await client.DownloadStringTaskAsync( new Uri( uriQuery ) );

      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( content );
    }

    public async Task PickList_Trademark( string id, string name ) {
      int page = 1;
      List<string> idlist = new List<string>();
      while ( true ) {
        string uriQuery = string.Format( Api_Paging_Trademark, id, name, page );
        string content = await client.DownloadStringTaskAsync( new Uri( uriQuery ) );
        // sbview("PUNNRTSMUTQL")
        var matchlist = Regex.Matches( content, Regex_Trademark2 );
        if ( matchlist == null || matchlist.Count == 0 )
          break;
        foreach ( Match m in matchlist ) {
          if ( !m.Success )
            continue;
          if ( idlist.Contains( m.Value ) )
            continue;
          idlist.Add( m.Value );
        }
        // save
        store.Qichacha_SaveTrademarkKey( idlist );
        // clear
        idlist.Clear();
        // continue?
        if ( matchlist.Count < countPerPage )
          break;
        page++;
      }
      // update tag
      store.Qichacha_UpdateTag_CompanyTrademark( id, true );
    }

    public void PickList_Patent( string id, string name ) {

    }

    public void PickList_Copyright( string id, string name ) {

    }

    public void PickList_SoftwareCopyright( string id, string name ) {

    }

    public void PickList_Certificate( string id, string name ) {

    }

    public void PickList_Websites( string id, string name ) {
    }

    // post
    public async Task PickDetail_Trademark( string id ) {
      string postString = "id=" + id;
      
      try {
        string json = await client.HttpPostTaskAsync( Api_Detail_Trademark, postString );
        // process
        var obj = JObject.Parse( json );
        var result = obj["Result"];
        int status = obj["Status"] == null ? -1 : (int)obj["Status"];
        if ( result == null || status != 200 ) {
          store.Qichacha_SaveDetail_Trademark( id, PlaceHolder_DataHasError, PlaceHolder_DataHasError, PlaceHolder_DataHasError, PlaceHolder_DataHasError, true );
          return;
        }
        string name = PlaceHolder_DataHasError,
          regNo = PlaceHolder_DataHasError,
          applicant = PlaceHolder_DataHasError;
        if ( result["Name"] != null )
          name = (string)result["Name"];
        if ( result["RegNo"] != null )
          regNo = (string)result["RegNo"];
        if ( result["Person"] != null )
          applicant = (string)result["Person"];
        // NOTE: 保存的时候不是读取源字符串，而是obj.ToString，前者中的汉字是Unicode，后者会将Unicode转为中文
        store.Qichacha_SaveDetail_Trademark( id, name, regNo, applicant, obj.ToString( Newtonsoft.Json.Formatting.Indented ), true );
      }
      catch ( Exception ex ) {
        throw ex;
      }
    }

    // post
    public async Task PickDetail_Patent( string id ) {
      string postString = "id=" + id;
      string json = await client.HttpPostTaskAsync( Api_Detail_Patent, postString );
    }

    // post
    public async Task PickDetail_Certificate( string id ) {
      string postString = "id=" + id;
      string json = await client.HttpPostTaskAsync( Api_Detail_Certificate, postString );
    }

    string getIdFromCompanyLink( string url ) {
      // /firm_CN_96a8e3822dee02c2a40cf3aae7d2af70.shtml
      int start = url.LastIndexOf( "_" );
      if ( start < 0 )
        return null;
      int end = url.IndexOf( ".shtml" );
      string id = url.Substring( start + 1, end - start - 1 );
      return id;
    }

  }

}
