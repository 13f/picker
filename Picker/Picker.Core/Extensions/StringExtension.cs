using System;
using System.Collections.Generic;
using System.Text;

namespace Picker.Core.Extensions {
  public static class StringExtension {

    /// <summary>
    /// 计算字符串的字节长度（UTF-8编码）
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static int GetSizeInBytes( this string source ) {
      return ASCIIEncoding.UTF8.GetByteCount( source );
    }

    /// <summary>
    /// 两个字符串都是用同一种字符进行分割的词组集，对包含的词组进行无序比较，只要有一个词组不同，即返回false；全部相同的话，返回true。
    /// </summary>
    /// <returns></returns>
    public static bool SimilarEqual( string strA, string strB, string splitString ) {
      if ( string.IsNullOrWhiteSpace( strA ) && string.IsNullOrWhiteSpace( strB ) )
        return true;
      string[] split = new string[1] { splitString };
      string[] A = strA.Split( split, StringSplitOptions.RemoveEmptyEntries );
      string[] B = strB.Split( split, StringSplitOptions.RemoveEmptyEntries );
      return SimilarEqual( A, B );
    }

    public static bool SimilarEqual( string[] A, string[] B ) {
      if ( A == null && B == null ||
        A.Length == B.Length && B.Length == 0 )
        return true;
      else if ( A.Length != B.Length )
        return false;

      List<string> listA = new List<string>( A ),
        listB = new List<string>( B );
      bool result = true;
      foreach ( string a in listA ) {
        if ( !listB.Contains( a ) ) {
          result = false;
          break;
        }
      }
      listA.Clear();
      listB.Clear();
      return result;
    }

  }

}
