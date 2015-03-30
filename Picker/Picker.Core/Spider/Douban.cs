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

    public Douban( DoubanApi _api, IStorage storeInstance ) {
      api = _api;
      store = storeInstance;
    }

    #region store

    public void SaveBook( JObject book, bool updateIfExists ) {
      if ( book == null )
        return;
      store.Douban_SaveBook( (string)book["alt"], book, updateIfExists, true );
    }

    public void SaveBooks( Dictionary<string, JObject> books, bool updateIfExists ) {
      if ( books == null || books.Count == 0 )
        return;
      store.Douban_SaveBooks( books, updateIfExists );
    }

    #endregion store

    public async Task StartUserTask() {
      string id = store.Douban_GetUndoneUserTask();
      JObject jobjFirtTime = null;
      // 判断两种情况：第一次运行用户任务；以taurenshaman开始辐射的用户抓取任务是否已经完成
      if ( string.IsNullOrWhiteSpace( id ) ) {
        string uid = "taurenshaman";
        bool exists = store.Douban_UserTaskExsits( uid );
        if ( exists ) { // 任务不存在，直接用
          bool complete = store.Douban_UserTaskIsComplete( uid );
          if ( complete ) // 任务未完成，直接用；若已完成，抛异常
            throw new Exception( "以taurenshaman开始辐射的用户抓取任务已经完成" );
          else
            id = store.DoubanUser_GetIdByUid( uid );
        }
        else { // 第一次运行用户任务
          jobjFirtTime = await api.GetUserInfo( uid );
          id = (string)jobjFirtTime["id"];
          // 保存用户信息
          var firstTimeTask1 = store.Douban_SaveUser( id, uid, jobjFirtTime.ToString(), false, true );
          // 新建用户任务
          var firstTimeTask2 = store.Douban_SaveUserTask( id, uid, true );
          Task.WaitAll( firstTimeTask1, firstTimeTask2 );
        }
      }

      // 正常的用户抓取流程
      int pageIndex = -1;
      bool hasMore = false;
      do {
        pageIndex++;
        // 获取关注的人
        var followers = await api.GetFollowers( id, pageIndex );
        if ( followers != null && followers.Count > 0 ) {
          // 保存关注者的信息
          var task1 = store.Douban_SaveUsers( followers, false );
          // 给关注者新建用户任务
          var task2 = store.Douban_SaveUserTasks( followers );
          Task.WaitAll( task1, task2 );
          // 更新用户的任务状态
          store.Douban_UpdateUserTask( id, true );
          // save log

        }
        // continue?
        hasMore = ( followers.Count >= DoubanApi.CountPerPage );
      }
      while ( hasMore );
      

    }

  }

}
