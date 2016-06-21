using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ConsoleApp {
  /// <summary>
  /// http://atlas.brainnetome.org/
  /// </summary>
  public static  class BrainNetomeAtlas {
    const string ImageUri_Connectogram = "http://atlas.brainnetome.org/images/connectogram/{0}.svg";
    const string ImageUri_Probabilities = "http://atlas.brainnetome.org/images/probabilities/{0}-{1}.jpg";

    public static void CreateJsonFile_SVG_Connectogram( string path ) {
      string prefix = "/images/connectogram/";
      JArray items = new JArray();

      Console.WriteLine( "ready..." );
      for ( int i = 0; i <= 245; i++ ) {
        string key = ( i + 1 ).ToString();
        key = key.PadLeft( 3, '0' );
        //string filename = prefix + key + ".svg";

        JObject jo = new JObject();
        jo["ind_1"] = i + 1;
        jo["src"] = prefix + key + ".svg";
        items.Add( jo );
      }
      Console.WriteLine( "saving..." );
      string json = items.ToString( Newtonsoft.Json.Formatting.Indented );
      File.WriteAllText( path, json );

      Console.WriteLine( "over..." );
    }

    public static void ProcessDataCenterFile( string pathCenter, string pathInd, string pathResult ) {
      string title2center = File.ReadAllText( pathCenter );
      JArray centerItems = JArray.Parse( title2center );

      string title2ind = File.ReadAllText( pathInd );
      JArray indItems = JArray.Parse( title2ind );

      JArray items = new JArray();
      for(int i = 0; i<= 245; i++ ) {
        var jtCenter = centerItems[i];
        var jtInd = indItems[i];

        JObject jo = new JObject();
        jo["title"] = jtCenter["title"];
        jo["center"] = jtCenter["center"];
        jo["ind"] = jtInd["ind"];
        items.Add( jo );
      }
      Console.WriteLine( "saving..." );
      string json = items.ToString( Newtonsoft.Json.Formatting.Indented );
      File.WriteAllText( pathResult, json );

      Console.WriteLine( "over..." );
    }

    public static async Task GetImages_Connectogram( string directory, int millisecondsDelay = 2500 ) {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      Console.WriteLine( "images/connectogram" );
      for(int i = 0; i <= 245; i++ ) {
        string key = ( i + 1 ).ToString();
        key = key.PadLeft( 3, '0' );
        string uri = string.Format( ImageUri_Connectogram, key );
        string path = directory + key + ".svg";
        Console.WriteLine( "  --> " + key + ".svg" );
        await client.DownloadFileTaskAsync( uri, path );
        // wait
        Console.WriteLine( " ...wait and continue..." );
        await Task.Delay( millisecondsDelay );
      }
      Console.WriteLine( "over..." );
    }

    public static async Task GetImages_Probabilities( string directory, int millisecondsDelay = 2500 ) {
      string prefixFun = "fun";
      string prefixDen = "den";
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      Console.WriteLine( "images/probabilities" );
      for ( int i = 0; i <= 245; i++ ) {
        string key = ( i + 1 ).ToString();
        key = key.PadLeft( 3, '0' );
        // fun
        string uri = string.Format( ImageUri_Probabilities, prefixFun, key );
        string path = directory + prefixFun + "-" + key + ".jpg";
        Console.WriteLine( "  --> " + prefixFun + "-" + key + ".jpg" );
        await client.DownloadFileTaskAsync( uri, path );
        // den
        uri = string.Format( ImageUri_Probabilities, prefixDen, key );
        path = directory + prefixDen + "-" + key + ".jpg";
        Console.WriteLine( "  --> " + prefixDen + "-" + key + ".jpg" );
        await client.DownloadFileTaskAsync( uri, path );
        // wait
        Console.WriteLine( " ...wait and continue..." );
        await Task.Delay( millisecondsDelay );
      }
      Console.WriteLine( "over..." );
    }

  }

}
