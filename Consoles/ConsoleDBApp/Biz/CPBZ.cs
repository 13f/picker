using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Picker.Core.Extensions;
using Picker.Core.Helpers;

namespace ConsoleDBApp {
  // TODO: 抓取逻辑有待整理，网站上的部分数据有待理解
  /// <summary>
  /// http://www.cpbz.gov.cn/
  /// </summary>
  public static class CPBZ {
    /// <summary>
    /// 0: orgCode, 1: standardId
    /// </summary>
    const string UriTemplate_OrgStandardDetail = "http://www.cpbz.gov.cn/standardProduct/showDetail/{0}/{1}.do";
    /// <summary>
    /// POST. 0: area code
    /// </summary>
    const string Uri_QueryByPage = "http://www.cpbz.gov.cn/area/{0}/queryListPaged.do";


    const int CountPerPage15 = 15;

    static ConsoleDBApp.Biz.CpbzBiz biz = null;

    public static void Run( string conn ) {
      biz = new Biz.CpbzBiz( conn );
    }

    public static async Task PickAreas() {
      string uri = "http://www.cpbz.gov.cn/index";
      HtmlDocument doc = new HtmlDocument();
      WebClient client = NetHelper.GetWebClient_UTF8();

      Console.WriteLine( "获取省份...." );
      string html = await client.DownloadStringTaskAsync( uri );
      doc.LoadHtml( html );
      var alist = doc.DocumentNode.SelectNodes( "//a[@class='nav_a']" );
      List<string> codes = new List<string>();
      foreach(var a in alist ) {
        var href = a.Attributes["href"];
        if ( href == null )
          continue;
        string data = href.Value
          .Replace( "javascript:selectDetailByArea(", "")
          .Replace(")", "")
          .Replace("'", "");
        var parts = data.Split( ',' );
        //if ( parts == null || parts.Length != 2 )
        //  continue;
        biz.SaveKey_Area( parts[0], parts[1], false );
        codes.Add( parts[0] );
      }
      biz.SaveChanges();

      foreach(string code in codes ) {
        Console.WriteLine( "get code = " + code + " ...." );
        await PickSubAreas( client, code, true );
      }
      Console.WriteLine( "over..." );
    }

    public static async Task PickListByArea( int millisecondsDelay = 3000 ) {
      while ( true ) {
        var kvp = biz.GetLastArea_CPBZ();
        if ( kvp.Key == null ) {
          Console.WriteLine( "all area data is processed..." );
          break;
        }
        Console.WriteLine( "processing: code = " + kvp.Key + " ..." );
        await pickListByArea1( kvp.Key, kvp.Value );

        Console.WriteLine( "wait some seconds, and continue..." );
        await Task.Delay( millisecondsDelay );
      }

      Console.WriteLine( "over..." );
    }

    public static async Task PickRecentList( DateTime lastTime, int millisecondsDelay = 3000 ) {
      var areas = biz.SelectCodesOfTopAreas_CPBZ( lastTime ); // 31 items
      foreach(string area in areas ) {
        Console.WriteLine( "process area: code = " + area + "..." );
        // reset page
        biz.UpdateQueryState_Area_CPBZ( area, 0, true );
        // pick
        await pickListByArea2( area, 0 );
        // wait and continue
        Console.WriteLine( "  wait some seconds, and continue..." );
        await Task.Delay( millisecondsDelay );
      }
      Console.WriteLine( "over..." );
    }

    public static async Task PickDetails( string directory, int millisecondsDelay = 2000 ) {
      WebClient client = NetHelper.GetWebClient_UTF8();
      while ( true ) {
        var kvp = biz.GetStandardToProcess();
        if ( kvp.Key == null ) {
          Console.WriteLine( "all standards data is processed..." );
          break;
        }

        try {
          Console.WriteLine( "==> org_code = " + kvp.Key + ", standard_id = " + kvp.Value + " ..." );
          await pickDetail( client, kvp.Key, kvp.Value, directory );
        }
        catch ( System.Net.WebException ) {
          Console.WriteLine( "  got System.Net.WebException o(╯□╰)o " );
          //Console.WriteLine( "  wait 10 seconds, and continue..." );
          //await Task.Delay( 10 * 1000 );
          //continue;
          break;
        }

        Console.WriteLine( "wait some seconds, and continue..." );
        await Task.Delay( millisecondsDelay );
      }

      Console.WriteLine( "over..." );
    }

