﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Npgsql;
using Picker.Core.Storage;

namespace Picker.Postgresql {
  public class StoreContext : IStorage {
    //NpgsqlConnection conn = null;
    DoubanEntities doubanContext = null;

    public StoreContext( string connString ) {
      OpenDatabase( connString );
    }

    public void OpenDatabase( string connString ) {
      //conn = new NpgsqlConnection( connString );
      doubanContext = new DoubanEntities( connString );
    }

    /// <summary>
    /// 保存变更；批量同步到数据库
    /// </summary>
    /// <returns></returns>
    public async Task<int> SaveChanges() {
      return await doubanContext.SaveChangesAsync();
    }


    #region Book

    public async Task<int> Douban_SaveBook( string url, JObject data, bool updateIfExists, bool saveChanges ) {
      Book book = doubanContext.Book.Where( i => i.Uri == url ).FirstOrDefault();
      if ( book != null && updateIfExists ) {
        book.Content = data.ToString();
        book.UpdatedAt = DateTime.UtcNow;
      }
      else {
        book = new Book();
        book.Content = data.ToString();
        book.CreatedAt = DateTime.UtcNow;
        book.UpdatedAt = DateTime.UtcNow;
        doubanContext.Book.Add( book );
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
      Book book = doubanContext.Book.Where( i => i.Uri == url ).FirstOrDefault();
      if ( book != null ) {
        doubanContext.Book.Remove( book );
        return await doubanContext.SaveChangesAsync();
      }
      return 0;
    }

    #endregion Book


    #region Movie

    public async Task<int> Douban_SaveMovie( string url, JObject data, bool updateIfExists, bool saveChanges ) {
      return await doubanContext.SaveChangesAsync();
    }

    public async Task<int> Douban_DeleteMovie( string url ) {
      return 0;
    }

    #endregion Movie


    #region User

    /// <summary>
    /// 获取一个未处理的UserTask（ProcessedAt为null）的id
    /// </summary>
    /// <returns></returns>
    public string Douban_GetUndoneUserTask() {
      var tmp = doubanContext.UserTask.Where( i => i.ProcessedAt == null ).FirstOrDefault();
      return tmp == null ? null : tmp.id;
    }

    public bool Douban_UserTaskExsits( string uid ) {
      var tmp = doubanContext.UserTask.Where( i => i.uid == uid ).FirstOrDefault();
      return ( tmp != null );
    }

    public bool Douban_UserTaskIsComplete( string uid ) {
      var tmp = doubanContext.UserTask.Where( i => i.uid == uid ).FirstOrDefault();
      return ( tmp.ProcessedAt != null );
    }

    public string DoubanUser_GetIdByUid( string uid ) {
      var tmp = doubanContext.UserTask.Where( i => i.uid == uid ).FirstOrDefault();
      return tmp == null ? null : tmp.id;
    }

    /// <summary>
    /// 增加UserTask，如果已经存在，什么也不做。
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="saveChanges"></param>
    /// <returns></returns>
    public async Task<int> Douban_SaveUserTask( string id, string uid, bool saveChanges ) {
      var tmp = doubanContext.UserTask.Where( i => i.id == id ).FirstOrDefault();
      if ( tmp != null )
        return 0;
      
      tmp = new UserTask();
      tmp.id = id;
      tmp.uid = uid;
      tmp.ProcessedAt = DateTime.UtcNow;
      doubanContext.UserTask.Add( tmp );

      if ( saveChanges )
        return await doubanContext.SaveChangesAsync();
      return 1;
    }

    /// <summary>
    /// 更新UserTask.ProcessedAt为当前时间
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="saveChanges"></param>
    /// <returns></returns>
    public async Task<int> Douban_UpdateUserTask( string id, bool saveChanges ) {
      var tmp = doubanContext.UserTask.Where( i => i.id == id ).FirstOrDefault();
      if ( tmp == null )
        return 0;
      tmp.ProcessedAt = DateTime.UtcNow;
      if ( saveChanges )
        return await doubanContext.SaveChangesAsync();
      return 1;
    }

    public async Task<int> Douban_SaveUser( string id, string uid, string content, bool updateIfExists, bool saveChanges ) {
      User item = doubanContext.User.Where( i => i.id == id ).FirstOrDefault();
      if ( item != null && updateIfExists ) {
        item.Content = content;
        item.UpdatedAt = DateTime.UtcNow;
      }
      else {
        item = new User();
        item.Content = content;
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        doubanContext.User.Add( item );
      }
      if ( saveChanges )
        return await doubanContext.SaveChangesAsync();
      return 0;
    }

    public async Task<int> Douban_SaveUsers( List<Tuple<string, string, string>> data, bool updateIfExists ) {
      if ( data == null || data.Count == 0 )
        return 0;
      foreach ( var tuple in data ) {
        await Douban_SaveUser( tuple.Item1, tuple.Item2, tuple.Item3, updateIfExists, false );
      }
      return await doubanContext.SaveChangesAsync();
    }

    #endregion User

  }

}