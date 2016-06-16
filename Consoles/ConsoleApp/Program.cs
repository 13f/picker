using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Picker.Core;
using Picker.Core.Spider;
using Picker.Core.Storage;
using Picker.Postgresql;

namespace ConsoleApp {
  class Program {
    static void Main( string[] args ) {
      // test1(); // ok @ 2015-3-29
      // test2(); // ok @ 2015-3-29
      // test3();
      // Food911cha.PickItems(); // ok @ 2015-5-8
      // Food911cha.PickItemsData(); // ok @ 2015-5-8
      // PeopleComCN.PickMedicineOrgs(); ok @2015-5-9
      //Global500.PickCatalog(); // ok @2015-9-28  // file = @"D:\github\picker\data\organization\global500-2014.xml", tableKey = "<table class=\"rankingtable\" id=\"yytable\"".

      // 1 food: xml->json // ok @2015-12-26
      //string source = @"E:\github\picker\data\food\food.xml";
      //string target = @"E:\github\picker\data\food\food.json";
      //Console.WriteLine( source + " -> " + target );
      //Food911cha.XmlToJson( source, target );
      //Console.WriteLine( "Over..." );

      // 2 food: xml->json // ok @2015-12-26
      //string source = @"E:\github\picker\data\food\food.xml";
      //string target = @"E:\github\picker\data\food\food";
      //Console.WriteLine( source + " -> " + target );
      //Food911cha.XmlToJson2( source, target );
      //Console.WriteLine( "Over..." );

      // Global-2015 // ok @2015-12-17
      //string tableKey = "<table id=\"yytable\" class=\"rankingtable\"";
      //string xmlFile = @"E:\github\picker\data\organization\global500-2015.xml";
      //string jsonFile = @"E:\github\picker\data\organization\global500-2015.json";
      //Global500.PickCatalog( xmlFile, tableKey );
      //Console.WriteLine( xmlFile + " -> " + jsonFile );
      //XElementHelper.XmlToJson( xmlFile, jsonFile, true );
      //Console.WriteLine( "Over..." );

      // Global-2014 // ok @2015-12-17
      //string source = @"E:\github\picker\data\organization\global500-2014.xml";
      //string target = @"E:\github\picker\data\organization\global500-2014.json";
      //Console.WriteLine( source + " -> " + target );
      //XElementHelper.XmlToJson( source, target, true );
      //Console.WriteLine( "Over..." );

      //// Global-2013 // ok @2016-1-23
      //string tableKey = "<table class=\"rankingtable\" id=\"yytable\"";
      //string file = @"E:\github\picker\data\organization\global500-2013";
      //Global500.PickToJson( "http://www.fortunechina.com/fortune500/c/2013-07/08/2013G500.htm", tableKey, "rank2013", "rank2012", file );

      // Global-2012 // 
      //string tableKey = "<table class=\"rankingtable\" cellspacing=";
      //string file = @"E:\github\picker\data\organization\global500-2012";
      //Global500.PickToJsonBefore2013( "http://www.fortunechina.com/fortune500/c/2012-07/09/content_106535.htm", tableKey, "rank2012", "rank2011", file );

      // emoji // ok @2016-4-27
      //string targetDir = @"E:\github\picker\data\emotion\";
      //EmojiCheatSheet.Run( targetDir, targetDir + "images\\" );

      // test: qichacha.com
      //testQichacha_GetTrademarkList();
      //testQichacha_Post_Trademark(); // ok
      //testQichacha_Post_Patent(); // ok
      //testQichacha_Post_Certificate(); // ok
      //Console.WriteLine( "Over..." );

      // test: http://spaqbz.zjwst.gov.cn/zjfsdata/html/eStandardList.jsp
      //testPost_EnterpriseStandard_Zhejiang_Food(); // ok
      // @2016-4-27
      //FoodSecurityStandard.PickItems_Zhejiang( @"F:\Data\浙江食品安全标准\企业标准.json" ); // ok
      //FoodSecurityStandard.PickList_Standards( @"F:\Data\食品安全标准\list.json" ); // ok
      //FoodSecurityStandard.PickDetails_Standards( @"F:\Data\食品安全标准\GB-list-v6.json", @"F:\Data\食品安全标准\国家标准\", 3000 ); // ok
      //FoodSecurityStandard.DownloadPdf_Standard( @"F:\Data\食品安全标准\GB-list-v1.json", @"F:\Data\食品安全标准\国家标准\", "GB 1886.148-2015" ); // ok
      // @2016-4-28
      //FoodSecurityStandard.CheckNotDownloaded_Standards( @"F:\Data\食品安全标准\GB-list-v6.json", @"F:\Data\食品安全标准\国家标准\" ); // ok
      // @2016-4-29
      //FoodSecurityStandard.PickList_LocalStandards( @"F:\Data\食品安全标准\DB-list.json" ); // ok
      //FoodSecurityStandard.PickDetails_Standards( @"F:\Data\食品安全标准\DB-list-v2.json", @"F:\Data\食品安全标准\地方标准\", true, 3000 ); // ok
      //FoodSecurityStandard.UpdateReplaceOrReference_Standards( @"F:\Data\食品安全标准\GB-list-v7.json", 3000 ); // ok
      //FoodSecurityStandard.UpdateReplaceOrReference_Standards( @"F:\Data\食品安全标准\DB-list-v3.json", 3000 );

      // @2016-4-30
      //Additives_GB2760.PickAdditivesList( @"F:\Data\食品安全标准\GB 2760-2014_additives.json", 3000 ); // ok
      //Additives_GB2760.PickAdditivesDetail( @"F:\Data\食品安全标准\GB 2760-2014_additives-v2.json", 3000 ); // ok
      //Additives_GB2760.PickFoodsList( @"F:\Data\食品安全标准\GB 2760-2014_foods-v1.json", 3000 ); // ok
      //Additives_GB2760.PickFoodsDetail( @"F:\Data\食品安全标准\GB 2760-2014_foods-v2.json", 3000 ); // ok

      // @2016-5-1
      //Additives_GB2760.PickProcessingAids( @"F:\Data\食品安全标准\GB 2760-2014_加工助剂-v1.json" ); // ok
      //Additives_GB2760.PickEnzymeList( @"F:\Data\食品安全标准\GB 2760-2014_酶制剂-v1.json" ); // ok
      //Additives_GB2760.PickSpices1List( @"F:\Data\食品安全标准\GB 2760-2014_表B.1_不得添加食品用香料香精的食品名单-v1.json" ); // ok
      //Additives_GB2760.PickSpices2List( @"F:\Data\食品安全标准\GB 2760-2014_表B.2_允许使用的食品用天然香料名单-v1.json" ); // ok
      //Additives_GB2760.PickSpices3List( @"F:\Data\食品安全标准\GB 2760-2014_表B.3_允许使用的食品用合成香料名单-v1.json" ); // ok

      // @2016-5-16
      //GallantlabHuth2016.FilterDistinctData( @"F:\Data\gallantlab.org\huth2016\pragmatic_areas.v0.json", @"F:\Data\gallantlab.org\huth2016\pragmatic_areas.v1.json" ); // ok
      //GallantlabHuth2016.PickAreasData( @"F:\Data\gallantlab.org\huth2016\pragmatic_areas.v1.json", @"F:\Data\gallantlab.org\huth2016\areas-data\" ); // ok
      //GallantlabHuth2016.MergeParts( @"F:\Data\gallantlab.org\huth2016\areas-data\", @"F:\Data\gallantlab.org\huth2016\areas-data\" ); // ok

      // @2016-5-17,18,19
      //JBK39.PickSymptomList( @"F:\Data\jbk.39.net\" ); // ok
      //JBK39.PickSymptomDetails( @"F:\Data\jbk.39.net\symptom-data-v3.json", @"F:\Data\jbk.39.net\symptom-data-v4.json" ); // ok
      //JBK39.PickDiseaseList( @"F:\Data\jbk.39.net\" ); // ok

      // @2016-5-27
      //Facemood.PickList( @"F:\Data\emotion\facemood.grtimed.com\" ); // ok
      // @2016-6-9
      //Facemood.PickCategoryDetails( @"F:\Data\emotion\facemood.grtimed.com\facemood-v2.json", @"F:\Data\emotion\facemood.grtimed.com\facemood-v3.json" ); // ok

      // wait for input
      Console.ReadKey();
    }

