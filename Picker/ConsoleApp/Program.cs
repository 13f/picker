using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
      //Console.ReadKey();

      // 2 food: xml->json // ok @2015-12-26
      //string source = @"E:\github\picker\data\food\food.xml";
      //string target = @"E:\github\picker\data\food\food";
      //Console.WriteLine( source + " -> " + target );
      //Food911cha.XmlToJson2( source, target );
      //Console.WriteLine( "Over..." );
      //Console.ReadKey();

      // Global-2015 // ok @2015-12-17
      //string tableKey = "<table id=\"yytable\" class=\"rankingtable\"";
      //string xmlFile = @"E:\github\picker\data\organization\global500-2015.xml";
      //string jsonFile = @"E:\github\picker\data\organization\global500-2015.json";
      //Global500.PickCatalog( xmlFile, tableKey );
      //Console.WriteLine( xmlFile + " -> " + jsonFile );
      //XElementHelper.XmlToJson( xmlFile, jsonFile, true );
      //Console.WriteLine( "Over..." );
      //Console.ReadKey();

      // Global-2014 // ok @2015-12-17
      //string source = @"E:\github\picker\data\organization\global500-2014.xml";
      //string target = @"E:\github\picker\data\organization\global500-2014.json";
      //Console.WriteLine( source + " -> " + target );
      //XElementHelper.XmlToJson( source, target, true );
      //Console.WriteLine( "Over..." );
      //Console.ReadKey();

      //// Global-2013 // ok @2016-1-23
      //string tableKey = "<table class=\"rankingtable\" id=\"yytable\"";
      //string file = @"E:\github\picker\data\organization\global500-2013";
      //Global500.PickToJson( "http://www.fortunechina.com/fortune500/c/2013-07/08/2013G500.htm", tableKey, "rank2013", "rank2012", file );
      //Console.ReadKey();

      // Global-2012 // 
      string tableKey = "<table class=\"rankingtable\" cellspacing=";
      string file = @"E:\github\picker\data\organization\global500-2012";
      Global500.PickToJsonBefore2013( "http://www.fortunechina.com/fortune500/c/2012-07/09/content_106535.htm", tableKey, "rank2012", "rank2011", file );
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

  }

}
