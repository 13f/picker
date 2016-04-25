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
        Count = companyCount,
        ProcessedCount = qichachaContext.QichachaCompany.Where( i => i.Content != null ).LongCount(),
        TaskCount = companyCount
      } );

      long trademarkCount = qichachaContext.QichachaTrademark.LongCount();
      data.Add( new StatisticsItem() {
        Key = "trademark",
        Title = "Trademark",
        ProcessedCount = qichachaContext.QichachaTrademark.Where( i => i.Content != null ).LongCount(),
        Count = trademarkCount,
        TaskCount = qichachaContext.QichachaCompany.Where( i => i.TrademarkUpdated == null ).LongCount()
      } );

      long patentCount = qichachaContext.QichachaPatent.LongCount();
      data.Add( new StatisticsItem() {
        Key = "patent",
        Title = "Patent",
        ProcessedCount = qichachaContext.QichachaPatent.Where( i => i.Content != null ).LongCount(),
        Count = patentCount,
        TaskCount = qichachaContext.QichachaCompany.Where( i => i.PatentUpdated == null ).LongCount()
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
        ProcessedCount = qichachaContext.QichachaCertificate.Where( i => i.Content != null ).LongCount(),
        Count = certificateCount,
        TaskCount = qichachaContext.QichachaCompany.Where( i => i.CertificateUpdated == null ).LongCount()
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

    public string Qichacha_GetAlbumUri_NeedToProcess() {
      QichachaAlbum item = qichachaContext.QichachaAlbum
        .Where( i => i.ProcessedAt == null || (
          i.ModifiedAt != null && i.ProcessedAt.Value < i.ModifiedAt.Value
        ) )
        .FirstOrDefault();
      return item == null ? null : item.Uri;
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

    public void Qichacha_UpdateTag_CompanyInvest( string id, bool saveChanges ) {
      try {
        QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.Id == id ).FirstOrDefault();
        if ( item != null ) {
          item.InvestUpdated = DateTime.UtcNow;
          if ( saveChanges )
            qichachaContext.SubmitChanges();
        }
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_UpdateTag_CompanyInvest #id: " + id );
        Console.WriteLine( ex.Message );
      }
    }

    public void Qichacha_UpdateTag_CompanyTrademark( string id, bool saveChanges ) {
      try {
        QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.Id == id ).FirstOrDefault();
        if ( item != null ) {
          item.TrademarkUpdated = DateTime.UtcNow;
          if ( saveChanges )
            qichachaContext.SubmitChanges();
        }
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_UpdateTag_CompanyTrademark #id: " + id );
        Console.WriteLine( ex.Message );
      }
    }

    public void Qichacha_UpdateTag_CompanyPatent( string id, bool saveChanges ) {
      try {
        QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.Id == id ).FirstOrDefault();
        if ( item != null ) {
          item.PatentUpdated = DateTime.UtcNow;
          if ( saveChanges )
            qichachaContext.SubmitChanges();
        }
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_UpdateTag_CompanyPatent #id: " + id );
        Console.WriteLine( ex.Message );
      }
    }

    public void Qichacha_UpdateTag_CompanyCertificate( string id, bool saveChanges ) {
      try {
        QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.Id == id ).FirstOrDefault();
        if ( item != null ) {
          item.CertificateUpdated = DateTime.UtcNow;
          if ( saveChanges )
            qichachaContext.SubmitChanges();
        }
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_UpdateTag_CompanyCertificate #id: " + id );
        Console.WriteLine( ex.Message );
      }
    }

    public void Qichacha_UpdateTag_CompanyCopyright( string id, bool saveChanges ) {
      try {
        QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.Id == id ).FirstOrDefault();
        if ( item != null ) {
          item.CopyrightUpdated = DateTime.UtcNow;
          if ( saveChanges )
            qichachaContext.SubmitChanges();
        }
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_UpdateTag_CompanyCopyright #id: " + id );
        Console.WriteLine( ex.Message );
      }
    }

    public void Qichacha_UpdateTag_CompanySoftwareCopyright( string id, bool saveChanges ) {
      try {
        QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.Id == id ).FirstOrDefault();
        if ( item != null ) {
          item.SoftwareCopyrightUpdated = DateTime.UtcNow;
          if ( saveChanges )
            qichachaContext.SubmitChanges();
        }
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_UpdateTag_CompanySoftwareCopyright #id: " + id );
        Console.WriteLine( ex.Message );
      }
    }

    public void Qichacha_UpdateTag_CompanyWebsite( string id, bool saveChanges ) {
      try {
        QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.Id == id ).FirstOrDefault();
        if ( item != null ) {
          item.WebsiteUpdated = DateTime.UtcNow;
          if ( saveChanges )
            qichachaContext.SubmitChanges();
        }
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_UpdateTag_CompanyWebsite #id: " + id );
        Console.WriteLine( ex.Message );
      }
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

    public void Qichacha_SaveTrademarkKey( List<string> idlist ) {
      if ( idlist == null || idlist.Count == 0 )
        return;
      try {
        foreach ( string id in idlist ) {
          QichachaTrademark item = qichachaContext.QichachaTrademark.Where( i => i.Id == id ).FirstOrDefault();
          if ( item == null ) {
            item = new QichachaTrademark();
            item.Id = id;
            item.CreatedAt = DateTime.UtcNow;
            qichachaContext.QichachaTrademark.InsertOnSubmit( item );
          }
        }
        // save changes
        qichachaContext.SubmitChanges();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_SaveTrademarkKey" );
        Console.WriteLine( ex.Message );
      }
    }


    public string Qichacha_GetTaskFromCompany_BasicInfo( out string companyName ) {
      QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.Content == null ).FirstOrDefault();
      companyName = item == null ? null : item.Name;
      return item == null ? null : item.Id;
    }

    public string Qichacha_GetTaskFromCompany_Invest( out string companyName ) {
      QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.InvestUpdated == null ).FirstOrDefault();
      companyName = item == null ? null : item.Name;
      return item == null ? null : item.Id;
    }

    public string Qichacha_GetTaskFromCompany_Trademark( out string companyName ) {
      QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.TrademarkUpdated == null ).FirstOrDefault();
      companyName = item == null ? null : item.Name;
      return item == null ? null : item.Id;
    }

    public string Qichacha_GetTaskFromCompany_Patent( out string companyName ) {
      QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.PatentUpdated == null ).FirstOrDefault();
      companyName = item == null ? null : item.Name;
      return item == null ? null : item.Id;
    }

    public string Qichacha_GetTaskFromCompany_Certificate( out string companyName ) {
      QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.CertificateUpdated == null ).FirstOrDefault();
      companyName = item == null ? null : item.Name;
      return item == null ? null : item.Id;
    }

    public string Qichacha_GetTaskFromCompany_Copyright( out string companyName ) {
      QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.CopyrightUpdated == null ).FirstOrDefault();
      companyName = item == null ? null : item.Name;
      return item == null ? null : item.Id;
    }

    public string Qichacha_GetTaskFromCompany_SoftwareCopyright( out string companyName ) {
      QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.SoftwareCopyrightUpdated == null ).FirstOrDefault();
      companyName = item == null ? null : item.Name;
      return item == null ? null : item.Id;
    }

    public string Qichacha_GetTaskFromCompany_Website( out string companyName ) {
      QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.WebsiteUpdated == null ).FirstOrDefault();
      companyName = item == null ? null : item.Name;
      return item == null ? null : item.Id;
    }

    public string Qichacha_GetTask_Trademark() {
      QichachaTrademark item = qichachaContext.QichachaTrademark.Where( i => i.Content == null ).FirstOrDefault();
      return item == null ? null : item.Id;
    }


    public void Qichacha_SaveCompany_BasicInfo( string id, string name, string regNum, string orgCode, string content, bool saveChanges ) {
      try {
        QichachaCompany item = qichachaContext.QichachaCompany.Where( i => i.Id == id ).FirstOrDefault();
        if ( item == null ) {
          item = new QichachaCompany();
          item.Id = id;
          item.CreatedAt = DateTime.UtcNow;
          qichachaContext.QichachaCompany.InsertOnSubmit( item );
        }
        item.Name = name;
        item.RegNum = regNum;
        item.OrgCode = orgCode;
        item.Content = content;
        item.UpdatedAt = DateTime.UtcNow;

        if ( saveChanges )
          qichachaContext.SubmitChanges();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_SaveCompany_BasicInfo #id: " + id );
        Console.WriteLine( ex.Message );
      }
    }

    public void Qichacha_SaveDetail_Trademark( string id, string name, string regNo, string applicant, string content, bool saveChanges ) {
      try {
        QichachaTrademark item = qichachaContext.QichachaTrademark.Where( i => i.Id == id ).FirstOrDefault();
        if ( item == null ) {
          item = new QichachaTrademark();
          item.Id = id;
          item.CreatedAt = DateTime.UtcNow;
          qichachaContext.QichachaTrademark.InsertOnSubmit( item );
        }
        item.Name = name;
        item.RegNo = regNo;
        item.Applicant = applicant;
        item.Content = content;
        item.UpdatedAt = DateTime.UtcNow;

        if ( saveChanges )
          qichachaContext.SubmitChanges();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "Qichacha_SaveDetail_Trademark #id: " + id );
        Console.WriteLine( ex.Message );
      }
    }

  }

}
