using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Picker.Core.Spider;
using Picker.Core.Storage;
using Picker.Core.Helpers;
using Picker.Core.Models;
using Picker.Postgresql;

namespace Picker.SqlServer {
  public partial class StoreContext : IStorage {
    //NpgsqlConnection conn = null;
    DoubanEntities doubanContext = null;
    pickerEntities fellowplusContext = null;
    const string propertyNameIsBanned = "is_banned";
    const string propertyNameType = "type";
    const string typeUser = "user";

    //public StoreContext( string connDouban, string connFellowPlus ) {
    //  OpenDoubanDatabase( connDouban );
    //}

    public void OpenDoubanDatabase( string connString ) {
      //conn = new NpgsqlConnection( connString );
      doubanContext = new DoubanEntities( connString );
    }

  }

}
