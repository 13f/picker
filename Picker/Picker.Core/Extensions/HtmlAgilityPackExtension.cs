using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Picker.Core.Extensions {
  public static class HtmlAgilityPackExtension {

    public static IEnumerable<HtmlNode> GetTables( this HtmlDocument doc ) {
      var items = doc.DocumentNode.Descendants( "table" );
      return items;
    }

    public static string GetHtmlString( this HtmlDocument doc ) {
      string html = null;
      using ( System.IO.StringWriter writer = new System.IO.StringWriter() ) {
        doc.Save( writer );
        html = writer.ToString();
      }
      return html;
    }

    /// <summary>
    /// 移除节点上的所有属性
    /// </summary>
    /// <param name="doc"></param>
    public static void RemoveAttributes( this HtmlDocument doc ) {
      var elementsWithStyleAttribute = doc.DocumentNode.SelectNodes( "//*" );
      if ( elementsWithStyleAttribute != null ) {
        foreach ( var element in elementsWithStyleAttribute ) {
          element.Attributes.RemoveAll();
        }
      }
    }

    /// <summary>
    /// 移除注释
    /// </summary>
    /// <param name="doc"></param>
    public static void RemoveComments( this HtmlDocument doc ) {
      //foreach ( var comment in doc.DocumentNode.SelectNodes( "//comment()" ).ToArray() )
      //  comment.Remove();
      doc.DocumentNode.Descendants()
       .Where( n => n.NodeType == HtmlNodeType.Comment )
       .ToList()
       .ForEach( n => n.Remove() );
    }

    /// <summary>
    /// 移除script
    /// </summary>
    /// <param name="doc"></param>
    public static void RemoveScripts( this HtmlDocument doc ) {
      foreach ( var script in doc.DocumentNode.Descendants( "script" ).ToArray() )
        script.Remove();
    }

    /// <summary>
    /// 移除CSS Style
    /// </summary>
    /// <param name="doc"></param>
    public static void RemoveStyles( this HtmlDocument doc ) {
      foreach ( var style in doc.DocumentNode.Descendants( "style" ).ToArray() )
        style.Remove();
    }

    /// <summary>
    /// 移除空白行
    /// </summary>
    /// <param name="doc"></param>
    public static string RemoveBlankLines( this string source ) {
      string r = System.Text.RegularExpressions.Regex.Replace( source, @"(\r\n){2,}", "\r\n", System.Text.RegularExpressions.RegexOptions.Multiline );
      r = System.Text.RegularExpressions.Regex.Replace( r, @"<br>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase );
      r = System.Text.RegularExpressions.Regex.Replace( r, @"<br />", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase );
      return r;
    }

    /// <summary>
    /// 移除nbsp
    /// </summary>
    /// <param name="doc"></param>
    public static string RemoveNbsp( this string source ) {
      return source.Replace( "&nbsp;", "" );
    }

  }

}