    public static async Task UpdateStandard_OrginalPdfUri( string directory, int millisecondsDelay = 2000 ) {
      WebClient client = NetHelper.GetWebClient_UTF8();
      while ( true ) {
        var kvp = biz.GetStandardToUpdateOriginalPdfUri();
        if ( kvp.Item1 == null ) {
          Console.WriteLine( "all standards data is processed..." );
          break;
        }

        try {
          Console.WriteLine( "==> org_code = " + kvp.Item1 + ", standard_id = " + kvp.Item2 + " ..." );
          await updateOriginalPdfUri( client, kvp.Item1, kvp.Item2, kvp.Item3 );
        }
        catch ( System.Net.WebException ) {
          Console.WriteLine( "  got System.Net.WebException o(╯□╰)o " );
          //Console.WriteLine( "  wait 10 seconds, and continue..." );
          //await Task.Delay( 10 * 1000 );
          //continue;
          break;
        }

        Console.WriteLine( "wait some seconds, and continue..." );
        await Task.Delay( millisecondsDelay );
      }

      Console.WriteLine( "over..." );
    }


    static async Task PickSubAreas( WebClient client, string code, bool hasSubAreas ) {
      if ( client == null )
        client = NetHelper.GetWebClient_UTF8();
      string uri = "http://www.cpbz.gov.cn/queryArea.do";

      string postString = "code=" + code;
      string jsonString = await client.HttpPostTaskAsync( uri, postString );
      JArray array = JArray.Parse( jsonString );
      List<string> codes = new List<string>();
      foreach ( var item in array ) {
        biz.SaveKey_Area( (string)item["areaCode"], (string)item["areaName"], false );
        codes.Add( (string)item["areaCode"] );
      }
      biz.SaveChanges();

      if ( hasSubAreas ) {
        foreach ( string c in codes ) {
          Console.WriteLine( "get code = " + c + " ...." );
          await PickSubAreas( client, c, false );
        }
      }
    }

    static async Task pickListByArea1( string area_code, int last_page, int millisecondsDelay = 3000 ) {
      int page = last_page;
      Console.WriteLine( "start from page " + page + "..." );

      WebClient client = NetHelper.GetWebClient_UTF8();
      List<string> orgsCodeCache = new List<string>();
      // orgCode, standardCode, standardId
      List<Tuple<string, string, int>> cache = new List<Tuple<string, string, int>>();
      while ( true ) {
        page++;
        Console.WriteLine( "  get page: " + page + "..." );

        // conType = "5";//按地区查询
        // condition = $("#areaValue").val();
        // standardFlag = 0;
        string postString = "conType=5" + "&condition=" + area_code + "&pageNum=" + page + "&standardFlag=0";
        string jsonString = await client.HttpPostTaskAsync( Uri_QueryByPage, postString );
        JObject json = JObject.Parse( jsonString );
        JArray array = (JArray)json["result"];

        int count = 0;
        orgsCodeCache.Clear();
        cache.Clear();
        foreach (var item in array ) {
          int standardId = (int)item["standardId"];
          string standardCode = (string)item["standardCode"];
          string standardName = (string)item["standardName"];

          string orgCode = (string)item["orgCode"];
          string enterpriseName = (string)item["enterpriseName"];

          count++;
          // <option value="" >所有 </option>
          // < option value = "0" > 现行 </ option >
          // < option value = "1" > 废止 </ option >
          if ( (int)item["standardStatus"] == 1 )
            continue;

          var exists = cache.Where( i => i.Item1 == orgCode && i.Item2 == standardCode && i.Item3 == standardId ).FirstOrDefault();
          if(exists != null && exists.Item3 > standardId ) {
            continue;
          }
          biz.SaveKey_Standard( orgCode, standardId, standardCode, standardName, false );
          // cache: remove old, add new
          if ( exists != null )
            cache.Remove( exists );
          cache.Add( Tuple.Create<string, string, int>( orgCode, standardCode, standardId ) );

          if ( !orgsCodeCache.Contains( orgCode ) ) {
            biz.SaveKey_Company( orgCode, enterpriseName, false );
            orgsCodeCache.Add( orgCode );
          }
        }
        // update page
        biz.UpdateQueryState_Area_CPBZ( area_code, page, false );
        // save changes
        biz.SaveChanges();
        
        // continue?
        if ( count < CountPerPage15 )
          break;
        await Task.Delay( millisecondsDelay );
      }
      // done, reset page
      biz.UpdateQueryState_Area_CPBZ( area_code, -1, true );
      Console.WriteLine( "done... ends with page " + page + "..." );
    }

