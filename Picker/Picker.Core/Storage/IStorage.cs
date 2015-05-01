﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Picker.Core.Models;

namespace Picker.Core.Storage {
  public interface IStorage {
    void OpenDatabase( string connString );
    /// <summary>
    /// 保存变更；批量同步到数据库
    /// </summary>
    /// <returns></returns>
    Task<int> SaveChanges();


    List<StatisticsItem> LoadStatistics();


    //Task<int> Douban_SaveBookTask( string id, string uid, bool saveChanges );
    //Task<int> Douban_UpdateBookTask( string id, string uid, bool saveChanges );
    Task<int> Douban_SaveBook( string url, JObject data, bool updateIfExists, bool saveChanges );
    Task<int> Douban_SaveBooks( Dictionary<string, JObject> data, bool updateIfExists );
    Task<int> Douban_DeleteBook( string url );


    //Task<int> Douban_SaveMovieTask( string id, string uid, bool saveChanges );
    //Task<int> Douban_UpdateMovieTask( string id, string uid, bool saveChanges );
    Task<int> Douban_SaveMovie( string url, JObject data, bool updateIfExists, bool saveChanges );
    Task<int> Douban_SaveMovies( Dictionary<string, JObject> data, bool updateIfExists );
    Task<int> Douban_DeleteMovie( string url );


    //Task<int> Douban_SaveMusicTask( string id, string uid, bool saveChanges );
    //Task<int> Douban_UpdateMusicTask( string id, string uid, bool saveChanges );


    Task<int> Douban_SaveTravel( string url, JObject data, bool updateIfExists, bool saveChanges );
    Task<int> Douban_SaveTravels( Dictionary<string, JObject> data, bool updateIfExists );
    Task<int> Douban_DeleteTravel( string url );


    bool Douban_UserTaskExsits( string uid );
    string DoubanUserTask_GetIdByUid( string uid );

    /// <summary>
    /// 获取一个未处理的UserTask（ProcessedAt为null，或者now-value>=interval）的id
    /// </summary>
    string Douban_GetUndoneUserTask( TimeSpan? interval );
    /// <summary>
    /// 获取一个未处理的UserTask（BooksProcessedAt为null，或者now-value>=interval）的id
    /// </summary>
    string DoubanBook_GetUndoneUserTask( TimeSpan? interval );
    /// <summary>
    /// 获取一个未处理的UserTask（TravelProcessedAt为null，或者now-value>=interval）的id
    /// </summary>
    string DoubanTravel_GetUndoneUserTask( TimeSpan? interval );

    bool Douban_UserTaskIsComplete( string uid, TimeSpan? interval );
    bool DoubanBook_UserTaskIsComplete( string uid, TimeSpan? interval );
    bool DoubanTravel_UserTaskIsComplete( string uid, TimeSpan? interval );

    /// <summary>
    /// 增加UserTask，如果已经存在，什么也不做。
    /// </summary>
    Task<int> Douban_SaveUserTask( string id, string uid, bool saveChanges );
    Task<int> Douban_SaveUserTasks( List<Tuple<string, string, string>> data );

    /// <summary>
    /// 更新UserTask.ProcessedAt为当前时间
    /// </summary>
    Task<int> Douban_UpdateUserTask( string id, bool saveChanges );
    /// <summary>
    /// 更新UserTask.BooksProcessedAt为当前时间
    /// </summary>
    Task<int> DoubanBook_UpdateUserTask( string id, bool saveChanges );
    /// <summary>
    /// 更新UserTask.TravelProcessedAt为当前时间
    /// </summary>
    Task<int> DoubanTravel_UpdateUserTask( string id, bool saveChanges );
    
    Task<bool> Douban_UserExists( string id );
    Task<int> Douban_SaveUser( string id, string uid, string content, bool updateIfExists, bool saveChanges );
    Task<int> Douban_SaveUsers( List<Tuple<string, string, string>> data, bool updateIfExists );

  }

}
