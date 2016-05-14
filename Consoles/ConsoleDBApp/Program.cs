﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDBApp {
  class Program {
    static void Main( string[] args ) {
      //string connSac = @"Data Source=revenger\sqlexpress;Initial Catalog=sac;Persist Security Info=True;User ID=sa; Password=whosyourdaddy";
      string connQichacha = @"Data Source=revenger\sqlexpress;Initial Catalog=qichacha;Persist Security Info=True;User ID=sa; Password=whosyourdaddy";

      // ==== SAC ====
      //SAC.Run( connSac );
      // 2016-4-30
      //SAC.PickList_Active(); // ok
      // 2016-5-5
      //SAC.PickList_Revocatory(); // ok
      //SAC.PickDetail(); // ok

      // ==== CPBZ ====
      CPBZ.Run( connQichacha );
      // 2016-5-2
      //CPBZ.PickAreas(); // ok
      //CPBZ.PickListByArea(); // ok
      // 2016-5-12
      //CPBZ.PickRecentList( DateTime.UtcNow ); // ok
      // 2016-5-13
      CPBZ.PickDetails( @"F:\Data\企业标准信息公共服务平台\企业标准\" );

      // wait...
      Console.ReadKey();
    }

  }

}
