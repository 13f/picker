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

namespace ConsoleDBApp {

  /// <summary>
  /// http://www.sac.gov.cn
  /// </summary>
  public static class SAC {
    const int CountPerPage20 = 20;

    /// <summary>
    /// 0: page(1,)
    /// </summary>
    const string UriTemplate_ActiveStandards = "http://www.sac.gov.cn/SACSearch/search?page={0}&channelid=97779&searchword=%25+and+EXECUTE_STATUS%21%3D%271%27&keyword=%25+and+EXECUTE_STATUS%21%3D%271%27&perpage=20&outlinepage=10&templet=gjcxjg.jsp&keyword=%%20and%20EXECUTE_STATUS!=%271%27&templet=gjcxjg.jsp";
    /// <summary>
    /// 0: page(1,)
    /// </summary>
    const string UriTemplate_RevocativeStandards = "http://www.sac.gov.cn/SACSearch/search?page={0}&channelid=97779&searchword=%25+and+EXECUTE_STATUS%3D%271%27&keyword=%25+and+EXECUTE_STATUS%3D%271%27&perpage=20&outlinepage=10&templet=gjcxjg.jsp&keyword=%%20and%20EXECUTE_STATUS=%271%27&templet=gjcxjg.jsp";
    /// <summary>
    /// 0: GB/T%205121.10-1996；1：Q(GB)，Z(GB/Z)，T(GB/T)
    /// </summary>
    const string UriTemplate_Standard = "http://www.sac.gov.cn/SACSearch/search?channelid=97779&templet=gjcxjg_detail.jsp&searchword=STANDARD_CODE=%27{0}%27&XZ={1}";

    static ConsoleDBApp.Biz.SacBiz biz = null;

    static JObject createConfig() {
      JObject joConfig = new JObject();
      joConfig.Add( "id", "ID" );
      joConfig.Add( "standard_code", "标准号" );
      joConfig.Add( "chinese_title", "标准名称（中文）" );
      joConfig.Add( "english_title", "Standard Title in English" );
      joConfig.Add( "category", "标准类别" );
      joConfig.Add( "ics", "国际标准分类号" );
      joConfig.Add( "ccs", "中国标准分类号" );
      joConfig.Add( "application_degree", "采用程度" );
      joConfig.Add( "adopted_international_standard", "采用国际标准" );
      joConfig.Add( "adopted_international_standard_number", "采用国际标准号" );
      joConfig.Add( "adopted_international_standard_name", "采标名称" );
      joConfig.Add( "governor", "主管部门" );
      joConfig.Add( "technical_committees", "归口单位" );
      joConfig.Add( "drafting_committee", "起草单位" );
      joConfig.Add( "number_of_pages", "标准页码" );
      joConfig.Add( "price", "标准价格(元)" );
      joConfig.Add( "state", "状态：现行/废止" );
      joConfig.Add( "plan_number", "计划编号" );
      joConfig.Add( "issuance_date", "发布日期" );
      joConfig.Add( "execute_date", "实施日期" );
      joConfig.Add( "first_issuance_date", "首次发布日期" );
      joConfig.Add( "review_affirmance_date", "复审确认日期" );
      joConfig.Add( "replaces", "代替国标号" );
      joConfig.Add( "replaced_by", "被代替国标号" );
      joConfig.Add( "revocatory_date", "废止时间" );
      joConfig.Add( "remark", "备注" );
      return joConfig;
    }

    public static void Run( string conn ) {
      biz = new ConsoleDBApp.Biz.SacBiz( conn );
    }

    /// <summary>
    /// 获取现行标准的列表
    /// </summary>
    /// <returns></returns>
    public static async Task PickList_Active( int millisecondsDelay = 2000 ) {
      Console.WriteLine( "ready..." );

      HtmlDocument doc = new HtmlDocument();
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      int page = biz.GetPageLastProcessed( UriTemplate_ActiveStandards );
      while ( true ) {
        page++;
        Console.WriteLine( "get page: " + page + "..." );
        string uri = string.Format( UriTemplate_ActiveStandards, page );
        string html = await client.DownloadStringTaskAsync( uri );
        doc.LoadHtml( html );
        // process
        int count = processList( doc.DocumentNode, false, false );
        // query state
        biz.UpdateQueryState( UriTemplate_ActiveStandards, page, false );
        // save
        biz.SaveChanges();
        // continue?
        if ( count < CountPerPage20 )
          break;
        // delay
        await Task.Delay( millisecondsDelay );
      }
      // reset state
      biz.UpdateQueryState( UriTemplate_ActiveStandards, -1, true );
      Console.WriteLine( "over..." );
    }

