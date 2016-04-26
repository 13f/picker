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
  public static class Update201604RefreshJson {
    /// <summary>
    /// 每次批处理n条提交一次
    /// </summary>
    const int batchCount = 20;

    public static void Run( string conn ) {
      QichachaDataContext db = new QichachaDataContext( conn );
      Console.WriteLine( "刷新Qichacha.Trademark表的Content字段，以将Unicode自动转为中文。" );
      process( db );
      Console.WriteLine( "Finished...." );
    }

    static void process( QichachaDataContext db ) {
      int count = 0;
      int processedCount = 0;
      DateTime now = DateTime.UtcNow;

      Console.WriteLine( "wait 3 seconds...." );
      Task.Delay( 3000 );
      Console.WriteLine( "start...." );

      long total = db.QichachaTrademark
        .Where( i => i.Content != null && i.Content != "----" )
        .LongCount();
      do {
        count = 0;
        var data = db.QichachaTrademark.Where( u => u.Content != null && u.Content != "----" && u.UpdatedAt != null && u.UpdatedAt.Value < now ).Take( batchCount );
        foreach ( QichachaTrademark item in data ) {
          var obj = JObject.Parse( item.Content );
          if ( obj == null )
            continue;
          item.Content = obj.ToString( Formatting.Indented );
          item.UpdatedAt = DateTime.UtcNow;
          processedCount++;
          count++;
        }
        db.SubmitChanges();
        Console.WriteLine( "Done: " + processedCount.ToString() + " / " + total.ToString() );
      }
      while ( count >= batchCount );
    }

  }

}
