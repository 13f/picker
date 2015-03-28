using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picker.Core.Storage {
  public interface IStorage {
    void OpenDatabase( string connString );

    Task<int> Douban_SaveBook( string url, string json );

    Task<int> Douban_DeleteBook( string url );

    Task<int> Douban_SaveMovie( string url, string json );

    Task<int> Douban_DeleteMovie( string url );

  }

}
