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
using Picker.Postgresql;

namespace Picker.SqlServer {
  public partial class StoreContext {

    /// <summary>
    /// 保存变更；批量同步到数据库
    /// </summary>
    /// <returns></returns>
    public async Task<int> Douban_SaveChanges() {
      return await doubanContext.SaveChangesAsync();
    }

    public List<StatisticsItem> Douban_LoadStatistics() {
      List<StatisticsItem> data = new List<StatisticsItem>();
      long userTaskCount = doubanContext.UserTask.LongCount();
      long usersProcessed = doubanContext.User.LongCount();
      data.Add( new StatisticsItem() {
        Title = "Users",
        Count = usersProcessed,
        TaskCount = userTaskCount,
        ProcessedCount = usersProcessed
      } );

      long booksProcessed = doubanContext.UserTask.Where( i => i.BooksProcessedAt != null ).LongCount();
      long booksCount = doubanContext.Book.LongCount();
      data.Add( new StatisticsItem() {
        Title = "Books",
        Count = booksCount,
        TaskCount = userTaskCount,
        ProcessedCount = booksProcessed
      } );

      long moviesCount = doubanContext.Movie.LongCount();
      data.Add( new StatisticsItem() {
        Title = "Movies",
        Count = moviesCount,
        TaskCount = moviesCount,
        ProcessedCount = moviesCount
      } );
      //data.Add( new StatisticsItem() {
      //  Title = "Music",
      //  TotalCount = doubanContext.UserTask.LongCount(),
      //  ProcessedCount = doubanContext.User.LongCount()
      //} );

      long travelProcessed = doubanContext.UserTask.Where( i => i.TravelProcessedAt != null ).LongCount();
      long travelCount = doubanContext.Travel.LongCount();
      data.Add( new StatisticsItem() {
        Title = "Travel Places",
        Count = travelCount,
        TaskCount = userTaskCount,
        ProcessedCount = travelProcessed
      } );
      return data;
    }


    #region Book

    public async Task<int> Douban_SaveBook( string url, JObject data, bool updateIfExists, bool saveChanges ) {
      Book item = doubanContext.Book.Where( i => i.Uri == url ).FirstOrDefault();
      if ( item != null && updateIfExists ) {
        item.Content = data.ToString();
        item.UpdatedAt = DateTime.UtcNow;
      }
      else if ( item == null ) {
        item = new Book();
        item.Uri = url;
        item.Content = data.ToString();
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        doubanContext.Book.Add( item );
      }
      if ( saveChanges )
        return await doubanContext.SaveChangesAsync();
      return 0;
    }

    public async Task<int> Douban_SaveBooks( Dictionary<string, JObject> data, bool updateIfExists ) {
      foreach ( var item in data ) {
        await Douban_SaveBook( item.Key, item.Value, updateIfExists, false );
      }
      return await doubanContext.SaveChangesAsync();
    }

    public async Task<int> Douban_DeleteBook( string url ) {
      Book item = doubanContext.Book.Where( i => i.Uri == url ).FirstOrDefault();
      if ( item != null ) {
        doubanContext.Book.Remove( item );
        return await doubanContext.SaveChangesAsync();
      }
      return 0;
    }

    #endregion Book


    #region Movie

    public async Task<int> Douban_SaveMovie( string url, JObject data, bool updateIfExists, bool saveChanges ) {
      Movie item = doubanContext.Movie.Where( i => i.Uri == url ).FirstOrDefault();
      if ( item != null && updateIfExists ) {
        item.Content = data.ToString();
        item.UpdatedAt = DateTime.UtcNow;
      }
      else if ( item == null ) {
        item = new Movie();
        item.Uri = url;
        item.Content = data.ToString();
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        doubanContext.Movie.Add( item );
      }
      if ( saveChanges )
        return await doubanContext.SaveChangesAsync();
      return 0;
    }

