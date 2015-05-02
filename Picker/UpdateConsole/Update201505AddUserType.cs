using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Picker.Postgresql;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UpdateConsole {
  public static class Update201505AddUserType {
    /// <summary>
    /// 每次批处理n条提交一次
    /// </summary>
    const int batchCount = 20;
    const string propertyName = "type";

    public static void Run( string conn ) {
      DoubanEntities db = new DoubanEntities( conn );
      Console.WriteLine( "给User表增加type字段，以区分user、virtual或site。" );
      process( db );
      Console.WriteLine( "Finished...." );
    }

    static void process( DoubanEntities db ) {
      int count = 0;
      int processedCount = 0;
      long total = db.User.LongCount();
      do {
        var data = db.User.Where( u => u.type == null || u.type.Length == 0 ).Take( batchCount );
        foreach ( User user in data ) {
          var obj = JObject.Parse( user.Content );
          if ( obj == null )
            continue;
          user.type = (string)obj[propertyName];
          processedCount++;
        }
        count = db.SaveChanges();
        Console.WriteLine( "Done: " + processedCount.ToString() + " / " + total.ToString() );
      }
      while ( count >= batchCount );
    }

  }

}
