using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Picker.Core.Helpers;

namespace ConsoleApp {
  public class Element {
    public const string UrlPrefix = "https://zh.wikipedia.org";
    public const string BlankCode = "&#160;";

    /// <summary>
    /// https://zh.wikipedia.org/zh-cn/化學元素
    /// </summary>
    public static async Task Wikipedia_ChemicalElementsList1(string path) {
      string url = "https://zh.wikipedia.org/zh-cn/化學元素";
      WebClient client = NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();

      string html = await client.DownloadStringTaskAsync( url );
      doc.LoadHtml( html );
      Console.WriteLine( "  got." );

      JObject root = new JObject();
      root["url"] = url;

      JArray items = new JArray();
      int count = 0;

      var rows = doc.DocumentNode.SelectSingleNode( "//table[@class='wikitable sortable']" )
        .SelectNodes( "./tr" )
        .Skip( 1 );
      foreach(var row in rows ) {
        JObject item = parseElementInList1( row );
        items.Add( item );
        count++;
      }

      root["config"] = createConfig();
      root["items"] = items;
      root["count"] = count;
      // save
      LocalStorageUtility.Save( root, path );
      Console.WriteLine( "saved...over..." );
    }

    /// <summary>
    /// https://zh.wikipedia.org/wiki/元素列表
    /// </summary>
    public static async Task Wikipedia_MergeChemicalElementsList2( string path ) {
      // load
      string json = File.ReadAllText( path );
      JObject root = JObject.Parse( json );
      JArray items = (JArray)root["items"];

      string url = "https://zh.wikipedia.org/wiki/元素列表";
      WebClient client = NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();

      string html = await client.DownloadStringTaskAsync( url );
      doc.LoadHtml( html );
      Console.WriteLine( "  got." );

      var rows = doc.DocumentNode.SelectSingleNode( "//table[@class='wikitable sortable']" )
        .SelectNodes( "./tr" )
        .Skip( 1 );
      foreach( var row in rows ) {
        var cols = row.SelectNodes( "./td" );
        var colLast = cols.Last();

        string symbol = cols[0].InnerText; // th是原子序数
        var item = items.Where( jt => (string)jt["symbol"] == symbol ).FirstOrDefault();
        var name = item["name"];
        name["english"] = colLast.InnerText;
      }
      root["items"] = items;
      // save
      LocalStorageUtility.Save( root, path );
      Console.WriteLine( "saved...over..." );
    }


    static JObject createConfig() {
      JObject item = new JObject();
      item["atomic_number"] = "原子序数";
      item["name"] = "名称";
      item["symbol"] = "符号";
      item["group "] = "族 (化学)";
      item["period"] = "元素周期";
      item["block"] = "元素分区";
      item["state_in_standard"] = "标准状况下的状态";
      item["exist"] = "存在情形";
      item["remark"] = "说明";
      item["reference"] = "参考";
      return item;
    }

    static string lastPeriod = null;
    static string lastBlock = null;
    static int rowspanPeriod = 0;
    static int rowspanBlock = 0;

    static JObject parseElementInList1( HtmlNode row ) {
      //< tr >
      //<th>< a href = "/wiki/%E5%8E%9F%E5%AD%90%E5%BA%8F" class="mw-redirect" title="原子序">原子序</a></th>
      //<th>繁体名称</th>
      //<th>简体名称</th>
      //<th><a href = "/wiki/%E5%85%83%E7%B4%A0%E7%AC%A6%E8%99%9F" class="mw-redirect" title="元素符号">符号</a></th>
      //<th><a href = "/wiki/%E6%97%8F_(%E5%8C%96%E5%AD%A6)" title="族 (化学)">族</a></th>
      //<th><a href = "/wiki/%E5%85%83%E7%B4%A0%E5%91%A8%E6%9C%9F" title="元素周期">周期</a></th>
      //<th><a href = "/wiki/%E5%85%83%E7%B4%A0%E5%88%86%E5%8C%BA" title="元素分区">分区</a></th>
      //<th><a href = "/wiki/%E6%A0%87%E5%87%86%E7%8A%B6%E5%86%B5" title="标准状况">标准状况</a> 下的<br />
      //  <a href = "/wiki/%E7%89%A9%E8%B3%AA%E7%8B%80%E6%85%8B" class="mw-redirect" title="物质状态">状态</a></th>
      //<th>存在情形</th>
      //<th>说明</th>
      //</tr>
      var cols = row.SelectNodes( "./td" );
      int index = 0;

      JObject item = new JObject();

      JObject name = new JObject();
      name["english"] = null;
      item["name"] = name;

      List<string> refList = new List<string>();

      foreach(var col in cols ) {
        if(index == 0 ) {
          string txt = col.InnerText.Replace( '\n', ' ' )
            .Replace( BlankCode, "" )
            .Trim();
          item["atomic_number"] = int.Parse( txt );
          Console.WriteLine( "  ==> " + txt );
        }
        else if( index == 1 ) {
          getName( col, item, refList );
        }
        else if( index == 2 ) {
          getName( col, item, refList );
        }
        else if( index == 3 ) {
          item["symbol"] = col.InnerText;
        }
        else if( index == 4 ) {
          item["group "] = int.Parse( col.InnerText );
          // peroid
          if( rowspanPeriod > 1 ) {
            item["period"] = lastPeriod;
            index++;
            rowspanPeriod--;
          }
          // block
          if( rowspanBlock > 1 ) {
            item["block"] = lastBlock;
            index++;
            rowspanBlock--;
          }
        }
        else if( index == 5 ) {
          var rowspan = col.Attributes["rowspan"];
          if(rowspan != null ) {
            lastPeriod = col.InnerText;
            rowspanPeriod = int.Parse( rowspan.Value );
          }
          item["period"] = col.InnerText;
        }
        else if( index == 6 ) {
          var rowspan = col.Attributes["rowspan"];
          if( rowspan != null ) {
            lastBlock = col.InnerText;
            rowspanBlock = int.Parse( rowspan.Value );
          }
          item["block"] = col.InnerText;
        }
        else if( index == 7 ) {
          item["state_in_standard"] = col.InnerText == BlankCode ? null : col.InnerText;
        }
        else if( index == 8 ) {
          item["exist"] = col.InnerText;
        }
        else if( index == 9 ) {
          item["remark"] = col.InnerText == BlankCode ? null : col.InnerText;
        }
        index++;
      }

      JArray reference = new JArray();
      refList.ForEach(
        r => reference.Add( r )
      );
      item["reference"] = reference;
      return item;
    }

    static void getName( HtmlNode col, JObject item, List<string> refList ) {
      // <td><span lang="zh-tw"><a href="/wiki/%E9%8E%9D" class="mw-redirect" title="鎝">鎝</a></span>、<span lang="zh-hk"><a href="/wiki/%E9%8D%80" class="mw-redirect" title="鍀">鍀</a></span></td>
      // <td><span lang = "zh-cn" >< a href="/wiki/%E9%94%86" title="锆">锆</a></span></td>
      var spans = col.SelectNodes( "./span" );
      if( spans != null && spans.Count > 0 ) {
        var tokenName = item["name"];
        foreach( var span in spans ) {
          string lang = span.Attributes["lang"].Value;
          var a = span.SelectSingleNode( "./a" );
          tokenName[lang.Replace( '-', '_' )] = a.InnerText;

          string url = UrlPrefix + a.Attributes["href"].Value;
          if( !refList.Contains( url ) )
            refList.Add( url );
        }
        item["name"] = tokenName;
      }
    }


  }

}