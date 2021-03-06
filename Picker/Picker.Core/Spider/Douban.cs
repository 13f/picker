﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Picker.Core.Storage;

namespace Picker.Core.Spider {
  public class Douban {
    DoubanApi api = null;
    IStorage store = null;
    Configuration config = null;

    public Douban( DoubanApi _api, IStorage _store, Configuration _config ) {
      api = _api;
      store = _store;
      config = _config;
    }


    #region Public Methods

    // TODO: updateIfExists vs. interval .............................................................

    /// <summary>
    /// 初始化的时候可以使用"taurenshaman"作为startingUserId，或者UI上读取
    /// </summary>
    /// <param name="startingUserId"></param>
    /// <returns>UserTask.id</returns>
    public async Task StartUserTask( TimeSpan? interval, string startingUserId, bool loopWhenfinished ) {
      // 读取一个未完成的任务
      string id = store.Douban_GetUndoneUserTask( interval );
      JObject jobjFirtTime = null;
      // 判断两种情况：第一次运行用户任务；以taurenshaman开始辐射的用户抓取任务是否已经完成
      if ( string.IsNullOrWhiteSpace( id ) ) {
        if ( string.IsNullOrWhiteSpace( startingUserId ) ) {
          throw new Exception( "当前没有未完成的UserTask，且初始的UserId为空" );
        }
        string uid = startingUserId;
        bool exists = store.Douban_UserTaskExsits( uid );
        if ( exists ) { // 任务存在，直接用
          bool complete = store.Douban_UserTaskIsComplete( uid, interval );
          if ( complete ) // 任务未完成，直接用；若已完成，抛异常
            throw new Exception( "以" + startingUserId + "开始辐射的用户抓取任务已经完成" );
          else
            id = store.DoubanUserTask_GetIdByUid( uid );
        }
        else { // 第一次运行初始的用户任务
          jobjFirtTime = await api.GetUserInfo( uid );
          id = (string)jobjFirtTime["id"];
          // 新建用户任务
          await store.Douban_SaveUserTask( id, uid, jobjFirtTime, true );
        }
      }

      // ==== 处理一个UserTask的流程 ====
      if ( jobjFirtTime == null )
        jobjFirtTime = await api.GetUserInfo( id );
      bool isBanned = (bool)jobjFirtTime["is_banned"];
      // user info
      await processUserInfo( id, jobjFirtTime, false );
      // followings
      if ( !isBanned )
        await processFollowings( id );
      // update tag
      await store.Douban_UpdateUserTask( id, true );
      
      // confinue?
      if ( loopWhenfinished )
        await StartUserTask( interval, null, loopWhenfinished );
    } // StartUserTask( bool loopWhenfinished )

    /// <summary>
    /// 初始化的时候可以使用"taurenshaman"作为startingUserId，或者UI上读取
    /// </summary>
    /// <param name="startingUserId"></param>
    /// <param name="loopWhenfinished"></param>
    /// <returns></returns>
    public async Task StartBookTask( TimeSpan? interval, bool loopWhenfinished ) {
      // 读取一个未完成的任务
      string id = store.DoubanBook_GetUndoneUserTask( interval );
      if ( string.IsNullOrWhiteSpace( id ) )
        throw new Exception( "任务都已完成" );
      
      // process
      await processBooks( id, false );
      // update tag
      await store.DoubanBook_UpdateUserTask( id, true );
      // confinue?
      if ( loopWhenfinished )
        await StartBookTask( interval, loopWhenfinished );
    }

    //public async Task StartBookTask( string api ) { }

    /// <summary>
    /// 暂时只能获取Top250
    /// </summary>
    /// <returns></returns>
    public async Task StartMovieTask_Top250( bool updateIfExists, int defaultPageIndex = 0 ) {
      bool hasMore = false;
      int pageIndex = defaultPageIndex;
      int CountPerPage = 20;
      do {
        int start = pageIndex * CountPerPage;
        var items = await api.GetMovies_Top250( start );
        if ( items != null && items.Count > 0 ) {
          await store.Douban_SaveMovies( items, updateIfExists );
          // save log
          config.Save( Configuration.Key_Douban_Movie, DoubanApi.Api_MovieTop250, pageIndex );
        }
        // continue?
        hasMore = ( items.Count >= CountPerPage );
        pageIndex++;
      } // do
      while ( hasMore );

      // 移除有关上一次访问API的记录
      config.RemoveAccessLog( Configuration.Key_Douban_Movie );
    }

