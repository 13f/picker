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
  /// http://jbk.39.net
  /// </summary>
  public static class JBK39 {
    /// <summary>
    /// 句号图片
    /// </summary>
    const string Gif_FullPoint = "<img alt=\"\" src=\"http://img.39.net/enc/2008/7/28/kg2qojm11wa73wuz.gif\"/>";
    /// <summary>
    /// 顿号图片
    /// </summary>
    const string Gif_IdeographicComma = "<img alt=\"\" src=\"http://img.39.net/enc/2008/7/28/18zuctx5yh42474s.gif\"/>";

    /// <summary>
    /// 抓取症状列表
    /// </summary>
    /// <returns></returns>
    public static async Task PickSymptomList( string directory, int millisecondsDelay = 2500 ) {
      WebClient client = NetHelper.GetWebClient_GB2312();
      HtmlDocument doc = new HtmlDocument();
      //int pagePerCount = 10;
      int pagesCount = 678;
      JObject root = new JObject();
      root["title"] = "症状";
      root["description"] = "";
      root["uri"] = "http://jbk.39.net/bw_t2/";

      JArray items = new JArray();
      int page = 0;
      int count = 0;
      while ( true ) {
        string uri = null;
        if ( page == 0 )
          uri = "http://jbk.39.net/bw_t2/";
        else
          uri = "http://jbk.39.net/bw_t2_p" + page.ToString();
        Console.WriteLine( "==> page: " + page.ToString() );
        string html = await client.DownloadStringTaskAsync( uri );
        Console.WriteLine( "  got html." );

        doc.LoadHtml( html );
        var tmp = processSymptomList( doc.DocumentNode );
        tmp.ForEach( i => {
          items.Add( i );
          count++;
        } );
        tmp.Clear();

        if ( page == pagesCount )
          break;
        page++;
        Console.WriteLine( "... wait... and then continue..." );
        await Task.Delay( millisecondsDelay );
      }
      root["count"] = count;
      root["items"] = items;
      // save
      save( root, directory + "symptom-data.json" );
      Console.WriteLine( "saved...over..." );
    }

    /// <summary>
    /// 抓取疾病列表
    /// </summary>
    /// <returns></returns>
    public static async Task PickDiseaseList( string directory, int millisecondsDelay = 2500 ) {
      string filename = "disease-data-v2.json";
      string path = directory + filename;

      WebClient client = NetHelper.GetWebClient_GB2312();
      HtmlDocument doc = new HtmlDocument();
      int countPerPage = 10;
      int pagesCount = 808;

      JObject root = null;
      JArray items = null;
      int page = 0;
      int count = 0;
      if ( File.Exists( path ) ) {
        string txt = File.ReadAllText( path );
        root = JObject.Parse( txt );
        items = (JArray)root["items"];
        count = (int)root["count"];
        page = ( count - countPerPage ) / countPerPage;
        if ( page < 0 )
          page = 0;
      }
      else {
        root = new JObject();
        root["title"] = "疾病";
        root["description"] = "";
        root["uri"] = "http://jbk.39.net/bw_t1/";

        items = new JArray();
      }

      while ( true ) {
        string uri = null;
        if ( page == 0 )
          uri = "http://jbk.39.net/bw_t1/";
        else
          uri = "http://jbk.39.net/bw_t1_p" + page.ToString();
        Console.WriteLine( "==> page: " + page.ToString() );
        string html = await client.DownloadStringTaskAsync( uri );
        Console.WriteLine( "  got html." );

        doc.LoadHtml( html );
        var tmp = processDiseaseList( doc.DocumentNode );
        tmp.ForEach( i => {
          var exists = items.Where( j => (string)j["url"] == (string)i["url"] ).FirstOrDefault();
          if( exists == null ) {
            items.Add( i );
            count++;
          }
        } );
        tmp.Clear();

        if( page > 0 && page % 5 == 0 ) { // 每5页保存一次
          root["count"] = count;
          root["items"] = items;
          save( root, path );
          Console.WriteLine( "...saved." );
        }
        
        if ( page == pagesCount )
          break;
        page++;
        Console.WriteLine( "... wait... and then continue..." );
        await Task.Delay( millisecondsDelay );
      }
      root["count"] = count;
      root["items"] = items;
      // save
      save( root, path );
      Console.WriteLine( "saved...over..." );
    }

    /// <summary>
    /// 抓取症状详情
    /// </summary>
    /// <returns></returns>
    public static async Task PickSymptomDetails( string pathSource, string pathResult, int millisecondsDelay = 2500 ) {
      WebClient client = NetHelper.GetWebClient_GB2312();
      HtmlDocument doc = new HtmlDocument();
      Console.WriteLine( "load data file..." );
      // load
      string json = File.ReadAllText( pathSource );
      JObject root = JObject.Parse( json );

      JArray items = (JArray)root["items"];
      int totalCount = items.Count;
      string stringTotal = totalCount.ToString();
      int count = 0;
      foreach(var item in items ) {
        count++;
        if ( item["updatedAt"] != null ) // skip
          continue;

        string url = (string)item["url"];
        Console.WriteLine( count.ToString() + " / " + stringTotal + " ==> " + url );
        // 综述
        Console.WriteLine( "  ==> 综述" );
        string html = await client.DownloadStringTaskAsync( url );
        doc.LoadHtml( html );
        processSummary( item, doc.DocumentNode );
        // 症状起因
        Console.WriteLine( "  ==> 症状起因" );
        string uri = url + "zzqy/";
        html = await client.DownloadStringTaskAsync( uri );
        doc.LoadHtml( html );
        processCauses( item, doc.DocumentNode );
        // 诊断详述
        Console.WriteLine( "  ==> 诊断详述" );
        uri = url + "zdxs/";
        html = await client.DownloadStringTaskAsync( uri );
        doc.LoadHtml( html );
        processDiagnosisDetail( item, doc.DocumentNode );
        // 检查鉴别
        Console.WriteLine( "  ==> 检查鉴别" );
        uri = url + "jcjb/";
        html = await client.DownloadStringTaskAsync( uri );
        doc.LoadHtml( html );
        processChecks( item, doc.DocumentNode );
        // update tag
        item["updatedAt"] = DateTime.UtcNow;
        // save
        save( root, pathResult );
        // wait
        Console.WriteLine( "... wait... and then continue..." );
        await Task.Delay( millisecondsDelay );
      }

      Console.WriteLine( "saved...over..." );
    }

    /// <summary>
    /// 抓取疾病详情
    /// </summary>
    /// <returns></returns>
    public static async Task PickDiseaseDetails( string pathSource, string pathResult, int millisecondsDelay = 2500 ) {
      WebClient client = NetHelper.GetWebClient_GB2312();
      HtmlDocument doc = new HtmlDocument();
      Console.WriteLine( "load data file..." );
      // load
      string json = File.ReadAllText( pathSource );
      JObject root = JObject.Parse( json );

      JArray items = (JArray)root["items"];
      int totalCount = items.Count;
      string stringTotal = totalCount.ToString();
      int count = 0;
      foreach ( var item in items ) {
        count++;
        if ( item["updatedAt"] != null ) // skip
          continue;

        string url = (string)item["url"];
        Console.WriteLine( count.ToString() + " / " + stringTotal + " ==> " + url );
        // 综述
        Console.WriteLine( "  ==> 综述" );
        string html = await client.DownloadStringTaskAsync( url );
        doc.LoadHtml( html );
        processSummary( item, doc.DocumentNode );
        // 症状起因
        Console.WriteLine( "  ==> 症状起因" );
        string uri = url + "zzqy/";
        html = await client.DownloadStringTaskAsync( uri );
        doc.LoadHtml( html );
        processCauses( item, doc.DocumentNode );
        // 诊断详述
        Console.WriteLine( "  ==> 诊断详述" );
        uri = url + "zdxs/";
        html = await client.DownloadStringTaskAsync( uri );
        doc.LoadHtml( html );
        processDiagnosisDetail( item, doc.DocumentNode );
        // 检查鉴别
        Console.WriteLine( "  ==> 检查鉴别" );
        uri = url + "jcjb/";
        html = await client.DownloadStringTaskAsync( uri );
        doc.LoadHtml( html );
        processChecks( item, doc.DocumentNode );
        // update tag
        item["updatedAt"] = DateTime.UtcNow;
        // save
        save( root, pathResult );
        // wait
        Console.WriteLine( "... wait... and then continue..." );
        await Task.Delay( millisecondsDelay );
      }

      Console.WriteLine( "saved...over..." );
    }


    static void save( JObject jo, string path ) {
      string json = jo.ToString( Newtonsoft.Json.Formatting.Indented );
      File.WriteAllText( path, json );
    }

    static List<JObject> processSymptomList( HtmlNode root ) {
      List<JObject> result = new List<JObject>();

      var divContainer = root.SelectSingleNode( "//div[@id='res_subtab_1']" );
      var divList = divContainer.SelectNodes( "./div[@class='res_list']" );

      foreach(var div in divList ) {
        JObject item = new JObject();
        var a = div.SelectSingleNode( "./dl" )
          .SelectSingleNode( "./dt" )
          .SelectSingleNode( "./h3" )
          .SelectSingleNode( "./a" );
        item["name"] = a.InnerText.Trim();
        item["url"] = a.Attributes["href"].Value;
        result.Add( item );
      }
      return result;
    }

    static List<JObject> processDiseaseList( HtmlNode root ) {
      List<JObject> result = new List<JObject>();

      var divContainer = root.SelectSingleNode( "//div[@id='res_tab_2']" );
      var divList = divContainer.SelectNodes( "./div[@class='res_list']" );

      foreach ( var div in divList ) {
        JObject item = new JObject();
        var a = div.SelectSingleNode( "./dl" )
          .SelectSingleNode( "./dt" )
          .SelectSingleNode( "./h3" )
          .SelectSingleNode( "./a" );
        item["name"] = a.InnerText.Trim();
        item["url"] = a.Attributes["href"].Value;
        result.Add( item );
      }
      return result;
    }

    /// <summary>
    /// 综述
    /// </summary>
    /// <param name="token"></param>
    /// <param name="root"></param>
    static void processSummary( JToken token, HtmlNode root ) {
      // 别名
      var alias = root.SelectSingleNode( "//h2[@class='alias']" );
      if( alias != null ) {
        token["alias"] = alias.InnerText
          .Replace( "（别名：", "" )
          .Replace( "）", "" )
          .Trim();
      }
      // 简介
      var intro = root.SelectSingleNode( "//p[@class='sort2']" );
      token["introduction"] = intro.InnerText
        .Trim( '\t', '\r', '\n' )
        .Trim();
    }

    /// <summary>
    /// 症状起因
    /// </summary>
    /// <param name="token"></param>
    /// <param name="root"></param>
    static void processCauses( JToken token, HtmlNode root ) {
      // 症状起因
      var divCause = root.SelectSingleNode( "//div[@class='lbox_con']" );
      if ( divCause != null ) {
        divCause = divCause.SelectSingleNode( "./div[@class='item catalogItem']" );
        if ( divCause != null ) {
          token["cause"] = divCause.InnerHtml
            .Replace( Gif_FullPoint, "。" )
            .Replace( Gif_IdeographicComma, "、" )
            .Trim( '\t', '\r', '\n' )
            .Trim();
        }
      }
      // 可能疾病
      var table = root.SelectSingleNode( "//table[@id='relateDis']" );
      var relatedDiseases = processRelatedDiseases( table );
      token["relatedDiseases"] = relatedDiseases;
    }

    /// <summary>
    /// 诊断详述
    /// </summary>
    /// <param name="token"></param>
    /// <param name="root"></param>
    static void processDiagnosisDetail( JToken token, HtmlNode root ) {
      // 诊断详述
      var divCause = root.SelectSingleNode( "//div[@class='lbox_con']" );
      if ( divCause != null ) {
        divCause = divCause.SelectSingleNode( "./div[@class='item catalogItem']" );
        if ( divCause != null ) {
          token["diagnosisDetail"] = divCause.InnerHtml
            .Replace( Gif_FullPoint, "。" )
            .Replace( Gif_IdeographicComma, "、" )
            .Trim( '\t', '\r', '\n' )
            .Trim();
        }
      }
      // 对症药品
      var div = root.SelectSingleNode( "//div[@id='relateDrug']" );
      if(div != null ) {
        var dls = div.SelectNodes( "./dl" )
          .Where( i => i.Attributes["style"] != null );
        JArray items = new JArray();
        foreach ( var dl in dls ) {
          var a = dl.SelectSingleNode( "./dt" )
            .SelectSingleNode( "./a" );
          JObject item = new JObject();
          item["name"] = a.Attributes["title"].Value;
          item["url"] = a.Attributes["href"].Value;
          items.Add( item );
        }
        token["relateDrugs"] = items;
      }
    }

    /// <summary>
    /// 检查鉴别
    /// </summary>
    /// <param name="token"></param>
    /// <param name="root"></param>
    static void processChecks( JToken token, HtmlNode root ) {
      // 相似症状
      var ulSymList = root.SelectSingleNode( "//ul[@id='symList']" );
      bool hasSimilarSymtoms = ulSymList != null;
      if ( hasSimilarSymtoms ) {
        JArray itemsSimilarSymptoms = new JArray();
        var dls = root.SelectNodes( "//dl[@class='item']" );
        foreach(var dl in dls ) {
          var a = dl.SelectSingleNode( "./dt" )
            .SelectSingleNode( "./a" );
          JObject item = new JObject();
          item["name"] = a.Attributes["title"].Value;
          item["url"] = a.Attributes["href"].Value;
          itemsSimilarSymptoms.Add( item );
        }
        token["similarSymptoms"] = itemsSimilarSymptoms;
      }
      // 常用检查
      JArray itemsCommonDiagnoses = new JArray();
      if( hasSimilarSymtoms ) { // 页面中有 相似症状
        var ul = root.SelectSingleNode( "//ul[@class='link clearfix']" );
        if(ul!= null ) {
          var lis = ul.SelectNodes( "./li" );
          foreach ( var li in lis ) {
            var a = li.SelectSingleNode( "./a" );
            JObject item = new JObject();
            item["name"] = a.Attributes["title"].Value;
            item["url"] = a.Attributes["href"].Value;
            itemsCommonDiagnoses.Add( item );
          }
        } // if(ul!= null ) {
      }
      else { // 页面中没有 相似症状
        // <li class="clearfix">
        var dls = root.SelectNodes( "//dl[@class='item']" );
        if(dls != null ) {
          foreach ( var dl in dls ) {
            var a = dl.SelectSingleNode( "./dt" )
              .SelectSingleNode( "./a" );
            JObject item = new JObject();
            item["name"] = a.Attributes["title"].Value;
            item["url"] = a.Attributes["href"].Value;
            itemsCommonDiagnoses.Add( item );
          }
        } // if(dls != null ) {
        // <ul class="zz_list clearfix">
        var ul = root.SelectSingleNode( "//ul[@class='zz_list clearfix']" );
        if(ul!= null ) {
          var lis = ul.SelectNodes( "./li" );
          if(lis != null ) {
            foreach ( var li in lis ) {
              var a = li.SelectSingleNode( "./a" );
              JObject item = new JObject();
              item["name"] = a.Attributes["title"].Value;
              item["url"] = a.Attributes["href"].Value;
              itemsCommonDiagnoses.Add( item );
            }
          }
        } // if(ul!= null ) {
      } // else
      token["commonDiagnoses"] = itemsCommonDiagnoses;
      
    }

    static JArray processRelatedDiseases(HtmlNode table ) {
      JArray items = new JArray();
      if ( table == null )
        return items;

      var rows = table.SelectNodes( "./tr" )
        .Where( i => i.Attributes["style"] != null );
      foreach(var row in rows ) {
        var a = row.SelectSingleNode( "./td[@class='name']" )
          .SelectSingleNode( "./a" );
        JObject item = new JObject();
        item["name"] = a.Attributes["title"].Value;
        item["url"] = a.Attributes["href"].Value;
        items.Add( item );
      }

      return items;
    }

  }

}
