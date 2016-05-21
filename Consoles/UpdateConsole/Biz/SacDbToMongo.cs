using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using Picker.Core.Extensions;
using Picker.Core.Helpers;
using Picker.Core.Models;

namespace UpdateConsole.Biz {
  public static class SacDbToMongo {
    static IMongoClient client;
    static IMongoDatabase db;

    public static async Task Run( int millisecondsDelay = 500 ) {
      Console.WriteLine( "ready..." );

      string connSac = @"Data Source=revenger\sqlexpress;Initial Catalog=sac;Persist Security Info=True;User ID=sa; Password=whosyourdaddy";
      var dbBiz = new ConsoleDBApp.Biz.SacBiz( connSac );

      client = new MongoClient();
      db = client.GetDatabase( "picker" );
      var collection = db.GetCollection<BsonDocument>( "china_standards" );

      int countPerPage = 50;
      int page = 0;
      int count = 0;
      while ( true ) {
        var list = dbBiz.SelectStandards( page, countPerPage );
        Console.WriteLine( "  got 50 items..." );

        List<BsonDocument> items = new List<BsonDocument>();
        foreach (var dbItem in list ) {
          NationalStandard item = new NationalStandard() {
            Id = CommonUtility.NewGuid_PlainLower(),
            ChineseTitle = dbItem.ChineseTitle,
            EnglishTitle = dbItem.EnglishTitle,
            Code = dbItem.StandardCode,
            ICS = dbItem.ICS,
            CCS = dbItem.CCS,

            IssuanceDate = dbItem.IssuanceDate,
            ExecuteDate = dbItem.ExecuteDate,
            RevocatoryDate = null,

            Remark = dbItem.Remark,
            BytesCount = dbItem.Content.GetSizeInBytes(),
            
            Created = dbItem.CreatedAt.Value,
            Modified = dbItem.UpdatedAt.Value,
            Version = 1,
            VersionId = CommonUtility.NewGuid_PlainLower()
          };
          if ( dbItem.Revocative )
            item.RevocatoryDate = dbItem.RevocatoryDate;
          item.Token = JObject.Parse( dbItem.Content );
          item.AreaScope = "http://www.chuci.info/object/place/20e52e1cb46f4e7e8a8f32009b3e7ec2"; // china

          string json = Newtonsoft.Json.JsonConvert.SerializeObject( item, Newtonsoft.Json.Formatting.Indented );
          var document = BsonSerializer.Deserialize<BsonDocument>( json );
          items.Add( document );
        }
        Console.WriteLine( "  inserting to mongodb..." );
        await collection.InsertManyAsync( items );

        count = count + list.Count;
        Console.WriteLine( "  " + count.ToString() + " items processed." );
        if ( list.Count < countPerPage )
          break;
        Console.WriteLine( "wait and continue..." );
        await Task.Delay( millisecondsDelay );
        page++;
      }

      Console.WriteLine( "over..." );
    }

    public static async Task Test() {
      Console.WriteLine( "ready..." );

      string connSac = @"Data Source=revenger\sqlexpress;Initial Catalog=sac;Persist Security Info=True;User ID=sa; Password=whosyourdaddy";
      var dbBiz = new ConsoleDBApp.Biz.SacBiz( connSac );

      client = new MongoClient();
      db = client.GetDatabase( "picker" );
      var collection = db.GetCollection<BsonDocument>( "test" );

      var list = dbBiz.SelectStandards( 0, 50 );
      Console.WriteLine( "  got 50 items..." );

      List<BsonDocument> items = new List<BsonDocument>();
      foreach ( var dbItem in list ) {
        NationalStandard item = new NationalStandard() {
          Id = CommonUtility.NewGuid_PlainLower(),
          ChineseTitle = dbItem.ChineseTitle,
          EnglishTitle = dbItem.EnglishTitle,
          Code = dbItem.StandardCode,
          ICS = dbItem.ICS,
          CCS = dbItem.CCS,

          IssuanceDate = dbItem.IssuanceDate,
          ExecuteDate = dbItem.ExecuteDate,
          RevocatoryDate = null,

          Remark = dbItem.Remark,
          BytesCount = dbItem.Content.GetSizeInBytes(),

          Created = dbItem.CreatedAt.Value,
          Modified = dbItem.UpdatedAt.Value,
          Version = 1,
          VersionId = CommonUtility.NewGuid_PlainLower()
        };
        if ( dbItem.Revocative )
          item.RevocatoryDate = dbItem.RevocatoryDate;
        item.Token = JObject.Parse( dbItem.Content );
        item.AreaScope = "http://www.chuci.info/object/place/20e52e1cb46f4e7e8a8f32009b3e7ec2"; // china

        string json = Newtonsoft.Json.JsonConvert.SerializeObject( item, Newtonsoft.Json.Formatting.Indented );
        var document = BsonSerializer.Deserialize<BsonDocument>( json );
        items.Add( document );
      }
      Console.WriteLine( "  inserting to mongodb..." );
      await collection.InsertManyAsync( items );

      Console.WriteLine( "over..." );
    }

  }

}