    /// <summary>
    /// 获取已经废止的标准列表
    /// </summary>
    /// <returns></returns>
    public static async Task PickList_Revocatory( int millisecondsDelay = 2000 ) {
      Console.WriteLine( "ready..." );

      HtmlDocument doc = new HtmlDocument();
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      int page = biz.GetPageLastProcessed( UriTemplate_RevocativeStandards );
      while ( true ) {
        page++;
        Console.WriteLine( "get page: " + page + "..." );
        string uri = string.Format( UriTemplate_RevocativeStandards, page );
        string html = await client.DownloadStringTaskAsync( uri );
        doc.LoadHtml( html );
        // process
        int count = await processList2( client, doc.DocumentNode, true, false );
        Console.WriteLine( "  save page tag, submit all changes..." );
        // query state
        biz.UpdateQueryState( UriTemplate_RevocativeStandards, page, false );
        // save
        biz.SaveChanges();
        // continue?
        if ( count < CountPerPage20 )
          break;
        Console.WriteLine( "  wait some seconds and continue..." );
        // delay
        await Task.Delay( millisecondsDelay );
      }
      // reset state
      biz.UpdateQueryState( UriTemplate_RevocativeStandards, -1, true );
      Console.WriteLine( "over..." );
    }

    public static async Task PickDetail( int millisecondsDelay = 3000 ) {
      Console.WriteLine( "ready..." );

      HtmlDocument doc = new HtmlDocument();
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      while ( true ) {
        string code = biz.GetStandardTask();
        if ( string.IsNullOrWhiteSpace( code ) )
          break;
        try {
          await pickDetail( client, code );
          // wait and continue
          await Task.Delay( millisecondsDelay );
        }
        catch ( System.Net.WebException ) {
          Console.WriteLine( "got WebException. wait 30 seconds to retry..." );
          await Task.Delay( 30 * 1000 );
          Console.WriteLine( "retry..." );
          continue;
        }

      }

      Console.WriteLine( "over..." );
    }

    static async Task<string> getDetailHtml( WebClient client, string code ) {
      string param1 = code.Replace( " ", "%20" );
      string param2 = null;
      if ( code.StartsWith( "GB/Z" ) )
        param2 = "Z";
      else if ( code.StartsWith( "GB/T" ) )
        param2 = "T";
      else if ( code.StartsWith( "GB " ) )
        param2 = "Q";

      string uri = string.Format( UriTemplate_Standard, param1, param2 );
      string html = await client.DownloadStringTaskAsync( uri );
      return html;
    }

    static async Task pickDetail( WebClient client, string code ) {
      if ( string.IsNullOrWhiteSpace( code ) )
        return;

      HtmlDocument doc = new HtmlDocument();

      Console.Write( "get standard: " + code );
      string html = await getDetailHtml( client, code );
      doc.LoadHtml( html );
      Console.WriteLine( "..." );

      processDetail( code, doc.DocumentNode, true );
    }

    static int processList( HtmlNode root, bool isRevocative, bool saveChanges ) {
      var table = root.SelectSingleNode( "//table[@id='Tbinput']" );
      var rows = table.SelectNodes( "./tr" );
      bool isHeader = true;
      int count = 0;
      foreach ( var row in rows ) {
        if ( isHeader ) {
          isHeader = false;
          continue;
        }
        string code = null,
          remark = null;
        int c = 0;
        var cols = row.SelectNodes( "./td" );
        foreach ( var col in cols ) {
          c++;
          if ( c == 2 ) {
            var a = col.SelectSingleNode( "./a" );
            code = a.InnerText.Trim( '\t', '\r', '\n' );
          }
          else if ( c == 6 ) {
            remark = col.InnerText.Trim( '\t', '\r', '\n' );
          }
        }
        biz.SaveKey_ChinaStandard( code, remark, isRevocative, saveChanges );
        count++;
      }
      return count;
    }

    static async Task<int> processList2( WebClient client, HtmlNode root, bool isRevocative, bool saveChanges ) {
      var table = root.SelectSingleNode( "//table[@id='Tbinput']" );
      var rows = table.SelectNodes( "./tr" );
      bool isHeader = true;
      int count = 0;
      foreach ( var row in rows ) {
        if ( isHeader ) {
          isHeader = false;
          continue;
        }
        string code = null,
          remark = null;
        int c = 0;
        var cols = row.SelectNodes( "./td" );
        foreach ( var col in cols ) {
          c++;
          if ( c == 2 ) {
            var a = col.SelectSingleNode( "./a" );
            code = a.InnerText.Trim( '\t', '\r', '\n' );
          }
          else if ( c == 6 ) {
            remark = col.InnerText.Trim( '\t', '\r', '\n' );
          }
        }
        Console.Write( "  --> " + code );
        string detailHtml = await getDetailHtml( client, code );
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml( detailHtml );
        Console.Write( " --> DB" );
        // save
        JObject json = getDetailJson( doc.DocumentNode );
        biz.UpdateStandard( code, json, remark, true, saveChanges );

        Console.WriteLine( " √" );

        count++;
      }
      return count;
    }

