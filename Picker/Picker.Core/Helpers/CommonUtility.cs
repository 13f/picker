using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Picker.Core.Helpers {
  public class CommonUtility {

    /// <summary>
    /// 创建一个Guid字符串，去掉了所有符号，只剩32位数字或小写字母，如bea8b23f69574b0c8832b5723c3aae71
    /// </summary>
    /// <returns></returns>
    public static string NewGuid_PlainLower() {
      return Guid.NewGuid().ToString().Replace( "-", "" ).ToLower();
    }

    /// <summary>
    /// 两个字符串都使用splitWord进行分隔，合并分隔的内容，并去掉相同项。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="splitWord"></param>
    /// <returns></returns>
    public static string MergeStrings( string a, string b, string splitWord ) {
      if ( a == null )
        return b;
      else if ( b == null )
        return a;
      else if ( string.IsNullOrWhiteSpace( splitWord ) )
        throw new Exception( "splitWord is null/empty." );
      string[] splitTmp = new string[1] { splitWord.Trim() };
      string[] aTmp = a.Split( splitTmp, StringSplitOptions.RemoveEmptyEntries );
      string[] bTmp = b.Split( splitTmp, StringSplitOptions.RemoveEmptyEntries );
      string[] tmp = aTmp.Union( bTmp ).Distinct().ToArray();
      StringBuilder sb = new StringBuilder();
      foreach ( string str in tmp )
        sb.Append( str.Trim() + splitWord );
      sb = sb.Remove( sb.Length - splitWord.Length, splitWord.Length );
      return sb.ToString();
    }
    
    /// <summary>
    /// return source.Replace( "；", ";" )
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ReplaceSemicolon( string source ) {
      if ( string.IsNullOrWhiteSpace( source ) )
        return null;
      return source.Replace( "；", ";" );
    }
    
  }

}
