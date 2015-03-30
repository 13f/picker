using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Picker.Core.Storage;

namespace Picker.Core.Spider {
  public class Douban {
    DoubanApi api = null;
    IStorage store = null;

    public Douban( DoubanApi _api, IStorage storeInstance ) {
      api = _api;
      store = storeInstance;
    }

    #region store

    public void SaveBook( JObject book ) {
      if ( book == null )
        return;
      store.Douban_SaveBook( (string)book["alt"], book, true );
    }

    public void SaveBooks( Dictionary<string, JObject> books ) {
      if ( books == null || books.Count == 0 )
        return;
      store.Douban_SaveBooks( books );
    }

    #endregion store

  }

}
