using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateConsole {
  class Program {
    static void Main( string[] args ) {
      //string conn = System.Configuration.ConfigurationManager.ConnectionStrings["Postgresql_Douban"].ConnectionString;
      string conn = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServer_Qichacha"].ConnectionString;

      // Update201505AddUserType.Run( conn ); // 2015-5-2
      // Update201505AddIsBanned.Run( conn ); // 2015-5-15
      Update201604RefreshJson.Run( conn ); // 2016-4-25

      Console.ReadKey();
    }
  }
}