    static async Task pickListByArea2( string area_code, int last_page, int millisecondsDelay = 3000 ) {
      int page = last_page;
      Console.WriteLine( "  start from page " + page + "..." );

      WebClient client = NetHelper.GetWebClient_UTF8();
      Dictionary<string, string> orgsCodeCache = new Dictionary<string, string>();
      // orgCode, standardCode, standardId
      List<Standard> cacheOriginal = new List<Standard>();
      while ( true ) {
        page++;
        Console.WriteLine( "  get page: " + page + "..." );
        orgsCodeCache.Clear();
        cacheOriginal.Clear();

        // conType = "5";//按地区查询
        // condition = $("#areaValue").val();
        // standardFlag = 0;
        string postString = "conType=5" + "&condition=" + area_code + "&pageNum=" + page + "&standardFlag=0&areaLevel=1";
        string jsonString = await client.HttpPostTaskAsync( Uri_QueryByPage, postString );
        JObject json = JObject.Parse( jsonString );
        JArray array = (JArray)json["result"];
        
        foreach ( var item in array ) {
          int standardId = (int)item["standardId"];
          string standardCode = (string)item["standardCode"];
          string standardName = (string)item["standardName"];

          string orgCode = (string)item["orgCode"];
          string enterpriseName = (string)item["enterpriseName"];

          // <option value="" >所有 </option>
          // < option value = "0" > 现行 </ option >
          // < option value = "1" > 废止 </ option >
          if ( (int)item["standardStatus"] == 1 )
            continue;

          var exists = cacheOriginal.Where( i => i.OrgCode == orgCode && i.StandardCode == standardCode && i.StandardId == standardId ).FirstOrDefault();
          if ( exists != null && exists.StandardId > standardId ) {
            continue;
          }
          
          // cache: remove old, add new
          if ( exists != null )
            cacheOriginal.Remove( exists );
          cacheOriginal.Add( new Standard( orgCode, standardId, standardCode, standardName ) );

          if ( !orgsCodeCache.ContainsKey( orgCode ) ) {
            orgsCodeCache.Add( orgCode, enterpriseName );
          }
        }
        // filter
        var tmpIds = cacheOriginal.Select( i => i.StandardId )
          .ToList();
        var ids = biz.GetStandardsNotSaved( tmpIds );
        // save standards keys
        var toSave = cacheOriginal.Where( i => ids.Contains( i.StandardId ) )
          .ToList();
        foreach (var i in toSave ) {
          biz.SaveKey_Standard( i.OrgCode, i.StandardId, i.StandardCode, i.StandardName, false );
        }
        // save orgs
        foreach(var org in orgsCodeCache ) {
          biz.SaveKey_Company( org.Key, org.Value, false );
        }

        // update page
        biz.UpdateQueryState_Area_CPBZ( area_code, page, false );
        // save changes
        biz.SaveChanges();

        // continue?
        if ( tmpIds.Count > ids.Count )
          break;
        await Task.Delay( millisecondsDelay );
      }
      // done, reset page
      biz.UpdateQueryState_Area_CPBZ( area_code, -1, true );
      Console.WriteLine( "  done... ends with page " + page + "..." );
    }

    static async Task pickDetail( WebClient client, string orgCode, int standardId, string directory ) {
      string uri = string.Format( UriTemplate_OrgStandardDetail, orgCode, standardId );
      string html = await client.DownloadStringTaskAsync( uri );
      Console.WriteLine( "  got html." );
      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );
      doc.RemoveComments();

      var root = doc.DocumentNode;
      string votum = getVotum( root );
      JObject company = null;
      JObject standard = null;
      JArray technicalIndicator = null;
      JArray products = null;

      var tables = root.SelectNodes( "//table" );
      foreach ( var table in tables ) {
        var tbody = table.SelectSingleNode( "./tbody" );
        var rows = tbody == null ? table.SelectNodes( "./tr" ) : tbody.SelectNodes( "./tr" );
        var firstRow = rows.FirstOrDefault();
        // get table head
        string tableHead = null;
        var th = firstRow.SelectSingleNode( "./th" );
        if ( th != null )
          tableHead = th.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        else {
          var td = firstRow.SelectSingleNode( "./td[@class='tab-color8']" );
          var strong = td.SelectSingleNode( "./strong" );
          tableHead = strong.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }

        if ( tableHead == "企业基本信息" ) {
          company = parseCompany( table );
        }
        else if ( tableHead == "标准信息" ) {
          standard = await parseStandard( orgCode, table, client, directory );
        }
        else if ( tableHead == "技术指标" ) {
          technicalIndicator = parseTechnical( table );
        }
        else if ( tableHead == "执行该标准的产品信息" ) {
          products = parseProducts( table );
        }
      } // foreach(var table in tables )

