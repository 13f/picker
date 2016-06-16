using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveApp {
  class Program {
    static void Main( string[] args ) {
      // 2016-6-11
      Emoji.Run( @"F:\Data\emotion\emoji-cheat-sheet.com\emoji-cheat-sheet-v2.json", @"F:\Data\emotion\emoji-cheat-sheet.com\emoji-cheat-sheet-v3.json" );

      Console.ReadKey();
    }

  }

}
