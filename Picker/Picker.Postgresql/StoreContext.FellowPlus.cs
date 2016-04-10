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
      int r = 0;
      try {
        r = await fellowplusContext.SaveChangesAsync();
        return r;
      }
      catch(Exception ex) {
        Console.WriteLine( "FellowPlus_SaveChanges" );
        Console.WriteLine( ex.Message );
        return r;
      }
    }

    public List<StatisticsItem> FellowPlus_LoadStatistics() {
      List<StatisticsItem> data = new List<StatisticsItem>();
      long projectsPreviewCount = fellowplusContext.FellowPlusProjectPreview.LongCount();
      data.Add( new StatisticsItem() {
        Key = "projects-preview",
        Title = "Projects Preview",
        Count = projectsPreviewCount
      } );

      long projectsCount = fellowplusContext.FellowPlusProject.LongCount();
      data.Add( new StatisticsItem() {
        Key = "projects",
        Title = "Projects",
        Count = projectsCount
      } );

      long companyCount = fellowplusContext.FellowPlusCompany.LongCount();
      data.Add( new StatisticsItem() {
        Key = "company",
        Title = "Company",
        Count = companyCount
      } );

      long investCount = fellowplusContext.FellowPlusInvest.LongCount();
      data.Add( new StatisticsItem() {
        Key = "invest",
        Title = "Invest",
        Count = investCount
      } );

      long websiteCount = fellowplusContext.FellowPlusWebsite.LongCount();
      data.Add( new StatisticsItem() {
        Key = "website",
        Title = "Website",
        Count = websiteCount
      } );

      long weiboCount = fellowplusContext.FellowPlusWeibo.LongCount();
      data.Add( new StatisticsItem() {
        Key = "weibo",
        Title = "Weibo",
        Count = weiboCount
      } );

      long weixinCount = fellowplusContext.FellowPlusWeixin.LongCount();
      data.Add( new StatisticsItem() {
        Key = "weixin",
        Title = "Weixin",
        Count = weixinCount
      } );

      long newsCount = fellowplusContext.FellowPlusNews.LongCount();
      data.Add( new StatisticsItem() {
        Key = "news",
        Title = "News",
        Count = newsCount
      } );

      return data;
    }

    public string FellowPlus_SelectId_ProjectPreview_NotProcessed( int skipRandomItems ) {
      int skip = skipRandomItems >= 0 ? skipRandomItems : 0;
      FellowPlusProjectPreview item = fellowplusContext.FellowPlusProjectPreview
        .Where( i => i.ProcessedAt == null &&
        i.Id != "K_M36KR_PROJECT:186289" && i.Id != "K_M36KR_PROJECT:4895" )
        .OrderByDescending( i => i.UpdatedAt )
        .Skip( skip )
        .FirstOrDefault();
      return item == null ? null : item.Id;
    }

    public async Task<int> FellowPlus_UpdateTag_ProjectPreview( string id, bool saveChanges ) {
      try {
        FellowPlusProjectPreview item = fellowplusContext.FellowPlusProjectPreview.Where( i => i.Id == id ).FirstOrDefault();
        if ( item != null ) {
          item.ProcessedAt = DateTime.UtcNow;
          if ( saveChanges )
            return await fellowplusContext.SaveChangesAsync();
        }
      }
      catch ( Exception ex ) {
        Console.WriteLine( "FellowPlus_UpdateTag_ProjectPreview #id: " + id );
        Console.WriteLine( ex.Message );
      }

      return 0;
    }

    public async Task<int> FellowPlus_SaveProjectPreview( string id, JToken data, bool updateIfExists, bool saveChanges ) {
      try {
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
      }
      catch ( Exception ex ) {
        Console.WriteLine( "FellowPlus_SaveProjectPreview #id: " + id );
        Console.WriteLine( ex.Message );
      }

      return 0;
    }

    public async Task<int> FellowPlus_SaveProjectPreviewList( JObject data, bool updateIfExists, bool saveChanges ) {
      if ( data == null )
        return 0;
      var token = data["projects"];
      if ( token == null ) {
        if ( data["error_msg"] != null )
          throw new Exception( (string)data["error_msg"] );
        return 0;
      }

      if ( token.Type == JTokenType.Array ) {
        JArray list = (JArray)token;
        foreach ( var item in list ) {
          string id = (string)item["id"];
          await FellowPlus_SaveProjectPreview( id, item, true, false );
        }

        try {
          return await FellowPlus_SaveChanges();
        }
        catch ( Exception ex ) {
          Console.WriteLine( "FellowPlus_SaveProjectPreviewList" );
          Console.WriteLine( ex.Message );
        }

      }
      return 0;
    }


    async Task<int> FellowPlus_SaveProject( string projectId, JToken data, bool updateIfExists, bool saveChanges ) {
      try {
        FellowPlusProject item = fellowplusContext.FellowPlusProject.Where( i => i.Id == projectId ).FirstOrDefault();
        if ( item != null && updateIfExists ) {
          item.Content = data.ToString();
          item.UpdatedAt = DateTime.UtcNow;
        }
        else if ( item == null ) {
          item = new FellowPlusProject();
          item.Id = projectId;
          item.Content = data.ToString();
          item.CreatedAt = DateTime.UtcNow;
          item.UpdatedAt = DateTime.UtcNow;
          fellowplusContext.FellowPlusProject.Add( item );
        }
        if ( saveChanges )
          return await fellowplusContext.SaveChangesAsync();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "FellowPlus_SaveProject #projectId: " + projectId );
        Console.WriteLine( ex.Message );
      }

      return 0;
    }

    async Task<int> FellowPlus_SaveCompany( string projectId, JToken data, bool updateIfExists, bool saveChanges ) {
      if ( data == null || data["name"] == null )
        return 0;
      string name = (string)data["name"];
      try {
        FellowPlusCompany item = fellowplusContext.FellowPlusCompany.Where( i => i.ProjectId == projectId && i.Name == name ).FirstOrDefault();
        if ( item != null && updateIfExists ) {
          item.Content = data.ToString();
          item.UpdatedAt = DateTime.UtcNow;
        }
        else if ( item == null ) {
          item = new FellowPlusCompany();
          item.ProjectId = projectId;
          item.Name = name;
          item.Content = data.ToString();
          item.CreatedAt = DateTime.UtcNow;
          item.UpdatedAt = DateTime.UtcNow;
          fellowplusContext.FellowPlusCompany.Add( item );
        }
        if ( saveChanges )
          return await fellowplusContext.SaveChangesAsync();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "FellowPlus_SaveCompany #projectId: " + projectId + "... #name: " + name );
        Console.WriteLine( ex.Message );
      }

      return 0;
    }

    async Task<int> FellowPlus_SaveInvest( string projectId, JToken data, bool updateIfExists, bool saveChanges ) {
      try {
        FellowPlusInvest item = fellowplusContext.FellowPlusInvest.Where( i => i.ProjectId == projectId ).FirstOrDefault();
        if ( item != null && updateIfExists ) {
          item.Content = data.ToString();
          item.UpdatedAt = DateTime.UtcNow;
        }
        else if ( item == null ) {
          item = new FellowPlusInvest();
          item.ProjectId = projectId;
          item.Content = data.ToString();
          item.CreatedAt = DateTime.UtcNow;
          item.UpdatedAt = DateTime.UtcNow;
          fellowplusContext.FellowPlusInvest.Add( item );
        }
        if ( saveChanges )
          return await fellowplusContext.SaveChangesAsync();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "FellowPlus_SaveInvest #projectId: " + projectId );
        Console.WriteLine( ex.Message );
      }

      return 0;
    }

    async Task<int> FellowPlus_SaveWebsite( string projectId, JToken data, bool updateIfExists, bool saveChanges ) {
      if ( data == null || data["id"] == null )
        return 0;
      string id = (string)data["id"];
      try {
        FellowPlusWebsite item = fellowplusContext.FellowPlusWebsite.Where( i => i.ProjectId == projectId && i.Id == id ).FirstOrDefault();
        if ( item != null && updateIfExists ) {
          item.Content = data.ToString();
          item.UpdatedAt = DateTime.UtcNow;
        }
        else if ( item == null ) {
          string uri = (string)data["detail_url"];
          // hack
          if ( id == "17zuoye.com" )
            uri = "http://www.17zuoye.com";
          item = new FellowPlusWebsite();
          item.ProjectId = projectId;
          item.Id = id;
          item.Uri = uri;
          item.Content = data.ToString();
          item.CreatedAt = DateTime.UtcNow;
          item.UpdatedAt = DateTime.UtcNow;
          fellowplusContext.FellowPlusWebsite.Add( item );
        }
        if ( saveChanges )
          return await fellowplusContext.SaveChangesAsync();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "FellowPlus_SaveWebsite #projectId: " + projectId + "... #id: " + id );
        Console.WriteLine( ex.Message );
      }

      return 0;
    }

    async Task<int> FellowPlus_SaveWeibo( string projectId, JToken data, bool updateIfExists, bool saveChanges ) {
      try {
        FellowPlusWeibo item = fellowplusContext.FellowPlusWeibo.Where( i => i.ProjectId == projectId ).FirstOrDefault();
        if ( item != null && updateIfExists ) {
          item.Content = data.ToString();
          item.UpdatedAt = DateTime.UtcNow;
        }
        else if ( item == null ) {
          item = new FellowPlusWeibo();
          item.ProjectId = projectId;
          item.Content = data.ToString();
          item.CreatedAt = DateTime.UtcNow;
          item.UpdatedAt = DateTime.UtcNow;
          fellowplusContext.FellowPlusWeibo.Add( item );
        }
        if ( saveChanges )
          return await fellowplusContext.SaveChangesAsync();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "FellowPlus_SaveWeibo #projectId: " + projectId );
        Console.WriteLine( ex.Message );
      }

      return 0;
    }

    async Task<int> FellowPlus_SaveWeixin( string projectId, JToken data, bool updateIfExists, bool saveChanges ) {
      try {
        FellowPlusWeixin item = fellowplusContext.FellowPlusWeixin.Where( i => i.ProjectId == projectId ).FirstOrDefault();
        if ( item != null && updateIfExists ) {
          item.Content = data.ToString();
          item.UpdatedAt = DateTime.UtcNow;
        }
        else if ( item == null ) {
          item = new FellowPlusWeixin();
          item.ProjectId = projectId;
          item.Content = data.ToString();
          item.CreatedAt = DateTime.UtcNow;
          item.UpdatedAt = DateTime.UtcNow;
          fellowplusContext.FellowPlusWeixin.Add( item );
        }
        if ( saveChanges )
          return await fellowplusContext.SaveChangesAsync();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "FellowPlus_SaveWeixin #projectId: " + projectId );
        Console.WriteLine( ex.Message );
      }

      return 0;
    }

    async Task<int> FellowPlus_SaveNews( string projectId, JToken data, bool updateIfExists, bool saveChanges ) {
      if ( data == null || data["detail_url"] == null )
        return 0;
      string uri = (string)data["detail_url"];
      try {
        FellowPlusNews item = fellowplusContext.FellowPlusNews.Where( i => i.ProjectId == projectId && i.Uri == uri ).FirstOrDefault();
        if ( item != null && updateIfExists ) {
          item.Content = data.ToString();
          item.UpdatedAt = DateTime.UtcNow;
        }
        else if ( item == null ) {
          item = new FellowPlusNews();
          item.ProjectId = projectId;
          item.Uri = uri;
          item.Content = data.ToString();
          item.CreatedAt = DateTime.UtcNow;
          item.UpdatedAt = DateTime.UtcNow;
          fellowplusContext.FellowPlusNews.Add( item );
        }
        if ( saveChanges )
          return await fellowplusContext.SaveChangesAsync();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "FellowPlus_SaveNews #projectId: " + projectId + "... #uri: " + uri );
        Console.WriteLine( ex.Message );
      }

      return 0;
    }

    public async Task<int> FellowPlus_SaveProjectInfo( string projectId,
      JToken tokenProject, JToken tokenCompany, JToken tokenInvest, JToken tokenWebsite, JToken tokenWeibo, JToken tokenWeixin, JToken tokenNews,
      bool updateIfExists, bool saveChanges ) {
      List<string> cache = new List<string>();
      // project
      if ( tokenProject != null && tokenProject["project"] != null ) {
        var project = tokenProject["project"];
        await FellowPlus_SaveProject( projectId, project, updateIfExists, false );
      }
      // company
      if ( tokenCompany != null && tokenCompany["companys"] != null ) {
        JArray array = (JArray)tokenCompany["companys"];
        foreach ( var item in array ) {
          string key = (string)item["name"];
          if ( string.IsNullOrWhiteSpace( key ) ) // K_YCPAI_PROJECT:31030
            continue;
          key = key.Replace( "(", "（" )
            .Replace( ")", "）" );
          if ( cache.Contains( key ) )
            continue;
          cache.Add( key );
          await FellowPlus_SaveCompany( projectId, item, updateIfExists, false );
        }
        cache.Clear();
      }
      // invest
      if ( tokenInvest != null && tokenInvest["invests"] != null ) {
        await FellowPlus_SaveInvest( projectId, tokenInvest, updateIfExists, false );
      }
      // website
      if ( tokenWebsite != null && tokenWebsite["websites"] != null ) {
        JArray array = (JArray)tokenWebsite["websites"];
        foreach ( var item in array ) {
          string key = (string)item["id"];
          if ( string.IsNullOrWhiteSpace( key ) )
            continue;
          if ( cache.Contains( key ) )
            continue;
          cache.Add( key );
          await FellowPlus_SaveWebsite( projectId, item, updateIfExists, false );
        }
        cache.Clear();
      }
      // news
      if ( tokenNews != null && tokenNews["news"] != null ) {
        JArray array = (JArray)tokenNews["news"];
        foreach ( var item in array ) {
          string key = (string)item["detail_url"];
          if ( string.IsNullOrWhiteSpace( key ) )
            continue;
          if ( cache.Contains( key ) )
            continue;
          cache.Add( key );
          await FellowPlus_SaveNews( projectId, item, updateIfExists, false );
        }
        cache.Clear();
      }
      // weibo
      if ( tokenWeibo != null && tokenWeibo["weibos"] != null ) {
        await FellowPlus_SaveWeibo( projectId, tokenWeibo, updateIfExists, false );
      }
      // weixin
      if ( tokenWeixin != null && tokenWeixin["weixins"] != null ) {
        await FellowPlus_SaveWeixin( projectId, tokenWeixin, updateIfExists, false );
      }
      
      // save changes
      int r = 0;
      if ( saveChanges )
        r = await FellowPlus_SaveChanges();
      return r;
    }

  }

}
