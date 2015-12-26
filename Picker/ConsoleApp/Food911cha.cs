using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;

namespace ConsoleApp {
  public static class Food911cha {
    //static Regex regex = new Regex( @"./w+.html" ); // ./asdfadf.html

    public static void PickItems() {
      string itemUrlPrefix = "http://yingyang.911cha.com/";
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      string configFile = @"G:\GitHub\picker\data\food\food.xml";
      XElement xeRoot = XElement.Load( configFile );
      var groups = xeRoot.Elements( "group" );
      Console.WriteLine( "配置文件中包含数据：" + groups.Count().ToString() );

      foreach ( var group in groups ) {
        Console.WriteLine( "处理中：" + group.Attribute( "name" ).Value );
        string data = client.DownloadString( group.Attribute( "url" ).Value );
        var children = parseItems( data, itemUrlPrefix );
        group.Add( children );
        xeRoot.Save( configFile, SaveOptions.None );
      }
      Console.WriteLine( "已完成！" );
      Console.ReadKey();
    }

    public static void PickItemsData() {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      string configFile = @"G:\GitHub\picker\data\food\food.xml";
      XElement xeRoot = XElement.Load( configFile );
      var groups = xeRoot.Elements( "group" );
      Console.WriteLine( "配置文件中包含数据组：" + groups.Count().ToString() );

      foreach ( var group in groups ) {
        var items = group.Elements( "item" );
        Console.WriteLine( "  该组包含数据：" + items.Count().ToString() );
        foreach ( var item in items ) {
          Console.WriteLine( "  处理中：" + item.Attribute( "name" ).Value );
          string data = client.DownloadString( item.Attribute( "url" ).Value );
          var children = parseFood( data );
          item.Add( children );
        }
        Console.WriteLine( "---------" );
        xeRoot.Save( configFile, SaveOptions.None );
      }
      Console.WriteLine( "已完成！" );
      Console.ReadKey();
    }

    static IEnumerable<XElement> parseItems( string html, string itemUrlPrefix ) {
      int start = html.IndexOf( "mcon f14" );
      start = html.IndexOf( "<ul class", start );
      int end = html.IndexOf( "</div>", start );
      string data = html.Substring( start, end - start );

      XElement xeRoot = XElement.Parse( data );
      var lis = xeRoot.Elements( "li" );
      foreach ( var li in lis ) {
        string name = li.Element( "a" ).Value;
        string url = li.Element( "a" ).Attribute( "href" ).Value;
        yield return new XElement( "item",
          new XAttribute( "name", name ),
          new XAttribute( "url", url.Replace("./", itemUrlPrefix ) )
          );
      }
    }

    static List<XElement> parseFood( string html ) {
      List<XElement> result = new List<XElement>();

      int start = html.IndexOf( "<table" );
      int end = html.IndexOf( "</table>", start );
      string data = html.Substring( start, end - start + 8 );

      XElement xeRoot = XElement.Parse( data );
      var rows = xeRoot.Elements( "tr" );
      // 每个row有3个th + td
      foreach ( var row in rows ) {
        var th1 = (XElement)row.FirstNode;
        var td1 = (XElement)th1.NextNode;
        result.Add( new XElement( "has",
          new XAttribute( "name", th1.Value ),
          new XAttribute( "count", getCount( td1.Value ) ),
          new XAttribute( "unit", td1.Element( "span" ).Value )
          ) );

        var th2 = (XElement)td1.NextNode;
        if ( th2 == null )
          continue;
        var td2 = (XElement)th2.NextNode;
        if ( td2 == null )
          continue;
        result.Add( new XElement( "has",
          new XAttribute( "name", th2.Value ),
          new XAttribute( "count", getCount( td2.Value ) ),
          new XAttribute( "unit", td2.Element( "span" ).Value )
          ) );

        var th3 = (XElement)td2.NextNode;
        if ( th3 == null )
          continue;
        var td3 = (XElement)th3.NextNode;
        if ( td3 == null )
          continue;
        result.Add( new XElement( "has",
          new XAttribute( "name", th3.Value ),
          new XAttribute( "count", getCount( td3.Value ) ),
          new XAttribute( "unit", td3.Element( "span" ).Value )
          ) );
      }
      return result;
    }

    static string getCount( string data ) {
      return data.Replace( "千卡", "" ).Replace( "微克", "" ).Replace( "毫克", "" ).Replace( "克", "" ).Trim();
    }

