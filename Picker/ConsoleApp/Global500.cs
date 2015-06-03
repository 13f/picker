using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp {
  public static class Global500 {
    const string domain = "http://www.fortunechina.com";

    const string strProcessed = "processed";
    const string strRank2014 = "rank2014";
    const string strRand2013 = "rank2013";
    const string strOperatingReceiptInMillionDollars = "operating_receipt_in_million_dollars";
    const string strProfitInMillionDollars = "profit_in_million_dollars";
    const string strCountry = "country";

    public static void PickCatalog() {
      Console.WriteLine( "==== 更新目录 ====" );
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      string file = @"D:\github\picker\data\organization\global500-2014.xml";
      XElement xeRoot = XElement.Load( file );
      var dataGroup = xeRoot.Elements( "group" ).Where( xe => xe.Attribute( "name" ).Value == "data" ).FirstOrDefault();
      string url = dataGroup.Attribute( "url" ).Value;
      Console.WriteLine( "正在处理：" + url );
      string data = client.DownloadString( url );
      var children = parseOrgCatalog( data );
      if ( children.Count == 0 ) { // 遇到报错，先处理后面的
        Console.WriteLine( "出现异常，跳过……" );
      }
      else {
        dataGroup.Add( children );
        // save
        xeRoot.Save( file, SaveOptions.None );
      }

      Console.WriteLine( "已完成！" );
      Console.ReadKey();
    }

    static List<XElement> parseOrgCatalog( string html ) {
      List<XElement> result = new List<XElement>();
      int start = html.IndexOf( "<table class=\"rankingtable\" id=\"yytable\"" );
      if ( start < 0 )
        return result;
      int end = html.IndexOf( "</table>", start );
      string data = html.Substring( start, end - start + 8 );

      XElement root = XElement.Parse( data );
      var rows = root.Elements( "tr" );

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
          new XAttribute( strRank2014, td1.Value ),
          new XAttribute( strRand2013, td2.Value ),
          new XAttribute( "name", td3.Value ),
          new XAttribute( strOperatingReceiptInMillionDollars, td4.Value ),
          new XAttribute( strProfitInMillionDollars, td5.Value ),
          new XAttribute( strCountry, td6.Value )
          );
        result.Add( item );
      }

      return result;
    }

  }

}
