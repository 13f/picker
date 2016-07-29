using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ConsoleDBApp.Models;
using Newtonsoft.Json;

namespace ConsoleDBApp.Biz {
  public class SacBiz {
    SacDataContext db = null;


    public SacBiz( string conn ) {
      db = new SacDataContext( conn );
    }

    public void SaveChanges() {
      db.SubmitChanges();
    }

    public void UpdateQueryState( string uri, int page, bool saveChanges ) {
      try {
        SACQueryState item = db.SACQueryState.Where( i => i.Uri == uri ).FirstOrDefault();
        if ( item == null ) {
          item = new SACQueryState();
          item.Uri = uri;
          item.CreatedAt = DateTime.UtcNow;
          db.SACQueryState.InsertOnSubmit( item );
        }
        item.ProcessedPage = page;
        item.UpdatedAt = DateTime.UtcNow;

        if ( saveChanges )
          db.SubmitChanges();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "UpdateQueryState #uri: " + uri );
        Console.WriteLine( ex.Message );
      }
    }

    public int GetPageLastProcessed(string uri ) {
      SACQueryState item = db.SACQueryState.Where( i => i.Uri == uri && i.ProcessedPage != null && i.ProcessedPage.Value >= 0 ).FirstOrDefault();
      if ( item == null )
        return 0;
      return item.ProcessedPage == null ? 0 : item.ProcessedPage.Value;
    }

    public string GetStandardTask() {
      SACChinaStandard item = db.SACChinaStandard.Where( i => i.Content == null ).FirstOrDefault(); //  || i.Content == "----"
      return item == null ? null : item.StandardCode;
    }

    public void UpdateStandard( string code, JObject jo ) {
      SACChinaStandard item = db.SACChinaStandard.Where( i => i.StandardCode == code ).FirstOrDefault();
      if(item == null ) {
        item = new SACChinaStandard();
        item.StandardCode = code;
        item.CreatedAt = DateTime.UtcNow;
        db.SACChinaStandard.InsertOnSubmit( item );
      }
      item.ChineseTitle = (string)jo["chinese_title"];
      item.EnglishTitle = (string)jo["english_title"];
      item.ICS = (string)jo["ics"];
      item.CCS = (string)jo["ccs"];
      item.IssuanceDate = (DateTime)jo["issuance_date"];
      item.ExecuteDate = (DateTime)jo["execute_date"];
      item.RevocatoryDate = (DateTime)jo["revocatory_date"];

      jo["remark"] = item.Remark;
      item.Content = jo.ToString( Formatting.Indented );
      item.UpdatedAt = DateTime.UtcNow;
      db.SubmitChanges();
    }

    public void UpdateStandard( string code, JObject jo, string remark, bool isRevocative, bool saveChanges ) {
      SACChinaStandard item = db.SACChinaStandard.Where( i => i.StandardCode == code ).FirstOrDefault();
      if ( item == null ) {
        item = new SACChinaStandard();
        item.StandardCode = code;
        item.CreatedAt = DateTime.UtcNow;
        db.SACChinaStandard.InsertOnSubmit( item );
      }
      item.Remark = remark;

      item.ChineseTitle = (string)jo["chinese_title"];
      item.EnglishTitle = (string)jo["english_title"];
      item.ICS = (string)jo["ics"];
      item.CCS = (string)jo["ccs"];
      item.IssuanceDate = (DateTime)jo["issuance_date"];
      item.ExecuteDate = (DateTime)jo["execute_date"];
      item.Revocative = isRevocative;
      item.RevocatoryDate = (DateTime)jo["revocatory_date"];

      jo["remark"] = item.Remark;
      item.Content = jo.ToString( Formatting.Indented );
      item.UpdatedAt = DateTime.UtcNow;

      if ( saveChanges )
        db.SubmitChanges();
    }

    public int UpdateStandards( int page, int countPerPage, DateTime updatedAtMaxValue ) {
      var list = db.SACChinaStandard.Where( i => i.UpdatedAt < updatedAtMaxValue )
        .Skip( page * countPerPage )
        .Take( countPerPage );
      int count = 0;
      foreach(var item in list ) {
        updateStandard( item );
        count++;
      }
      db.SubmitChanges();
      return count;
    }

    public int UpdateStandards201606( int page, int countPerPage ) {
      var maxDate = DateTime.MaxValue.AddMonths( -1 ); // 9999-11-30 23:59:59
      var list = db.SACChinaStandard.Where( i => i.Revocative && i.RevocatoryDate >= maxDate && i.Remark != null && i.Remark.Contains( "废止" ) )
        .Skip( page * countPerPage )
        .Take( countPerPage );
      int count = 0;
      foreach ( var item in list ) {
        bool r = updateStandard201606( item );
        if ( r )
          count++;
      }
      db.SubmitChanges();
      return count;
    }

