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

  }

}
