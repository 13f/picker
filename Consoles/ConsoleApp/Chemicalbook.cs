using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Picker.Core.Helpers;

namespace ConsoleApp {
  /// <summary>
  /// chemicalbook.com
  /// </summary>
  public static class Chemicalbook {
    /// <summary>
    /// "http://www.chemicalbook.com"
    /// </summary>
    public const string Website = "http://www.chemicalbook.com";

    static void save( JObject jo, string path ) {
      string json = jo.ToString( Newtonsoft.Json.Formatting.Indented );
      File.WriteAllText( path, json );
    }

    public static async Task PickCategories( string path ) {
      string url = "http://www.chemicalbook.com/ChemicalProductsList_0.htm";

      JObject root = new JObject();
      root["title"] = "化工产品目录";
      root["url"] = url;

      JArray items = new JArray();
      pickCategories( items, url );
      root["items"] = items;

      // save
      save( root, path );
      Console.WriteLine( "saved... over..." );
    }

    static void pickCategories( JArray items, string url ) {
      WebClient client = NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();
      string html = client.DownloadString( url );
      doc.LoadHtml( html );
      Console.WriteLine( "  got html: " + url );

      var tabsort = doc.DocumentNode.SelectSingleNode( "//div[@id='tabsort']" );
      if ( tabsort == null )
        return;

      var tmpDiv = tabsort.SelectSingleNode( "./div" );
      if ( tmpDiv == null )
        return;

      var ul = tmpDiv.SelectSingleNode( "./ul" );
      if ( ul == null )
        return;

      var lis = ul.SelectNodes( "./li" );
      if ( lis == null || lis.Count == 0 )
        return;

      foreach(var li in lis ) {
        JObject item = new JObject();

        var a = li.SelectSingleNode( "./a" );
        item["title"] = a.InnerText;

        string newUrl = Website + a.Attributes["href"].Value;
        if ( newUrl == url ) // http://www.chemicalbook.com/ChemicalProductsList_530.htm
          break;

        item["url"] = newUrl;

        JArray newItems = new JArray();
        pickCategories( newItems, newUrl );
        item["items"] = newItems;

        items.Add( item );
      }
      Console.WriteLine( "  √ " + url );
    }

  }

}
