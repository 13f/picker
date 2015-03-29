using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Picker.Core;

namespace ConsoleApp {
  class Program {
    static void Main( string[] args ) {
      // test1(); // 0k @ 2015-3-29
      // test2(); // 0k @ 2015-3-29
    }

    /// <summary>
    /// 测试JSON的用法；测试读取某人收藏的书籍
    /// </summary>
    static void test1() {
      Picker.Core.Spider.DoubanApi douban = new Picker.Core.Spider.DoubanApi();
      var task = douban.GetMyBookCollections( "taurenshaman", 0 );
      Task.WaitAll( task );
      Console.Write( task.Result );
      Console.ReadKey();
    }

    /// <summary>
    /// 测试读取某个系列的书籍
    /// </summary>
    static void test2() {
      Picker.Core.Spider.DoubanApi douban = new Picker.Core.Spider.DoubanApi();
      var task = douban.GetBooksOfSerie( "1163", 0 );
      Task.WaitAll( task );
      Console.Write( task.Result );
      Console.ReadKey();
    }

  }

}