      standard["technicalIndicator"] = technicalIndicator;
      standard["products"] = products;

      Console.WriteLine( "  save company..." );
      string jsonCompany = company.ToString( Formatting.Indented );
      biz.SaveCompany( orgCode, (string)company["name"], jsonCompany, false );

      Console.WriteLine( "  save standard..." );
      string jsonStandard = standard.ToString( Formatting.Indented );
      biz.SaveStandard( orgCode, standardId, votum, (DateTime)standard["opened"], jsonStandard, false );

      biz.SaveChanges();
      Console.WriteLine( "  ... √" );
    }

    static async Task updateOriginalPdfUri( WebClient client, string orgCode, int standardId, string json ) {
      string uri = string.Format( UriTemplate_OrgStandardDetail, orgCode, standardId );
      string html = await client.DownloadStringTaskAsync( uri );
      Console.WriteLine( "  got html." );
      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );
      doc.RemoveComments();

      var root = doc.DocumentNode;
      JObject standard = JObject.Parse( json );

      var tables = root.SelectNodes( "//table" );
      foreach ( var table in tables ) {
        var tbody = table.SelectSingleNode( "./tbody" );
        var rows = tbody == null ? table.SelectNodes( "./tr" ) : tbody.SelectNodes( "./tr" );
        var firstRow = rows.FirstOrDefault();
        // get table head
        string tableHead = null;
        var th = firstRow.SelectSingleNode( "./th" );
        if ( th != null )
          tableHead = th.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        else {
          var td = firstRow.SelectSingleNode( "./td[@class='tab-color8']" );
          var strong = td.SelectSingleNode( "./strong" );
          tableHead = strong.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }

        if ( tableHead == "标准信息" ) {
          parseStandard_OriginalPdfUri( standard, table );
        }
      } // foreach(var table in tables )

      Console.WriteLine( "  save standard..." );
      string jsonStandard = standard.ToString( Formatting.Indented );
      biz.UpdateStandard_Content( orgCode, standardId, jsonStandard, false );

