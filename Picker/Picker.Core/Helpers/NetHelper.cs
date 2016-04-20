using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Picker.Core.Helpers {
  public static class NetHelper {
    public static WebClient GetWebClient_UTF8(){
      WebClient client = new WebClient();
      client.Encoding = System.Text.Encoding.UTF8;
      client.UseDefaultCredentials = true;
      client.Headers.Add( "User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36 Edge/12.0" );
      return client;
    }

    public static WebClient GetWebClient_GB2312() {
      WebClient client = new WebClient();
      client.Encoding = System.Text.Encoding.GetEncoding( "GB2312" );
      client.UseDefaultCredentials = true;
      client.Headers.Add( "User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36 Edge/12.0" );
      return client;
    }

    public static string HttpPost( this WebClient client, string url, string postString ) {
      byte[] postData = Encoding.UTF8.GetBytes( postString );

      if ( client.Headers["Content-Type"] == null ) // post
        client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" );

      byte[] responseData = client.UploadData( url, "POST", postData ); // 得到返回字符流
      string result = Encoding.UTF8.GetString( responseData ); // 解码
      return result;
    }

  }

}
