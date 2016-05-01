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

namespace ConsoleApp {
  public static class FoodSecurityStandard {
    const string Regex_FileId = "(?<=load\\(')[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}(?='\\))";

    const string Key_Replace = "replace";
    const string Key_Quote = "quote";

    static async Task<string> doPost_Zhejiang_GBK(string uri ) {
      var gbk = Encoding.GetEncoding( "gbk" );

      string postString = "id2=F65FC0275F670896";
      byte[] postData = gbk.GetBytes( postString );

      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_GBK();

      string cookie = "JSESSIONID=94F92A1951D5D01F93A1633996047FD2";
      client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // post
      client.Headers.Add( "Cookie", cookie );

      byte[] responseData = await client.UploadDataTaskAsync( uri, "POST", postData ); // 得到返回字符流
      string result = gbk.GetString( responseData ); // 解码

      return result;
    }
    
    public static async Task PickList_Standards( string filepath ) {
      int total = 677;
      string uri = "http://bz.cfsa.net.cn/db";
      JObject joRoot = new JObject();
      joRoot.Add( "title", "食品安全国家标准" );
      //joRoot.Add( "description", "" );
      joRoot.Add( "url", "http://bz.cfsa.net.cn/db" );
      joRoot.Add( "updated_at", "2016-4-27" );
      joRoot.Add( "total_count", total );
      // config
      JObject joConfig = new JObject();
      joConfig.Add( "id", "ID" );
      joConfig.Add( "number", "标准号" );
      joConfig.Add( "name", "标准名称" );
      joConfig.Add( "category", "标准类别" );
      joConfig.Add( "status", "状态：现行/废止" );
      joRoot.Add( "config", joConfig );

      // get
      Console.WriteLine( "get html..." );
      string postString = "task=listStandardGJ";
      byte[] postData = Encoding.UTF8.GetBytes( postString );

      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // post

      //string html = await client.DownloadStringTaskAsync( uri );
      byte[] responseData = await client.UploadDataTaskAsync( uri, "POST", postData ); // 得到返回字符流
      string html = Encoding.UTF8.GetString( responseData ); // 解码
      // parse
      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );
      var table = doc.DocumentNode.SelectSingleNode( "//table[@class='table_list']" );
      var rows = table.SelectNodes( "./tr" );
      bool isHeaderRow = true;
      int count = 0;

      JArray items = new JArray();
      foreach (var row in rows ) {
        if ( isHeaderRow ) {
          isHeaderRow = false;
          continue;
        }
        var r = parseRow_Standards( row );
        items.Add( r );
        count++;
        if ( count % 5 == 0 || count == total )
          Console.WriteLine( count.ToString() + " items processed..." );
      }
      joRoot.Add( "items", items );

      // save
      Console.WriteLine( "save json..." );
      string json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    public static async Task PickList_LocalStandards( string filepath ) {
      int total = 154;
      string uri = "http://bz.cfsa.net.cn/db";
      JObject joRoot = new JObject();
      joRoot.Add( "title", "食品安全地方标准" );
      //joRoot.Add( "description", "" );
      joRoot.Add( "url", "http://bz.cfsa.net.cn/db" );
      joRoot.Add( "updated_at", "2016-4-29" );
      joRoot.Add( "total_count", total );
      // config
      JObject joConfig = new JObject();
      joConfig.Add( "id", "ID" );
      joConfig.Add( "number", "标准号" );
      joConfig.Add( "name", "标准名称" );
      joConfig.Add( "category", "标准类别" );
      joConfig.Add( "province", "省份" );
      joRoot.Add( "config", joConfig );

      // get
      Console.WriteLine( "get html..." );
      string postString = "task=listStandardDF";
      byte[] postData = Encoding.UTF8.GetBytes( postString );

      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // post

      //string html = await client.DownloadStringTaskAsync( uri );
      byte[] responseData = await client.UploadDataTaskAsync( uri, "POST", postData ); // 得到返回字符流
      string html = Encoding.UTF8.GetString( responseData ); // 解码
      // parse
      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );
      var table = doc.DocumentNode.SelectSingleNode( "//table[@class='table_list']" );
      var rows = table.SelectNodes( "./tr" );
      bool isHeaderRow = true;
      int count = 0;

