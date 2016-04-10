using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Picker.Core.Models;

namespace Picker.Core.Storage {
  public partial interface IStorage {
    void OpenFellowPlusDatabase( string connString );
    Task<int> FellowPlus_SaveChanges();
    List<StatisticsItem> FellowPlus_LoadStatistics();

    string FellowPlus_SelectId_ProjectPreview_NotProcessed( int skipRandomItems );
    Task<int> FellowPlus_UpdateTag_ProjectPreview( string id, bool saveChanges );
    Task<int> FellowPlus_SaveProjectPreview( string url, JToken data, bool updateIfExists, bool saveChanges );
    Task<int> FellowPlus_SaveProjectPreviewList( JObject data, bool updateIfExists, bool saveChanges );

    Task<int> FellowPlus_SaveProjectInfo( string projectId,
      JToken tokenProject, JToken tokenCompany, JToken tokenInvest, JToken tokenWebsite, JToken tokenWeibo, JToken tokenWeixin, JToken tokenNews,
      bool updateIfExists, bool saveChanges );


  }

}