    ///// <summary>
    ///// 测试JSON的用法；测试读取某人收藏的书籍
    ///// </summary>
    //static void test1() {
    //  Picker.Core.Spider.DoubanApi douban = new Picker.Core.Spider.DoubanApi();
    //  var task = douban.GetMyBookCollections( "taurenshaman", 0 );
    //  Task.WaitAll( task );
    //  Console.Write( task.Result );
    //  Console.ReadKey();
    //}

    ///// <summary>
    ///// 测试读取某个系列的书籍
    ///// </summary>
    //static void test2() {
    //  Picker.Core.Spider.DoubanApi douban = new Picker.Core.Spider.DoubanApi();
    //  var task = douban.GetBooksOfSerie( "1163", 0 );
    //  Task.WaitAll( task );
    //  Console.Write( task.Result );
    //  Console.ReadKey();
    //}

    //static void test3() {
    //  string executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
    //  string executableFolder = System.IO.Path.GetDirectoryName( executablePath );
    //  var config = new Configuration( executableFolder );

    //  //var store = new StoreContext( "name=Douban" );
    //  var store = new StoreContext( "metadata=res://*/DoubanModel.csdl|res://*/DoubanModel.ssdl|res://*/DoubanModel.msl;provider=Npgsql;provider connection string=&quot;PORT=5432;TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=20;COMMANDTIMEOUT=20;COMPATIBLE=2.2.5.0;HOST=localhost;DATABASE=douban;USER ID=postgres;PASSWORD=whosyourdaddy&quot;" );
    //  var api = new DoubanApi( "" );
    //  var biz = new Douban( api, store, config );

