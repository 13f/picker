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
using Picker.Core.Models;

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

    public static void Grouping( string sourcePath ) {
      Console.WriteLine( "load source json..." );
      string json = System.IO.File.ReadAllText( sourcePath );
      JObject joRootSource = JObject.Parse( json );
      JArray itemsSource = (JArray)joRootSource["items"];

      // config
      JObject joConfig = createConfig();

      JObject jo1 = new JObject();
      jo1.Add( "title", "机场三字码（a-g）" );
      jo1.Add( "description", "a-g部分" );
      jo1.Add( "url", "http://airportcode.911cha.com/" );
      jo1.Add( "updated_at", "2016-7-2" );
      jo1.Add( "config", joConfig );

      JObject jo2 = new JObject();
      jo2.Add( "title", "机场三字码（h-n）" );
      jo2.Add( "description", "h-n部分" );
      jo2.Add( "url", "http://airportcode.911cha.com/" );
      jo2.Add( "updated_at", "2016-7-2" );
      jo2.Add( "config", joConfig );

      JObject jo3 = new JObject();
      jo3.Add( "title", "机场三字码（o-t）" );
      jo3.Add( "description", "o-t部分" );
      jo3.Add( "url", "http://airportcode.911cha.com/" );
      jo3.Add( "updated_at", "2016-7-2" );
      jo3.Add( "config", joConfig );

      JObject jo4 = new JObject();
      jo4.Add( "title", "机场三字码（u-z）" );
      jo4.Add( "description", "u-z部分" );
      jo4.Add( "url", "http://airportcode.911cha.com/" );
      jo4.Add( "updated_at", "2016-7-2" );
      jo4.Add( "config", joConfig );

      JArray items1 = new JArray();
      JArray items2 = new JArray();
      JArray items3 = new JArray();
      JArray items4 = new JArray();

      foreach(var item in itemsSource ) {
        string code = (string)item["code3"];
        if(string.IsNullOrWhiteSpace(code))
          code = (string)item["code4"];
        if ( string.IsNullOrWhiteSpace( code ) )
          code = (string)item["city_english_name"];
        if ( string.IsNullOrWhiteSpace( code ) )
          code = "abc";

        char c = code.ToLower()
          .First();
        if ( SimpleGrouping.EnglishLetters_4_Group1.Contains( c ) )
          items1.Add( item );
        else if ( SimpleGrouping.EnglishLetters_4_Group2.Contains( c ) )
          items2.Add( item );
        else if ( SimpleGrouping.EnglishLetters_4_Group3.Contains( c ) )
          items3.Add( item );
        else if ( SimpleGrouping.EnglishLetters_4_Group4.Contains( c ) )
          items4.Add( item );
      }

      jo1.Add( "total_count", items1.Count );
      jo1.Add( "items", items1 );

      jo2.Add( "total_count", items2.Count );
      jo2.Add( "items", items2 );

      jo3.Add( "total_count", items3.Count );
      jo3.Add( "items", items3 );

      jo4.Add( "total_count", items4.Count );
      jo4.Add( "items", items4 );

      // save
      Console.WriteLine( "save json..." );

      json = jo1.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( sourcePath.Replace( ".json", "-1.json" ), json, Encoding.UTF8 );

      json = jo2.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( sourcePath.Replace( ".json", "-2.json" ), json, Encoding.UTF8 );

      json = jo3.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( sourcePath.Replace( ".json", "-3.json" ), json, Encoding.UTF8 );

      json = jo4.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( sourcePath.Replace( ".json", "-4.json" ), json, Encoding.UTF8 );

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
