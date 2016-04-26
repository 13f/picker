using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp {
  public static class PeopleComCN {
    const string domain = "http://health.people.com.cn";
    const string strProcessed = "processed";

    public static void PickMedicineOrgs() {
      Console.WriteLine( "==== 更新目录 ====" );
      PickCatalog_MedicineOrg();
      Console.WriteLine( "==== 更新具体数据 ====" );
      PickItems_MedicineOrg();
      Console.WriteLine( "已完成！" );
      Console.ReadKey();
    }

    public static void PickCatalog_MedicineOrg() {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_GB2312();

      string configFile = @"G:\GitHub\picker\data\organization\药品生产企业-人民网.xml";
      XElement xeRoot = XElement.Load( configFile );
      var catalogGroup = xeRoot.Elements( "group" ).Where( xe => xe.Attribute( "name" ).Value == "catalog" ).FirstOrDefault();
      var dataGroup = xeRoot.Elements( "group" ).Where( xe => xe.Attribute( "name" ).Value == "data" ).FirstOrDefault();
      string urlPrefix = catalogGroup.Attribute( "urlprefix" ).Value;
      var items = catalogGroup.Elements( "item" );
      Console.WriteLine( "目录中包含数据页数：" + items.Count().ToString() );

      foreach ( var item in items ) {
        // check tag
        if ( item.Attribute( strProcessed ) != null )
          continue;

        string url = urlPrefix + item.Value;
        Console.WriteLine( "正在处理：" + url );
        string data = client.DownloadString( url );
        var children = parseOrgCatalog( data, domain, urlPrefix );
        if ( children.Count == 0 ) { // 遇到报错，先处理后面的
          Console.WriteLine( "  出现异常，跳过……" );
          continue;
        }
        dataGroup.Add( children );
        // set tag
        item.Add( new XAttribute( strProcessed, DateTime.Now.ToString( "yyyyMMddHHmmss" ) ) );
        // save
        xeRoot.Save( configFile, SaveOptions.None );
        Thread.Sleep( 3000 );
      }
    }

    public static void PickItems_MedicineOrg() {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_GB2312();

      string configFile = @"G:\GitHub\picker\data\organization\药品生产企业-人民网.xml";
      XElement xeRoot = XElement.Load( configFile );
      var dataGroup = xeRoot.Elements( "group" ).Where( xe => xe.Attribute( "name" ).Value == "data" ).FirstOrDefault();
      string urlPrefix = dataGroup.Attribute( "urlprefix" ).Value;
      var items = dataGroup.Elements( "item" );
      Console.WriteLine( "数据组中包含数据条数：" + items.Count().ToString() );

      foreach ( var item in items ) {
        // check tag
        if ( item.Attribute( strProcessed ) != null )
          continue;

        Console.WriteLine( "正在处理：" + item.Attribute( "name" ).Value );
        string url = urlPrefix + item.Attribute( "url" ).Value;
        string data = client.DownloadString( url );
        data = data.Replace( "<br>", "<br />" )
          .Replace( "align=center", "" )
          .Replace( "width=100%", "" )
          .Replace( "width=17%", "" )
          .Replace( "width=83%", "" );
        var children = parseOrg( data );
        if ( children.Count == 0 ) { // 遇到报错，先处理后面的
          Console.WriteLine( "  出现异常，跳过……" );
          continue;
        }
        item.Add( children );
        // set tag
        item.Add( new XAttribute( strProcessed, DateTime.Now.ToString( "yyyyMMddHHmmss" ) ) );
        // save
        xeRoot.Save( configFile, SaveOptions.None );
        Thread.Sleep( 3000 );
      }
    }

    static List<XElement> parseOrgCatalog( string html, string domain, string urlPrefix ) {
      List<XElement> result = new List<XElement>();
      int start = html.IndexOf( "liebiao" );
      if ( start < 0 )
        return result;

      start = html.IndexOf( "<ul>", start );
      int end = html.IndexOf( "</ul>", start );
      string data = html.Substring( start, end - start + 5 );
      
      XElement root = XElement.Parse( data );
      var lis = root.Elements( "li" );
      foreach ( var li in lis ) {
        XElement a = li.Element( "a" );
        if ( a == null )
          continue;

        string url = domain + a.Attribute( "href" ).Value;
        XElement item = new XElement( "item",
          new XAttribute( "name", a.Value ),
          new XAttribute( "url", url.Replace( urlPrefix, "" ) )
          );
        result.Add( item );
      }

      return result;
    }

    static List<XElement> parseOrg( string html ) {
      List<XElement> result = new List<XElement>();
      int start = html.IndexOf( "<table" );
      if ( start < 0 )
        return result;
      int end = html.IndexOf( "</table>", start );
      string data = html.Substring( start, end - start + 8 );

      XElement root = XElement.Parse( data );
      var rows = root.Elements( "tr" );
      if ( rows.Count() == 0 ) // hack -- 网页源码不统一
        rows = root.Element( "tbody" ).Elements( "tr" );

      foreach ( var row in rows ) {
        XElement td1 = (XElement)row.FirstNode;
        XElement td2 = (XElement)td1.NextNode;
        if ( td1.Attribute( "bgcolor" ) == null || td2.Attribute( "bgcolor" ) == null )
          continue;

        XElement item = new XElement( "item",
          new XAttribute( "key", td1.Value.Trim() ),
          new XAttribute( "value", td2.Value.Trim() )
          );
        result.Add( item );
      }

      return result;
    }

  }

}
