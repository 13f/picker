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

namespace Picker.Postgresql {
  public partial class StoreContext : IStorage {
    //NpgsqlConnection conn = null;
    DoubanEntities doubanContext = null;
    pickerEntities fellowplusContext = null;
    QichachaDataContext qichachaContext = null;

    const string propertyNameIsBanned = "is_banned";
    const string propertyNameType = "type";
    const string typeUser = "user";


  }

}
