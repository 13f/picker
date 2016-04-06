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
  public partial class StoreContext {

    public void OpenFellowPlusDatabase( string connString ) {
      fellowplusContext = new pickerEntities( connString );
    }

    public async Task<int> FellowPlus_SaveChanges() {
      int r = await fellowplusContext.SaveChangesAsync();
      return r;
    }

    public List<StatisticsItem> FellowPlus_LoadStatistics() {
      List<StatisticsItem> data = new List<StatisticsItem>();
      long projectsPreviewCount = fellowplusContext.FellowPlusProjectPreview.LongCount();
      data.Add( new StatisticsItem() {
        Key = "projects-preview",
        Title = "Projects Preview",
        Count = projectsPreviewCount
      } );

      return data;
    }

    public async Task<int> FellowPlus_SaveProjectPreview( string id, JToken data, bool updateIfExists, bool saveChanges ) {
      FellowPlusProjectPreview item = fellowplusContext.FellowPlusProjectPreview.Where( i => i.Id == id ).FirstOrDefault();
      if ( item != null && updateIfExists ) {
        item.Content = data.ToString();
        item.UpdatedAt = DateTime.UtcNow;
      }
      else if ( item == null ) {
        item = new FellowPlusProjectPreview();
        item.Id = id;
        item.Content = data.ToString();
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        fellowplusContext.FellowPlusProjectPreview.Add( item );
      }
      if ( saveChanges )
        return await fellowplusContext.SaveChangesAsync();
      return 0;
    }

    public async Task<int> FellowPlus_SaveProjectPreviewList( JObject data, bool updateIfExists, bool saveChanges ) {
      if ( data == null )
        return 0;
      var token = data["projects"];
      if(token.Type == JTokenType.Array ) {
        JArray list = (JArray)token;
        foreach(var item in list ) {
          string id = (string)item["id"];
          await FellowPlus_SaveProjectPreview( id, item, true, false );
        }
        return await FellowPlus_SaveChanges();
      }
      return 0;
    }

  }

}
