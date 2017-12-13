using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Picker.Core.Extensions;
using Picker.Core.Helpers;
using ConsoleDBApp.Models;

namespace ConsoleDBApp.Biz {
  public class LicenseBiz {
    LicenseDataContext db = null;

    public LicenseBiz(string conn) {
      db = new LicenseDataContext( conn );
    }

    public void SaveChanges() {
      db.SubmitChanges();
    }

  }

}
