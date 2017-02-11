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
using Picker.Core.Extensions;

namespace ConsoleApp {
  // http://www.hanzizidian.com/
  // http://zidian.911cha.com/bushou.html

  public static class ChineseCharacter {
    public const string UrlPrefix = "http://www.hanzizidian.com/";

    /// <summary>
    /// 按笔画分共15类：http://www.hanzizidian.com/bs.html
    /// </summary>
    public static async Task PickRadicals( string path ) {
      WebClient client = NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();

      string url = UrlPrefix + "bs.html";
      JObject root = new JObject();
      root["url"] = url;

      JArray items = new JArray();
      int count = 0;

      string html = await client.DownloadStringTaskAsync( url );
      doc.LoadHtml( html );

      var groups = doc.DocumentNode.SelectNodes( "//ul[@class='bs_list']" );
      int groupCount = 0;
      foreach( var group in groups ) {
        groupCount++;
        var nodesInGroup = group.SelectNodes( "./li" );
        foreach( var node in nodesInGroup ) {
          JObject item = parseRadical( node, groupCount );
          items.Add( item );
          count++;
        }
      }
      root["items"] = items;
      root["count"] = count;
      // save
      LocalStorageUtility.Save( root, path );
      Console.WriteLine( "saved...over..." );
    }

    public static async Task PickWordsList( string path ) {
      // load
      string json = File.ReadAllText( path );
      JObject root = JObject.Parse( json );
      JArray items = (JArray)root["items"];

      foreach( var item in items ) {
        if( item["words"] != null )
          continue;
        await parseWordsList( item );
      }
      root["items"] = items;
      // save
      LocalStorageUtility.Save( root, path );
      Console.WriteLine( "saved...over..." );
    }

    public static void SeparateWords( string pathRadicals, string pathWords ) {
      // load
      string json = File.ReadAllText( pathRadicals );
      JObject rootRadicals = JObject.Parse( json );
      JArray radicals = (JArray)rootRadicals["items"];

      JObject rootWords = new JObject();
      rootWords["url"] = UrlPrefix;

      JArray words = new JArray();
      int count = 0;

      foreach( var radical in radicals ) {
        var tmpItems = (JArray)radical["words"];
        foreach( var w in tmpItems )
          words.Add( w );
        count += tmpItems.Count;
      }
      rootWords["items"] = words;
      rootWords["count"] = count;
      // save
      LocalStorageUtility.Save( rootWords, pathWords );
      Console.WriteLine( "saved...over..." );
    }

    // TODO
    public static async Task PickWordsDetail( string path ) {
      // load
      string json = File.ReadAllText( path );
      JObject root = JObject.Parse( json );
      JArray items = (JArray)root["items"];

      foreach( var item in items ) {
        if( item["id"] != null )
          continue;
        await parseWordDetail( item );

        //root["items"] = items;
        // save
        LocalStorageUtility.Save( root, path );
        Console.WriteLine( "saved...over..." );
      }
    }


    static JObject parseRadical( HtmlNode node, int group ) {
      var a = node.SelectSingleNode( "./a" );

      JObject item = new JObject();
      item["radical"] = a.InnerText;
      item["hanzizidian_id"] = a.Attributes["href"].Value;
      item["number_of_strokes"] = group;
      return item;
    }

    static async Task parseWordsList( JToken radical ) {
      string radicalId = (string)radical["hanzizidian_id"];
      string url = UrlPrefix + radicalId;
      Console.WriteLine( "-> " + radicalId );

      WebClient client = NetHelper.GetWebClient_UTF8();
      string html = await client.DownloadStringTaskAsync( url );

      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );

      var groups = doc.DocumentNode.SelectNodes( "//div[@class='bs']" );
      int count = 0;
      JArray items = new JArray();
      foreach( var group in groups ) {
        count++;
        var nodesInGroup = group.SelectNodes( "./li" );
        foreach( var node in nodesInGroup ) {
          var a = node.SelectSingleNode( "./a" );
          string id = a.Attributes["href"].Value;
          if( id == radicalId )
            continue;

          JObject item = new JObject();
          item["hanzizidian_id"] = id;
          items.Add( item );
          count++;
        }
      }
      radical["words"] = items;
      radical["count"] = count;
    }

    static async Task parseWordDetail( JToken word ) {
      string id = (string)word["hanzizidian_id"];
      // generate chuci id
      string chuciId = CommonUtility.NewGuid_PlainLower();
      Console.WriteLine( "-> " + id + " >>> " + chuciId );
      word["chuci_id"] = chuciId;

      WebClient client = NetHelper.GetWebClient_UTF8();
      string url = UrlPrefix + id;
      string html = await client.DownloadStringTaskAsync( url );

      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );

      var div = doc.DocumentNode.SelectSingleNode( "//div[@id='main_content']" );
      var table = div.SelectSingleNode( "./table" );
      var pInTable = table.SelectSingleNode( "./tr" )
        .SelectNodes( "./td" )
        .Skip( 1 )
        .FirstOrDefault()
        .SelectSingleNode( "./p" );
      processZhsZht( word, pInTable );

      var pBasic = table.NextSibling;
      processBasic( word, pBasic );

      //JArray items = new JArray();
      //foreach (var group in groups) {
      //  count++;
      //  var nodesInGroup = group.SelectNodes("./li");
      //  foreach (var node in nodesInGroup) {
      //    var a = node.SelectSingleNode("./a");
      //    string id = a.Attributes["href"].Value;
      //    if (id == radicalId)
      //      continue;

      //    JObject item = new JObject();
      //    item["hanzizidian_id"] = id;
      //    items.Add(item);
      //    count++;
      //  }
      //}

    }

    /// <summary>
    /// 获取简体字、繁体字
    /// </summary>
    /// <param name="word"></param>
    /// <param name="hnP"></param>
    static void processZhsZht( JToken word, HtmlNode hnP ) {
      string innerTxt = hnP.InnerText;
    }

    static void processBasic(JToken word, HtmlNode p ) {

    }

  }

}
