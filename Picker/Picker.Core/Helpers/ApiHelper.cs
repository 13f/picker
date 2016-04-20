using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picker.Core.Helpers {
  public static class ApiHelper {
    /// <summary>
    /// 返回最原始的API地址，即去掉URI的?及其后面的部分
    /// </summary>
    /// <param name="queryUri"></param>
    /// <returns></returns>
    public static string GetApi( string queryUri ) {
      int index = queryUri.IndexOf( "?" );
      if ( index < 0 )
        return queryUri;
      return queryUri.Substring( index );
    }

    public static int GetInt( string uri, string before, string after ) {
      int start = uri.IndexOf( before ) + before.Length;
      int end = uri.IndexOf( after );
      string idString = uri.Substring( start, end - start );
      int id = 0;
      Int32.TryParse( idString, out id );
      return id;
    }

  }

}
