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

  }

}
