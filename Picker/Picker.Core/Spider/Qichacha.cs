using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
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
    public const string Api_Company_Base = Website + "/company_base?unique={0}&companyname={1}&p={2}";
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
    public const string Api_Detail_Certificate = Website + "/company_zhuanliView";

    public Qichacha( IStorage _store ) {
      store = _store;
      client = NetHelper.GetWebClient_UTF8();

      byte[] buff = client.DownloadData( "http://qichacha.com/" );
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


    public void PickCompanyBase( string id, string name ) {

    }

    public void PickList_Invest( string id, string name ) {

    }

    public void PickAssets( string id, string name ) {

    }

    public void PickList_Trademark( string id, string name ) {

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
    public void PickDetail_Trademark( string id ) {
      string postString = "id=" + id;
      string j = client.HttpPost( Api_Detail_Trademark, postString );
    }

    // post
    public void PickDetail_Patent( string id ) {
      string postString = "id=" + id;
      string j = client.HttpPost( Api_Detail_Patent, postString );
    }

    // post
    public void PickDetail_Certificate( string id ) {
      string postString = "id=" + id;
      string j = client.HttpPost( Api_Detail_Certificate, postString );
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