      biz.SaveChanges();
      Console.WriteLine( "  ... √" );
    }

    static string getVotum( HtmlNode root ) {
      var divVotum = root.SelectSingleNode( "//div[@class='cl-w-top2']" );
      if ( divVotum == null )
        return null;
      var p = divVotum.SelectSingleNode( "./p" );
      return p.InnerText
        .Trim( '\r', '\n', '\t' )
        .Trim();
    }

    static JObject parseCompany( HtmlNode table ) {
      JObject jo = new JObject();
      var tbody = table.SelectSingleNode( "./tbody" );
      var rows = tbody.SelectNodes( "./tr" ).Skip( 1 ); // skip head row
      int index = 0;
      foreach(var row in rows ) {
        index++;
        var columns = row.SelectNodes( "./td" );
        if (index == 1 ) { // 名称，法定代表人
          var td2 = columns.Skip( 1 ).FirstOrDefault();
          string name = td2.SelectSingleNode( "./span" )
            .InnerText
            .Trim();
          jo["name"] = string.IsNullOrWhiteSpace( name ) ? null : name;

          var td4 = columns.Skip( 3 ).FirstOrDefault();
          string legalRepresentative = td4.SelectSingleNode( "./span" )
            .InnerText
            .Trim();
          jo["legalRepresentative"] = string.IsNullOrWhiteSpace( legalRepresentative ) ? null : legalRepresentative;
        }
        else if ( index == 2 ) { // 组织机构代码，邮编
          var td2 = columns.Skip( 1 ).FirstOrDefault();
          string organizationCode = td2.SelectSingleNode( "./span" )
            .InnerText
            .Trim();
          jo["organizationCode"] = string.IsNullOrWhiteSpace( organizationCode ) ? null : organizationCode;

          var td4 = columns.Skip( 3 ).FirstOrDefault();
          string postcode = td4.SelectSingleNode( "./span" )
            .InnerText
            .Trim();
          jo["postcode"] = string.IsNullOrWhiteSpace( postcode ) ? null : postcode;
        }
        else if ( index == 3 ) { // 注册地址，行政区划
          var td2 = columns.Skip( 1 ).FirstOrDefault();
          string registeredAddress = td2.SelectSingleNode( "./span" )
            .InnerText
            .Trim();
          jo["registeredAddress"] = string.IsNullOrWhiteSpace( registeredAddress ) ? null : registeredAddress;

          var td4 = columns.Skip( 3 ).FirstOrDefault();
          string administrativeDistrict = td4.SelectSingleNode( "./span" )
            .InnerText
            .Trim();
          jo["administrativeDistrict"] = string.IsNullOrWhiteSpace( administrativeDistrict ) ? null : administrativeDistrict;
        }
      } // foreach(var row in rows )
      return jo;
    }

    static async Task<JObject> parseStandard( string orgCode, HtmlNode table, WebClient client, string directory ) {
      JObject jo = new JObject();
      var tbody = table.SelectSingleNode( "./tbody" );
      var rows = tbody.SelectNodes( "./tr" ).Skip( 1 ); // skip head row
      int index = 0;
      string name = null;
      string standard_code = null;
      foreach ( var row in rows ) {
        index++;
        var columns = row.SelectNodes( "./td" );
        if ( index == 1 ) { // 标准名称，标准编号
          var td2 = columns.Skip( 1 ).FirstOrDefault();
          name = td2.SelectSingleNode( "./span" )
            .InnerText
            .Replace( "\n", " " )
            .Trim();
          jo["name"] = name;

          var td4 = columns.Skip( 3 ).FirstOrDefault();
          standard_code = td4.SelectSingleNode( "./span" )
            .InnerText
            .Trim();
          jo["standard_code"] = standard_code;
        }
        else if ( index == 2 ) { // 公开时间
          var td2 = columns.Skip( 1 ).FirstOrDefault();
          string opened = td2.SelectSingleNode( "./span" )
            .InnerText
            .Trim();
          if ( string.IsNullOrWhiteSpace( opened ) )
            jo["opened"] = null;
          else
            jo["opened"] = DateTime.Parse( opened ).ToUniversalTime();
        }
        else if ( index == 3 ) { // PDF
          var td2 = columns.Skip( 1 ).FirstOrDefault();
          if(td2 != null ) {
            var div = td2.SelectSingleNode( "./div" );
            if(div != null ) {
              var a = div.SelectSingleNode( "./a" );
              if(a != null ) {
                string href = a.Attributes["href"].Value;
                string pdfUri = "http://www.cpbz.gov.cn" + href;
                jo["original_pdf_uri"] = pdfUri;
                try {
                  string filename = standard_code + " " + name + "(" + orgCode + ")" + ".pdf";
                  filename = filename.Replace( "/", "%2F" )
                    .Replace( "\\", "%5C" )
                    .Replace( ":", "：" )
                    .Replace( "*", "" )
                    .Replace( "\u001f", "" );
                  await client.DownloadFileTaskAsync( pdfUri, directory + filename );
                  jo["file_name"] = filename;
                  Console.WriteLine( "  PDF downloaded." );
                }
                catch {
                  Console.WriteLine( "  failed to download PDF -_-" );
                }
                
              }
            }
            
          }
        } // index == 3
      } // foreach(var row in rows )
      return jo;
    }

    static void parseStandard_OriginalPdfUri( JObject jo, HtmlNode table ) {
      var tbody = table.SelectSingleNode( "./tbody" );
      var rows = tbody.SelectNodes( "./tr" ).Skip( 1 ); // skip head row
      int index = 0;
      foreach ( var row in rows ) {
        index++;
        if ( index != 3 )
          continue;
        var columns = row.SelectNodes( "./td" );
        // get pdf
        var td2 = columns.Skip( 1 ).FirstOrDefault();
        if ( td2 != null ) {
          var div = td2.SelectSingleNode( "./div" );
          if ( div != null ) {
            var a = div.SelectSingleNode( "./a" );
            if ( a != null ) {
              string href = a.Attributes["href"].Value;
              string pdfUri = "http://www.cpbz.gov.cn" + href;
              jo["original_pdf_uri"] = pdfUri;
            }
          }
        }
        break;
      } // foreach(var row in rows )
    }

    static JArray parseTechnical( HtmlNode table ) {
      JArray array = new JArray();
      var rows = table.SelectNodes( "./tr" ).Skip( 2 ); // skip head rows
      foreach ( var row in rows ) {
        var columns = row.SelectNodes( "./td" );
        JObject item = new JObject();
        int index = 0;
        foreach(var td in columns ) {
          index++;
          if(index == 1 ) { // 指标名称
            string name = td.InnerText
              .Replace( "&nbsp;", " " )
              .Trim( '\r', '\n', '\t' )
              .Trim();
            item["name"] = string.IsNullOrWhiteSpace( name ) ? null : name;
          }
          else if ( index == 2 ) { // 指标要求
            string requirement = td.InnerText
              .Replace( "&nbsp;", " " )
              .Trim( '\r', '\n', '\t' )
              .Trim();
            item["requirement"] = string.IsNullOrWhiteSpace( requirement ) ? null : requirement;
          }
          else if ( index == 3 ) { // 测试方法
            string testingMethod = td.InnerText
              .Replace( "&nbsp;", " " )
              .Trim( '\r', '\n', '\t' )
              .Trim();
            item["testingMethod"] = string.IsNullOrWhiteSpace( testingMethod ) ? null : testingMethod;
          }
          else if ( index == 4 ) { // 指标水平说明
            string indicatorLevelRemark = td.InnerText
              .Replace( "&nbsp;", " " )
              .Trim( '\r', '\n', '\t' )
              .Trim();
            item["indicatorLevelRemark"] = string.IsNullOrWhiteSpace( indicatorLevelRemark ) ? null : indicatorLevelRemark;
          }
        } // foreach(var td in columns )
        array.Add( item );
      } // foreach(var row in rows )
      return array;
    }

    static JArray parseProducts( HtmlNode table ) {
      JArray array = new JArray();
      var rows = table.SelectNodes( "./tr" ).Skip( 2 ); // skip head rows
      foreach ( var row in rows ) {
        var columns = row.SelectNodes( "./td" );
        JObject item = new JObject();
        int index = 0;
        foreach ( var td in columns ) {
          index++;
          if ( index == 1 ) { // 产品名称
            string name = td.InnerText
              .Replace( "&nbsp;", " " )
              .Trim( '\r', '\n', '\t' )
              .Trim();
            item["name"] = string.IsNullOrWhiteSpace( name ) ? null : name;
          }
          else if ( index == 2 ) { // 通用名
            string universalName = td.InnerText
              .Replace( "&nbsp;", " " )
              .Trim( '\r', '\n', '\t' )
              .Trim();
            item["universalName"] = string.IsNullOrWhiteSpace( universalName ) ? null : universalName;
          }
          else if ( index == 3 ) { // 品牌
            string brand = td.InnerText
              .Replace( "&nbsp;", " " )
              .Trim( '\r', '\n', '\t' )
              .Trim();
            item["brand"] = string.IsNullOrWhiteSpace( brand ) ? null : brand;
          }
          else if ( index == 4 ) { // 条码
            string barcode = td.InnerText
              .Replace( "&nbsp;", " " )
              .Trim( '\r', '\n', '\t' )
              .Trim();
            item["barcode"] = string.IsNullOrWhiteSpace( barcode ) ? null : barcode;
          }
          else if ( index == 5 ) { // 规格/型号
            // 40KG&nbsp;<b>/</b>&nbsp;820
            string content = td.InnerText
              .Replace( "&nbsp;", " " )
              .Replace( "<b>", "" )
              .Replace( "</b>", "" )
              .Trim( '\r', '\n', '\t' )
              .Trim();
            var r = content.Split( '/' );
            string specification = null;
            string modelNumber = null;
            if (r!= null && r.Length == 2 ) {
              specification = r[0]
                .Replace( "——", "" )
                .Trim();
              modelNumber = r[1]
                .Replace( "——", "" )
                .Trim();
            }
            item["specification"] = string.IsNullOrWhiteSpace( specification ) ? null : specification;
            item["modelNumber"] = string.IsNullOrWhiteSpace( modelNumber ) ? null : modelNumber;
          }
          else if ( index == 6 ) { // 分类
            string category = td.InnerText
              .Replace( "&nbsp;", " " )
              .Trim( '\r', '\n', '\t' )
              .Trim();
            item["category"] = string.IsNullOrWhiteSpace( category ) ? null : category;
          }
        } // foreach(var td in columns )
        array.Add( item );
      } // foreach(var row in rows )
      return array;
    }

  }


  public class Standard {
    public string OrgCode { get; set; }
    public int StandardId { get; set; }
    public string StandardCode { get; set; }
    public string StandardName { get; set; }

    public Standard( string orgCode, int standardId, string standardCode, string standardName ) {
      OrgCode = orgCode;
      StandardId = standardId;
      StandardCode = standardCode;
      StandardName = standardName;
    }

  }

}
