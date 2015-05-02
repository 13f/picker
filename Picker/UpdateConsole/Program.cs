using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateConsole {
  class Program {
    static void Main( string[] args ) {
      string conn = System.Configuration.ConfigurationManager.ConnectionStrings["Postgresql_Douban"].ConnectionString;
      Update201505AddUserType.Run( conn );
      Console.ReadKey();
    }
  }
}