    //public async Task StartMusicTask() { }

    /// <summary>
    /// 初始化的时候可以使用"taurenshaman"作为startingUserId，或者UI上读取
    /// </summary>
    /// <param name="startingUserId"></param>
    /// <param name="loopWhenfinished"></param>
    /// <returns></returns>
    public async Task StartTravelTask( TimeSpan? interval, bool loopWhenfinished ) {
      // 读取一个未完成的任务
      string id = store.DoubanTravel_GetUndoneUserTask( interval );
      if ( string.IsNullOrWhiteSpace( id ) )
        throw new Exception( "任务都已完成" );

      // process
      await processTravel( id, false );
      // update tag
      await store.DoubanTravel_UpdateUserTask( id, true );
      // confinue?
      if ( loopWhenfinished )
        await StartTravelTask( interval, loopWhenfinished );
    }

    /// <summary>
    /// 处理特定的豆瓣用户：创建UserTask，获取UserInfo，获取Followings，获取Books，获取Travel，每次获取信息都会更新UserTask
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task StartTask( TimeSpan? interval, string userId ) {
      if ( string.IsNullOrWhiteSpace( userId ) )
        throw new Exception( "UserId为空" );
      
      string id = null;
      JObject data = null;
      bool userInfoComplete = false,
        booksComplete = false,
        travelComplete = false;
      bool isBanned = false;

      bool exists = store.Douban_UserTaskExsits( userId );
      if ( exists ) { // 任务存在，直接查询
        userInfoComplete = store.Douban_UserTaskIsComplete( userId, interval );
        booksComplete = store.DoubanBook_UserTaskIsComplete( userId, interval );
        travelComplete = store.DoubanTravel_UserTaskIsComplete( userId, interval );
        id = store.DoubanUserTask_GetIdByUid( userId );
      }
      else { // 任务不存在，生成一个
        data = await api.GetUserInfo( userId );
        id = (string)data["id"];
        isBanned = (bool)data["is_banned"];
        // 新建用户任务
        await store.Douban_SaveUserTask( id, userId, data, true );
      }

      if ( userInfoComplete && booksComplete && travelComplete )
        throw new Exception( "注意：该用户所有任务已经完成了一遍。" );

      if ( !userInfoComplete ) { // 即使被封，也要获取一下UserInfo
        if ( data == null )
          data = await api.GetUserInfo( userId );
        isBanned = (bool)data["is_banned"];
        // user info
        await processUserInfo( id, data, false );
        // followings
        if ( !isBanned )
          await processFollowings( id );
        // update task
        await store.Douban_UpdateUserTask( id, true );
      }
      if ( !booksComplete && !isBanned ) {
        // process
        await processBooks( id, false );
        // update tag
        await store.DoubanBook_UpdateUserTask( id, true );
      }
      if ( !travelComplete && !isBanned ) {
        // process
        await processTravel( id, false );
        // update tag
        await store.DoubanTravel_UpdateUserTask( id, true );
      }
    }

    /// <summary>
    /// 继续上次的任务
    /// </summary>
    /// <param name="lastApi"></param>
    /// <param name="lastPageIndex"></param>
    /// <param name="lastUserId"></param>
    /// <returns></returns>
    public async Task ContinueLastTask( string group, string lastApi, int lastPageIndex, string lastUserId, int countPerPage ) {
      if ( group == Configuration.Key_Douban_Page ) { // douban page
        await PickItemsOfPage( lastApi, countPerPage, false, lastPageIndex );
        return;
      }

      switch ( lastApi ) {
        case DoubanApi.Api_MyFollowing:
          await processFollowings( lastUserId, lastPageIndex );
          // update tag
          await store.Douban_UpdateUserTask( lastUserId, true );
          break;
        case DoubanApi.Api_MyBookCollections:
          await processBooks( lastUserId, false, lastPageIndex );
          // update tag
          await store.DoubanBook_UpdateUserTask( lastUserId, true );
          break;
        case DoubanApi.Api_MyTravelCollections:
          await processTravel( lastUserId, false, lastPageIndex );
          // update tag
          await store.DoubanTravel_UpdateUserTask( lastUserId, true );
          break;
        case DoubanApi.Api_MovieTop250:
          await StartMovieTask_Top250( false, lastPageIndex );
          break;
      }
    }