    public static void XmlToJson( string source, string target) {
      var root = XElement.Load( source );
      var groups = root.Elements( "group" );

      Newtonsoft.Json.Linq.JObject jsonObj = new Newtonsoft.Json.Linq.JObject();
      jsonObj["title"] = "";
      jsonObj["url"] = "http://yingyang.911cha.com/";

      Newtonsoft.Json.Linq.JArray categories = new Newtonsoft.Json.Linq.JArray();
      Newtonsoft.Json.Linq.JArray items = new Newtonsoft.Json.Linq.JArray();
      int id = 1;
      foreach ( XElement group in groups ) {
        Newtonsoft.Json.Linq.JObject category = new Newtonsoft.Json.Linq.JObject();
        category["id"] = id;
        category["name"] = group.Attribute( "name" ).Value;
        category["url"] = group.Attribute( "url" ).Value;
        categories.Add( category );

        var foods = group.Elements( "item" );
        foreach ( XElement food in foods ) {
          var hasItems = food.Elements( "has" );
          Newtonsoft.Json.Linq.JObject jsonFood = new Newtonsoft.Json.Linq.JObject();
          jsonFood["name"] = food.Attribute( "name" ).Value;
          jsonFood["url"] = food.Attribute( "url" ).Value;
          jsonFood["category"] = id;

          Newtonsoft.Json.Linq.JArray contains = new Newtonsoft.Json.Linq.JArray();
          foreach ( XElement hi in hasItems ) {
            Newtonsoft.Json.Linq.JObject jsonHas = new Newtonsoft.Json.Linq.JObject();
            jsonHas["name"] = hi.Attribute( "name" ).Value;
            jsonHas["quantity"] = float.Parse( hi.Attribute( "count" ).Value );
            jsonHas["unit"] = hi.Attribute( "unit" ).Value;
            contains.Add( jsonHas );
          }
          jsonFood["contains"] = contains;
          items.Add( jsonFood );
        }
        id++;
      }
      jsonObj["categories"] = categories;
      jsonObj["items"] = items;

      string json = jsonObj.ToString( Newtonsoft.Json.Formatting.Indented );
      System.IO.File.WriteAllText( target, json, System.Text.Encoding.UTF8 );
    }

    /// <summary>
    /// target: xxxx no ".json"
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public static void XmlToJson2( string source, string target ) {
      var root = XElement.Load( source );
      var groups = root.Elements( "group" );

      int id = 1;
      foreach ( XElement group in groups ) {
        Newtonsoft.Json.Linq.JObject jsonObj = new Newtonsoft.Json.Linq.JObject();
        jsonObj["title"] = "营养信息：" + group.Attribute( "name" ).Value;
        jsonObj["url"] = "http://yingyang.911cha.com/";

        Newtonsoft.Json.Linq.JObject category = new Newtonsoft.Json.Linq.JObject();
        category["id"] = id;
        category["name"] = group.Attribute( "name" ).Value;
        category["url"] = group.Attribute( "url" ).Value;
        jsonObj["category"] = category;

        Newtonsoft.Json.Linq.JArray items = new Newtonsoft.Json.Linq.JArray();
        var foods = group.Elements( "item" );
        foreach ( XElement food in foods ) {
          var hasItems = food.Elements( "has" );
          Newtonsoft.Json.Linq.JObject jsonFood = new Newtonsoft.Json.Linq.JObject();
          jsonFood["name"] = food.Attribute( "name" ).Value;
          jsonFood["url"] = food.Attribute( "url" ).Value;
          jsonFood["category"] = id;

          Newtonsoft.Json.Linq.JArray contains = new Newtonsoft.Json.Linq.JArray();
          foreach ( XElement hi in hasItems ) {
            Newtonsoft.Json.Linq.JObject jsonHas = new Newtonsoft.Json.Linq.JObject();
            jsonHas["name"] = hi.Attribute( "name" ).Value;
            jsonHas["quantity"] = float.Parse( hi.Attribute( "count" ).Value );
            jsonHas["unit"] = hi.Attribute( "unit" ).Value;
            contains.Add( jsonHas );
          }
          jsonFood["contains"] = contains;
          items.Add( jsonFood );
        }
        jsonObj["items"] = items;

        string json = jsonObj.ToString( Newtonsoft.Json.Formatting.Indented );
        System.IO.File.WriteAllText( target + id.ToString() + ".json", json, System.Text.Encoding.UTF8 );

        id++;
      } // groups

    }

  }

}
