using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Picker.Core.Helpers {
  public static class LocalStorageUtility {

    public static void Save(JObject jo, string path) {
      string json = jo.ToString( Newtonsoft.Json.Formatting.Indented );
      File.WriteAllText( path, json );
    }

  }

}