    /// <summary>
    /// 根据条目链接获取条目
    /// </summary>
    /// <param name="subjectUrl"></param>
    /// <param name="updateIfExists"></param>
    /// <returns></returns>
    public async Task PickOneItem( string subjectUrl, bool updateIfExists ) {
      if ( string.IsNullOrWhiteSpace( subjectUrl ) )
        return;
      string id = null;
      if ( subjectUrl.StartsWith( DoubanApi.UriPrefix_Book_Subject ) ) {
        id = api.GetBookId( subjectUrl );
        if ( string.IsNullOrWhiteSpace( id ) )
          return;
        var item = await api.GetBookById( id );
        await store.Douban_SaveBook( subjectUrl, item, updateIfExists, true );
      }
      else if ( subjectUrl.StartsWith( DoubanApi.UriPrefix_Movie_Subject ) ) {
        id = api.GetMovieId( subjectUrl );
        if ( string.IsNullOrWhiteSpace( id ) )
          return;
        var item = await api.GetMovieById( id );
        await store.Douban_SaveMovie( subjectUrl, item, updateIfExists, true );
      }
      
    }

    /// <summary>
    /// 抓取一个页面的所有条目
    /// </summary>
    /// <param name="pageUrl"></param>
    /// <param name="countPerPage"></param>
    /// <param name="updateIfExists"></param>
    /// <param name="defaultPageIndex"></param>
    /// <returns></returns>
    public async Task PickItemsOfPage( string pageUrl, int countPerPage, bool updateIfExists, int defaultPageIndex = 0 ) {
      if ( countPerPage < 0 )
        countPerPage = DoubanApi.CountPerPage;
      int pageIndex = defaultPageIndex;
      bool hasMore = false;
      do {
        var items = await api.GetItemsOfPage( pageUrl, pageIndex, countPerPage );
        if ( items != null && items.Count > 0 ) {
          foreach ( var item in items ) {
            if ( item.Key.StartsWith( DoubanApi.UriPrefix_Book_Subject ) ) {
              await store.Douban_SaveBook( item.Key, item.Value, updateIfExists, false );
            }
            else if ( item.Key.StartsWith( DoubanApi.UriPrefix_Movie_Subject ) ) {
              await store.Douban_SaveMovie( item.Key, item.Value, updateIfExists, false );
            }
          }
          // 遍历完成再保存
          await store.Douban_SaveChanges();
          // save log
          config.Save( Configuration.Key_Douban_Page, pageUrl, pageIndex );
          config.SaveCountPerPage( Configuration.Key_Douban_Page, countPerPage );
        }
        // continue?
        hasMore = ( items.Count >= countPerPage );
        pageIndex++;
        // wait 2 seconds
        await Task.Delay( 2000 );
      } // do
      while ( hasMore );

      // 移除有关上一次访问API的记录
      config.RemoveAccessLog( Configuration.Key_Douban_Page );
    }

