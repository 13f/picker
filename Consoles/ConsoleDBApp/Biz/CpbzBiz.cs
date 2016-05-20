using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDBApp.Models;

namespace ConsoleDBApp.Biz {
  public class CpbzBiz {
    CpbzDataContext db = null;


    public CpbzBiz( string conn ) {
      db = new CpbzDataContext( conn );
    }

    public void SaveChanges() {
      db.SubmitChanges();
    }

    public void UpdateQueryState_Area_CPBZ( string code, int page, bool saveChanges ) {
      try {
        Area item = db.Area.Where( i => i.Code == code ).FirstOrDefault();
        if ( item == null ) {
          item = new Area();
          item.Code = code;
          item.CreatedAt = DateTime.UtcNow;
          db.Area.InsertOnSubmit( item );
        }
        item.CpbzProcessedAt = DateTime.UtcNow;
        item.CpbzProcessedPage = page;

        if ( saveChanges )
          db.SubmitChanges();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "UpdateQueryState_Area_CPBZ #code: " + code );
        Console.WriteLine( ex.Message );
      }
    }

    public void UpdateQueryState_Area_Qichacha( string code, int page, bool saveChanges ) {
      try {
        Area item = db.Area.Where( i => i.Code == code ).FirstOrDefault();
        if ( item == null ) {
          item = new Area();
          item.Code = code;
          item.CreatedAt = DateTime.UtcNow;
          db.Area.InsertOnSubmit( item );
        }
        item.QichachaProcessedAt = DateTime.UtcNow;
        item.QichachaProcessedPage = page;

        if ( saveChanges )
          db.SubmitChanges();
      }
      catch ( Exception ex ) {
        Console.WriteLine( "UpdateQueryState_Area_Qichacha #code: " + code );
        Console.WriteLine( ex.Message );
      }
    }

    /// <summary>
    /// 获取31个一级省市自治区的代码（以0000结束）
    /// </summary>
    /// <returns></returns>
    public List<string> SelectCodesOfTopAreas_CPBZ( DateTime lastTime ) {
      var areas = db.Area.Where( i => i.Code.EndsWith( "0000" ) && ( i.CpbzProcessedAt == null || i.CpbzProcessedAt.Value < lastTime ) ).ToList();
      var codes = areas.Select( i => i.Code ).ToList();
      return codes;
    }

    /// <summary>
    /// key: code, value: last page
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<string, int> GetLastArea_CPBZ() {
      // 以0000结束，即指获取31个一级省市自治区
      Area item = db.Area.Where( i => i.Code.EndsWith( "0000" ) && i.CpbzProcessedPage >= 0 ).FirstOrDefault();
      if ( item == null )
        return new KeyValuePair<string, int>( null, -1 );
      return new KeyValuePair<string, int>( item.Code, item.CpbzProcessedPage );
    }
   
    /// <summary>
    /// key: code, value: last page
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<string, int> GetLastArea_Qichacha() {
      Area item = db.Area.Where( i => i.QichachaProcessedPage >= 0 ).FirstOrDefault();
      if ( item == null )
        return new KeyValuePair<string, int>( null, -1 );
      return new KeyValuePair<string, int>( item.Code, item.QichachaProcessedPage );
    }

    public KeyValuePair<string, int> GetStandardToProcess() {
      var item = db.CpbzStandard.Where( i => i.Content == null ).FirstOrDefault();
      if ( item == null )
        return new KeyValuePair<string, int>( null, -1 );
      return new KeyValuePair<string, int>( item.OrgCode, item.StandardId );
    }

    public Tuple<string, int, string> GetStandardToUpdateOriginalPdfUri() {
      DateTime dt = DateTime.Parse( "2016-5-14 12:00" );
      var item = db.CpbzStandard.Where( i => i.UpdatedAt != null && i.UpdatedAt.Value < dt && i.Content!= "----" ).FirstOrDefault();
      if ( item == null )
        return new Tuple<string, int, string>( null, -1, null );
      return new Tuple<string, int, string>( item.OrgCode, item.StandardId, item.Content );
    }

