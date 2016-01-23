using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace ConsoleApp {
  public static class Global500 {
    const string domain = "http://www.fortunechina.com";

    const string strProcessed = "processed";
    const string strRank2015 = "rank2015";
    const string strRank2014 = "rank2014";
    const string strRank2013 = "rank2013";
    const string strOperatingReceiptInMillionDollars = "operating_receipt_in_million_dollars";
    const string strProfitInMillionDollars = "profit_in_million_dollars";
    const string strCountry = "country";

    static JArray createConfigSection( string strRankCurrentYear, string strRankLastYear ) {
      JArray config = new JArray();
      JObject currentYear = new JObject(
        new JProperty( "name", strRankCurrentYear ),
        new JProperty( "title", strRankCurrentYear )
        );
      config.Add( currentYear );
      JObject lastYear = new JObject(
        new JProperty( "name", strRankLastYear ),
        new JProperty( "title", strRankLastYear )
        );
      config.Add( lastYear );
      JObject name = new JObject(
        new JProperty( "name", "name" ),
        new JProperty( "title", "名称" )
        );
      config.Add( name );
      JObject operatingReceiptInMillionDollars = new JObject(
        new JProperty( "name", strOperatingReceiptInMillionDollars ),
        new JProperty( "title", "营业收入（百万美元）" )
        );
      config.Add( operatingReceiptInMillionDollars );
      JObject profitInMillionDollars = new JObject(
        new JProperty( "name", strProfitInMillionDollars ),
        new JProperty( "title", "利润（百万美元）" )
        );
      config.Add( profitInMillionDollars );
      JObject country = new JObject(
        new JProperty( "name", "country" ),
        new JProperty( "title", "国家" )
        );
      config.Add( country );

      return config;
    }

    public static void PickToJson(string urlYear, string tableKey, string strRankCurrentYear, string strRankLastYear, string saveToFileWithoutExtension ) {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      JObject json = new JObject();

      Console.WriteLine( "==== set config section ====" );
      JArray config = createConfigSection( strRankCurrentYear, strRankLastYear );
      json["config"] = config;

      Console.WriteLine( "==== 获取目录 ====" );
      string data = client.DownloadString( urlYear );

      Console.WriteLine( "==== 处理目录 ====" );
      JArray array = parseOrgCatalogToJson( data, tableKey, strRankCurrentYear, strRankLastYear );
      json["data"] = array;

      Console.WriteLine( "==== save ====" );
      string strJson = json.ToString( Newtonsoft.Json.Formatting.Indented );
      System.IO.File.WriteAllText( saveToFileWithoutExtension + ".json", strJson, Encoding.UTF8 );

      Console.WriteLine( "已完成！" );
    }

    public static void PickToJsonBefore2013( string urlYear, string tableKey, string strRankCurrentYear, string strRankLastYear, string saveToFileWithoutExtension ) {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      JObject json = new JObject();

      Console.WriteLine( "==== set config section ====" );
      JArray config = createConfigSection( strRankCurrentYear, strRankLastYear );
      json["config"] = config;

      JArray array = new JArray();

      Console.WriteLine( "==== 获取目录-1 ====" );
      string data = client.DownloadString( urlYear );
      Console.WriteLine( "==== 处理目录-1 ====" );
      JArray tmpArray = parseOrgCatalogToJsonBefore2013( data, tableKey, strRankCurrentYear, strRankLastYear );
      foreach ( var item in tmpArray )
        array.Add( item );

      Console.WriteLine( "==== 获取目录-2 ====" );
      string url = urlYear.Replace( ".htm", "_2.htm" );
      data = client.DownloadString( url );
      Console.WriteLine( "==== 处理目录-2 ====" );
      tmpArray = parseOrgCatalogToJsonBefore2013( data, tableKey, strRankCurrentYear, strRankLastYear );
      foreach ( var item in tmpArray )
        array.Add( item );

      Console.WriteLine( "==== 获取目录-3 ====" );
      url = urlYear.Replace( ".htm", "_3.htm" );
      data = client.DownloadString( url );
      Console.WriteLine( "==== 处理目录-3 ====" );
      tmpArray = parseOrgCatalogToJsonBefore2013( data, tableKey, strRankCurrentYear, strRankLastYear );
      foreach ( var item in tmpArray )
        array.Add( item );

      Console.WriteLine( "==== 获取目录-4 ====" );
      url = urlYear.Replace( ".htm", "_4.htm" );
      data = client.DownloadString( url );
      Console.WriteLine( "==== 处理目录-4 ====" );
      tmpArray = parseOrgCatalogToJsonBefore2013( data, tableKey, strRankCurrentYear, strRankLastYear );
      foreach ( var item in tmpArray )
        array.Add( item );

      Console.WriteLine( "==== 获取目录-5 ====" );
      url = urlYear.Replace( ".htm", "_5.htm" );
      data = client.DownloadString( url );
      Console.WriteLine( "==== 处理目录-5 ====" );
      tmpArray = parseOrgCatalogToJsonBefore2013( data, tableKey, strRankCurrentYear, strRankLastYear );
      foreach ( var item in tmpArray )
        array.Add( item );

      json["data"] = array;

      Console.WriteLine( "==== save ====" );
      string strJson = json.ToString( Newtonsoft.Json.Formatting.Indented );
      System.IO.File.WriteAllText( saveToFileWithoutExtension + ".json", strJson, Encoding.UTF8 );

      Console.WriteLine( "已完成！" );
    }

    public static void PickCatalog( string sourceXmlFile, string tableKey, WebClient client = null ) {
      Console.WriteLine( "==== 更新目录 ====" );
      if ( client == null )
        client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      
      XElement xeRoot = XElement.Load( sourceXmlFile );
      var dataGroup = xeRoot.Elements( "group" ).Where( xe => xe.Attribute( "name" ).Value == "data" ).FirstOrDefault();
      string url = dataGroup.Attribute( "url" ).Value;
      Console.WriteLine( "正在处理：" + url );
      string data = client.DownloadString( url );
      var children = parseOrgCatalog( data, tableKey, strRank2015, strRank2014 );
      if ( children.Count == 0 ) { // 遇到报错，先处理后面的
        Console.WriteLine( "出现异常，跳过……" );
      }
      else {
        dataGroup.Add( children );
        // save
        xeRoot.Save( sourceXmlFile, SaveOptions.None );
      }

      Console.WriteLine( "已完成！" );
      Console.ReadKey();
    }

    static List<XElement> parseOrgCatalog( string html, string tableKey, string strRankCurrentYear, string strRankLastYear ) {
      List<XElement> result = new List<XElement>();
      int start = html.IndexOf( tableKey );
      if ( start < 0 )
        return result;
      int end = html.IndexOf( "</table>", start );
      string data = html.Substring( start, end - start + 8 );

      XElement root = XElement.Parse( data );
      var rows = root.Element( "tbody" )
        .Elements( "tr" );

      foreach ( var row in rows ) {
        XElement td1 = (XElement)row.FirstNode;
        XElement td2 = (XElement)td1.NextNode;
        XElement td3 = (XElement)td2.NextNode;

        // href="../../../../global500/3/2014"
        string url = td3.Element( "a" ).Attribute( "href" ).Value;
        url = url.Replace( "../../../..", domain );
        
        XElement td4 = (XElement)td3.NextNode;
        XElement td5 = (XElement)td4.NextNode;
        XElement td6 = (XElement)td5.NextNode;

        XElement item = new XElement( "item",
          new XAttribute( "url", url ),
          new XAttribute( strRankCurrentYear, td1.Value ),
          new XAttribute( strRankLastYear, td2.Value ),
          new XAttribute( "name", td3.Value ),
          new XAttribute( strOperatingReceiptInMillionDollars, td4.Value ),
          new XAttribute( strProfitInMillionDollars, td5.Value ),
          new XAttribute( strCountry, td6.Value )
          );
        result.Add( item );
      }

      return result;
    }

    static JArray parseOrgCatalogToJson( string html, string tableKey, string strRankCurrentYear, string strRankLastYear ) {
      JArray result = new JArray();
      int start = html.IndexOf( tableKey );
      if ( start < 0 )
        return result;
      int end = html.IndexOf( "</table>", start );
      string data = html.Substring( start, end - start + 8 );
      data = data.Replace( "&nbsp;", "" )
        .Replace( "&amp;", "&" );
      data = data.Replace( "&", "&amp;" );

      XElement root = XElement.Parse( data );
      var tbody = root.Element( "tbody" );
      var rows = tbody == null ? root.Elements( "tr" ) : tbody.Elements( "tr" );

      foreach ( var row in rows ) {
        XElement td1 = (XElement)row.FirstNode;
        XElement td2 = (XElement)td1.NextNode;
        XElement td3 = (XElement)td2.NextNode;

        // href="../../../../global500/3/2014"
        string url = td3.Element( "a" ).Attribute( "href" ).Value;
        url = url.Replace( "../../../..", domain );

        XElement td4 = (XElement)td3.NextNode;
        XElement td5 = (XElement)td4.NextNode;
        XElement td6 = (XElement)td5.NextNode;

        JObject jo = new JObject();
        jo["url"] = url;
        jo[strRankCurrentYear] = td1.Value;
        jo[strRankLastYear] = td2.Value;
        jo["name"] = td3.Value;
        jo[strOperatingReceiptInMillionDollars] = td4.Value;
        jo[strProfitInMillionDollars] = td5.Value;
        jo[strCountry] = td6.Value;
        result.Add( jo );

        Console.WriteLine( "item: " + td1.Value + " >> " + url );
      }

      return result;
    }

    static JArray parseOrgCatalogToJsonBefore2013( string html, string tableKey, string strRankCurrentYear, string strRankLastYear ) {
      JArray result = new JArray();
      int start = html.IndexOf( tableKey );
      if ( start < 0 )
        return result;
      int end = html.IndexOf( "</table>", start );
      string data = html.Substring( start, end - start + 8 );
      data = data.Replace( "&nbsp;", "" )
        .Replace( "&amp;", "&" );
      data = data.Replace( "&", "&amp;" )
        .Replace( "border=0", "" );

      XElement root = XElement.Parse( data );
      var tbody = root.Element( "tbody" );
      var rows = tbody == null ? root.Elements( "tr" ) : tbody.Elements( "tr" );
      // <tbody><tr><th>排名<br/></th><th>上年<br/>排名</th><th> 公司名称<br/>(中英文)</th><th> 营业收入<br/>(百万美元)</th><th> 利润<br/>(百万美元)</th><th>国家</th></tr><tr><td class="f500c1">1</td>
      rows = rows.Skip( 1 );

      foreach ( var row in rows ) {
        XElement td1 = (XElement)row.FirstNode;
        XElement td2 = (XElement)td1.NextNode;
        XElement td3 = (XElement)td2.NextNode;

        // href="../../../../global500/3/2014"
        string url = td3.Element( "a" ).Attribute( "href" ).Value;
        url = url.Replace( "../../../..", domain );

        XElement td4 = (XElement)td3.NextNode;
        XElement td5 = (XElement)td4.NextNode;
        XElement td6 = (XElement)td5.NextNode;

        JObject jo = new JObject();
        jo["url"] = url;
        jo[strRankCurrentYear] = td1.Value;
        jo[strRankLastYear] = td2.Value;
        jo["name"] = td3.Value;
        jo[strOperatingReceiptInMillionDollars] = td4.Value;
        jo[strProfitInMillionDollars] = td5.Value;
        jo[strCountry] = td6.Value;
        result.Add( jo );

        Console.WriteLine( "item: " + td1.Value + " >> " + url );
      }

      return result;
    }

  }

}
