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
      return client;
    }

    public static WebClient GetWebClient_GB2312() {
      WebClient client = new WebClient();
      client.Encoding = System.Text.Encoding.GetEncoding( "GB2312" );
      client.UseDefaultCredentials = true;
      return client;
    }

  }

}