    //  var task = biz.StartUserTask( false );
    //  Task.WaitAll( task );
    //  Console.ReadKey();
    //}

    /// <summary>
    /// failed.
    /// </summary>
    /// <returns></returns>
    static async Task<string> testQichacha() {
      string urlShangbiao = "http://www.qichacha.com/company_shangbiaoView?id=TUQMTRULRNML";
      System.Collections.Specialized.NameValueCollection query = new System.Collections.Specialized.NameValueCollection();
      query.Add( "pspt", "%7B%22id%22%3A%221080040%22%2C%22pswd%22%3A%228835d2c1351d221b4ab016fbf9e8253f%22%2C%22_code%22%3A%22ead7a9fe7d7a71179c49b157d9cf3099%22%7D" );

      System.Net.WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();
      client.QueryString = query;
      string r = await client.DownloadStringTaskAsync( urlShangbiao );
      Console.WriteLine( "====" );
      Console.WriteLine( r );
      Console.WriteLine( "====" );
      return r;
    }

    static void testQichacha_Get() {
      string urlShangbiao = "http://www.qichacha.com/company_shangbiaoView";

      //System.Collections.Specialized.NameValueCollection cookie = new System.Collections.Specialized.NameValueCollection();
      //cookie.Add( "pspt", "%7B%22id%22%3A%221080040%22%2C%22pswd%22%3A%228835d2c1351d221b4ab016fbf9e8253f%22%2C%22_code%22%3A%22ead7a9fe7d7a71179c49b157d9cf3099%22%7D" );

      System.Collections.Specialized.NameValueCollection query = new System.Collections.Specialized.NameValueCollection();
      query.Add( "id", "TUQMTRULRNML" );

      System.Net.WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      byte[] buff = client.DownloadData( "http://qichacha.com/" );
      string cookie = client.ResponseHeaders.Get( "Set-Cookie" );
      cookie = cookie + ";pspt=%7B%22id%22%3A%221080040%22%2C%22pswd%22%3A%228835d2c1351d221b4ab016fbf9e8253f%22%2C%22_code%22%3A%22ead7a9fe7d7a71179c49b157d9cf3099%22%7D";

      client.Headers.Add( "Cookie", cookie );
      client.QueryString = query;
      client.DownloadStringCompleted += Client_DownloadStringCompleted;
      client.DownloadStringAsync( new Uri( urlShangbiao ) );
    }

    private static void Client_DownloadStringCompleted( object sender, System.Net.DownloadStringCompletedEventArgs e ) {
      Console.WriteLine( "====" );
      Console.WriteLine( e.Result );
      Console.WriteLine( "====" );
    }

    static void testQichacha_GetTrademarkList() {
      string urlShangbiao = "http://www.qichacha.com/company_shangbiao?unique=3f603703d59a04cbe427e5825099a565&companyname=百度在线网络技术（北京）有限公司&p=1";
      System.Net.WebClient clientTrademarkList = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      byte[] buff = clientTrademarkList.DownloadData( "http://qichacha.com/" );
      string cookie = clientTrademarkList.ResponseHeaders.Get( "Set-Cookie" );
      cookie = cookie + ";pspt=%7B%22id%22%3A%221080040%22%2C%22pswd%22%3A%228835d2c1351d221b4ab016fbf9e8253f%22%2C%22_code%22%3A%22ead7a9fe7d7a71179c49b157d9cf3099%22%7D";

      clientTrademarkList.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // post
      clientTrademarkList.Headers.Add( "Cookie", cookie );

      clientTrademarkList.DownloadStringCompleted += ClientTrademarkList_DownloadStringCompleted;
      clientTrademarkList.DownloadStringAsync( new Uri( urlShangbiao ) );
    }

    private static void ClientTrademarkList_DownloadStringCompleted( object sender, System.Net.DownloadStringCompletedEventArgs e ) {
      // sbview("PUNNRTSMUTQL")
      string Regex_Trademark = "sbview\\(\"\\w+\"\\)";
      string regex2 = "(?<=sbview\\(\")\\w+(?=\"\\))";
      var matchlist = Regex.Matches( e.Result, Regex_Trademark );
      if ( matchlist != null || matchlist.Count > 0 ) {
        Console.WriteLine( "count: " + matchlist.Count );
        foreach ( Match m in matchlist ) {
          if ( !m.Success )
            continue;
          string itemId = m.Value.Substring( 8, m.Value.Length - 10 ); // 10 = 8 + 2
          Console.WriteLine( itemId );
        }
      }

    }