    public void SaveKey_ChinaStandard( string code, string remark, bool isRevocative, bool saveChanges ) {
      var item = db.SACChinaStandard.Where( i => i.StandardCode == code ).FirstOrDefault();
      if ( item != null )
        return;
      item = new SACChinaStandard();
      item.StandardCode = code;
      item.Remark = remark;
      item.Revocative = isRevocative;
      item.CreatedAt = DateTime.UtcNow;
      item.UpdatedAt = DateTime.UtcNow;
      
      db.SACChinaStandard.InsertOnSubmit( item );

      if ( saveChanges )
        db.SubmitChanges();
    }

    public List<SACChinaStandard> SelectStandards( int page, int countPerPage ) {
      var list = db.SACChinaStandard
        .Skip( page * countPerPage )
        .Take( countPerPage )
        .ToList();
      return list;
    }

    public List<SACChinaStandard> SelectStandards( int page, int countPerPage, DateTime updatedAtMaxValue ) {
      var list = db.SACChinaStandard.Where( i => i.UpdatedAt < updatedAtMaxValue )
        .Skip( page * countPerPage )
        .Take( countPerPage )
        .ToList();
      return list;
    }

    void updateStandard( SACChinaStandard standard ) {
      JObject jo = JObject.Parse( standard.Content );
      jo["state"] = standard.Revocative ? "废止" : "现行";
      // remark
      string remark = null;
      if( jo["remark"] != null ) {
        remark = (string)jo["remark"];
        remark = remark.Trim( '\r', '\n', '\t' )
        .Trim();
        jo["remark"] = remark;
      }
      if ( jo["first_issuance_date"] != null && jo["first_issuance_date"].Type != JTokenType.Null && (DateTime)jo["first_issuance_date"] == DateTime.MaxValue )
        jo["first_issuance_date"] = null;

      if ( jo["review_affirmance_date"] != null && jo["review_affirmance_date"].Type != JTokenType.Null && (DateTime)jo["review_affirmance_date"] == DateTime.MaxValue )
        jo["review_affirmance_date"] = null;

      if ( jo["revocatory_date"] != null && jo["revocatory_date"].Type != JTokenType.Null && (DateTime)jo["revocatory_date"] == DateTime.MaxValue )
        jo["revocatory_date"] = null;

      if ( jo["plan_number"] != null && jo["plan_number"].Type != JTokenType.Null && string.IsNullOrWhiteSpace( (string)jo["plan_number"] ) )
        jo["plan_number"] = null;

      if ( jo["price"] != null && jo["price"].Type != JTokenType.Null && string.IsNullOrWhiteSpace( (string)jo["price"] ) )
        jo["price"] = null;

      if ( jo["replaces"] != null && jo["replaces"].Type != JTokenType.Null && string.IsNullOrWhiteSpace( (string)jo["replaces"] ) )
        jo["replaces"] = null;

      if ( jo["replaced_by"] != null && jo["replaced_by"].Type != JTokenType.Null && string.IsNullOrWhiteSpace( (string)jo["replaced_by"] ) )
        jo["replaced_by"] = null;

      if ( jo["adopted_international_standard_number"] != null && jo["adopted_international_standard_number"].Type != JTokenType.Null && string.IsNullOrWhiteSpace( (string)jo["adopted_international_standard_number"] ) )
        jo["adopted_international_standard_number"] = null;

      if ( jo["adopted_international_standard_name"] != null && jo["adopted_international_standard_name"].Type != JTokenType.Null && string.IsNullOrWhiteSpace( (string)jo["adopted_international_standard_name"] ) )
        jo["adopted_international_standard_name"] = null;

      if ( jo["adopted_international_standard"] != null && jo["adopted_international_standard"].Type != JTokenType.Null && string.IsNullOrWhiteSpace( (string)jo["adopted_international_standard"] ) )
        jo["adopted_international_standard"] = null;

      if ( jo["application_degree"] != null && jo["application_degree"].Type != JTokenType.Null && string.IsNullOrWhiteSpace( (string)jo["application_degree"] ) )
        jo["application_degree"] = null;

      // technical_committees
      if ( jo["technical_committees"] != null && jo["technical_committees"].Type != JTokenType.Null ) {
        string technical_committees = (string)jo["technical_committees"];
        int start = technical_committees.IndexOf( "<!--" );
        if( start >= 0 ) {
          jo["technical_committees"] = technical_committees.Substring(0, start)
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
      }

      standard.Remark = remark;
      standard.Content = jo.ToString( Formatting.Indented );
      standard.UpdatedAt = DateTime.UtcNow;
    }

    bool updateStandard201606( SACChinaStandard standard ) {
      if ( string.IsNullOrWhiteSpace( standard.Remark ) )
        return false;

      int end = standard.Remark.IndexOf( "废止" );
      if ( end < 0 )
        return false;

      string revDate = standard.Remark.Substring( 0, end );
      if ( revDate.Contains( "," ) ) {
        int start = revDate.LastIndexOf( "," );
        revDate = revDate.Substring( start + 1 ).Trim();
      }
      DateTime date = DateTime.Parse( revDate );
      JObject jo = JObject.Parse( standard.Content );

      jo["revocatory_date"] = date;

      standard.RevocatoryDate = date;
      
      standard.Content = jo.ToString( Formatting.Indented );
      standard.UpdatedAt = DateTime.UtcNow;

      return true;
    }

  }

}
