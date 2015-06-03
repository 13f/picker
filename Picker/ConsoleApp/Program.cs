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
      Global500.PickCatalog();
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
