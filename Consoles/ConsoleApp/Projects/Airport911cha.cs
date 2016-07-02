using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Picker.Core.Extensions;

namespace ConsoleApp {
  /// <summary>
  /// http://airportcode.911cha.com
  /// </summary>
  public static class Airport911cha {

    const string UriTemplate_Page = "http://airportcode.911cha.com/list_{0}.html";

    public static async Task Run( string filepath, int millisecondsDelay = 2000 ) {
     // int total = 8491;
      int countPerPage = 30;

      JObject joRoot = new JObject();
      joRoot.Add( "title", "机场三字码" );
      //joRoot.Add( "description", "" );
      joRoot.Add( "url", "http://airportcode.911cha.com/" );
      joRoot.Add( "updated_at", "2016-7-2" );
      // config
      JObject joConfig = createConfig();
      joRoot.Add( "config", joConfig );

      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      int page = 1;
      int count = 0;
      JArray items = new JArray();
      while ( true ) {
        Console.WriteLine( "get page: " + page.ToString() );
        string uri = string.Format( UriTemplate_Page, page );
        string html = await client.DownloadStringTaskAsync( uri );
        var tmp = parsePage( html );
        foreach ( var it in tmp )
          items.Add( it );
        count += tmp.Count;
        if ( tmp.Count < countPerPage )
          break;

        Console.WriteLine( "  wait and continue..." );
        await Task.Delay( millisecondsDelay );
        page++;
      }
      joRoot.Add( "total_count", count );
      joRoot.Add( "items", items );
      Console.WriteLine( "got all " + count + " items..." );

      // save
      Console.WriteLine( "save json..." );
      string json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    static List<JObject> parsePage(string html ) {
      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );

      List<JObject> result = new List<JObject>();
      var rows = doc.DocumentNode.SelectSingleNode( "//table" )
        .SelectSingleNode( "./tbody" )
        .SelectNodes( "./tr" );
      foreach(var row in rows ) {
        JObject jo = new JObject();
        var cols = row.SelectNodes( "./td" );
        int index = 1;
        foreach(var col in cols ) {
          string v = col.InnerText
            .Replace( "&nbsp;", "" )
            .Trim();
          if ( string.IsNullOrWhiteSpace( v ) )
            v = null;

          if (index == 1 ) {
            jo["city_chinese_name"] = v;
          }
          else if ( index == 2 ) {
            jo["code3"] = v;
          }
          else if ( index == 3 ) {
            jo["code4"] = v;
          }
          else if ( index == 4 ) {
            jo["airport_name"] = v;
          }
          else if ( index == 5 ) {
            jo["city_english_name"] = v;
          }
          index++;
        }
        result.Add( jo );
      }
      return result;
    }

    static JObject createConfig() {
      JObject joConfig = new JObject();
      joConfig.Add( "city_chinese_name", "中文名称" );
      joConfig.Add( "city_english_name", "英文名称" );
      joConfig.Add( "airport_name", "机场名称" );
      joConfig.Add( "code3", "机场三字码" );
      joConfig.Add( "code4", "机场四字码" );
      return joConfig;
    }

  }

}
