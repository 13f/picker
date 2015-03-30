using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Picker.Core.Storage {
  public interface IStorage {
    void OpenDatabase( string connString );
    /// <summary>
    /// 保存变更；批量同步到数据库
    /// </summary>
    /// <returns></returns>
    Task<int> SaveChanges();


    Task<int> Douban_SaveBook( string url, JObject data, bool updateIfExists, bool saveChanges );
    Task<int> Douban_SaveBooks( Dictionary<string, JObject> data, bool updateIfExists );
    Task<int> Douban_DeleteBook( string url );


    Task<int> Douban_SaveMovie( string url, JObject data, bool updateIfExists, bool saveChanges );
    Task<int> Douban_DeleteMovie( string url );


    /// <summary>
    /// 获取一个未处理的UserTask（ProcessedAt为null）的id
    /// </summary>
    string Douban_GetUndoneUserTask();
    bool Douban_UserTaskExsits( string uid );
    bool Douban_UserTaskIsComplete( string uid );
    /// <summary>
    /// 增加UserTask，如果已经存在，什么也不做。
    /// </summary>
    Task<int> Douban_SaveUserTask( string id, string uid, bool saveChanges );
    /// <summary>
    /// 更新UserTask.ProcessedAt为当前时间
    /// </summary>
    Task<int> Douban_UpdateUserTask( string id, bool saveChanges );
    Task<int> Douban_SaveUserTasks( List<Tuple<string, string, string>> data );
    string DoubanUser_GetIdByUid( string uid );
    Task<int> Douban_SaveUser( string id, string uid, string content, bool updateIfExists, bool saveChanges );
    Task<int> Douban_SaveUsers( List<Tuple<string, string, string>> data, bool updateIfExists );

  }

}
