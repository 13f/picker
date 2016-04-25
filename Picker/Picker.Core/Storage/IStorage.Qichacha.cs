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
    string Qichacha_GetAlbumUri_NeedToProcess();

    void Qichacha_UpdateTag_Album( string uri, bool saveChanges );
    void Qichacha_UpdateTag_CompanyInvest( string id, bool saveChanges );
    void Qichacha_UpdateTag_CompanyTrademark( string id, bool saveChanges );
    void Qichacha_UpdateTag_CompanyPatent( string id, bool saveChanges );
    void Qichacha_UpdateTag_CompanyCertificate( string id, bool saveChanges );
    void Qichacha_UpdateTag_CompanyCopyright( string id, bool saveChanges );
    void Qichacha_UpdateTag_CompanySoftwareCopyright( string id, bool saveChanges );
    void Qichacha_UpdateTag_CompanyWebsite( string id, bool saveChanges );


    string Qichacha_GetTaskFromCompany_BasicInfo( out string companyName );
    string Qichacha_GetTaskFromCompany_Invest( out string companyName );
    string Qichacha_GetTaskFromCompany_Trademark( out string companyName );
    string Qichacha_GetTaskFromCompany_Patent( out string companyName );
    string Qichacha_GetTaskFromCompany_Certificate( out string companyName );
    string Qichacha_GetTaskFromCompany_Copyright( out string companyName );
    string Qichacha_GetTaskFromCompany_SoftwareCopyright( out string companyName );
    string Qichacha_GetTaskFromCompany_Website( out string companyName );

    string Qichacha_GetTask_Trademark();


    Task<int> Qichacha_SaveCompanyKey( string id, string name, bool updateIfExists, bool saveChanges );
    void Qichacha_SaveTrademarkKey( List<string> idlist );


    void Qichacha_UpdateCompanySearch( string keyword, int page, bool saveChanges );
    KeyValuePair<string, int> Qichacha_GetLast_CompanySearch();
    void Qichacha_SaveCompany_BasicInfo( string id, string name, string regNum, string orgCode, string content, bool saveChanges );
    void Qichacha_SaveDetail_Trademark( string id, string name, string regNo, string applicant, string content, bool saveChanges );

  }

}
