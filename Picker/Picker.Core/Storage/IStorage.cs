using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Picker.Core.Storage {
  public interface IStorage {
    void OpenDatabase( string connString );

    Task<int> Douban_SaveBook( string url, JObject data, bool saveChanges );

    Task<int> Douban_SaveBooks( Dictionary<string, JObject> data );

    Task<int> Douban_DeleteBook( string url );

    Task<int> Douban_SaveMovie( string url, JObject data, bool saveChanges );

    Task<int> Douban_DeleteMovie( string url );

  }

}
