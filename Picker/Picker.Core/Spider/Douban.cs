using System;
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
          await store.Douban_SaveUserTask( id, uid, true );
        }
      }

      // ==== 处理一个UserTask的流程 ====
      // user info
      await processUserInfo( id, jobjFirtTime, false );
      // followers
      await processFollowers( id );
      // update task
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

    #endregion Public Methods


    async Task<int> processUserInfo( string id, JObject data, bool updateIfExists ) {
      bool exists = await store.Douban_UserExists( id );
      if ( !exists || updateIfExists ) {
        if ( data == null )
          data = await api.GetUserInfo( id );
        id = (string)data["id"];
        string uid = (string)data["uid"];
        // 保存用户信息
        return await store.Douban_SaveUser( id, uid, data.ToString(), updateIfExists, true );
      }
      return 0;
    }

    async Task processFollowers( string id ) {
      // 获取原始的API地址
      string apiUri = String.Format( DoubanApi.Api_MyFollowing, id, 0, api.AppKey );
      string originalApiUri = Helpers.ApiHelper.GetApi( apiUri );

      // 正常的用户抓取流程
      int pageIndex = -1;
      bool hasMore = false;
      do {
        pageIndex++;
        // 获取关注的人
        var followers = await api.GetFollowers( id, pageIndex );
        if ( followers != null && followers.Count > 0 ) {
          // 给关注者新建用户任务
          await store.Douban_SaveUserTasks( followers ); // var task2 =
          // save log
          config.Save( Configuration.Key_Douban_User, originalApiUri, pageIndex );
        }
        // continue?
        hasMore = ( followers.Count >= DoubanApi.CountPerPage );
      } // do
      while ( hasMore );

      // 移除有关上一次访问API的记录
      config.RemoveAccessLog( Configuration.Key_Douban_User );
    }

    async Task processBooks( string userId, bool updateIfExists ) {
      // 获取原始的API地址
      string apiUri = String.Format( DoubanApi.Api_MyBookCollections, userId, 0, api.AppKey );
      string originalApiUri = Helpers.ApiHelper.GetApi( apiUri );

      // 正常的用户抓取流程
      int pageIndex = -1;
      bool hasMore = false;
      do {
        pageIndex++;
        var books = await api.GetMyBookCollections( userId, pageIndex );
        if ( books != null && books.Count > 0 ) {
          await store.Douban_SaveBooks( books, updateIfExists );
          // save log
          config.Save( Configuration.Key_Douban_Book, originalApiUri, pageIndex );
        }
        // continue?
        hasMore = ( books.Count >= DoubanApi.CountPerPage );
      } // do
      while ( hasMore );

      // 移除有关上一次访问API的记录
      config.RemoveAccessLog( Configuration.Key_Douban_Book );
    }

    async Task processTravel( string userId, bool updateIfExists ) {
      // 获取原始的API地址
      string apiUri = String.Format( DoubanApi.Api_MyTravelCollections, userId, 0, api.AppKey );
      string originalApiUri = Helpers.ApiHelper.GetApi( apiUri );

      // 正常的用户抓取流程
      int pageIndex = -1;
      bool hasMore = false;
      do {
        pageIndex++;
        var places = await api.GetMyTravelCollections( userId, pageIndex );
        if ( places != null && places.Count > 0 ) {
          // Api_TravelPlaceById 无法获取内容，所以直接保存collection中的数据
          await store.Douban_SaveTravels( places, updateIfExists );
          // save log
          config.Save( Configuration.Key_Douban_Travel, originalApiUri, pageIndex );
        }
        // continue?
        hasMore = ( places.Count >= DoubanApi.CountPerPage );
      } // do
      while ( hasMore );

      // 移除有关上一次访问API的记录
      config.RemoveAccessLog( Configuration.Key_Douban_Travel );
    }

  }

}
