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
  public static class Fortune {
    const string domain = "http://www.fortunechina.com";

    const string strProcessed = "processed";
    const string strRank2015 = "rank2015";
    const string strRank2014 = "rank2014";
    const string strRank2013 = "rank2013";
    const string strOperatingReceiptInMillionDollars = "operating_receipt_in_million_dollars";
    const string strProfitInMillionDollars = "profit_in_million_dollars";
    const string strNetProfitLastYearInMillionDollars = "net_profit_last_year_in_million_dollars";
    const string strAssetInMillionDollars = "asset_in_million_dollars";
    const string strShareholdersEquityInMillionDollars = "shareholders_equity_in_million_dollars";
    const string strMarketValueInMilllionDollars = "market_value_in_million_dollars";
    const string strIndustry = "industry";
    const string strStockCode = "stock_code";
    const string strEmployees = "employees";
    const string strCountry = "country";
    const string strRegion = "region";

    static JArray createConfigSection( string strRankCurrentYear, string strRankLastYear ) {
      JArray config = new JArray();
      JObject currentYear = new JObject(
        new JProperty( "name", strRankCurrentYear ),
        new JProperty( "title", strRankCurrentYear )
        );
      config.Add( currentYear );

      if ( !string.IsNullOrWhiteSpace( strRankLastYear ) ) {
        JObject lastYear = new JObject(
        new JProperty( "name", strRankLastYear ),
        new JProperty( "title", strRankLastYear )
        );
        config.Add( lastYear );
      }
      
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

      JObject netProfitLastYearInMillionDollars = new JObject(
        new JProperty( "name", strNetProfitLastYearInMillionDollars ),
        new JProperty( "title", "上一年净利润（百万美元）" )
        );
      config.Add( netProfitLastYearInMillionDollars );

      JObject asset = new JObject(
        new JProperty( "name", strAssetInMillionDollars ),
        new JProperty( "title", "资产（百万美元）" )
        );
      config.Add( asset );

      JObject shareholderEquity = new JObject(
        new JProperty( "name", strShareholdersEquityInMillionDollars ),
        new JProperty( "title", "股东权益（百万美元）" )
        );
      config.Add( shareholderEquity );

      JObject marketValue = new JObject(
        new JProperty( "name", strMarketValueInMilllionDollars ),
        new JProperty( "title", "市值（百万美元）" )
        );
      config.Add( marketValue );

      JObject industry = new JObject(
        new JProperty( "name", strIndustry ),
        new JProperty( "title", "行业" )
        );
      config.Add( industry );

      JObject stockCode = new JObject(
        new JProperty( "name", strStockCode ),
        new JProperty( "title", "股票代码" )
        );
      config.Add( stockCode );

      JObject employees = new JObject(
        new JProperty( "name", strEmployees ),
        new JProperty( "title", "雇员人数" )
        );
      config.Add( employees );

      JObject country = new JObject(
        new JProperty( "name", "region" ),
        new JProperty( "title", "区域" )
        );
      config.Add( country );

      JObject linkedin = new JObject(
        new JProperty( "name", "linkedin" ),
        new JProperty( "title", "Linkedin" )
        );
      config.Add( linkedin );

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

    public static void PickToJsonBefore2013( string urlYear, string tableKey, string strRankCurrentYear, string strRankLastYear, string saveToFileWithoutExtension, int rowsSkip = 1 ) {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      JObject json = new JObject();

      Console.WriteLine( "==== set config section ====" );
      JArray config = createConfigSection( strRankCurrentYear, strRankLastYear );
      json["config"] = config;

      JArray array = new JArray();
      int pagesCount = 5;
      for ( int i = 1; i <= pagesCount; i++ ) {
        Console.WriteLine( "==== 获取目录-" + i.ToString() + " ====" );
        string url = i == 1 ? urlYear : urlYear.Replace( ".htm", "_ " + i.ToString() + ".htm" );
        string data = client.DownloadString( url );

        Console.WriteLine( "==== 处理目录-" + i.ToString() + " ====" );
        JArray tmpArray = parseOrgCatalogToJsonBefore2013( data, tableKey, strRankCurrentYear, strRankLastYear, rowsSkip );
        foreach ( var item in tmpArray )
          array.Add( item );
      }

      //Console.WriteLine( "==== 获取目录-1 ====" );
      //string data = client.DownloadString( urlYear );
      //Console.WriteLine( "==== 处理目录-1 ====" );
      //JArray tmpArray = parseOrgCatalogToJsonBefore2013( data, tableKey, strRankCurrentYear, strRankLastYear, rowsSkip );
      //foreach ( var item in tmpArray )
      //  array.Add( item );

      //Console.WriteLine( "==== 获取目录-2 ====" );
      //string url = urlYear.Replace( ".htm", "_2.htm" );
      //data = client.DownloadString( url );
      //Console.WriteLine( "==== 处理目录-2 ====" );
      //tmpArray = parseOrgCatalogToJsonBefore2013( data, tableKey, strRankCurrentYear, strRankLastYear, rowsSkip );
      //foreach ( var item in tmpArray )
      //  array.Add( item );

      //Console.WriteLine( "==== 获取目录-3 ====" );
      //url = urlYear.Replace( ".htm", "_3.htm" );
      //data = client.DownloadString( url );
      //Console.WriteLine( "==== 处理目录-3 ====" );
      //tmpArray = parseOrgCatalogToJsonBefore2013( data, tableKey, strRankCurrentYear, strRankLastYear, rowsSkip );
      //foreach ( var item in tmpArray )
      //  array.Add( item );

      //Console.WriteLine( "==== 获取目录-4 ====" );
      //url = urlYear.Replace( ".htm", "_4.htm" );
      //data = client.DownloadString( url );
      //Console.WriteLine( "==== 处理目录-4 ====" );
      //tmpArray = parseOrgCatalogToJsonBefore2013( data, tableKey, strRankCurrentYear, strRankLastYear, rowsSkip );
      //foreach ( var item in tmpArray )
      //  array.Add( item );

      //Console.WriteLine( "==== 获取目录-5 ====" );
      //url = urlYear.Replace( ".htm", "_5.htm" );
      //data = client.DownloadString( url );
      //Console.WriteLine( "==== 处理目录-5 ====" );
      //tmpArray = parseOrgCatalogToJsonBefore2013( data, tableKey, strRankCurrentYear, strRankLastYear, rowsSkip );
      //foreach ( var item in tmpArray )
      //  array.Add( item );

      json["data"] = array;

      Console.WriteLine( "==== save ====" );
      string strJson = json.ToString( Newtonsoft.Json.Formatting.Indented );
      System.IO.File.WriteAllText( saveToFileWithoutExtension + ".json", strJson, Encoding.UTF8 );

      Console.WriteLine( "已完成！" );
    }

    public static void PickToJsonBefore2010( string urlYear, string tableKey, string strRankCurrentYear, string strRankLastYear, string saveToFileWithoutExtension, int pagesCount, int rowsSkip = 1 ) {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      JObject json = new JObject();

      Console.WriteLine( "==== set config section ====" );
      JArray config = createConfigSection( strRankCurrentYear, strRankLastYear );
      json["config"] = config;

      JArray array = new JArray();

      for(int i = 1; i <= pagesCount; i++ ) {
        Console.WriteLine( "==== 获取目录-" + i.ToString() + " ====" );
        string url = i == 1 ? urlYear : urlYear.Replace( ".htm", "_ " + i.ToString() + ".htm" );
        string data = client.DownloadString( url );

        Console.WriteLine( "==== 处理目录-" + i.ToString() + " ====" );
        JArray tmpArray = parseOrgCatalogToJsonBefore2010( data, tableKey, strRankCurrentYear, strRankLastYear, rowsSkip );
        foreach ( var item in tmpArray )
          array.Add( item );
      }

      json["data"] = array;

      Console.WriteLine( "==== save ====" );
      string strJson = json.ToString( Newtonsoft.Json.Formatting.Indented );
      System.IO.File.WriteAllText( saveToFileWithoutExtension + ".json", strJson, Encoding.UTF8 );

      Console.WriteLine( "已完成！" );
    }

    public static void PickToJson2008( string urlYear, string tableKey, string strRankCurrentYear, string strRankLastYear, string saveToFileWithoutExtension, int pagesCount, int rowsSkip = 1 ) {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      JObject json = new JObject();

      Console.WriteLine( "==== set config section ====" );
      JArray config = createConfigSection( strRankCurrentYear, strRankLastYear );
      json["config"] = config;

      JArray array = new JArray();

      for ( int i = 1; i <= pagesCount; i++ ) {
        Console.WriteLine( "==== 获取目录-" + i.ToString() + " ====" );
        string url = i == 1 ? urlYear : urlYear.Replace( ".htm", "_ " + i.ToString() + ".htm" );
        string data = client.DownloadString( url );

        Console.WriteLine( "==== 处理目录-" + i.ToString() + " ====" );
        JArray tmpArray = parseOrgCatalogToJson2008( data, tableKey, strRankCurrentYear, strRankLastYear, rowsSkip );
        foreach ( var item in tmpArray )
          array.Add( item );
      }

      json["data"] = array;

      Console.WriteLine( "==== save ====" );
      string strJson = json.ToString( Newtonsoft.Json.Formatting.Indented );
      System.IO.File.WriteAllText( saveToFileWithoutExtension + ".json", strJson, Encoding.UTF8 );

      Console.WriteLine( "已完成！" );
    }

    public static void PickToJson2007( string urlYear, string tableKey, string strRankCurrentYear, string saveToFileWithoutExtension, int pagesCount ) {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      JObject json = new JObject();

      Console.WriteLine( "==== set config section ====" );
      JArray config = createConfigSection( strRankCurrentYear, null );
      json["config"] = config;

      JArray array = new JArray();

      for ( int i = 1; i <= pagesCount; i++ ) {
        Console.WriteLine( "==== 获取目录-" + i.ToString() + " ====" );
        string url = i == 1 ? urlYear : urlYear.Replace( ".htm", "_ " + i.ToString() + ".htm" );
        string data = client.DownloadString( url );

        Console.WriteLine( "==== 处理目录-" + i.ToString() + " ====" );
        JArray tmpArray = parseOrgCatalogToJson2007( data, tableKey, strRankCurrentYear );
        foreach ( var item in tmpArray )
          array.Add( item );
      }

      json["data"] = array;

      Console.WriteLine( "==== save ====" );
      string strJson = json.ToString( Newtonsoft.Json.Formatting.Indented );
      System.IO.File.WriteAllText( saveToFileWithoutExtension + ".json", strJson, Encoding.UTF8 );

      Console.WriteLine( "已完成！" );
    }

    public static void PickToJson2010_China( string urlYear, string tableKey, string strRankCurrentYear, string saveToFileWithoutExtension ) {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      JObject json = new JObject();

      Console.WriteLine( "==== set config section ====" );
      JArray config = createConfigSection( strRankCurrentYear, null );
      json["config"] = config;

      Console.WriteLine( "==== 获取目录 ====" );
      string data = client.DownloadString( urlYear );

      Console.WriteLine( "==== 处理目录 ====" );
      JArray array = parseOrgCatalogToJson2010_China( data, tableKey, strRankCurrentYear );
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
        XElement td4 = (XElement)td3.NextNode;
        XElement td5 = (XElement)td4.NextNode;
        XElement td6 = (XElement)td5.NextNode;

        JObject jo = new JObject();

        var alist = td3.Elements( "a" );
        if ( alist.Count() > 0 ) { // url
          var a = alist.ElementAt( 0 );
          // href="../../../../global500/3/2014"
          string url = a.Attribute( "href" ).Value;
          url = url.Replace( "../../../..", domain );
          jo["url"] = url;
        }
        if ( alist.Count() == 2 ) { // linkedin
          var a = alist.ElementAt( 1 );
          jo["linkedin"] = a.Attribute( "href" ).Value;
        }

        jo[strRankCurrentYear] = td1.Value;
        jo[strRankLastYear] = td2.Value;
        jo["name"] = td3.Value;
        jo[strOperatingReceiptInMillionDollars] = td4.Value;
        jo[strProfitInMillionDollars] = td5.Value;

        if ( td6 != null && !string.IsNullOrWhiteSpace( td6.Value ) )
          jo[strRegion] = td6.Value;

        result.Add( jo );

        Console.WriteLine( "item: " + td1.Value + " >> " + td3.Value );
      }

      return result;
    }

    static JArray parseOrgCatalogToJsonBefore2013( string html, string tableKey, string strRankCurrentYear, string strRankLastYear, int rowsSkip = 1 ) {
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
      // 2012
      data = data.Replace( "title='", "title=\"" )
        .Replace( "' alt='", "\" alt=\"" )
        .Replace( "' src=", "\" src=" );

      XElement root = XElement.Parse( data );
      var tbody = root.Element( "tbody" );
      var rows = tbody == null ? root.Elements( "tr" ) : tbody.Elements( "tr" );
      // <tbody><tr><th>排名<br/></th><th>上年<br/>排名</th><th> 公司名称<br/>(中英文)</th><th> 营业收入<br/>(百万美元)</th><th> 利润<br/>(百万美元)</th><th>国家</th></tr><tr><td class="f500c1">1</td>
      rows = rows.Skip( rowsSkip );

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
        // 2010年没有country
        if ( td6 != null && !string.IsNullOrWhiteSpace( td6.Value ) )
          jo[strRegion] = td6.Value;
        result.Add( jo );

        Console.WriteLine( "item: " + td1.Value + " >> " + url );
      }

      return result;
    }

    static JArray parseOrgCatalogToJsonBefore2010( string html, string tableKey, string strRankCurrentYear, string strRankLastYear, int rowsSkip = 1 ) {
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
      // 2012
      data = data.Replace( "title='", "title=\"" )
        .Replace( "' alt='", "\" alt=\"" )
        .Replace( "' src=", "\" src=" );

      XElement root = XElement.Parse( data );
      var tbody = root.Element( "tbody" );
      var rows = tbody == null ? root.Elements( "tr" ) : tbody.Elements( "tr" );
      rows = rows.Skip( rowsSkip );

      foreach ( var row in rows ) {
        XElement td1 = (XElement)row.FirstNode;
        XElement td2 = (XElement)td1.NextNode;
        XElement td3 = (XElement)td2.NextNode;

        // href="../../../../global500/3/2014"
        string url = td3.Element( "b" )
          .Element( "a" )
          .Attribute( "href" ).Value;
        url = url.Replace( "../../../..", domain );

        XElement td4 = (XElement)td3.NextNode;
        XElement td5 = (XElement)td4.NextNode;
        XElement td6 = (XElement)td5.NextNode;

        JObject jo = new JObject();
        jo["url"] = url;
        jo[strRankCurrentYear] = td1.Value;
        jo[strRankLastYear] = td2.Value;
        jo["name"] = td3.Value;
        jo[strCountry] = td4.Value;
        jo[strOperatingReceiptInMillionDollars] = td5.Value;
        jo[strProfitInMillionDollars] = td6.Value;
        result.Add( jo );

        Console.WriteLine( "item: " + td1.Value + " >> " + url );
      }

      return result;
    }

    static JArray parseOrgCatalogToJson2008( string html, string tableKey, string strRankCurrentYear, string strRankLastYear, int rowsSkip = 1 ) {
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
      rows = rows.Skip( rowsSkip );

      /*
<td bgcolor="#008080" height="17" align="right"><font color="#ffffff" face="黑体"><b>2008<br/>排名</b></font></td>
<td bgcolor="#008080" width="36" align="right"><font color="#ffffff" face="黑体"><b>2007<br/>排名</b></font></td>
<td bgcolor="#008080" width="128"><font color="#ffffff" face="黑体"><b>公司</b></font></td>
<td bgcolor="#008080" width="38"><font color="#ffffff" face="黑体"><b>国家<br/>地区</b></font></td>
<td bgcolor="#008080" width="73" align="right"><font color="#ffffff" face="黑体"><b>营业收入<br/>百万美元</b></font></td>
<td bgcolor="#008080" width="71" align="right"><font color="#ffffff" face="黑体"><b>利润<br/>百万美元</b></font></td>
<td bgcolor="#008080" width="89" align="right"><font color="#ffffff" face="黑体"><b>资产<br/>百万美元</b></font></td>
<td bgcolor="#008080" align="right"><font color="#ffffff" face="黑体"><b>股东权益<br/>百万美元</b></font></td>
<td bgcolor="#008080" width="72" align="right"><font color="#ffffff" face="黑体"><b>雇员人数</b></font></td></tr>
       */
      foreach ( var row in rows ) {
        XElement td1 = (XElement)row.FirstNode;
        XElement td2 = (XElement)td1.NextNode;
        XElement td3 = (XElement)td2.NextNode;

        XElement td4 = (XElement)td3.NextNode;
        XElement td5 = (XElement)td4.NextNode;
        XElement td6 = (XElement)td5.NextNode;

        XElement td7 = (XElement)td6.NextNode;
        XElement td8 = (XElement)td7.NextNode;
        XElement td9 = (XElement)td8.NextNode;

        JObject jo = new JObject();
        //jo["url"] = url;
        jo[strRankCurrentYear] = td1.Value;
        jo[strRankLastYear] = td2.Value;
        jo["name"] = td3.Value;
        jo[strRegion] = td4.Value;
        jo[strOperatingReceiptInMillionDollars] = td5.Value;
        jo[strProfitInMillionDollars] = td6.Value;
        jo[strAssetInMillionDollars] = td7.Value;
        jo[strShareholdersEquityInMillionDollars] = td8.Value;
        jo[strEmployees] = td9.Value;
        result.Add( jo );

        Console.WriteLine( "item: " + td1.Value + " >> " );
      }

      return result;
    }

    static JArray parseOrgCatalogToJson2007( string html, string tableKey, string strRankCurrentYear, int rowsSkip = 1 ) {
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
      rows = rows.Skip( rowsSkip );

      /*
<td>排名</td>
<td>公司名称</td>
<td>国家</td>
<td>营业收入<br/>(百万美元)</td>
<td>利润<br/>(百万美元)</td>
<td>雇员人数</td>
       */
      foreach ( var row in rows ) {
        XElement td1 = (XElement)row.FirstNode;
        XElement td2 = (XElement)td1.NextNode;
        XElement td3 = (XElement)td2.NextNode;

        XElement td4 = (XElement)td3.NextNode;
        XElement td5 = (XElement)td4.NextNode;
        XElement td6 = (XElement)td5.NextNode;
        
        JObject jo = new JObject();
        //jo["url"] = url;
        jo[strRankCurrentYear] = td1.Value;
        jo["name"] = td2.Value;
        jo[strRegion] = td3.Value;
        jo[strOperatingReceiptInMillionDollars] = td4.Value;
        jo[strProfitInMillionDollars] = td5.Value;
        jo[strEmployees] = td6.Value;
        result.Add( jo );

        Console.WriteLine( "item: " + td1.Value + " >> " );
      }

      return result;
    }

    static JArray splitString(string source, string splitWord ) {
      string[] split = new string[1] { splitWord };
      var array = source.Split( split, StringSplitOptions.RemoveEmptyEntries );

      JArray result = new JArray();
      foreach ( string i in array )
        result.Add( i );
      return result;
    }

    static JArray parseOrgCatalogToJson2010_China( string html, string tableKey, string strRankCurrentYear ) {
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
      // 2012
      data = data.Replace( "title='", "title=\"" )
        .Replace( "' alt='", "\" alt=\"" )
        .Replace( "' src=", "\" src=" );

      XElement root = XElement.Parse( data );
      var tbody = root.Element( "tbody" );
      var rows = tbody == null ? root.Elements( "tr" ) : tbody.Elements( "tr" );
      rows = rows.Skip( 1 );

      foreach ( var row in rows ) {
        XElement td1 = (XElement)row.FirstNode;
        XElement td2 = (XElement)td1.NextNode;
        XElement td3 = (XElement)td2.NextNode;

        XElement td4 = (XElement)td3.NextNode;
        XElement td5 = (XElement)td4.NextNode;
        XElement td6 = (XElement)td5.NextNode;

        XElement td7 = (XElement)td6.NextNode;
        XElement td8 = (XElement)td7.NextNode;
        XElement td9 = (XElement)td8.NextNode;

        JObject jo = new JObject();
        jo[strRankCurrentYear] = td1.Value;
        jo["name"] = td2.Value;
        jo[strOperatingReceiptInMillionDollars] = td3.Value;
        jo[strProfitInMillionDollars] = td4.Value;
        jo[strMarketValueInMilllionDollars] = td5.Value;
        jo[strAssetInMillionDollars] = td6.Value;
        jo[strNetProfitLastYearInMillionDollars] = td8.Value;

        jo[strIndustry] = splitString( td7.Value, "、" );

        jo[strStockCode] = splitString( td9.Value, "、" );

        result.Add( jo );

        Console.WriteLine( "item: " + td1.Value + " >> " );
      }

      return result;
    }

  }

}