    public async Task<int> PickItemsOfPage( string pageUrl, string pageContent, int countPerPage, bool updateIfExists, int pageIndex = 0 ) {
      var items = await api.GetItemsOfPage( pageContent );
      if ( items != null ) {
        if ( items.Count > 0 ) { // save data
          foreach ( var item in items ) {
            if ( item.Key.StartsWith( DoubanApi.UriPrefix_Book_Subject ) ) {
              await store.Douban_SaveBook( item.Key, item.Value, updateIfExists, false );
            }
            else if ( item.Key.StartsWith( DoubanApi.UriPrefix_Movie_Subject ) ) {
              await store.Douban_SaveMovie( item.Key, item.Value, updateIfExists, false );
            }
          }
          // 遍历完成再保存
          await store.Douban_SaveChanges();
          // save log
          config.Save( Configuration.Key_Douban_Page, pageUrl, pageIndex );
          config.SaveCountPerPage( Configuration.Key_Douban_Page, countPerPage );
        }
        // 移除有关上一次访问API的记录
        if(items.Count < countPerPage)
          config.RemoveAccessLog( Configuration.Key_Douban_Page );
        return items.Count;
      }
      return 0;
    }

    #endregion Public Methods


    async Task<int> processUserInfo( string id, JObject data, bool updateIfExists ) {
      bool exists = await store.Douban_UserExists( id );
      if ( !exists || updateIfExists ) {
        if ( data == null )
          data = await api.GetUserInfo( id );
        id = (string)data["id"];
        string uid = (string)data["uid"];
        string type = (string)data["type"];
        bool isBanned = (bool)data["is_banned"];
        // 保存用户信息
        int r = await store.Douban_SaveUser( id, uid, data, updateIfExists, false );
        // update task
        await store.Douban_UpdateUserTask( id, type, isBanned, false );
        await store.Douban_SaveChanges();
        return r;
      }
      return 0;
    }

    async Task processFollowings( string id, int defaultPageIndex = 0 ) {
      // 正常的用户抓取流程
      int pageIndex = defaultPageIndex;
      bool hasMore = false;
      do {
        // 获取关注的人
        var items = await api.GetFollowings( id, pageIndex );
        if ( items != null && items.Count > 0 ) {
          // 给关注者新建用户任务
          await store.Douban_SaveUserTasks( items ); // var task2 =
          // save log
          config.Save( Configuration.Key_Douban_User, DoubanApi.Api_MyFollowing, pageIndex );
          config.SaveUserId( Configuration.Key_Douban_User, id );
        }
        // continue?
        hasMore = ( items.Count >= DoubanApi.CountPerPage );
        pageIndex++;
      } // do
      while ( hasMore );

      // 移除有关上一次访问API的记录
      config.RemoveAccessLog( Configuration.Key_Douban_User );
    }

    async Task processBooks( string userId, bool updateIfExists, int defaultPageIndex = 0 ) {
      // 正常的抓取流程
      int pageIndex = defaultPageIndex;
      bool hasMore = false;
      do {
        var items = await api.GetMyBookCollections( userId, pageIndex );
        if ( items != null && items.Count > 0 ) {
          await store.Douban_SaveBooks( items, updateIfExists );
          // save log
          config.Save( Configuration.Key_Douban_Book, DoubanApi.Api_MyBookCollections, pageIndex );
          config.SaveUserId( Configuration.Key_Douban_Book, userId );
        }
        // continue?
        hasMore = ( items.Count >= DoubanApi.CountPerPage );
        pageIndex++;
      } // do
      while ( hasMore );

      // 移除有关上一次访问API的记录
      config.RemoveAccessLog( Configuration.Key_Douban_Book );
    }

    async Task processTravel( string userId, bool updateIfExists, int defaultPageIndex = 0 ) {
      // 正常的抓取流程
      int pageIndex = defaultPageIndex;
      bool hasMore = false;
      do {
        
        var items = await api.GetMyTravelCollections( userId, pageIndex );
        if ( items != null && items.Count > 0 ) {
          // Api_TravelPlaceById 无法获取内容，所以直接保存collection中的数据
          await store.Douban_SaveTravels( items, updateIfExists );
          // save log
          config.Save( Configuration.Key_Douban_Travel, DoubanApi.Api_MyTravelCollections, pageIndex );
          config.SaveUserId( Configuration.Key_Douban_Travel, userId );
        }
        // continue?
        hasMore = ( items.Count >= DoubanApi.CountPerPage );
        pageIndex++;
      } // do
      while ( hasMore );

      // 移除有关上一次访问API的记录
      config.RemoveAccessLog( Configuration.Key_Douban_Travel );
    }

  }

}