    static void testQichacha_Post_Trademark() {
      string urlShangbiao = "http://www.qichacha.com/company_shangbiaoView";

      string postString = "id=MLLLLLMQOPTMO"; // TUQMTRULRNML, QUTQLSUTMONM, MLLLLLMQOPTMO
      byte[] postData = Encoding.UTF8.GetBytes( postString );

      System.Net.WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      byte[] buff = client.DownloadData( "http://qichacha.com/" );
      string cookie = client.ResponseHeaders.Get( "Set-Cookie" );
      cookie = cookie + ";pspt=%7B%22id%22%3A%221080040%22%2C%22pswd%22%3A%228835d2c1351d221b4ab016fbf9e8253f%22%2C%22_code%22%3A%22ead7a9fe7d7a71179c49b157d9cf3099%22%7D";

      client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // post
      client.Headers.Add( "Cookie", cookie );

      byte[] responseData = client.UploadData( urlShangbiao, "POST", postData ); // 得到返回字符流
      string result = Encoding.UTF8.GetString( responseData ); // 解码
      Console.WriteLine( "====" );
      Console.WriteLine( result );
      Console.WriteLine( "====" );
    }

    static void testQichacha_Post_Patent() {
      string urlShangbiao = "http://www.qichacha.com/company_zhuanliView";

      string postString = "id=f2ed03cad538a5750a3eef6ffed1a630";//P_SIPO-CN105427281A, 9a764483f3b5e1dec9b7769b7c3eb869
      byte[] postData = Encoding.UTF8.GetBytes( postString );

      System.Net.WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      byte[] buff = client.DownloadData( "http://qichacha.com/" );
      string cookie = client.ResponseHeaders.Get( "Set-Cookie" );
      cookie = cookie + ";pspt=%7B%22id%22%3A%221080040%22%2C%22pswd%22%3A%228835d2c1351d221b4ab016fbf9e8253f%22%2C%22_code%22%3A%22ead7a9fe7d7a71179c49b157d9cf3099%22%7D";

      client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // post
      client.Headers.Add( "Cookie", cookie );

      byte[] responseData = client.UploadData( urlShangbiao, "POST", postData ); // 得到返回字符流
      string result = Encoding.UTF8.GetString( responseData ); // 解码
      Console.WriteLine( "====" );
      Console.WriteLine( result );
      Console.WriteLine( "====" );
    }

    static void testQichacha_Post_Certificate() {
      string urlShangbiao = "http://www.qichacha.com/company_zhengshuView";

      string postString = "id=C_264-CQC13001103798"; // C_264-CQC13001103798, C_262-2016010101835848
      byte[] postData = Encoding.UTF8.GetBytes( postString );

      System.Net.WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_UTF8();

      byte[] buff = client.DownloadData( "http://qichacha.com/" );
      string cookie = client.ResponseHeaders.Get( "Set-Cookie" );
      cookie = cookie + ";pspt=%7B%22id%22%3A%221080040%22%2C%22pswd%22%3A%228835d2c1351d221b4ab016fbf9e8253f%22%2C%22_code%22%3A%22ead7a9fe7d7a71179c49b157d9cf3099%22%7D";

      client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // post
      client.Headers.Add( "Cookie", cookie );

      byte[] responseData = client.UploadData( urlShangbiao, "POST", postData ); // 得到返回字符流
      string result = Encoding.UTF8.GetString( responseData ); // 解码
      Console.WriteLine( "====" );
      Console.WriteLine( result );
      Console.WriteLine( "====" );
    }

    static void testPost_EnterpriseStandard_Zhejiang_Food() {
      string uri = "http://spaqbz.zjwst.gov.cn/zjfsdata/proxy/eStandProxy.jsp?startrecord=21&endrecord=40&perpage=20&totalRecord=5424";

      var gbk = Encoding.GetEncoding( "gbk" );

      string postString = "id2=F65FC0275F670896";
      byte[] postData = gbk.GetBytes( postString );
      
      System.Net.WebClient client = Picker.Core.Helpers.NetHelper.GetWebClient_GBK();

      string cookie = "JSESSIONID=94F92A1951D5D01F93A1633996047FD2";
      client.Headers.Add( "Content-Type", "application/x-www-form-urlencoded" ); // post
      client.Headers.Add( "Cookie", cookie );

      byte[] responseData = client.UploadData( uri, "POST", postData ); // 得到返回字符流
      string result = gbk.GetString( responseData ); // 解码
      Console.WriteLine( "====" );
      Console.WriteLine( result );
      Console.WriteLine( "====" );
    }
    

  }

}
