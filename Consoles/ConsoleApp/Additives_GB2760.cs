using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Picker.Core.Extensions;

namespace ConsoleApp {
  public static class Additives_GB2760 {
    const int CountPerPage30 = 30;

    /// <summary>
    /// 0: page(1,)
    /// </summary>
    const string UriTemplate_AdditivesList = "http://111.203.7.144/index.php?m=additives&a=index&p={0}";
    /// <summary>
    /// 0: additive id
    /// </summary>
    const string UriTemplate_AdditiveDetail = "http://111.203.7.144/index.php?m=additives&a=show&faid={0}";
    /// <summary>
    /// 0: food id
    /// </summary>
    const string UriTemplate_FoodDetail = "http://111.203.7.144/index.php?m=additives&a=product&catid={0}";

    public static async Task PickAdditivesList( string filepath, int? millisecondsDelay ) {
      int total = 285;
      JObject joRoot = new JObject();
      joRoot.Add( "title", "GB 2760-2014 食品添加剂" );
      //joRoot.Add( "description", "" );
      joRoot.Add( "url", "http://111.203.7.144/index.php?m=additives&a=index&p=1" );
      joRoot.Add( "updated_at", "2016-4-30" );
      joRoot.Add( "total_count", total );
      // config
      JObject joConfig = createAdditiveConfig();
      joRoot.Add( "config", joConfig );

      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();

      JArray items = new JArray();
      int page = 0;
      while ( true ) {
        page++;
        Console.WriteLine( "get page: " + page );
        string uri = string.Format( UriTemplate_AdditivesList, page );
        string html = await client.DownloadStringTaskAsync( uri );
        doc.LoadHtml( html );

        var list = parseAdditivesTable( doc.DocumentNode );
        list.ForEach( i => items.Add( i ) );
        // continue?
        if ( list.Count < CountPerPage30 )
          break;
        // delay
        if ( millisecondsDelay != null )
          await Task.Delay( millisecondsDelay.Value );
      }
      joRoot.Add( "items", items );

      // save
      Console.WriteLine( "save json..." );
      string json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    public static async Task PickFoodsList( string filepath, int? millisecondsDelay ) {
      JObject joRoot = new JObject();
      joRoot.Add( "title", "GB 2760-2014 食品添加剂 · 食品类别" );
      //joRoot.Add( "description", "" );
      joRoot.Add( "url", "http://111.203.7.144/index.php?m=additives&a=category" );
      joRoot.Add( "updated_at", "2016-4-30" );
      
      // config
      JObject joConfig = createFoodConfig();
      joRoot.Add( "config", joConfig );

      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();
      string html = await client.DownloadStringTaskAsync( "http://111.203.7.144/index.php?m=additives&a=category" );
      doc.LoadHtml( html );

      var table = doc.DocumentNode.SelectSingleNode( "//table[@class='beige1']" );
      var rows = table.SelectNodes( "./tr[@class='py_ py2']" );

      JArray items = new JArray();
      int count = 0;
      foreach ( var row in rows ) {
        var cols = row.SelectNodes( "./td" );
        int index = 0;
        JObject item = new JObject();
        foreach ( var col in cols ) {
          index++;
          if ( index == 1 ) {
            item["food_category_number"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 2 ) {
            var a = col.SelectSingleNode( "./a" );
            string href = a.Attributes["href"].Value;
            int tmp = href.IndexOf( "&catid=" );
            string strId = href.Substring( tmp + 7 );
            item["food_id"] = int.Parse( strId );
            item["food_name"] = a.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
        }
        items.Add( item );
        count++;
        if ( count % 5 == 0 )
          Console.WriteLine( count.ToString() + " items processed..." );
      }
      joRoot.Add( "total_count", count );
      joRoot.Add( "items", items );
      Console.WriteLine( "got all " + count + " items..." );

      // save
      Console.WriteLine( "save json..." );
      string json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    public static async Task PickProcessingAids( string filepath ) {
      JObject joRoot = new JObject();
      joRoot.Add( "title", "GB 2760-2014 食品添加剂 · 加工助剂" );
      //joRoot.Add( "description", "" );
      joRoot.Add( "url", "http://111.203.7.144/index.php?m=processing_aids&a=index" );
      joRoot.Add( "updated_at", "2016-5-1" );

      // config
      JObject joConfig = createProcessingAidsConfig();
      joRoot.Add( "config", joConfig );

      Console.WriteLine( "get html..." );
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();
      string html = await client.DownloadStringTaskAsync( "http://111.203.7.144/index.php?m=processing_aids&a=index" );
      doc.LoadHtml( html );

      var table = doc.DocumentNode.SelectSingleNode( "//table[@class='beige1']" );
      var rows = table.SelectNodes( "./tr" );

      Console.WriteLine( "processing..." );
      JArray items = new JArray();
      int count = 0;
      foreach ( var row in rows ) {
        if ( row.Attributes["class"] == null ) // 第一行是表头
          continue;

        var cols = row.SelectNodes( "./td" );
        int index = 0;
        JObject item = new JObject();
        foreach ( var col in cols ) {
          index++;
          if ( index == 1 ) {
            item["chinese_name"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 2 ) {
            item["english_name"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 3 ) {
            string f = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
            item["function"] = string.IsNullOrWhiteSpace( f ) ? null : f;
          }
          else if ( index == 4 ) {
            item["scope"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
        }
        items.Add( item );
        count++;
        if ( count % 5 == 0 )
          Console.WriteLine( count.ToString() + " items processed..." );
      }
      joRoot.Add( "total_count", count );
      joRoot.Add( "items", items );
      Console.WriteLine( "got all " + count + " items..." );

      // save
      Console.WriteLine( "save json..." );
      string json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    public static async Task PickEnzymeList( string filepath ) {
      JObject joRoot = new JObject();
      joRoot.Add( "title", "GB 2760-2014 食品添加剂 · 酶制剂" );
      //joRoot.Add( "description", "" );
      joRoot.Add( "url", "http://111.203.7.144/index.php?m=enzyme&a=index" );
      joRoot.Add( "updated_at", "2016-5-1" );

      // config
      JObject joConfig = createEnzymeConfig();
      joRoot.Add( "config", joConfig );

      Console.WriteLine( "get html..." );
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();
      string html = await client.DownloadStringTaskAsync( "http://111.203.7.144/index.php?m=enzyme&a=index" );
      doc.LoadHtml( html );

      var table = doc.DocumentNode.SelectSingleNode( "//table[@class='beige1']" );
      var rows = table.SelectNodes( "./tr" );

      Console.WriteLine( "processing..." );
      JArray items = new JArray();
      int count = 0;
      bool isHeader = true;
      foreach ( var row in rows ) {
        if ( isHeader ) {
          isHeader = false;
          continue;
        }

        var cols = row.SelectNodes( "./td" );
        int index = 0;
        JObject item = new JObject();
        foreach ( var col in cols ) {
          index++;
          if ( index == 1 ) {
            var a = col.SelectSingleNode( "./a" );
            string href = a.Attributes["href"].Value;
            int tmp = href.IndexOf( "&no=" );
            string strId = href.Substring( tmp + 4 );
            item["id"] = int.Parse( strId );
            item["chinese_name"] = a.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 2 ) {
            item["english_name"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 3 ) {
            item["source"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 4 ) {
            string f = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
            item["donator"] = string.IsNullOrWhiteSpace( f ) ? null : f;
          }
          else if ( index == 5 ) {
            string f = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
            item["remark"] = string.IsNullOrWhiteSpace( f ) ? null : f;
          }
        }
        items.Add( item );
        count++;
        if ( count % 5 == 0 )
          Console.WriteLine( count.ToString() + " items processed..." );
      }
      joRoot.Add( "total_count", count );
      joRoot.Add( "items", items );
      Console.WriteLine( "got all " + count + " items..." );

      // save
      Console.WriteLine( "save json..." );
      string json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    public static async Task PickSpices1List( string filepath ) {
      JObject joRoot = new JObject();
      joRoot.Add( "title", "GB 2760-2014 食品添加剂 · 表B.1 不得添加食品用香料、香精的食品名单" );
      //joRoot.Add( "description", "" );
      joRoot.Add( "url", "http://111.203.7.144/index.php?m=spices&a=index" );
      joRoot.Add( "updated_at", "2016-5-1" );

      // config
      JObject joConfig = createSpices1Config();
      joRoot.Add( "config", joConfig );

      Console.WriteLine( "get html..." );
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();
      string html = await client.DownloadStringTaskAsync( "http://111.203.7.144/index.php?m=spices&a=index" );
      doc.LoadHtml( html );

      var table = doc.DocumentNode.SelectSingleNode( "//table[@class='beige1']" );
      var rows = table.SelectNodes( "./tr" );
      string category_table = "B.1";
      joRoot["category_table"] = category_table;

      Console.WriteLine( "processing..." );
      JArray items = new JArray();
      int count = 0;
      bool isHeader = true;
      foreach ( var row in rows ) {
        if ( isHeader ) {
          isHeader = false;
          continue;
        }

        var cols = row.SelectNodes( "./td" );
        int index = 0;
        JObject item = new JObject();
        if ( cols == null ) { // 最后一行备注
          var col = row.SelectSingleNode( "./th" );
          if ( col != null ) {
            joRoot["remark"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
            continue;
          }
        }

        foreach ( var col in cols ) {
          index++;
          if ( index == 1 ) {
            item["category_number"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 2 ) {
            item["name"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
        }
        items.Add( item );
        count++;
        if ( count % 5 == 0 )
          Console.WriteLine( count.ToString() + " items processed..." );
      }
      joRoot.Add( "total_count", count );
      joRoot.Add( "items", items );
      Console.WriteLine( "got all " + count + " items..." );

      // save
      Console.WriteLine( "save json..." );
      string json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    public static async Task PickSpices2List( string filepath ) {
      await PickSpices23List( filepath,
        "GB 2760-2014 食品添加剂 · 表B.2 允许使用的食品用天然香料名单",
        "http://111.203.7.144/index.php?m=spices&a=search&b=2",
        "B.2" );
    }

    public static async Task PickSpices3List( string filepath ) {
      await PickSpices23List( filepath,
        "GB 2760-2014 食品添加剂 · 表B.3 允许使用的食品用合成香料名单",
        "http://111.203.7.144/index.php?m=spices&a=search&b=3",
        "B.3" );
    }

    public static async Task PickAdditivesDetail( string filepath, int? millisecondsDelay ) {
      Console.WriteLine( "load list json..." );
      string json = System.IO.File.ReadAllText( filepath );
      JObject joRoot = JObject.Parse( json );
      JArray items = (JArray)joRoot["items"];

      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      int total = 285;
      int count = 0;
      foreach ( JObject joItem in items ) {
        await pickAdditiveDetail( client, joItem );
        count++;
        if ( count % 5 == 0 || count == total )
          Console.WriteLine( count.ToString() + " items processed..." );
        if ( millisecondsDelay != null )
          await Task.Delay( millisecondsDelay.Value );
      }

      // save
      Console.WriteLine( "save..." );
      json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    public static async Task PickFoodsDetail( string filepath, int? millisecondsDelay ) {
      Console.WriteLine( "load list json..." );
      string json = System.IO.File.ReadAllText( filepath );
      JObject joRoot = JObject.Parse( json );
      JArray items = (JArray)joRoot["items"];

      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      int total = 285;
      int count = 0;
      foreach ( JObject joItem in items ) {
        await pickFoodDetail( client, joItem );
        count++;
        if ( count % 5 == 0 || count == total )
          Console.WriteLine( count.ToString() + " items processed..." );
        if ( millisecondsDelay != null )
          await Task.Delay( millisecondsDelay.Value );
      }

      // save
      Console.WriteLine( "save..." );
      json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }


    static async Task PickSpices23List( string filepath, string title, string uri, string category_table ) {
      JObject joRoot = new JObject();
      joRoot.Add( "title", title );
      //joRoot.Add( "description", "" );
      joRoot.Add( "url", uri );
      joRoot.Add( "updated_at", "2016-5-1" );

      // config
      JObject joConfig = createSpices23Config();
      joRoot.Add( "config", joConfig );

      Console.WriteLine( "get html..." );
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();
      string html = await client.DownloadStringTaskAsync( uri );
      doc.LoadHtml( html );

      var table = doc.DocumentNode.SelectSingleNode( "//table[@class='beige1']" );
      var rows = table.SelectNodes( "./tr" );
      joRoot["category_table"] = category_table;

      Console.WriteLine( "processing..." );
      JArray items = new JArray();
      int count = 0;
      foreach ( var row in rows ) {
        if ( row.Attributes["class"] == null ) // 第一行是表头
          continue;

        var cols = row.SelectNodes( "./td" );
        int index = 0;
        JObject item = new JObject();
        foreach ( var col in cols ) {
          index++;
          if ( index == 1 ) {
            item["category"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 2 ) {
            item["chinese_name"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 3 ) {
            item["english_name"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 4 ) {
            item["code"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 5 ) {
            item["fema_code"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          //else if ( index == 6 ) { }
          else if ( index == 7 ) {
            string remark = col.InnerHtml.Trim( '\t', '\r', '\n' ).Trim();
            item["remark"] = string.IsNullOrWhiteSpace( remark ) ? null : remark;
          }
        }
        items.Add( item );
        count++;
        if ( count % 5 == 0 )
          Console.WriteLine( count.ToString() + " items processed..." );
      }
      joRoot.Add( "total_count", count );
      joRoot.Add( "items", items );
      Console.WriteLine( "got all " + count + " items..." );

      // save
      Console.WriteLine( "save json..." );
      string json = joRoot.ToString( Formatting.Indented );
      System.IO.File.WriteAllText( filepath, json, Encoding.UTF8 );
      Console.WriteLine( "over..." );
    }

    static async Task pickAdditiveDetail( WebClient client, JObject joItem ) {
      int id = joItem["additive_id"].Value<int>();
      Console.WriteLine( "get id=" + id );
      string uri = string.Format( UriTemplate_AdditiveDetail, id );
      string html = await client.DownloadStringTaskAsync( uri );

      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );

      // 质量规格标准
      joItem["quality_standard"] = await getQualityStandard( client, html );
      // JECFA规格资料
      joItem["jecfa"] = await getJECFA( client, html );
      // 允许使用品种、使用范围 以及最大使用量或残留量
      var table = doc.DocumentNode.SelectNodes( "//table[@class='beige1']" )
        .Skip( 1 ).Take( 1 ).FirstOrDefault();
      var foods = parseAllowsFoodTable( table );
      JArray foodsArray = new JArray();
      foods.ForEach( i => foodsArray.Add( i ) );
      joItem["allows_food"] = foodsArray;
    }

    static async Task pickFoodDetail( WebClient client, JObject joItem ) {
      int id = joItem["food_id"].Value<int>();
      Console.WriteLine( "get id=" + id );
      string uri = string.Format( UriTemplate_FoodDetail, id );
      string html = await client.DownloadStringTaskAsync( uri );

      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );

      var tables = doc.DocumentNode.SelectNodes( "//table[@class='beige1']" );
      int index = 0;
      foreach(var table in tables ) {
        index++;
        if(index == 1 ) {
          var rows = table.SelectNodes( "./tr" )
            .Skip( 2 ); // skip: 食品分类号, 食品名称
          List<string> nameSourcesStringList = new List<string>();
          foreach(var row in rows ) {
            var th = row.SelectSingleNode( "./th" );
            var td = row.SelectSingleNode( "./td" );

            string thValue = th.InnerText.Trim();
            if ( thValue.StartsWith( "MCMS" ) ) {
              joItem["mcms"] = td.InnerHtml;
            }
            else if ( thValue.StartsWith( "名称描述来源" ) || string.IsNullOrWhiteSpace( thValue ) ) {
              var a = td.SelectSingleNode( "./a" );
              if(a != null ) {
                string s = a.InnerText.Trim();
                if ( !nameSourcesStringList.Contains( s ) )
                  nameSourcesStringList.Add( s );
              }
            }
          }
          // 名称描述来源
          JArray nameSources = new JArray();
          nameSourcesStringList.ForEach( i => nameSources.Add( i ) );
          joItem["name_source"] = nameSources;
        }
        else if(index == 2 ) {
          // 食品添加剂详情
          var additives = parseAllowsAdditivesTable( table );
          JArray additivesArray = new JArray();
          additives.ForEach( i => additivesArray.Add( i ) );
          joItem["allows_additives"] = additivesArray;
        }
      }

    }

    static async Task<JArray> getQualityStandard( WebClient client, string html ) {
      string key1 = "/index.php?g=home&m=ajax&a=fooddown\"+\"&id=";
      string key2 = "\",function(obj)";
      int start = html.IndexOf( key1 );
      if ( start < 0 )
        return null;
      int end = html.IndexOf( key2, start );
      start = start + key1.Length;
      string id = html.Substring( start, end - start );
      if ( string.IsNullOrWhiteSpace( id ) )
        return null;
      if ( id.EndsWith( "|" ) )
        id = id.Substring( 0, id.Length - 1 );
      if ( string.IsNullOrWhiteSpace( id ) )
        return null;
      string uri = string.Format( "http://111.203.7.144/index.php?g=home&m=ajax&a=fooddown&id={0}", id );
      string response = await client.DownloadStringTaskAsync( uri );

      JArray array = getResponseValue( response );
      return array;
    }

    static async Task<JArray> getJECFA( WebClient client, string html ) {
      string key1 = "/index.php?g=home&m=ajax&a=fooddown\"+\"&type=jecfa&id=";
      string key2 = "\",function(obj)";
      int start = html.IndexOf( key1 );
      if ( start < 0 )
        return null;
      int end = html.IndexOf( key2, start );
      start = start + key1.Length;
      string id = html.Substring( start, end - start );
      if ( string.IsNullOrWhiteSpace( id ) )
        return null;
      if ( id.EndsWith( "|" ) )
        id = id.Substring( 0, id.Length - 1 );
      if ( string.IsNullOrWhiteSpace( id ) )
        return null;
      string uri = string.Format( "http://111.203.7.144/index.php?g=home&m=ajax&a=fooddown&type=jecfa&id={0}", id );
      string response = await client.DownloadStringTaskAsync( uri );

      JArray array = getResponseValue( response );
      return array;
    }

    static JArray getResponseValue(string response ) {
      JArray array = new JArray();
      if ( string.IsNullOrWhiteSpace( response ) )
        return array;
      JObject joRoot = JObject.Parse( response );
      response = joRoot.ToString( Formatting.Indented ); // unicode -> 中文
      joRoot = JObject.Parse( response );
      if ( joRoot["status"] == null || (int)joRoot["status"] != 1 )
        return array;
      string content = (string)joRoot["data"];

      XElement xeRoot = XElement.Parse( "<xml>" + content.Replace( "&nbsp;", "" ) + "</xml>" );
      var rows = xeRoot.Elements( "tr" );
      foreach(var row in rows ) {
        var a = row.Element( "td" )
          .Element( "a" );
        array.Add( a.Value );
      }
      
      return array;
    }

    static List<JObject> parseAdditivesTable( HtmlNode root ) {
      List<JObject> result = new List<JObject>();

      var table = root.SelectSingleNode( "//table[@class='beige1']" );
      var rows = table.SelectNodes( "./tr" );

      bool isHeader = true;
      foreach ( var row in rows ) {
        if ( isHeader ) {
          isHeader = false;
          continue;
        }
        if ( row.Attributes["class"] == null ) // 最后一行是分页
          continue;

        var cols = row.SelectNodes( "./td" );
        int index = 0;
        JObject item = new JObject();
        foreach ( var col in cols ) {
          index++;
          if ( index == 1 ) {
            var a = col.SelectSingleNode( "./a" );
            string href = a.Attributes["href"].Value;
            int tmp = href.IndexOf( "&faid=" );
            string strId = href.Substring( tmp + 6 );
            item["additive_id"] = int.Parse( strId );
            item["chinese_name"] = a.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 2 ) {
            item["english_name"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 3 ) {
            item["cns"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 4 ) {
            item["ins"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 5 ) {
            item["function"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
        }
        result.Add( item );
      }

      return result;
    }

    static List<JObject> parseAllowsFoodTable( HtmlNode table ) {
      List<JObject> result = new List<JObject>();
      if ( table == null )
        return result;

      var rows = table.SelectNodes( "./tr" );

      bool isHeader = true;
      foreach ( var row in rows ) {
        if ( isHeader ) {
          isHeader = false;
          continue;
        }

        var cols = row.SelectNodes( "./td" );
        int index = 0;
        JObject item = new JObject();
        foreach ( var col in cols ) {
          index++;
          if ( index == 1 ) {
            item["food_category_number"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 2 ) {
            var a = col.SelectSingleNode( "./a" );
            string href = a.Attributes["href"].Value;
            int tmp = href.IndexOf( "&catid=" );
            string strId = href.Substring( tmp + 7 );
            item["food_id"] = int.Parse( strId );
            item["food_name"] = a.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 3 ) {
            item["max_usage"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 4 ) {
            item["remark"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
        }
        result.Add( item );
      }

      return result;
    }

    static List<JObject> parseAllowsAdditivesTable( HtmlNode table ) {
      List<JObject> result = new List<JObject>();
      if ( table == null )
        return result;

      var rows = table.SelectNodes( "./tr" );

      bool isHeader = true;
      JArray groupItems = new JArray();
      JObject group = new JObject();
      group.Add( "name", "默认" );
      group.Add( "items", groupItems );
      result.Add( group );
      foreach ( var row in rows ) {
        if ( isHeader ) {
          isHeader = false;
          continue;
        }

        var cols = row.SelectNodes( "./td" );
        var tdGroup = row.SelectSingleNode( "./td[@class='f_red f_cen f_b']" );
        if (cols.Count == 1 && tdGroup != null ) { // 新分组
          groupItems = new JArray();
          group = new JObject();
          group.Add( "name", tdGroup.InnerText );
          group.Add( "items", groupItems );
          result.Add( group );
          continue;
        }

        int index = 0;
        JObject item = new JObject();
        foreach ( var col in cols ) {
          index++;
          if ( index == 1 ) {
            var a = col.SelectSingleNode( "./a" );
            string href = a.Attributes["href"].Value;
            int tmp = href.IndexOf( "&faid=" );
            string strId = href.Substring( tmp + 6 );
            item["additive_id"] = int.Parse( strId );
            item["additive_name"] = a.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 2 ) {
            item["additive_function"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 3 ) {
            item["additive_max_usage"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 4 ) {
            item["additive_cns"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 5 ) {
            item["additive_ins"] = col.InnerText.Trim( '\t', '\r', '\n' ).Trim();
          }
          else if ( index == 6 ) {
            item["additive_remark"] = col.InnerHtml.Trim( '\t', '\r', '\n' ).Trim();
          }
        }
        groupItems.Add( item );
      }

      return result;
    }

    static JObject createAdditiveConfig() {
      JObject joConfig = new JObject();
      joConfig.Add( "additive_id", "ID" );
      joConfig.Add( "chinese_name", "中文名称" );
      joConfig.Add( "english_name", "英文名称" );
      joConfig.Add( "cns", "CNS号" );
      joConfig.Add( "ins", "INS号" );
      joConfig.Add( "function", "功能" );
      return joConfig;
    }

    static JObject createFoodConfig() {
      JObject joConfig = new JObject();
      joConfig.Add( "food_id", "ID" );
      joConfig.Add( "food_category_number", "食品分类号" );
      joConfig.Add( "food_name", "食品名称" );
      joConfig.Add( "mcms", "MCMS" );
      return joConfig;
    }

    static JObject createProcessingAidsConfig() {
      JObject joConfig = new JObject();
      joConfig.Add( "chinese_name", "中文名称" );
      joConfig.Add( "english_name", "英文名称" );
      joConfig.Add( "function", "功能" );
      joConfig.Add( "scope", "使用范围" );
      return joConfig;
    }

    static JObject createEnzymeConfig() {
      JObject joConfig = new JObject();
      joConfig.Add( "id", "ID" );
      joConfig.Add( "chinese_name", "中文名称" );
      joConfig.Add( "english_name", "英文名称" );
      joConfig.Add( "source", "来源" );
      joConfig.Add( "donator", "供体" );
      joConfig.Add( "remark", "备注" );
      return joConfig;
    }

    static JObject createSpices1Config() {
      JObject joConfig = new JObject();
      joConfig.Add( "category_number", "食品分类号" );
      joConfig.Add( "name", "食品名称" );
      joConfig.Add( "category_table", "分类表" );
      return joConfig;
    }

    static JObject createSpices23Config() {
      JObject joConfig = new JObject();
      joConfig.Add( "category", "分类" );
      joConfig.Add( "chinese_name", "中文名称" );
      joConfig.Add( "english_name", "英文名称" );
      joConfig.Add( "code", "编码" );
      joConfig.Add( "fema_code", "FEMA编码" );
      joConfig.Add( "category_table", "分类表" );
      joConfig.Add( "remark", "备注" );
      return joConfig;
    }

  }

}