    static JObject getDetailJson( HtmlNode root ) {
      var table = root.SelectSingleNode( "//table" );
      var rows = table.SelectNodes( "./tr" );
      int rowNumber = 0;
      JObject item = new JObject();

      foreach ( var row in rows ) {
        rowNumber++;
        // 14 rows
        if ( rowNumber == 1 ) { // 标准号
          var td = row.SelectSingleNode( "./td[@colspan='5']" );
          item["standard_code"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( rowNumber == 2 ) { // 中文标准名称
          var td = row.SelectSingleNode( "./td[@colspan='5']" );
          item["chinese_title"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( rowNumber == 3 ) { // 英文标准名称
          var td = row.SelectSingleNode( "./td[@colspan='5']" );
          item["english_title"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( rowNumber == 4 ) { // 发布日期, 实施日期, 首次发布日期
          var tds = row.SelectNodes( "./td" );
          var td = tds.Skip( 1 ).FirstOrDefault();
          string v = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
          item["issuance_date"] = string.IsNullOrWhiteSpace( v ) ? DateTime.MaxValue : DateTime.Parse( v );

          td = tds.Skip( 3 ).FirstOrDefault();
          v = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
          item["execute_date"] = string.IsNullOrWhiteSpace( v ) ? DateTime.MaxValue : DateTime.Parse( v );

          td = tds.Skip( 5 ).FirstOrDefault();
          v = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
          item["first_issuance_date"] = string.IsNullOrWhiteSpace( v ) ? DateTime.MaxValue : DateTime.Parse( v );
        }
        else if ( rowNumber == 5 ) { // 标准状态, 复审确认日期, 计划编号
          var tds = row.SelectNodes( "./td" );
          var td = tds.Skip( 1 ).FirstOrDefault();
          item["state"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();

          td = tds.Skip( 3 ).FirstOrDefault();
          string v = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
          item["review_affirmance_date"] = string.IsNullOrWhiteSpace( v ) ? DateTime.MaxValue : DateTime.Parse( v );

          td = tds.Skip( 5 ).FirstOrDefault();
          item["plan_number"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( rowNumber == 6 ) { // 代替国标号, 被代替国标号, 废止时间
          var tds = row.SelectNodes( "./td" );
          var td = tds.Skip( 1 ).FirstOrDefault();
          item["replaces"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();

          td = tds.Skip( 3 ).FirstOrDefault();
          item["replaced_by"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();

          td = tds.Skip( 5 ).FirstOrDefault();
          string v = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
          item["revocatory_date"] = string.IsNullOrWhiteSpace( v ) ? DateTime.MaxValue : DateTime.Parse( v );
        }
        else if ( rowNumber == 7 ) { // 采用国际标准号
          var td = row.SelectSingleNode( "./td[@colspan='5']" );
          item["adopted_international_standard_number"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( rowNumber == 8 ) { // 采标名称
          var td = row.SelectSingleNode( "./td[@colspan='5']" );
          item["adopted_international_standard_name"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( rowNumber == 9 ) { // 采用程度, 采用国际标准
          var td = row.SelectSingleNode( "./td[@colspan='3']" );
          item["application_degree"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();

          var tdLast = row.SelectNodes( "./td" ).Last();
          item["adopted_international_standard"] = tdLast == null ? null : tdLast.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( rowNumber == 10 ) { // 国际标准分类号, 中国标准分类号
          var td = row.SelectSingleNode( "./td[@colspan='3']" );
          item["ics"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();

          var tdLast = row.SelectNodes( "./td" ).Last();
          item["ccs"] = tdLast == null ? null : tdLast.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( rowNumber == 11 ) { // 标准类别, 标准页码, 标准价格(元)
          var tds = row.SelectNodes( "./td" );
          var td = tds.Skip( 1 ).FirstOrDefault();
          item["category"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();

          td = tds.Skip( 3 ).FirstOrDefault();
          item["number_of_pages"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();

          td = tds.Skip( 5 ).FirstOrDefault();
          item["price"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( rowNumber == 12 ) { // 主管部门
          var td = row.SelectSingleNode( "./td[@colspan='5']" );
          item["governor"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( rowNumber == 13 ) { // 归口单位
          var td = row.SelectSingleNode( "./td[@colspan='5']" );
          item["technical_committees"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( rowNumber == 14 ) { // 起草单位
          var td = row.SelectSingleNode( "./td[@colspan='5']" );
          item["drafting_committee"] = td == null ? null : td.InnerText
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
      }
      return item;
    }

    static void processDetail( string code, HtmlNode root, bool saveChanges ) {
      JObject item = getDetailJson( root );
      // save
      biz.UpdateStandard( code, item );
    }

  }

}
