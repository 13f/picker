using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Picker.Core.Helpers;

namespace ConsoleApp {
  public static class GallantlabHuth2016 {
    /// <summary>
    /// 0: l, r. 1: int value.
    /// </summary>
    const string UriTemplate_AreaData = "http://gallantlab.org/huth2016/pragmatic/area-{0}-{1}.json";

    public static void FilterDistinctData( string pathSource, string pathResult ) {
      Console.Write( "==> read ... " );
      string jsonSource = File.ReadAllText( pathSource );
      JObject source = JObject.Parse( jsonSource );
      Console.WriteLine( "done." );

      JObject result = new JObject();

      Console.WriteLine( "  -> left...." );
      filterArray( source, result, "left" );

      Console.WriteLine( "  -> right...." );
      filterArray( source, result, "right" );

      string j = result.ToString( Newtonsoft.Json.Formatting.Indented );
      File.WriteAllText( pathResult, j );

      Console.WriteLine( "over...." );
    }

    public static async Task PickAreasData( string path, string destDirectory ) {
      Console.WriteLine( "==> read areas ... " );
      string source = File.ReadAllText( path );
      JObject joSource = JObject.Parse( source );

      Console.WriteLine( "  ==> left...." );
      await processArray( joSource, "left", "l", destDirectory );

      Console.WriteLine( "  ==> right...." );
      await processArray( joSource, "right", "r", destDirectory );

      Console.WriteLine( "over...." );
    }

    public static void MergeParts( string sourceDirectory, string destDirectory ) {
      Console.WriteLine( "==> merge areas in LEFT ... " );
      mergePartFiles( sourceDirectory, destDirectory, "left" );

      Console.WriteLine( "==> merge areas in RIGHT ... " );
      mergePartFiles( sourceDirectory, destDirectory, "right" );

      Console.WriteLine( "over...." );
    }


    private static void filterArray( JObject source, JObject result, string key ) {
      List<int> dataLeft = new List<int>();
      JArray array = (JArray)source[key];
      foreach ( var item in array ) {
        float f = item.Value<float>();
        int v = (int)f;
        if ( !dataLeft.Contains( v ) )
          dataLeft.Add( v );
      }
      JArray leftNew = new JArray();
      dataLeft.ForEach( i => leftNew.Add( i ) );
      result[key] = leftNew;
    }

    private static async Task processArray( JObject source, string key, string keyShort, string destDirectory ) {
      JArray array = (JArray)source[key];
      WebClient client = NetHelper.GetWebClient_UTF8();
      foreach ( var item in array ) {
        int v = item.Value<int>();
        string filename = "area-" + keyShort + "-" + v.ToString() + ".json";
        Console.Write( "    ==> " + filename );
        string uri = string.Format( UriTemplate_AreaData, keyShort, v );
        await client.DownloadFileTaskAsync( uri, destDirectory + key + "\\" + filename );
        Console.WriteLine( " ... √" );
      }
    }

    private static void mergePartFiles( string sourceDirectory, string destDirectory, string key ) {
      JObject jo = new JObject();
      jo["title"] = key;
      jo["description"] = "The PrAGMATiC atlas divides the left hemisphere into 192 distinct functional areas, 77 of which are semantically selective. The right hemisphere is divided into 128 functional areas, 63 of which are semantically selective.";
      jo["url"] = "http://gallantlab.org/huth2016/";

      var files = Directory.EnumerateFiles( sourceDirectory + key + "\\" );
      JArray items = new JArray();
      foreach(var file in files ) {
        string content = File.ReadAllText( file );
        JObject token = JObject.Parse( content );
        string filename = Path.GetFileName( file );

        JObject joItem = new JObject();
        joItem["url"] = "http://gallantlab.org/huth2016/pragmatic/" + filename;
        joItem["file"] = filename;
        joItem["object"] = token;
        items.Add( joItem );
      }
      jo["items"] = items;

      string json = jo.ToString( Newtonsoft.Json.Formatting.Indented );
      File.WriteAllText( destDirectory + key + "-data.json", json );
      Console.WriteLine( "  ... √" );
    }

  }

}