    public async Task<int> Douban_SaveMovies( Dictionary<string, JObject> data, bool updateIfExists ) {
      foreach ( var item in data ) {
        await Douban_SaveMovie( item.Key, item.Value, updateIfExists, false );
      }
      return await doubanContext.SaveChangesAsync();
    }

    public async Task<int> Douban_DeleteMovie( string url ) {
      Movie item = doubanContext.Movie.Where( i => i.Uri == url ).FirstOrDefault();
      if ( item != null ) {
        doubanContext.Movie.Remove( item );
        return await doubanContext.SaveChangesAsync();
      }
      return 0;
    }

    #endregion Movie


    #region Music

    //public async Task<int> Douban_SaveMusicTask( string id, string uid, bool saveChanges ) {
    //  return 0;
    //}

    //public async Task<int> Douban_UpdateMusicTask( string id, string uid, bool saveChanges ) {
    //  return 0;
    //}

    #endregion Music


    #region Travel

    public async Task<int> Douban_SaveTravel( string url, JObject data, bool updateIfExists, bool saveChanges ) {
      Travel item = doubanContext.Travel.Where( i => i.Uri == url ).FirstOrDefault();
      if ( item != null && updateIfExists ) {
        item.Content = data.ToString();
        item.UpdatedAt = DateTime.UtcNow;
      }
      else if ( item == null ) {
        item = new Travel();
        item.Uri = url;
        item.Content = data.ToString();
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        doubanContext.Travel.Add( item );
      }
      if ( saveChanges )
        return await doubanContext.SaveChangesAsync();
      return 0;
    }

    public async Task<int> Douban_SaveTravels( Dictionary<string, JObject> data, bool updateIfExists ) {
      foreach ( var item in data ) {
        await Douban_SaveTravel( item.Key, item.Value, updateIfExists, false );
      }
      return await doubanContext.SaveChangesAsync();
    }

    public async Task<int> Douban_DeleteTravel( string url ) {
      Travel item = doubanContext.Travel.Where( i => i.Uri == url ).FirstOrDefault();
      if ( item != null ) {
        doubanContext.Travel.Remove( item );
        return await doubanContext.SaveChangesAsync();
      }
      return 0;
    }

    #endregion Travel


    #region User

    public bool Douban_UserTaskExsits( string uid ) {
      var tmp = doubanContext.UserTask.Where( i => i.uid == uid ).FirstOrDefault();
      return ( tmp != null );
    }

    public string DoubanUserTask_GetIdByUid( string uid ) {
      var tmp = doubanContext.UserTask.Where( i => i.uid == uid ).FirstOrDefault();
      return tmp == null ? null : tmp.id;
    }


    /// <summary>
    /// 获取一个未处理的UserTask（ProcessedAt为null，或者now-value>=interval）的id
    /// </summary>
    /// <returns></returns>
    public string Douban_GetUndoneUserTask( TimeSpan? interval ) {
      double intervalSeconds = interval.HasValue ? interval.Value.TotalSeconds : 0;
      // 旧版本没有设置type，即前两个判断；新版本只判断"user"类型的（其它还有virtual、site等）
      var tmp = doubanContext.UserTask.Where( i => ( i.type == null || i.type.Length == 0 || i.type == typeUser ) && !i.IsBanned &&
        ( i.ProcessedAt == null || ( interval.HasValue && ( System.Data.Entity.DbFunctions.DiffSeconds( DateTime.UtcNow, i.ProcessedAt.Value ) < intervalSeconds ) ) )
        )
        .FirstOrDefault();
      return tmp == null ? null : tmp.id;
    }

