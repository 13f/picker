using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Picker.Core.Helpers {
  public static class NetHelper {
    // Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36 Edge/12.0
    // Opera: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.89 Safari/537.36 OPR/49.0.2725.47 Edge/41.16299.15.0
    // IE11: Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko
    const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.89 Safari/537.36 OPR/49.0.2725.47 Edge/41.16299.15.0";

    public static WebClient GetWebClient_DefaultEncoding() {
      WebClient client = new WebClient();
      client.Encoding = System.Text.Encoding.Default;
      client.UseDefaultCredentials = true;

      client.Headers.Add( "User-Agent", UserAgent );
      return client;
    }

    public static WebClient GetWebClient_UTF8() {
      WebClient client = new WebClient();
      client.Encoding = System.Text.Encoding.UTF8;
      client.UseDefaultCredentials = true;
      client.Headers.Add( "User-Agent", UserAgent );
      return client;
    }

    public static WebClient GetWebClient_GB2312() {
      WebClient client = new WebClient();
      client.Encoding = System.Text.Encoding.GetEncoding( "GB2312" );
      client.UseDefaultCredentials = true;
      client.Headers.Add( "User-Agent", UserAgent );
      return client;
    }

    public static WebClient GetWebClient_GBK() {
      WebClient client = new WebClient();
      client.Encoding = System.Text.Encoding.GetEncoding( "GBK" );
      client.UseDefaultCredentials = true;

      client.Headers.Add( "User-Agent", UserAgent );
      return client;
    }

    public static string HttpPost(this WebClient client, string url, string postString) {
      byte[] postData = Encoding.UTF8.GetBytes( postString );

      if (client.Headers["Content-Type"] == null) // post
        client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" );

      byte[] responseData = client.UploadData( url, "POST", postData ); // 得到返回字符流
      string result = Encoding.UTF8.GetString( responseData ); // 解码
      return result;
    }

    public static async Task<string> HttpPostTaskAsync(this WebClient client, string url, string postString) {
      byte[] postData = Encoding.UTF8.GetBytes( postString );

      if (client.Headers["Content-Type"] == null) // post
        client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" );

      byte[] responseData = await client.UploadDataTaskAsync( url, "POST", postData ); // 得到返回字符流
      string result = Encoding.UTF8.GetString( responseData ); // 解码
      return result;
    }

  }

}
