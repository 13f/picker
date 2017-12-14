using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
  /// 商务部：拍卖企业
  /// </summary>
  public class AuctionChina {
    const int CountPerPage20 = 20;

    const string UriTemplate_List = "http://auc.mofcom.gov.cn/auc_new/corpFront/corpQuery.html?goto=list";
    const string UriTemplate_CorporationDetail = "http://auc.mofcom.gov.cn/auc_new/corpFront/corpQuery.html?goto=view&id={0}";

    public static void PickList( string jsonPath ) {
      Console.WriteLine( "load data file..." );
      // load
      string json = File.Exists( jsonPath ) ? File.ReadAllText( jsonPath ) : null;

      string[] CityList = new string[] {
        "11", "12", "13", "14", "15",
        "21", "22", "23",
        "31", "32", "33", "34", "35", "36", "37",
        "41", "42", "43", "44", "45", "46",
        "50", "51", "52", "53", "54",
        "61", "62", "63", "64", "65"
      };

      JObject rootResult = new JObject();
      JArray items = new JArray();
      JArray itemsAreas = new JArray();
      if(json != null) {
        rootResult = JObject.Parse( json );
        itemsAreas = (JArray)rootResult["areas"];
        items = (JArray)rootResult["items"];
      }

      WebClient client = NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();
      foreach (string cityPreifx in CityList) {
        string uri = UriTemplate_List;
        string city = cityPreifx + "0000";
        Console.Write( "  pick city: " + city );
        if (itemsAreas.Any( jt => (string)jt["id"] == city )) {
          Console.Write( "  ... Skip!" + Environment.NewLine );
          continue;
        }

        int page = 1;
        while (true) {
          NameValueCollection nvc = new NameValueCollection();
          nvc.Add( "page.page", page.ToString() );
          nvc.Add( "page.pageSize", CountPerPage20.ToString() );
          nvc.Add( "goto", "list" );
          nvc.Add( "countyId", city );

          var rd = client.UploadValues( new Uri( uri ), "POST", nvc );
          string html = System.Text.Encoding.UTF8.GetString( rd );
          doc.LoadHtml( html );
          if(page == 1) {
            JObject joArea = new JObject();
            joArea["id"] = city;
            joArea["name"] = GetSelectOptionText( doc.DocumentNode, "countyId2", city );
            itemsAreas.Add( joArea );
          }

          var lilist = doc.DocumentNode.SelectSingleNode( "//div[@class='qycxText']" )
            .SelectSingleNode( "./ul" )
            .SelectNodes( "./li" );
          if(lilist != null) {
            foreach (var li in lilist) {
              var anode = li.SelectSingleNode( "./a" );
              JObject joCorporation = new JObject();
              joCorporation["id"] = anode.Attributes["href"].Value.Replace( "/auc_new/corpFront/corpQuery.html?goto=view&id=", "" );
              joCorporation["areaId"] = city;
              joCorporation["name"] = anode.InnerText;
              items.Add( joCorporation );
            }
          }
          // wait a second
          Console.Write( "... wait 1 second" );
          Task.Delay( 1000 ).Wait();
          if (lilist == null || lilist.Count < CountPerPage20)
            break;
          page++;
        }
        rootResult["areas"] = itemsAreas;
        rootResult["items"] = items;
        LocalStorageUtility.Save( rootResult, jsonPath );
        Console.Write( "  ... Done!" + Environment.NewLine );
      }
      rootResult["areas"] = itemsAreas;
      rootResult["items"] = items;
      rootResult["count"] = items.Count;

      // save
      LocalStorageUtility.Save( rootResult, jsonPath );
      Console.WriteLine( "saved...over..." );
    }

    public static void PickDetail(string jsonPath) {
      Console.WriteLine( "load data file..." );
      // load
      string json = File.ReadAllText( jsonPath );
      JObject root = JObject.Parse( json );
      JArray items = (JArray)root["items"];
      
      WebClient client = NetHelper.GetWebClient_UTF8();
      HtmlDocument doc = new HtmlDocument();
      int count = 0;
      foreach (var item in items) {
        string uri = string.Format( UriTemplate_CorporationDetail, (string)item["id"] );
        Console.Write( "  pick corporation: " + (string)item["name"] );
        if (item["area"] != null) {
          Console.Write( "  ... Skip!" + Environment.NewLine );
          continue;
        }

        string html = client.DownloadString( uri );
        doc.LoadHtml( html );
        var tables = doc.DocumentNode
          .SelectNodes( "//table[@class='editTable']" );
        foreach(var table in tables) {
          var header = table.SelectSingleNode( "./thead" )
            .SelectSingleNode( "./tr" )
            .SelectSingleNode( "./th" );
          if (header == null)
            continue;
          string headerText = header.InnerText.Trim();
          if(headerText == "基本情况")
            GetBasicInfo( table, item );
          else if (headerText == "人员情况")
            GetPeopleInfo( table, item );
        }

        count++;
        if (count % 10 == 0) {
          // update
          root["items"] = items;
          // save
          LocalStorageUtility.Save( root, jsonPath );

          Console.Write( "  ... Saved!" + Environment.NewLine );
          // delay
          Task.Delay( 300 ).Wait();
        }
        else
          Console.Write( Environment.NewLine );

        // delay
        Task.Delay( 800 ).Wait();
      }
      
      Console.WriteLine( "saved...over..." );
    }

    static string GetSelectOptionText(HtmlNode docNode, string selectId, string optionValue) {
      if (docNode == null)
        return null;
      var option = docNode.SelectSingleNode( "//select[@id='" + selectId + "']" )
        .SelectNodes("./option")
        .Where( hn => hn.Attributes["value"].Value == optionValue )
        .FirstOrDefault();
      if (option == null)
        return null;
      return option.InnerText;
    }

    static void GetBasicInfo( HtmlNode hnTable, JToken item ) {
      // 10 rows
      var tbody = hnTable.SelectSingleNode( "./tbody" );
      var rows = tbody == null ? hnTable.SelectNodes( "./tr" ) : tbody.SelectNodes( "./tr" );
      //int index = 0;
      foreach (var row in rows) {
        //index++;
        string header = row.SelectSingleNode( "./th" ).InnerText.Trim();
        var columns = row.SelectNodes( "./td" );
        //if (header == "公司名称") { // 公司名称
        //  continue;
        //}
        if (header == "所属拍卖企业名称") { // 所属拍卖企业名称
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          item["parentCompany"] = string.IsNullOrWhiteSpace( v1 ) ? null : v1;
        }
        else if (header == "所属地区") { // 所属地区
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          item["area"] = string.IsNullOrWhiteSpace( v1 ) ? null : v1;
        }
        else if (header == "法人代表") { // 法人代表
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          item["legalEntity"] = string.IsNullOrWhiteSpace( v1 ) ? null : v1;
        }
        else if (header == "分公司负责人") { // 分公司负责人
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          item["principalOfSubcompany"] = string.IsNullOrWhiteSpace( v1 ) ? null : v1;
        }
        else if (header == "经营地址") { // 经营地址
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          item["officeAddress"] = string.IsNullOrWhiteSpace( v1 ) ? null : v1;
        }
        else if (header == "邮政编码") { // 邮政编码
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          item["postalCode"] = string.IsNullOrWhiteSpace( v1 ) ? null : v1;
        }
        else if (header.StartsWith( "注册资本" )) { // 注册资本（万元）
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          if (string.IsNullOrWhiteSpace( v1 ))
            item["registeredCapital"] = null;
          else
            item["registeredCapital"] = float.Parse( v1 );
        }
        else if (header == "经营范围") { // 经营范围
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          item["scope"] = string.IsNullOrWhiteSpace( v1 ) ? null : v1;
        }
        else if (header.StartsWith( "场地面积" )) { // 场地面积（平方米）
          var td1 = columns[0];
          var td2 = columns[1];
          JObject joSpace = new JObject();

          string v1 = td1.InnerText.Trim();
          if (string.IsNullOrWhiteSpace( v1 ))
            joSpace["own"] = null;
          else
            joSpace["own"] = float.Parse( v1 );

          string v2 = td2.InnerText.Trim();
          if (string.IsNullOrWhiteSpace( v2 ))
            joSpace["rent"] = null;
          else
            joSpace["rent"] = float.Parse( v2 );

          item["space"] = joSpace;
        }
        else if (header == "企业组织形式") { // 企业组织形式
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          item["orgType"] = string.IsNullOrWhiteSpace( v1 ) ? null : v1;
        }
        else if (header == "内外资形式") { // 内外资形式
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          item["domesticOrOverseas"] = string.IsNullOrWhiteSpace( v1 ) ? null : v1;
        }
      } // foreach(var row in rows )
    }

    static void GetPeopleInfo(HtmlNode hnTable, JToken item) {
      // 3 rows
      var tbody = hnTable.SelectSingleNode( "./tbody" );
      var rows = tbody == null ? hnTable.SelectNodes( "./tr" ) : tbody.SelectNodes( "./tr" );
      int index = 0;
      foreach (var row in rows) {
        index++;
        var columns = row.SelectNodes( "./td" );
        if (index == 1) { // 具有拍卖从业资格人员（人）
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          if (string.IsNullOrWhiteSpace( v1 ))
            item["eligiblePeople"] = null;
          else
            item["eligiblePeople"] = float.Parse( v1 );
        }
        else if (index == 2) { // 其中：拍卖师（人）
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          if (string.IsNullOrWhiteSpace( v1 ))
            item["auctioneer"] = null;
          else
            item["auctioneer"] = float.Parse( v1 );
        }
        else if (index == 3) { // 其他人员（人）
          var td1 = columns.FirstOrDefault();
          string v1 = td1.InnerText.Trim();
          if (string.IsNullOrWhiteSpace( v1 ))
            item["otherPeople"] = null;
          else
            item["otherPeople"] = float.Parse( v1 );
        }
      } // foreach(var row in rows )
    }

  }

}