    /// <summary>
    /// 获取一个未处理的UserTask（BooksProcessedAt为null，或者now-value>=interval）的id
    /// </summary>
    public string DoubanBook_GetUndoneUserTask( TimeSpan? interval ) {
      double intervalSeconds = interval.HasValue ? interval.Value.TotalSeconds : 0;
      // 旧版本没有设置type，即前两个判断；新版本只判断"user"类型的（其它还有virtual、site等）
      var tmp = doubanContext.UserTask.Where( i => ( i.type == null || i.type.Length == 0 || i.type == typeUser ) && !i.IsBanned &&
        ( i.BooksProcessedAt == null || ( interval.HasValue && ( System.Data.Entity.DbFunctions.DiffSeconds( DateTime.UtcNow, i.BooksProcessedAt.Value ) < intervalSeconds ) ) ) )
        .FirstOrDefault();
      return tmp == null ? null : tmp.id;
    }

    /// <summary>
    /// 获取一个未处理的UserTask（TravelProcessedAt为null，或者now-value>=interval）的id
    /// </summary>
    public string DoubanTravel_GetUndoneUserTask( TimeSpan? interval ) {
      double intervalSeconds = interval.HasValue ? interval.Value.TotalSeconds : 0;
      // 旧版本没有设置type，即前两个判断；新版本只判断"user"类型的（其它还有virtual、site等）
      var tmp = doubanContext.UserTask.Where( i => ( i.type == null || i.type.Length == 0 || i.type == typeUser ) && !i.IsBanned &&
        ( i.TravelProcessedAt == null || ( interval.HasValue && ( System.Data.Entity.DbFunctions.DiffSeconds( DateTime.UtcNow, i.TravelProcessedAt.Value ) < intervalSeconds ) ) ) )
        .FirstOrDefault();
      return tmp == null ? null : tmp.id;
    }


    public bool Douban_UserTaskIsComplete( string uid, TimeSpan? interval ) {
      var tmp = doubanContext.UserTask.Where( i => i.uid == uid ).FirstOrDefault();
      return TimeHelper.IsCompleted( tmp.ProcessedAt, interval );
    }

    public bool DoubanBook_UserTaskIsComplete( string uid, TimeSpan? interval ) {
      var tmp = doubanContext.UserTask.Where( i => i.uid == uid ).FirstOrDefault();
      return TimeHelper.IsCompleted( tmp.BooksProcessedAt, interval );
    }

    public bool DoubanTravel_UserTaskIsComplete( string uid, TimeSpan? interval ) {
      var tmp = doubanContext.UserTask.Where( i => i.uid == uid ).FirstOrDefault();
      return TimeHelper.IsCompleted( tmp.TravelProcessedAt, interval );
    }


    /// <summary>
    /// 增加UserTask，但不设置ProcessedAt，如果已经存在，什么也不做。
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="saveChanges"></param>
    /// <returns></returns>
    public async Task<int> Douban_SaveUserTask( string id, string uid, JObject obj, bool saveChanges ) {
      var tmp = doubanContext.UserTask.Where( i => i.id == id ).FirstOrDefault();
      if ( tmp != null )
        return 0;

      tmp = new UserTask();
      tmp.id = id;
      tmp.uid = uid;
      tmp.Content = obj.ToString();
      tmp.type = (string)obj[propertyNameType];
      doubanContext.UserTask.Add( tmp );

      if ( saveChanges )
        //return await doubanContext.SaveChangesAsync();
        return doubanContext.SaveChanges();
      return 1;
    }

    public async Task<int> Douban_SaveUserTasks( List<Tuple<string, string, JObject>> data ) {
      if ( data == null || data.Count == 0 )
        return 0;
      foreach ( var tuple in data ) {
        await Douban_SaveUserTask( tuple.Item1, tuple.Item2, tuple.Item3, false );
      }
      return await doubanContext.SaveChangesAsync();
    }

    /// <summary>
    /// 更新type和IsBanned
    /// </summary>
    public async Task<int> Douban_UpdateUserTask( string id, string type, bool isBanned, bool saveChanges ) {
      var tmp = doubanContext.UserTask.Where( i => i.id == id ).FirstOrDefault();
      if ( tmp == null )
        return 0;
      tmp.type = type;
      tmp.IsBanned = isBanned;
      if ( saveChanges )
        return await doubanContext.SaveChangesAsync();
      return 1;
    }

