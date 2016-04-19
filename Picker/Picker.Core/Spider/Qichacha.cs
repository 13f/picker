using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Picker.Core.Storage;

namespace Picker.Core.Spider {
  public class Qichacha {
    IStorage store = null;
    WebClient client = null;

    int countPerPage = 20;

    public Qichacha( IStorage _store ) {
      store = _store;
      client = Helpers.NetHelper.GetWebClient_UTF8();
    }

    public void PickCompanyList() {

    }

  }

}
