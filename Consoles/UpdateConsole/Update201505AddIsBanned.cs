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
  public static class Update201505AddIsBanned {
    /// <summary>
    /// 每次批处理n条提交一次
    /// </summary>
    const int batchCount = 20;
    const string propertyNameType = "type";
    const string propertyNameIsBanned = "is_banned";

    public static void Run( string conn ) {
      DoubanEntities db = new DoubanEntities( conn );
      Console.WriteLine( "给User和UserTask表增加type和IsBanned字段 @2015-5-15。" );
      process( db );
      Console.WriteLine( "Finished...." );
    }

    static void process( DoubanEntities db ) {
      int count = 0;
      int processedCount = 0;
      long total = db.User.LongCount();
      do {
        var data = db.User.OrderBy( i => i.id ).Skip( processedCount ).Take( batchCount );
        foreach ( User user in data ) {
          //var task = db.UserTask.Where( i => i.id == user.id ).FirstOrDefault();
          var obj = JObject.Parse( user.Content );
          if ( obj == null )
            continue;
          user.type = (string)obj[propertyNameType];
          if ( user.type == "user" )
            user.IsBanned = (bool)obj[propertyNameIsBanned];
          //task.type = user.type;
          //task.IsBanned = user.IsBanned;
          processedCount++;
        }
        db.SaveChanges();
        count = data.Count();
        Console.WriteLine( "Done: " + processedCount.ToString() + " / " + total.ToString() );
      }
      while ( count >= batchCount );
    }

  }

}
