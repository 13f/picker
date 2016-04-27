using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp {
  public static class EmojiCheatSheet {
    const string website = "http://www.emoji-cheat-sheet.com/";
    static List<string> images = new List<string>();

    public static async Task Run( string jsonDir, string imagesDir ) {
      string jsonFilename = "emoji-cheat-sheet.json";

      Console.WriteLine( "网站：www.emoji-cheat-sheet.com" );
      Console.WriteLine( "项目：github.com/arvida/emoji-cheat-sheet.com" );

      Console.WriteLine( "get html..." );
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      string html = await client.DownloadStringTaskAsync( website );
      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );

      JObject joRoot = new JObject();
      joRoot.Add( "title", "Emoji cheat sheet" );
      joRoot.Add( "description", "" );
      joRoot.Add( "url", website );
      joRoot.Add( "project", "https://github.com/arvida/emoji-cheat-sheet.com" );

      JArray groups = new JArray();
      images.Clear();

      Console.WriteLine( "parse html..." );
      var ullist = doc.DocumentNode.SelectNodes( "//ul" );
      foreach(var ul in ullist) {
        JObject joGroup = getGroupData( ul );
        groups.Add( joGroup );
      }
      joRoot.Add( "data", groups );

      Console.WriteLine( "save json..." );
      string json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( jsonDir + jsonFilename, json, Encoding.UTF8 );

      Console.WriteLine( "download images..." );
      int total = images.Count;
      int count = 0;
      foreach ( string imageUri in images ) {
        int index = imageUri.LastIndexOf( "/" );
        string imageFile = imageUri.Substring( index + 1 );
        // 1
        //var bytes = await client.DownloadDataTaskAsync( url );
        //System.IO.File.WriteAllBytes( directory + filename, bytes );
        // 2
        await client.DownloadFileTaskAsync( imageUri, imagesDir + imageFile );
        count++;
        Console.WriteLine( count.ToString() + "/" + total.ToString() );
      }

      Console.WriteLine( "over..." );
    }

    public static JObject getGroupData( HtmlNode ul ) {
      JObject joGroup = new JObject();
      try {
        string name = ul.Attributes["class"].Value;
        int index = name.IndexOf( " " );
        name = name.Substring( 0, index );

        Console.WriteLine( "Group: " + name );
        joGroup.Add( "name", name );

        var lilist = ul.SelectNodes( "./li" );
        JArray items = new JArray();
        foreach ( var li in lilist ) {
          JObject item = getItem( li );
          items.Add( item );
        }
        joGroup.Add( "items", items );
      }
      catch (Exception ex ) {
      }

      return joGroup;
    }

    public static JObject getItem( HtmlNode li ) {
      JObject item = new JObject();
      try {
        var div = li.SelectSingleNode( "./div" );
        string shortName = div.InnerText.Trim();

        var spanEmoji = div.SelectSingleNode( "./span[@class='emoji']" );
        string image = spanEmoji.Attributes["data-src"].Value;
        image = website + image;
        if ( !images.Contains( image ) )
          images.Add( image );

        var spanName = div.SelectSingleNode( "./span[@class='name']" );
        string names = spanName == null ? null : spanName.Attributes["data-alternative-name"].Value;

        item.Add( "short-name", shortName );
        item.Add( "alternative-names", names );
        item.Add( "image", image );
      }
      catch ( Exception ex ) {
      }

      return item;
    }

  }

}