      JArray items = new JArray();
      foreach ( var row in rows ) {
        if ( isHeaderRow ) {
          isHeaderRow = false;
          continue;
        }
        var r = parseRow_LocalStandards( row );
        items.Add( r );
        count++;
        if ( count % 5 == 0 || count == total )
          Console.WriteLine( count.ToString() + " items processed..." );
      }
      joRoot.Add( "items", items );

      // save
      Console.WriteLine( "save json..." );
      string json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    public static async Task PickDetails_Standards( string listFilePath, string directory, bool encodeFileName, int? millisecondsDelay ) {
      Console.WriteLine( "load list json..." );
      string json = System.IO.File.ReadAllText( listFilePath );
      JObject joRoot = JObject.Parse( json );
      JArray items = (JArray)joRoot["items"];

      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      int total = 677;
      int count = 0;
      foreach (JObject joItem in items ) {
        await PickDetail_Standard( client, joItem, directory, true, encodeFileName );
        count++;
        if ( count % 5 == 0 || count == total )
          Console.WriteLine( count.ToString() + " items processed..." );
        if ( millisecondsDelay != null )
          await Task.Delay( millisecondsDelay.Value );
      }

      // save
      Console.WriteLine( "save..." );
      json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( listFilePath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    //public static async Task DownloadPdf_Standards( string listFilePath, string directory, int? millisecondsDelay ) {
    //  Console.WriteLine( "load list json..." );
    //  string json = System.IO.File.ReadAllText( listFilePath );
    //  JObject joRoot = JObject.Parse( json );
    //  JArray items = (JArray)joRoot["items"];

    //  int total = 677;
    //  int count = 0;
    //  foreach ( JObject joItem in items ) {
    //    string id = (string)joItem["id"];
    //    string number = (string)joItem["number"];
    //    await downloadPdf_Standard( id, number, directory );
    //    count++;
    //    if ( count % 5 == 0 || count == total )
    //      Console.WriteLine( count.ToString() + " items processed..." );
    //    if ( millisecondsDelay != null )
    //      await Task.Delay( millisecondsDelay.Value );
    //  }
    //  Console.WriteLine( "over..." );
    //}

    public static async Task DownloadPdf_Standard( string listFilePath, string directory, string standardNumber ) {
      Console.WriteLine( "load list json..." );
      string json = System.IO.File.ReadAllText( listFilePath );
      JObject joRoot = JObject.Parse( json );
      JArray items = (JArray)joRoot["items"];

      JObject jo = null;
      Console.WriteLine( "finding..." );
      foreach ( JObject joItem in items ) {
        string number = (string)joItem["number"];
        if ( number == standardNumber ) {
          jo = joItem;
          break;
        }
      }
      if(jo != null ) {
        Console.WriteLine( "found..." );
        string id = (string)jo["id"];
        string uri = string.Format( "http://bz.cfsa.net.cn/staticPages/{0}.html", id );
        WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
        string html = await client.DownloadStringTaskAsync( uri );

        string filename = standardNumber + " " + (string)jo["name"] + ".pdf";
        var m = Regex.Match( html, Regex_FileId );
        Console.WriteLine( "downloading " + filename + " ..." );
        await downloadPdf_Standard( m.Value, filename, directory );
      }
      else
        Console.WriteLine( "not found..." );
      Console.WriteLine( "over..." );
    }

    public static async Task PickDetail_Standard( WebClient client, JObject joItem, string directory, bool downloadPdf, bool encodeFileName ) {
      string id = (string)joItem["id"];
      // test
      //if ( id != "C8617E50-BCA4-4820-96C5-00E4FE05FA6C" )
      //  return;
      
      Console.WriteLine( "get detail of: " + id );
      // http://bz.cfsa.net.cn/staticPages/2F5BA85F-715C-41D5-B095-C09F1695363F.html
      string uri = string.Format( "http://bz.cfsa.net.cn/staticPages/{0}.html", id );
      string html = await client.DownloadStringTaskAsync( uri );
      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );
      doc.RemoveComments();
      doc.RemoveScripts();

      string keyEnglishName = "英文名称：";
      string keyIssueDate = "发布日期：";
      string keyImplementationDate = "实施日期：";
      string keyTags = "标&nbsp;&nbsp;&nbsp;签：";
      string keyDraftingUnit = "起草单位：";

      // update detail
      string number = (string)joItem["number"];
      string filename = number + " " + (string)joItem["name"] + ".pdf";
      if ( encodeFileName )
        filename = filename.Replace( "/", "%2F" );
      joItem["file_name"] = filename;
      var spanDetail = doc.DocumentNode.SelectSingleNode( "//span[@class='list_zt_top']" );
      var ilist = spanDetail.SelectNodes( "./i" );
      foreach(var i in ilist ) {
        string txt = i.InnerText;
        if ( string.IsNullOrWhiteSpace( txt ) )
          continue;
        else if ( txt.StartsWith( keyDraftingUnit ) )
          joItem["drafting_unit"] = txt.Substring( keyDraftingUnit.Length );
        else if ( txt.StartsWith( keyEnglishName ) )
          joItem["english_name"] = txt.Substring( keyEnglishName.Length );
        else if ( txt.StartsWith( keyTags ) )
          joItem["tags"] = txt.Substring( keyTags.Length );
        else if ( txt.StartsWith( keyIssueDate ) )
          joItem["issue_date"] = txt.Substring( keyIssueDate.Length );
        else if ( txt.StartsWith( keyImplementationDate ) )
          joItem["implementation_date"] = txt.Substring( keyImplementationDate.Length );
      }
      // summary
      //var divTmpSummary = doc.DocumentNode.SelectSingleNode( "//div[@class='bz_det_tit']" );
      //var divSummary = divTmpSummary.NextSibling;
      var divSummary = doc.DocumentNode.SelectNodes( "//div[@class='bz_det_cot']" ).First();
      var p = divSummary.SelectSingleNode( "./p" );
      joItem["summary"] = p.InnerHtml.Trim( '\r', '\n', '\t' );
      // 替代
      joItem["has_replace_data"] = false;
      var divTmpReplace = doc.DocumentNode.SelectSingleNode( "//div[@class='contents_yy']" );
      var divReplace = divTmpReplace.SelectNodes( "./div[@class='contents_yy_cont border_bottom_dashed']" );
      if ( divReplace == null || divReplace.Count == 0 )
        joItem["replaces"] = null;
      else if( divReplace.Count < 5) {
        joItem["replaces"] = getReplaceOrRefDirectly( divReplace );
        joItem["has_replace_data"] = true;
      }
      else {
        joItem["replaces"] = await getReplaceOrRef( id, Key_Replace );
        joItem["has_replace_data"] = true;
      }
      // 引用
      joItem["has_reference_data"] = false;
      var divTmpRef = doc.DocumentNode.SelectSingleNode( "//div[@class='contents_cy']" );
      var divRef = divTmpRef.SelectNodes( "./div[@class='contents_yy_cont border_bottom_dashed']" );
      if ( divRef == null || divRef.Count == 0 )
        joItem["references"] = null;
      else if ( divRef.Count < 5 ) {
        joItem["references"] = getReplaceOrRefDirectly( divRef );
        joItem["has_reference_data"] = true;
      }
      else {
        joItem["references"] = await getReplaceOrRef( id, Key_Quote );
        joItem["has_reference_data"] = true;
      }

      // download pdf
      if( downloadPdf ) {
        var m = Regex.Match( html, Regex_FileId );
        Console.WriteLine( "downloading " + filename + " ..." );
        await downloadPdf_Standard( m.Value, filename, directory );
      }
    }

    public static void CheckNotDownloaded_Standards( string listFilePath, string directory ) {
      List<string> numbers = new List<string>();

      Console.WriteLine( "load list json..." );
      string json = System.IO.File.ReadAllText( listFilePath );
      JObject joRoot = JObject.Parse( json );
      JArray items = (JArray)joRoot["items"];

      Console.WriteLine( "repeat items in json..." );
      var numbersInJson = items.Select( i => (string)i["number"] ).ToList();
      var repeat = numbersInJson.GroupBy( l => l )
             .Where( g => g.Count() > 1 )
             .Select( i => i.Key);
      int count = 0;
      foreach ( string n in repeat ) {
        count++;
        Console.WriteLine( count.ToString() + "、" + n );
      }

      var files = System.IO.Directory.GetFiles( directory );

      Console.WriteLine( "JSON Items count = " + items.Count );
      Console.WriteLine( "Files count = " + files.Length );

      Console.WriteLine( "check..." );
      foreach ( JObject joItem in items ) {
        string number = (string)joItem["number"];
        bool exists = files.Contains( directory + number + ".pdf" );
        if ( !exists )
          numbers.Add( number );
      }

      Console.WriteLine( "result:" );
      count = 0;
      foreach(string n in numbers ) {
        count++;
        Console.WriteLine( count.ToString() + "、" + n );
      }
    }

    public static async Task UpdateReplaceOrReference_Standards( string listFilePath, int? millisecondsDelay ) {
      Console.WriteLine( "load list json..." );
      string json = System.IO.File.ReadAllText( listFilePath );
      JObject joRoot = JObject.Parse( json );
      JArray items = (JArray)joRoot["items"];

      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      int total = 677;
      int count = 0;
      foreach ( JObject joItem in items ) {
        // test
        //if ( (string)joItem["id"] != "D6B7D8B2-F931-4F5D-9C70-81E142F27E92" )
        //  continue;

        if ( (bool)joItem["has_replace_data"] ) {
          await updateReplaceOrRef( joItem, "replaces", Key_Replace );
        }
        if ( (bool)joItem["has_reference_data"] ) {
          await updateReplaceOrRef( joItem, "references", Key_Quote );
        }
        count++;
        if ( count % 5 == 0 || count == total )
          Console.WriteLine( count.ToString() + " items processed..." );
        if ( millisecondsDelay != null )
          await Task.Delay( millisecondsDelay.Value );
      }

      // save
      Console.WriteLine( "save..." );
      json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( listFilePath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    //public static async Task UpdateReplaceOrReference_Standard( WebClient client, JObject joItem ) {
    //  string id = (string)joItem["id"];
    //  // test
    //  //if ( id != "C8617E50-BCA4-4820-96C5-00E4FE05FA6C" )
    //  //  return;

    //  Console.WriteLine( "get detail of: " + id );
    //  // http://bz.cfsa.net.cn/staticPages/2F5BA85F-715C-41D5-B095-C09F1695363F.html
    //  string uri = string.Format( "http://bz.cfsa.net.cn/staticPages/{0}.html", id );
    //  string html = await client.DownloadStringTaskAsync( uri );
    //  HtmlDocument doc = new HtmlDocument();
    //  doc.LoadHtml( html );
    //  doc.RemoveComments();
    //  doc.RemoveScripts();
      
    //  // summary
    //  //var divTmpSummary = doc.DocumentNode.SelectSingleNode( "//div[@class='bz_det_tit']" );
    //  //var divSummary = divTmpSummary.NextSibling;
    //  var divSummary = doc.DocumentNode.SelectNodes( "//div[@class='bz_det_cot']" ).First();
    //  var p = divSummary.SelectSingleNode( "./p" );
    //  joItem["summary"] = p.InnerHtml.Trim( '\r', '\n', '\t' );
    //  // 替代
    //  joItem["has_replace_data"] = false;
    //  var divTmpReplace = doc.DocumentNode.SelectSingleNode( "//div[@class='contents_yy']" );
    //  var divReplace = divTmpReplace.SelectNodes( "./div[@class='contents_yy_cont border_bottom_dashed']" );
    //  if ( divReplace == null || divReplace.Count == 0 )
    //    joItem["replaces"] = null;
    //  else {
    //    JArray arrayOld = (JArray)joItem["replaces"];
    //    JArray arrayNew = null;
    //    if ( divReplace.Count < 5 )
    //      arrayNew = getReplaceOrRefDirectly( divReplace );
    //    else
    //      arrayNew = await getReplaceOrRef( id, Key_Replace );
    //    //if ( arrayNew != null && ( arrayOld == null || arrayNew.Count >= arrayOld.Count ) )
    //      joItem["replaces"] = arrayNew;
    //    joItem["has_replace_data"] = true;
    //  }

    //  // 引用
    //  joItem["has_reference_data"] = false;
    //  var divTmpRef = doc.DocumentNode.SelectSingleNode( "//div[@class='contents_cy']" );
    //  var divRef = divTmpRef.SelectNodes( "./div[@class='contents_yy_cont border_bottom_dashed']" );
    //  if ( divRef == null || divRef.Count == 0 )
    //    joItem["references"] = null;
    //  else {
    //    JArray arrayOld = (JArray)joItem["references"];
    //    JArray arrayNew = null;
    //    if ( divRef.Count < 5 )
    //      arrayNew = getReplaceOrRefDirectly( divRef );
    //    else
    //      arrayNew = await getReplaceOrRef( id, Key_Quote );
    //    //if ( arrayNew != null && ( arrayOld == null || arrayNew.Count >= arrayOld.Count ) )
    //      joItem["references"] = arrayNew;
    //    joItem["has_reference_data"] = true;
    //  }
    //}

    static async Task updateReplaceOrRef( JObject joItem, string itemKey, string queryType ) {
      string id = (string)joItem["id"];
      Console.WriteLine( "get [" + queryType + "] of " + id + "..." );

      JArray arrayOld = (JArray)joItem[itemKey];
      JArray arrayNew = await getReplaceOrRef( id, queryType );
      if ( arrayNew != null && ( arrayOld == null || arrayNew.Count >= arrayOld.Count ) )
        joItem[itemKey] = arrayNew;
    }

    static JArray getReplaceOrRefDirectly( HtmlNodeCollection divList ) {
      JArray items = new JArray();
      foreach(var div in divList ) {
        var span = div.SelectSingleNode( "./span" );
        string txt = span.InnerText.Trim( '\r', '\n', '\t' );
        items.Add( txt );
      }
      return items;
    }

    static async Task<JArray> getReplaceOrRef( string id, string queryType ) {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // post

      string uri = "http://bz.cfsa.net.cn/db";
      HtmlDocument doc = new HtmlDocument();
      JArray items = new JArray();

      string postString = string.Format( "guid={0}&type={1}&task=list_Quote_Or_Replace", id, queryType );
      int num = 1;
      while ( true ) {
        if(num > 1)
          postString = string.Format( "guid={0}&type={1}&task=list_Quote_Or_Replace&num={2}", id, queryType, num );
        byte[] postData = Encoding.UTF8.GetBytes( postString );

        byte[] responseData = await client.UploadDataTaskAsync( uri, "POST", postData ); // 得到返回字符流
        string html = Encoding.UTF8.GetString( responseData ); // 解码

        doc.LoadHtml( html );

        HtmlNodeCollection divList = doc.DocumentNode.SelectNodes( "//div[@class='contents_yy_cont border_bottom_dashed']" );
        if ( divList == null || divList.Count == 0 )
          break;
        foreach ( var div in divList ) {
          var span = div.SelectSingleNode( "./span" );
          string txt = span.InnerText.Trim( '\r', '\n', '\t' );
          items.Add( txt );
        }
        if ( divList.Count < 10 )
          break;
        // continue
        num++;
      }
      
      return items;
    }

    static async Task downloadPdf_Standard( string fileId, string filename, string directory ) {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // post

      string uri = "http://bz.cfsa.net.cn/cfsa_aiguo";
      var values = new System.Collections.Specialized.NameValueCollection
        {
            { "file_guid", fileId },
            { "task", "d_p" },
            { "filePath", "D:/Res_upload/swfupload/" }
        };
      byte[] responseData = client.UploadValues( uri, values );

      System.IO.File.WriteAllBytes( directory + filename, responseData );
    }

    static JObject parseRow_Standards( HtmlNode row ) {
      JObject jo = new JObject();
      var tdlist = row.SelectNodes( "./td" );
      int index = -1;
      // javascript:goto('2F5BA85F-715C-41D5-B095-C09F1695363F','2')
      string key1 = "javascript:goto('",
        key2 = "','2')";
      foreach(var td in tdlist ) {
        index++;
        if ( index == 1 ) { // id
          var a = td.SelectSingleNode( "./a" );
          string onclick = a.Attributes["onclick"].Value;
          string id = onclick.Substring( key1.Length, onclick.Length - key1.Length - key2.Length );
          jo.Add( "id", id );
        }
        else if (index == 2 ) { // 标准号
          jo.Add( "number", td.InnerText );
        }
        else if ( index == 3 ) { // 状态
          jo.Add( "status", td.InnerText );
        }
        else if ( index == 4 ) { // 名称
          jo.Add( "name", td.InnerText );
        }
        else if ( index == 5 ) { // 类别
          jo.Add( "category", td.InnerText );
        }
      }
      return jo;
    }

    static JObject parseRow_LocalStandards( HtmlNode row ) {
      JObject jo = new JObject();
      var tdlist = row.SelectNodes( "./td" );
      int index = -1;
      // javascript:goto('2F5BA85F-715C-41D5-B095-C09F1695363F','2')
      string key1 = "javascript:goto('",
        key2 = "','2')";
      foreach ( var td in tdlist ) {
        index++;
        if ( index == 1 ) { // id
          var a = td.SelectSingleNode( "./a" );
          string onclick = a.Attributes["onclick"].Value;
          string id = onclick.Substring( key1.Length, onclick.Length - key1.Length - key2.Length );
          jo.Add( "id", id );
        }
        else if ( index == 2 ) { // 标准号
          jo.Add( "number", td.InnerText );
        }
        else if ( index == 3 ) { // 名称
          jo.Add( "name", td.InnerText );
        }
        else if ( index == 4 ) { // 类别
          jo.Add( "category", td.InnerText );
        }
        else if ( index == 5 ) { // 省份
          jo.Add( "province", td.InnerText );
        }
      }
      return jo;
    }

    public static async Task PickItems_Zhejiang( string filepath ) {
      int totalCount = 5424;
      string uriTemplate = "http://spaqbz.zjwst.gov.cn/zjfsdata/proxy/eStandProxy.jsp?startrecord={0}&endrecord={1}&perpage=20&totalRecord=5424";
      
      Console.WriteLine( "Get data from: http://spaqbz.zjwst.gov.cn/zjfsdata/html/eStandardList.jsp" );

      JObject joRoot = new JObject();
      joRoot.Add( "title", "食品安全标准·企业标准·浙江省" );
      joRoot.Add( "description", "有效期为自发证日期起三年。" );
      joRoot.Add( "url", "http://spaqbz.zjwst.gov.cn/zjfsdata/html/eStandardList.jsp" );
      joRoot.Add( "updated_at", "2016-4-27" );
      joRoot.Add( "total_count", 5424 );
      // config
      JObject joConfig = new JObject();
      joConfig.Add( "id", "ID" );
      joConfig.Add( "company_name", "企业名称" );
      joConfig.Add( "standard_name", "标准名称" );
      joConfig.Add( "standard_number", "标准代号" );
      joConfig.Add( "record_number", "备案号" );
      joConfig.Add( "issue_date", "发证日期" );
      joRoot.Add( "config", joConfig );

      int countPerPage = 20;
      int pageIndex = 0;
      int start = 1;

      JArray items = new JArray();
      while( true ) {
        Console.WriteLine( "page index: " + pageIndex.ToString() );
        start = pageIndex * countPerPage + 1;
        int end = pageIndex * countPerPage + 20;
        if ( end > totalCount )
          end = totalCount;
        string uri = string.Format( uriTemplate, start, end );
        string data = await doPost_Zhejiang_GBK( uri );
        var list = parseItems_Zhejiang( data );
        if ( list != null ) {
          list.ForEach( i => items.Add( i ) );
        }
        if ( end >= totalCount )
          break;
        pageIndex++;
      }
      joRoot.Add( "items", items );
      // save
      Console.WriteLine( "save json..." );
      string json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    static List<JObject> parseItems_Zhejiang( string content ) {
      content = content.Trim('\r', '\n');
      List<JObject> result = new List<JObject>();
      string key = "dataStore = [";
      int index = content.IndexOf( key );
      if ( index < 0 )
        return result;

      index = index + key.Length - 1;
      string data = null;
      if ( content.EndsWith( ",];" ) )
        data = content.Substring( index, content.Length - index - 3 ) + "]";
      else
        data = content.Substring( index, content.Length - index - 1 );

      var array = JArray.Parse( data );
      foreach(var v in array ) {
        string tmp = v.Value<string>();
        var list = tmp.Split( '$' );
        if ( list.Count() != 7 )
          continue;
        JObject item = new JObject();
        // list[0]: e
        item.Add( "id", list[1] );
        item.Add( "company_name", list[2] );
        item.Add( "standard_name", list[3] );
        item.Add( "standard_number", list[4] );
        item.Add( "record_number", list[5] );
        item.Add( "issue_date", list[6] );
        result.Add( item );
      }
      return result;
    }

  }

}