    /// <summary>
    /// 更新UserTask.ProcessedAt为当前时间
    /// </summary>
    public async Task<int> Douban_UpdateUserTask( string id, bool saveChanges ) {
      var tmp = doubanContext.UserTask.Where( i => i.id == id ).FirstOrDefault();
      if ( tmp == null )
        return 0;
      tmp.ProcessedAt = DateTime.UtcNow;
      if ( saveChanges )
        return await doubanContext.SaveChangesAsync();
      return 1;
    }

    /// <summary>
    /// 更新UserTask.BooksProcessedAt为当前时间
    /// </summary>
    public async Task<int> DoubanBook_UpdateUserTask( string id, bool saveChanges ) {
      var tmp = doubanContext.UserTask.Where( i => i.id == id ).FirstOrDefault();
      if ( tmp == null )
        return 0;
      tmp.BooksProcessedAt = DateTime.UtcNow;
      if ( saveChanges )
        return await doubanContext.SaveChangesAsync();
      return 1;
    }

    /// <summary>
    /// 更新UserTask.TravelProcessedAt为当前时间
    /// </summary>
    public async Task<int> DoubanTravel_UpdateUserTask( string id, bool saveChanges ) {
      var tmp = doubanContext.UserTask.Where( i => i.id == id ).FirstOrDefault();
      if ( tmp == null )
        return 0;
      tmp.TravelProcessedAt = DateTime.UtcNow;
      if ( saveChanges )
        return await doubanContext.SaveChangesAsync();
      return 1;
    }


    public async Task<bool> Douban_UserExists( string id ) {
      User item = doubanContext.User.Where( i => i.id == id ).FirstOrDefault();
      return ( item != null );
    }

    public async Task<int> Douban_SaveUser( string id, string uid, JObject data, bool updateIfExists, bool saveChanges ) {
      User item = doubanContext.User.Where( i => i.id == id ).FirstOrDefault();
      if ( item != null && updateIfExists ) {
        item.Content = data.ToString();
        item.IsBanned = (bool)data[propertyNameIsBanned];
        item.UpdatedAt = DateTime.UtcNow;
      }
      else {
        item = new User();
        item.id = id;
        item.uid = uid;
        item.type = (string)data[propertyNameType];
        item.Content = data.ToString();
        item.IsBanned = (bool)data[propertyNameIsBanned];
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        doubanContext.User.Add( item );
      }
      if ( saveChanges )
        //return await doubanContext.SaveChangesAsync();
        return doubanContext.SaveChanges();
      return 0;
    }

    public async Task<int> Douban_SaveUsers( List<Tuple<string, string, JObject>> data, bool updateIfExists ) {
      if ( data == null || data.Count == 0 )
        return 0;
      foreach ( var tuple in data ) {
        await Douban_SaveUser( tuple.Item1, tuple.Item2, tuple.Item3, updateIfExists, false );
        // update task
        string type = (string)tuple.Item3[propertyNameType];
        bool isBanned = (bool)tuple.Item3[propertyNameIsBanned];
        await Douban_UpdateUserTask( tuple.Item1, type, isBanned, false );
      }
      return await doubanContext.SaveChangesAsync();
    }

    #endregion User

    /// <summary>
    /// 在事务中，同时保存多个表的Task
    /// </summary>
    /// <param name="id"></param>
    /// <param name="uid"></param>
    /// <returns></returns>
    public async Task<int> Douban_SaveTasks( string id, string uid ) {
      //using ( NpgsqlTransaction t = new NpgsqlTransaction() ) {
      //  Douban_SaveUserTask( id, uid, false );
      //  t.Commit();
      //  t.Dispose();
      //}
      return 0;
    }

  }

}
