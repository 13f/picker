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

    public void OpenQichachaDatabase( string connString ) {
      qichachaContext = new QichachaDataContext( connString );
    }

    public List<StatisticsItem> Qichacha_LoadStatistics() {
      List<StatisticsItem> data = new List<StatisticsItem>();
      long companyCount = qichachaContext.QichachaCompany.LongCount();
      data.Add( new StatisticsItem() {
        Key = "company",
        Title = "Company",
        Count = companyCount
      } );

      long trademarkCount = qichachaContext.QichachaTrademark.LongCount();
      data.Add( new StatisticsItem() {
        Key = "trademark",
        Title = "Trademark",
        Count = trademarkCount
      } );

      long patentCount = qichachaContext.QichachaPatent.LongCount();
      data.Add( new StatisticsItem() {
        Key = "patent",
        Title = "Patent",
        Count = patentCount
      } );

      long investCount = qichachaContext.QichachaInvest.LongCount();
      data.Add( new StatisticsItem() {
        Key = "invest",
        Title = "Invest",
        Count = investCount
      } );

      long copyrightCount = qichachaContext.QichachaCopyright.LongCount();
      data.Add( new StatisticsItem() {
        Key = "copyright",
        Title = "Copyright",
        Count = copyrightCount
      } );

      long softwareCopyrightCount = qichachaContext.QichachaSoftwareCopyright.LongCount();
      data.Add( new StatisticsItem() {
        Key = "software-copyright",
        Title = "SoftwareCopyright",
        Count = softwareCopyrightCount
      } );

      long certificateCount = qichachaContext.QichachaCertificate.LongCount();
      data.Add( new StatisticsItem() {
        Key = "certificate",
        Title = "Certificate",
        Count = certificateCount
      } );

      long websiteCount = qichachaContext.QichachaWebsite.LongCount();
      data.Add( new StatisticsItem() {
        Key = "website",
        Title = "Website",
        Count = websiteCount
      } );

      long albumCount = qichachaContext.QichachaAlbum.LongCount();
      data.Add( new StatisticsItem() {
        Key = "album",
        Title = "Company Album",
        Count = albumCount,
        ProcessedCount = qichachaContext.QichachaAlbum.Where( i => i.ProcessedAt != null ).LongCount()
      } );

      return data;
    }

    public async Task<int> Qichacha_SaveChanges() {
      int r = 0;
      try {
        //r = await qichachaContext.SaveChangesAsync();
        qichachaContext.SubmitChanges();
        return r;
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_SaveChanges" );
        Console.WriteLine( ex.Message );
        return r;
      }
    }

    public void Qichacha_SaveAlbum( string uri, string title, DateTime? modified, bool updateIfExists, bool saveChanges ) {
      try {
        QichachaAlbum item = qichachaContext.QichachaAlbum.Where( i => i.Uri == uri ).FirstOrDefault();
        if ( item != null && updateIfExists ) {
          item.Title = title;
          item.ModifiedAt = modified;
        }
        else if ( item == null ) {
          // get id from http://qichacha.com/album_view_id_1050.shtml
          int id = ApiHelper.GetInt( uri, "album_view_id_", ".shtml" );

          item = new QichachaAlbum();
          item.Uri = uri;
          item.Id = id;
          item.Title = title;
          item.ModifiedAt = modified;
          item.CreatedAt = DateTime.UtcNow;
          qichachaContext.QichachaAlbum.InsertOnSubmit( item );
        }
        if ( saveChanges )
          //return await qichachaContext.SaveChangesAsync();
          qichachaContext.SubmitChanges();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_SaveAlbum #uri: " + uri );
        Console.WriteLine( ex.Message );
      }
    }

    public void Qichacha_UpdateTag_Album( string uri, bool saveChanges ) {
      try {
        QichachaAlbum item = qichachaContext.QichachaAlbum.Where( i => i.Uri == uri ).FirstOrDefault();
        if ( item != null ) {
          item.ProcessedAt = DateTime.UtcNow;
          if ( saveChanges )
            qichachaContext.SubmitChanges();
        }
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_UpdateTag_Album #uri: " + uri );
        Console.WriteLine( ex.Message );
      }
    }

    public string Qichacha_GetAlbumUri_NeedToProcess() {
      QichachaAlbum item = qichachaContext.QichachaAlbum
        .Where( i => i.ProcessedAt == null || (
          i.ModifiedAt != null && i.ProcessedAt.Value < i.ModifiedAt.Value
        ) )
        .FirstOrDefault();
      return item == null ? null : item.Uri;
    }

    public void Qichacha_UpdateCompanySearch( string keyword, int page, bool saveChanges ) {
      try {
        QichachaCompanySearch item = qichachaContext.QichachaCompanySearch.Where( i => i.Keyword == keyword ).FirstOrDefault();
        if ( item == null ) {
          item = new QichachaCompanySearch();
          item.Keyword = keyword;
          item.CreatedAt = DateTime.UtcNow;
          qichachaContext.QichachaCompanySearch.InsertOnSubmit( item );
        }
        item.ProcessedPage = page;
        item.UpdatedAt = DateTime.UtcNow;

        if ( saveChanges )
          qichachaContext.SubmitChanges();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_UpdateCompanySearch #keyword: " + keyword );
        Console.WriteLine( ex.Message );
      }
    }

    public KeyValuePair<string, int> Qichacha_GetLast_CompanySearch() {
      QichachaCompanySearch item = qichachaContext.QichachaCompanySearch.Where( i => i.ProcessedPage >= 0 ).FirstOrDefault();
      if ( item == null )
        return new KeyValuePair<string, int>( null, -1 );
      return new KeyValuePair<string, int>( item.Keyword, item.ProcessedPage );
    }

    public async Task<int> Qichacha_SaveCompanyKey( string id, string name, bool updateIfExists, bool saveChanges ) {
      try {
        QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.Id == id ).FirstOrDefault();
        if ( item != null && updateIfExists ) {
          item.Name = name;
          item.UpdatedAt = DateTime.UtcNow;
        }
        else if ( item == null ) {
          item = new QichachaCompany();
          item.Id = id;
          item.Name = name;
          item.CreatedAt = DateTime.UtcNow;
          item.UpdatedAt = DateTime.UtcNow;
          qichachaContext.QichachaCompany.InsertOnSubmit( item );
        }
        if ( saveChanges )
          //return await qichachaContext.SaveChangesAsync();
          qichachaContext.SubmitChanges();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_SaveCompanyKey #id: " + id );
        Console.WriteLine( ex.Message );
      }

      return 0;
    }


  }

}
