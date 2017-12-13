using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Picker.Core.Extensions;
using Picker.Core.Helpers;

namespace ConsoleDBApp.Biz {
  // 未解决MmEwMD问题：http://www.bijishequ.com/detail/267034?p=
  public static class LicenseTask {
    const int CountPerPage20 = 20;
    const int CountPerPage15 = 15;


    const string UriTemplate_FormulaList = "http://app1.sfda.gov.cn/datasearch/face3/search.jsp?MmEwMD={0}";
    /// <summary>
    /// 0: formula id
    /// </summary>
    const string UriTemplate_FormulaDetail = "http://app1.sfda.gov.cn/datasearch/face3/content.jsp?tableId=124&tableName=TABLE124&Id={0}";
    /// <summary>
    /// 0: formula id
    /// </summary>
    const string UriTemplate_FormulaNutritionData = "http://app1.sfda.gov.cn/datasearch/face3/content.jsp?ytableId=124&tableId=125&tableName=TABLE125&linkId=COLUMN1667&Id={0}";

    static LicenseBiz biz = null;
    
    public static void Run(string conn) {
      biz = new LicenseBiz( conn );
    }

    /// <summary>
    /// 获取奶粉列表
    /// </summary>
    public static void PickInfantFormulaList() {
      WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_GB2312();
      int start = 1;
      client.Headers["cache-control"] = "no-cache";
      //if (client.Headers["Content-Type"] == null) // post
      //  client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" );

      string uri = UriTemplate_FormulaList;
      string MmEwMD = "gb2312-1_xyN9EjGbNynT3VTGiJVJdtz3MyLKThfcY1Nx8rvrJuPs3irRAxn6O1TZxsUB4GM.bCb3WSQ4sNdMcClYNdEmGpKJn2dWTneire.BDTc5jVt.s6XkHswJLRJbSypVjjBqQdlwarUSJlzybxlTJCRacvk2MClWk6xbnacBGG1CCdL1gobU_3IVjwuAYXCTrajH52GycCEiFtVKlaS1MOAINZO941DnktJ16UyQ77XKnqBe_hp_rAdYFet5AECtVEF4YPx419SJpYuQB0zPgI3dqBJjPx_ky3j9fdzY.Bfkbas922MCfm6Jae_SSp6_KF2PuahDH1S.xsy7Q.A5d_BJv2lAWQLw9EeSQEL8MhBvLg_d5dvLWfgzVvzbxD0DBOuWiW.u1ROupvRKG";

      while (true) {
        //byte[] formData = CreateFormData( start );
        string formData = CreateFormDataString( start );
        if (start == 1) {
          uri = "http://app1.sfda.gov.cn/datasearch/face3/search.jsp?tableId=124&bcId=150225369505818928066514190553&MmEwMD={0}";
          client.QueryString["tableId"] = "124";
          client.QueryString["bcId"] = "150225369505818928066514190553";
        }
        else {
          client.QueryString["tableId"] = "124";
          client.QueryString["bcId"] = "150225369505818928066514190553";
          client.QueryString["State"] = "1";
          client.QueryString["curstart"] = start.ToString();
          client.QueryString["tableName"] = "TABLE124";
          client.QueryString["viewtitleName"] = "COLUMN1654";
          client.QueryString["viewsubTitleName"] = "COLUMN1661,COLUMN1653";
          client.QueryString["tableView"] = "%E5%A9%B4%E5%B9%BC%E5%84%BF%E9%85%8D%E6%96%B9%E4%B9%B3%E7%B2%89%E4%BA%A7%E5%93%81%E9%85%8D%E6%96%B9";
          client.QueryString["cid"] = "0";
          client.QueryString["ytableId"] = "0";
          client.QueryString["searchType"] = "search";
          
        }
        //var task = client.UploadDataTaskAsync( new Uri( string.Format( uri, MmEwMD ) ), "POST", formData );
        //task.Wait();
        //var data = task.Result;
        var data = client.HttpPost( string.Format( uri, MmEwMD ), formData );
        Console.WriteLine( "Html got, page = " + start.ToString() );
        //string s = System.Text.Encoding.Default.GetString( data );

        start++;
      } // while
      
    }

    /// <summary>
    /// 获取奶粉详细信息
    /// </summary>
    public static void PickInfantFormulaDetails() {

    }

    /// <summary>
    /// 获取奶粉的营养数据
    /// </summary>
    public static void PickInfantFormulaNutritionData() {

    }

    static byte[] CreateFormData(int start) {
      StringBuilder sb = new StringBuilder();
      sb.Append( "tableId=124&State=1&bcId=150225369505818928066514190553&curstart=" + start.ToString() );
      sb.Append( "&tableName=TABLE124&viewtitleName=COLUMN1654&viewsubTitleName=COLUMN1661,COLUMN1653" );
      sb.Append( "&tableView=%25E5%25A9%25B4%25E5%25B9%25BC%25E5%2584%25BF%25E9%2585%258D%25E6%2596%25B9%25E4%25B9%25B3%25E7%25B2%2589%25E4%25BA%25A7%25E5%2593%2581%25E9%2585%258D%25E6%2596%25B9" );
      sb.Append( "&cid=0&ytableId=0&searchType=search" );
      string encoded = System.Net.WebUtility.UrlEncode( sb.ToString() );
      var fd = System.Text.Encoding.UTF8.GetBytes( encoded );
      return fd;
    }

    static string CreateFormDataString(int start) {
      StringBuilder sb = new StringBuilder();
      sb.Append( "tableId=124&State=1&bcId=150225369505818928066514190553&curstart=" + start.ToString() );
      sb.Append( "&tableName=TABLE124&viewtitleName=COLUMN1654&viewsubTitleName=COLUMN1661,COLUMN1653" );
      sb.Append( "&tableView=%25E5%25A9%25B4%25E5%25B9%25BC%25E5%2584%25BF%25E9%2585%258D%25E6%2596%25B9%25E4%25B9%25B3%25E7%25B2%2589%25E4%25BA%25A7%25E5%2593%2581%25E9%2585%258D%25E6%2596%25B9" );
      sb.Append( "&cid=0&ytableId=0&searchType=search" );
      return sb.ToString();
    }

  }

}
