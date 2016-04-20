using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Picker.Core.Models;

namespace Picker.Core.Storage {
  public partial interface IStorage {
    void OpenQichachaDatabase( string connString );
    List<StatisticsItem> Qichacha_LoadStatistics();
    Task<int> Qichacha_SaveChanges();

    void Qichacha_SaveAlbum( string uri, string title, DateTime? modified, bool updateIfExists, bool saveChanges );
    void Qichacha_UpdateTag_Album( string uri, bool saveChanges );
    string Qichacha_GetAlbumUri_NeedToProcess();

    void Qichacha_UpdateCompanySearch( string keyword, int page, bool saveChanges );
    KeyValuePair<string, int> Qichacha_GetLast_CompanySearch();
    Task<int> Qichacha_SaveCompanyKey( string id, string name, bool updateIfExists, bool saveChanges );

  }

}
