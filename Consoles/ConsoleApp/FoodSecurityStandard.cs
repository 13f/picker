using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp {
  public static class FoodSecurityStandard {

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
