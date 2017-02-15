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
  /// 
  /// </summary>
  public static class Facemood {
    /// <summary>
    /// 每页最多20条
    /// </summary>
    const int CountPerPage = 20;
    
    public static async Task PickList( string directory ) {
      string uri = "http://facemood.grtimed.com";
      WebClient client = NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();

      string html = await client.DownloadStringTaskAsync( uri );
      doc.LoadHtml( html );
      Console.WriteLine( "got html." );

      JObject root = new JObject();
      root["title"] = "顏文字";
      root["description"] = "";
      root["uri"] = uri;

      JArray tags = new JArray();
      var alist = doc.DocumentNode
          .SelectSingleNode( "//div[@class='tagMenu']" )
          .SelectNodes( "./a" );
      foreach ( var a in alist ) {
        string href = a.Attributes["href"].Value;
        var div = a.SelectSingleNode( "./div" );
        string name = div.InnerText
          .Trim( '\r', '\n', '\t' )
          .Trim();
        JObject joTag = new JObject();
        joTag["name"] = name;
        joTag["url"] = uri + "/" + href;
        tags.Add( joTag );
      }
      root["tags"] = tags;

      //var ids = new int[] { 1, 2, 3, 4, 5, 6, 9, 10, 11, 12, 13, 14, 15 };
      JArray category_list = new JArray();
      alist = doc.DocumentNode
          .SelectSingleNode( "//div[@class='cateMenu']" )
          .SelectNodes( "./a" );
      foreach ( var a in alist ) {
        string href = a.Attributes["href"].Value;
        var div = a.SelectSingleNode( "./div" )
          .SelectSingleNode( "./div[@class='cateItemTitle']" );
        string name = div.InnerText
          .Trim( '\r', '\n', '\t' )
          .Trim();
        JObject joc = new JObject();
        joc["name"] = name;
        joc["url"] = uri + "/" + href;
        category_list.Add( joc );
      }
      root["category_list"] = category_list;

      // save
      LocalStorageUtility.Save( root, directory + "facemood.json" );
      Console.WriteLine( "saved...over..." );
    }

    public static async Task PickCategoryDetails( string pathSource, string pathResult, int millisecondsDelay = 2500 ) {
      Console.WriteLine( "load data file..." );
      // load
      string json = File.ReadAllText( pathSource );
      JObject root = JObject.Parse( json );

      JArray items = (JArray)root["category_list"];
      int totalItems = 0;
      foreach(var item in items ) {
        if ( item["updatedAt"] != null ) // skip
          continue;

        int count = await pickCategoryDetail( item, millisecondsDelay );
        totalItems += count;
        // update tag
        item["updatedAt"] = DateTime.UtcNow;
        // save
        LocalStorageUtility.Save( root, pathResult );
        // wait
        Console.WriteLine( "... wait... and then continue..." );
        await Task.Delay( millisecondsDelay );
      }

      root["total_items_count_by_category"] = totalItems;
      // save
      LocalStorageUtility.Save( root, pathResult );
      Console.WriteLine( "saved...over..." );
    }

    public static async Task PickTagDetails( string pathSource, string pathResult, int millisecondsDelay = 2500 ) {
      Console.WriteLine( "load data file..." );
      // load
      string json = File.ReadAllText( pathSource );
      JObject root = JObject.Parse( json );

      JArray items = (JArray)root["tags"];
      int totalItems = 0;
      foreach( var item in items ) {
        if( item["updatedAt"] != null ) // skip
          continue;

        int count = await pickCategoryDetail( item, millisecondsDelay );
        totalItems += count;
        // update tag
        item["updatedAt"] = DateTime.UtcNow;
        // save
        LocalStorageUtility.Save( root, pathResult );
        // wait
        Console.WriteLine( "... wait... and then continue..." );
        await Task.Delay( millisecondsDelay );
      }

      root["total_items_count_by_tag"] = totalItems;
      // save
      LocalStorageUtility.Save( root, pathResult );
      Console.WriteLine( "saved...over..." );
    }

    public static async Task GetDistinctItems( string pathSource, string pathResult ) {
      Console.WriteLine( "load data file..." );
      // load
      string json = File.ReadAllText( pathSource );
      JObject root = JObject.Parse( json );

      JArray tags = (JArray)root["tags"];
      JArray category_list = (JArray)root["category_list"];
      List<JToken> tmpList = new List<JToken>();
      tmpList.AddRange( tags );
      tmpList.AddRange( category_list );

      Dictionary<string, JToken> items = new Dictionary<string, JToken>();
      foreach( var item in tmpList ) {
        var itemsInItem = item["items"];
        foreach(var iii in itemsInItem ) {
          string mood_text = (string)iii["mood_text"];
          if( !items.ContainsKey( mood_text ) )
            items.Add( mood_text, iii );
        }
      }
      root.Remove( "tags" );
      root.Remove( "category_list" );
      root.Remove( "total_items_count_by_category" );
      root.Remove( "total_items_count_by_tag" );

      JArray array = new JArray();
      items.Values
        .ToList()
        .ForEach( i => array.Add( i ) );
      root["items"] = array;
      root["count"] = items.Count;
      // save
      LocalStorageUtility.Save( root, pathResult );
      Console.WriteLine( "saved...over..." );
    }


    static async Task<int> pickCategoryDetail( JToken category, int millisecondsDelay = 2500 ) {
      if ( category == null )
        return 0;
      
      WebClient client = NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();

      string url = (string)category["url"];
      Console.WriteLine( "  get " + url );

      JArray items = new JArray();
      int page = 1;
      while ( true ) {
        Console.WriteLine( "  > page: " + page.ToString() );
        string uri = url + "&page=" + page.ToString();
        string html = await client.DownloadStringTaskAsync( uri );
        doc.LoadHtml( html );
        Console.WriteLine( "  > got html." );

        var tmp = parseCategoryDetail( doc.DocumentNode );
        tmp.ForEach( i => items.Add( i ) );
        if ( tmp.Count < CountPerPage )
          break;
        // wait
        Console.WriteLine( "  > wait... and then continue..." );
        await Task.Delay( millisecondsDelay );
        page++;
      }
      category["items"] = items;
      category["count"] = items.Count;
      return items.Count;
    }

    static List<JObject> parseCategoryDetail(HtmlNode root ) {
      List<JObject> result = new List<JObject>();
      var divs = root.SelectNodes( "//div[@class='facemoodItem']" );
      if(divs!= null ) {
        foreach(var div in divs ) {
          JObject item = new JObject();
          var facemoodItemText = div.SelectSingleNode( "./div[@class='facemoodItemText facemoodItemText-js']" );
          string moodText = facemoodItemText.Attributes["data-f-text"].Value;
          item["mood_text"] = moodText;

          var aList = div.SelectSingleNode( "./div[@class='facemoodItemBar']" )
            .SelectSingleNode( "./div[@class='faceMatchTagBox']" )
            .SelectSingleNode( "./div[@class='faceMatchTagListBox hide']" )
            .SelectNodes( "./a" );
          if( aList != null ) {
            JArray tags = new JArray();
            foreach(var a in aList ) {
              JObject tag = new JObject();
              string href = a.Attributes["href"].Value;
              tag["url"] = "http://facemood.grtimed.com/" + href;

              var tmpDiv = a.SelectSingleNode( "./div" );
              if(tmpDiv != null ) {
                tag["tag"] = tmpDiv.InnerText
                  .Trim('\r', '\n', '\t')
                  .Trim();
              }
              tags.Add( tag );
            }
            item["tags"] = tags;
          }
          result.Add( item );
        }
      }
      return result;
    }
    
  }

}
