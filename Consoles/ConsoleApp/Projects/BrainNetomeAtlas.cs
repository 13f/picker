using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp {
  /// <summary>
  /// http://atlas.brainnetome.org/
  /// </summary>
  public static  class BrainNetomeAtlas {
    const string ImageUri_Connectogram = "http://atlas.brainnetome.org/images/connectogram/{0}.svg";
    const string ImageUri_Probabilities = "http://atlas.brainnetome.org/images/probabilities/{0}-{1}.jpg";

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
