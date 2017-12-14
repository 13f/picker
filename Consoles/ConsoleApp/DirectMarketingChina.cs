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
  /// 商务部：直销企业
  /// </summary>
  public class DirectMarketingChina {
    const int CountPerPage30 = 30;

    /// <summary>
    /// 直销企业基本信息。0：直销企业的licenseNumber
    /// </summary>
    const string UriTemplate_EnterpriseInfo = "http://zxgl.mofcom.gov.cn/front/getEnterpriseInfo/{0}";
    /// <summary>
    /// 直销区域->分支机构。0：直销企业的licenseNumber
    /// </summary>
    const string UriTemplate_Affiliates = "http://zxgl.mofcom.gov.cn/front/corpBranchJSON?filter_EQS_enterpriseLicense={0}";
    /// <summary>
    /// 直销区域->服务网点。0：直销企业的licenseNumber
    /// </summary>
    const string UriTemplate_ServiceOutlets = "http://zxgl.mofcom.gov.cn/front/corpNetworkJSON?filter_EQS_enterpriseLicense={0}&filter_EQI_status={1}";
    /// <summary>
    /// 直销产品。0：直销企业的licenseNumber
    /// </summary>
    const string UriTemplate_Products = "http://zxgl.mofcom.gov.cn/front/corpProductJSON?enterpriseLicense={0}";
    /// <summary>
    /// 直销培训员。0：直销企业的licenseNumber
    /// </summary>
    const string UriTemplate_Trainers = "http://zxgl.mofcom.gov.cn/front/corpTrainersJSON?filter_EQS_enterpriseLicense={0}&filter_EQI_status={1}";

    public static void PickAffiliates( string jsonPathEnterpriseList, string jsonPathAffiliates) {
      Console.WriteLine( "load data file..." );
      // load
      string jsonSource = File.ReadAllText( jsonPathEnterpriseList );
      JObject rootSource = JObject.Parse( jsonSource );
      JArray itemsEnterprise = (JArray)rootSource["items"];

      JObject rootResult = new JObject();
      JArray items = new JArray();

      WebClient client = NetHelper.GetWebClient_UTF8();
      foreach (var enterprise in itemsEnterprise) {
        string uri = string.Format( UriTemplate_Affiliates, (string)enterprise["licenseNumber"] );
        Console.Write( "  pick: " + (string)enterprise["licenseNumber"] );

        int page = 1;
        while (true) {
          NameValueCollection nvc = new NameValueCollection();
          nvc.Add( "page", page.ToString() );
          nvc.Add( "rows", CountPerPage30.ToString() );
          var rd = client.UploadValues( new Uri( uri ), "POST", nvc );
          string response = System.Text.Encoding.UTF8.GetString( rd );

          JObject rootResponse = JObject.Parse( response );
          JArray itemsResponse = (JArray)rootResponse["rows"];
          
          foreach (var ir in itemsResponse) {
            items.Add( ir );
          }
          
          if (itemsResponse.Count < CountPerPage30)
            break;
          page++;
        }
        Console.Write( "  ... done!" + Environment.NewLine );
      }
      rootResult["items"] = items;
      rootResult["count"] = items.Count;

      // save
      LocalStorageUtility.Save( rootResult, jsonPathAffiliates );
      Console.WriteLine( "saved...over..." );
    }

    public static void PickServiceOutlets(string jsonPathEnterpriseList, string jsonPathServiceOutlets, int status) {
      Console.WriteLine( "load data file..." );
      // load
      string jsonSource = File.ReadAllText( jsonPathEnterpriseList );
      JObject rootSource = JObject.Parse( jsonSource );
      JArray itemsEnterprise = (JArray)rootSource["items"];

      JObject rootResult = new JObject();
      JArray items = new JArray();

      WebClient client = NetHelper.GetWebClient_UTF8();
      foreach (var enterprise in itemsEnterprise) {
        string uri = string.Format( UriTemplate_ServiceOutlets, (string)enterprise["licenseNumber"], status );
        Console.Write( "  pick: " + (string)enterprise["licenseNumber"] );

        int page = 1;
        while (true) {
          NameValueCollection nvc = new NameValueCollection();
          nvc.Add( "page", page.ToString() );
          nvc.Add( "rows", CountPerPage30.ToString() );
          var rd = client.UploadValues( new Uri( uri ), "POST", nvc );
          string response = System.Text.Encoding.UTF8.GetString( rd );

          JObject rootResponse = JObject.Parse( response );
          JArray itemsResponse = (JArray)rootResponse["rows"];

          foreach (var ir in itemsResponse) {
            items.Add( ir );
          }

          if (itemsResponse.Count < CountPerPage30)
            break;
          page++;
        }
        Console.Write( "  ... done!" + Environment.NewLine );
      }
      rootResult["items"] = items;
      rootResult["count"] = items.Count;

      // save
      LocalStorageUtility.Save( rootResult, jsonPathServiceOutlets );
      Console.WriteLine( "saved...over..." );
    }

    public static void PickProducts(string jsonPathEnterpriseList, string jsonPathProducts) {
      Console.WriteLine( "load data file..." );
      // load
      string jsonSource = File.ReadAllText( jsonPathEnterpriseList );
      JObject rootSource = JObject.Parse( jsonSource );
      JArray itemsEnterprise = (JArray)rootSource["items"];

      JObject rootResult = new JObject();
      JArray items = new JArray();

      WebClient client = NetHelper.GetWebClient_UTF8();
      foreach (var enterprise in itemsEnterprise) {
        string uri = string.Format( UriTemplate_Products, (string)enterprise["licenseNumber"] );
        Console.Write( "  pick: " + (string)enterprise["licenseNumber"] );

        int page = 1;
        while (true) {
          NameValueCollection nvc = new NameValueCollection();
          nvc.Add( "page", page.ToString() );
          nvc.Add( "rows", CountPerPage30.ToString() );
          var rd = client.UploadValues( new Uri( uri ), "POST", nvc );
          string response = System.Text.Encoding.UTF8.GetString( rd );

          JObject rootResponse = JObject.Parse( response );
          JArray itemsResponse = (JArray)rootResponse["rows"];

          foreach (var ir in itemsResponse) {
            items.Add( ir );
          }

          if (itemsResponse.Count < CountPerPage30)
            break;
          page++;
        }
        Console.Write( "  ... done!" + Environment.NewLine );
      }
      rootResult["items"] = items;
      rootResult["count"] = items.Count;

      // save
      LocalStorageUtility.Save( rootResult, jsonPathProducts );
      Console.WriteLine( "saved...over..." );
    }

    public static void PickTrainers(string jsonPathEnterpriseList, string jsonPathTrainers, int status) {
      Console.WriteLine( "load data file..." );
      // load
      string jsonSource = File.ReadAllText( jsonPathEnterpriseList );
      JObject rootSource = JObject.Parse( jsonSource );
      JArray itemsEnterprise = (JArray)rootSource["items"];

      JObject rootResult = new JObject();
      JArray items = new JArray();

      WebClient client = NetHelper.GetWebClient_UTF8();
      foreach (var enterprise in itemsEnterprise) {
        string uri = string.Format( UriTemplate_Trainers, (string)enterprise["licenseNumber"], status );
        Console.Write( "  pick: " + (string)enterprise["licenseNumber"] );

        int page = 1;
        while (true) {
          NameValueCollection nvc = new NameValueCollection();
          nvc.Add( "page", page.ToString() );
          nvc.Add( "rows", CountPerPage30.ToString() );
          var rd = client.UploadValues( new Uri( uri ), "POST", nvc );
          string response = System.Text.Encoding.UTF8.GetString( rd );

          JObject rootResponse = JObject.Parse( response );
          JArray itemsResponse = (JArray)rootResponse["rows"];

          foreach (var ir in itemsResponse) {
            items.Add( ir );
          }

          if (itemsResponse.Count < CountPerPage30)
            break;
          page++;
        }
        Console.Write( "  ... done!" + Environment.NewLine );
      }
      rootResult["items"] = items;
      rootResult["count"] = items.Count;

      // save
      LocalStorageUtility.Save( rootResult, jsonPathTrainers );
      Console.WriteLine( "saved...over..." );
    }

  }

}