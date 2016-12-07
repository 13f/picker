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
  public static class ChineseCharacter {

    /// <summary>
    /// 按笔画分共15类：http://www.hanzizidian.com/bs.html
    /// </summary>
    public static async Task PickRadicals(string path) {
      WebClient client = NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();

      string url = "http://www.hanzizidian.com/bs.html";
      JObject root = new JObject();
      root["url"] = url;

      JArray items = new JArray();
      int count = 0;

      string html = await client.DownloadStringTaskAsync( url );
      doc.LoadHtml( html );

      var groups = doc.DocumentNode.SelectNodes( "//ul[@class='bs_list']" );
      int groupCount = 0;
      foreach (var group in groups) {
        groupCount++;
        var nodesInGroup = group.SelectNodes( "./li" );
        foreach(var node in nodesInGroup) {
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


    static JObject parseRadical(HtmlNode node, int group) {
      var a = node.SelectSingleNode( "./a" );

      JObject item = new JObject();
      item["radical"] = a.InnerText;
      item["hanzizidian_id"] = a.Attributes["href"].Value;
      item["number_of_strokes"] = group;
      return item;
    }


  }

}
