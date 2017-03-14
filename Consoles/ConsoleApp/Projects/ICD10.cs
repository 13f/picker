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
  /// <summary>
  /// 国际疾病分类编码; ICD-10疾病编码.
  /// http://icd.bm8.com.cn/bm/D_1.html
  /// </summary>
  public static class ICD10 {

    const string UrlPrefix = "http://icd.bm8.com.cn/";
    const int CountPerPage = 30;

    public static async Task PickDetails(string pathList, string pathDetails, int millisecondsDelay = 2500 ) {
      Console.WriteLine( "load data file..." );
      // load
      string json = File.ReadAllText( pathList );
      JObject rootList = JObject.Parse( json );
      JArray categories = (JArray)rootList["items"];

      JObject root = new JObject();
      JArray items = new JArray();
      //Dictionary<string, JToken> cache = new Dictionary<string, JToken>();
      List<string> cache = new List<string>();
      int totalItems = 0;
      if( File.Exists( pathDetails ) ) { // 继续上一次的处理？
        string jsonDetails = File.ReadAllText( pathDetails );
        root = JObject.Parse( jsonDetails );
        items = (JArray)root["items"];
        totalItems = items.Count;
        foreach(var item in items ) {
          cache.Add( (string)item["icd10_code"] );
        }
      }

      foreach( var category in categories ) {
        if( category["updatedAt"] != null ) // skip
          continue;

        string id = (string)category["bm8_id"];
        Console.WriteLine( "> " + id );

        string url = UrlPrefix + id;
        var tmpItems = await pickCategoryDetail( url, millisecondsDelay );
        tmpItems.ForEach( i => {
          string icd10_code = (string)i["icd10_code"];
          if(!cache.Contains( icd10_code ) ) {
            items.Add( i );
            cache.Add( icd10_code );
          }
        } );
        root["items"] = items;
        root["count"] = items.Count;

        var ids = tmpItems.Select( i => (string)i["icd10_code"] )
          .OrderBy( i => i )
          .ToArray();
        JArray itemsInCategory = new JArray( ids );
        category["items"] = itemsInCategory;
        category["count"] = itemsInCategory.Count;
        // update tag
        category["updatedAt"] = DateTime.UtcNow;

        int count = tmpItems.Count;
        totalItems += count;

        // save
        LocalStorageUtility.Save( rootList, pathList );
        LocalStorageUtility.Save( root, pathDetails );
        // wait
        Console.WriteLine( "... wait... and then continue..." );
        await Task.Delay( millisecondsDelay );
      }

      root["total_items_count"] = totalItems;
      // save
      LocalStorageUtility.Save( root, pathDetails );
      Console.WriteLine( "saved...over..." );
    }

    static async Task<List<JObject>> pickCategoryDetail(string url, int millisecondsDelay = 2500 ) {
      WebClient client = NetHelper.GetWebClient_GB2312();
      HtmlDocument doc = new HtmlDocument();

      Dictionary<string, JObject> items = new Dictionary<string, JObject>();
      int page = 0;
      int lastCount = 0;
      while( true ) {
        page++;
        Console.WriteLine( "  > page: " + page.ToString() );
        string pageUrl = url.Replace( ".html", "_" + page.ToString() + ".html" );
        string html = await client.DownloadStringTaskAsync( pageUrl );
        doc.LoadHtml( html );
        Console.Write( "  > got html." );

        var table = doc.DocumentNode
          .SelectNodes( "//table" )
          .Skip( 2 )
          .FirstOrDefault();
        if( table == null )
          break;
        // row1: 包含以下标准编码
        // row2: 疾病名称  疾病编码  助记码
        var rows = table.SelectNodes( "./tr" )
          .Skip( 2 )
          .Take( CountPerPage );
        // rows.Count()始终等于30，没有数据的行也是空白行。所以需要一个计数器记录真实的条目数。
        //int countInPage = 0;
        bool countLessThan30 = false; // 当前页的条目数是否小于30
        foreach(var row in rows ) {
          var cols = row.SelectNodes( "./td" );
          JObject item = null;
          int index = 1;
          foreach(var col in cols ) {
            if( index == 1 ) {
              // <td><a href="/icd10/3757.html" target="_blank">面部良性肿瘤</a></td>
              var a = col.SelectSingleNode( "./a" );
              if( string.IsNullOrWhiteSpace( a.InnerText ) ) { // <td><a href="/icd10/.html" target="_blank"></a></td>
                countLessThan30 = true;
                break;
              }
              item = new JObject();
              item["bm8_id"] = a.Attributes["href"].Value.Substring( 1 );
              item["name"] = a.InnerText;
              //countInPage++;
            }
            else if( index == 2 ) {
              // <td><font color="#666666">D36.705</font></td>
              string code = col.SelectSingleNode( "./font " ).InnerText;
              //if( items.ContainsKey( code ) ) {
              //  item = null;
              //  break;
              //}
              item["icd10_code"] = code;
            }
            else if( index == 3 ) {
              // <td><font color="#666666">MBLXZL</font></td>
              string help_code = col.SelectSingleNode( "./font" ).InnerText;
              item["help_code"] = string.IsNullOrWhiteSpace( help_code) ? null : help_code;
            }
            index++;
          } // foreach col in cols
          // new?
          if( item != null && !items.ContainsKey( (string)item["icd10_code"] ) ) {
            items.Add( (string)item["icd10_code"], item );
          }

          // 遇到导航条这个tr需要跳出；遇到条目数<30需要跳出
          //if( countLessThan30 )
          //  break;
        } // rows

        // 0、如果本身只有15页数据，仍然可以通过page=16获取到html，此时显示的是15页数据。此时需要注意最后一页恰好有30条数据的情形。
        // 1、遇到小于30条的页面，跳出
        // 2、遇到满30条，但是数据不再增加的情况（0），跳出
        if( countLessThan30 || lastCount == items.Count )
          break;

        // update lastCount
        lastCount = items.Count;

        // wait
        Console.WriteLine( "... wait... continue..." );
        await Task.Delay( millisecondsDelay );
      }
      client.Dispose();
      return items.Values.ToList();
    }

    public static void OrderByCode( string pathSource, string pathResult ) {
      Console.WriteLine( "load data file..." );
      // load
      string json = File.ReadAllText( pathSource );
      JObject rootSource = JObject.Parse( json );
      JArray itemsSource = (JArray)rootSource["items"];
      Console.WriteLine( "loaded." );

      var array = itemsSource.OrderBy( i => i["icd10_code"] );
      JArray itemsResult = new JArray();
      foreach(var a in array) {
        itemsResult.Add( a );
      }

      JObject rootResult = new JObject();
      rootResult["title"] = rootSource["title"];
      rootResult["url"] = rootSource["url"];
      rootResult["items"] = itemsResult;
      rootResult["count"] = rootSource["count"];
      
      // save
      LocalStorageUtility.Save( rootResult, pathResult );
      Console.WriteLine( "saved...over..." );
    }

    public static void RemoveBm8Id( string pathSource, string pathResult ) {
      Console.WriteLine( "load data file..." );
      // load
      string json = File.ReadAllText( pathSource );
      JObject rootSource = JObject.Parse( json );
      JArray itemsSource = (JArray)rootSource["items"];
      Console.WriteLine( "loaded." );

      JArray itemsResult = new JArray();
      foreach( var a in itemsSource ) {
        JObject obj = new JObject();
        obj["name"] = a["name"];
        obj["icd10_code"] = a["icd10_code"];
        obj["help_code"] = a["help_code"];
        itemsResult.Add( obj );
      }

      JObject rootResult = new JObject();
      rootResult["title"] = rootSource["title"];
      rootResult["url"] = rootSource["url"];
      rootResult["items"] = itemsResult;
      rootResult["count"] = rootSource["count"];

      // save
      LocalStorageUtility.Save( rootResult, pathResult );
      Console.WriteLine( "saved...over..." );
    }

  }

}