    public void SaveKey_Area( string code, string name, bool saveChanges ) {
      var item = db.Area.Where( i => i.Code == code ).FirstOrDefault();
      if ( item != null )
        return;
      item = new Area();
      item.Code = code;
      item.Name = name;
      item.CreatedAt = DateTime.UtcNow;
      
      db.Area.InsertOnSubmit( item );

      if ( saveChanges )
        db.SubmitChanges();
    }

    public void SaveKey_Company(string orgCode, string name, bool saveChanges ) {
      var item = db.CpbzCompanyTask.Where( i => i.OrgCode == orgCode ).FirstOrDefault();
      if ( item != null )
        return;
      item = new CpbzCompanyTask();
      item.OrgCode = orgCode;
      item.Name = name;
      item.CreatedAt = DateTime.UtcNow;
      item.UpdatedAt = DateTime.UtcNow;

      db.CpbzCompanyTask.InsertOnSubmit( item );

      if ( saveChanges )
        db.SubmitChanges();
    }

    public void SaveKey_Standard( string orgCode, int standardId, string standardCode, string standardName, bool saveChanges ) {
      var item = db.CpbzStandard.Where( i => i.OrgCode == orgCode && i.StandardCode == standardCode ).FirstOrDefault();
      bool isNewItem = item == null || item.StandardId < standardId;
      if ( !isNewItem ) // 只保留standardId最大的数据
        return;

      if (item == null ) {
        item = new CpbzStandard();
        item.OrgCode = orgCode;
        item.StandardCode = standardCode;

        db.CpbzStandard.InsertOnSubmit( item );
      }
      
      item.StandardId = standardId;
      item.StandardName = standardName;
      item.CreatedAt = DateTime.UtcNow;
      item.UpdatedAt = DateTime.UtcNow;

      if ( saveChanges || isNewItem )
        db.SubmitChanges();
    }

    public void SaveCompany( string orgCode, string name, string json, bool saveChanges ) {
      var item = db.CpbzCompanyTask.Where( i => i.OrgCode == orgCode ).FirstOrDefault();
      if ( item == null ) {
        item = new CpbzCompanyTask();
        item.OrgCode = orgCode;
        item.Name = name;
        item.CreatedAt = DateTime.UtcNow;
        db.CpbzCompanyTask.InsertOnSubmit( item );
      }

      item.Content = json;
      item.UpdatedAt = DateTime.UtcNow;

      if ( saveChanges )
        db.SubmitChanges();
    }

    // 有很多standardID相同而orgCode不同的数据，比如：
    // SELECT * FROM [qichacha].[dbo].[CpbzStandard] where StandardId = 35501
    public void SaveStandard( string orgCode, int standardId, string votum, DateTime? opened, string json, bool saveChanges ) {
      var item = db.CpbzStandard.Where( i => i.OrgCode == orgCode && i.StandardId == standardId ).FirstOrDefault();
      if ( item == null )
        return;
      item.Votum = votum;
      item.Content = json;
      item.OpenedAt = opened;
      item.UpdatedAt = DateTime.UtcNow;

      if ( saveChanges )
        db.SubmitChanges();
    }

    public void UpdateStandard_Content( string orgCode, int standardId, string json, bool saveChanges ) {
      var item = db.CpbzStandard.Where( i => i.OrgCode == orgCode && i.StandardId == standardId ).FirstOrDefault();
      if ( item == null )
        return;
      item.Content = json;
      item.UpdatedAt = DateTime.UtcNow;

      if ( saveChanges )
        db.SubmitChanges();
    }


    /// <summary>
    /// 从inputIds中过滤出数据库中每有的id
    /// </summary>
    /// <param name="inputIds"></param>
    /// <returns></returns>
    public List<int> GetStandardsNotSaved( List<int> inputIds ) {
      List<int> outputIds = new List<int>();
      foreach (int id in inputIds ) {
        bool exists = ContainsStandard( id );
        if ( !exists )
          outputIds.Add( id );
      }
      return outputIds;
    }

    public bool ContainsStandard( int standardId ) {
      var item = db.CpbzStandard.Where( i => i.StandardId == standardId ).FirstOrDefault();
      return item != null;
    }

  }

}
