using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Picker.Core.Storage;

namespace Picker.Core.Spider {
  public class DoubanApi {
    public const int CountPerPage = 20;
    public const string ApiPrefix = "https://api.douban.com/v2/";
    public const string ApiPrefix_Shuo = "https://api.douban.com/shuo/v2/";
    public const string ApiPrefix_Book = ApiPrefix + "book/";
    public const string ApiPrefix_Movie = ApiPrefix + "movie/";
    public const string ApiPrefix_music = ApiPrefix + "music/";

    public const string Api_MyBookCollections = ApiPrefix_Book + "user/{0}/collections?start={1}&apikey={2}";
    public const string Api_BooksOfSerie = ApiPrefix_Book + "series/{0}/books?start={1}&apikey={1}";
    public const string Api_BookById = ApiPrefix_Book + "{0}?apikey={1}";
    public const string Api_BookByIsbn = ApiPrefix_Book + "isbn/{0}?apikey={1}";

    public const string Api_UserInfo = ApiPrefix + "user/{0}?apikey={1}";
    public const string Api_MyFollowers = ApiPrefix_Shuo + "users/{0}/followers?start={1}&apikey={2}";

    const string UriPrefix_Book_Subject = "http://book.douban.com/subject/";

    string appKey = null;
    WebClient client = null;

    public DoubanApi( string _appKey ) {
      appKey = _appKey;
      client = new WebClient();
      client.Encoding = System.Text.Encoding.UTF8;
      client.UseDefaultCredentials = true;
    }

    #region book

    /// <summary>
    /// 获取某个用户收藏的书籍：bookApi + user/{0}/collections?start={1}
    /// </summary>
    /// <param name="username"></param>
    public async Task<Dictionary<string, JObject>> GetMyBookCollections( string username, int pageIndex ) {
      string uri = string.Format( Api_MyBookCollections, username, pageIndex, appKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return getItems( (JArray)obj["collections"], "book" );
    }

    /// <summary>
    /// 获取某个系列的书籍：bookApi + series/{0}/books?start={1}
    /// </summary>
    /// <param name="username"></param>
    public async Task<Dictionary<string, JObject>> GetBooksOfSerie( string serieId, int pageIndex ) {
      string uri = string.Format( Api_BooksOfSerie, serieId, pageIndex, appKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return getItems( (JArray)obj["books"] );
    }

    /// <summary>
    /// bookApi + id（在豆瓣的ID）
    /// </summary>
    /// <param name="username"></param>
    public async Task<JObject> GetBookById( string id ) {
      string uri = string.Format( Api_BookById, id, appKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return obj;
    }

    /// <summary>
    /// bookApi + isbn（10位或13位，注意某些书可能没有ISBN或者ISBN错误）
    /// </summary>
    /// <param name="username"></param>
    public async Task<JObject> GetBookByIsbn( string isbn ) {
      string uri = string.Format( Api_BookByIsbn, isbn, appKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return obj;
    }

    /// <summary>
    /// 获取book的subject链接.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    public string GetSubjectUriOfBook( JObject book ) {
      if ( book == null )
        return null;
      return (string)book["alt"];
    }

    #endregion book

    public async Task<JObject> GetUserInfo( string username ) {
      string uri = string.Format( Api_UserInfo, username, appKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return obj;
    }

    public async Task<string> GetIdByUsername( string username ) {
      var obj = await GetUserInfo( username );
      return (string)obj["id"];
    }

    public async Task<string> GetUidById( string id ) {
      var obj = await GetUserInfo( id );
      return (string)obj["uid"];
    }

    /// <summary>
    /// 返回（id, uid, json）数组
    /// </summary>
    /// <param name="username"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    public async Task<List<Tuple<string, string, string>>> GetFollowers( string username, int pageIndex ) {
      string uri = string.Format( Api_MyFollowers, username, pageIndex, appKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var array = JArray.Parse( json );
      List<Tuple<string, string, string>> result = new List<Tuple<string, string, string>>();
      foreach ( var obj in array ) {
        var t = Tuple.Create( (string)obj["id"], (string)obj["uid"], obj.ToString() );
        result.Add( t );
      }
      return result;
    }


    /// <summary>
    /// 返回(keyUri, JObject)集合
    /// </summary>
    /// <param name="collections"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    Dictionary<string, JObject> getItems( JArray collections ) {
      Dictionary<string, JObject> result = new Dictionary<string, JObject>();
      foreach ( JObject obj in collections ) {
        // book.alt = "http://book.douban.com/subject/3011518/" = UriPrefix_Book_Subject + book.id
        result.Add( (string)obj["alt"], obj );
      }
      return result;
    }

    /// <summary>
    /// 返回(keyUri, JObject)集合
    /// </summary>
    /// <param name="collections"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    Dictionary<string, JObject> getItems( JArray collections, string propertyName ) {
      Dictionary<string, JObject> result = new Dictionary<string, JObject>();
      foreach ( JObject tmpItem in collections ) {
        var jtoken = tmpItem[propertyName];
        if ( jtoken != null && jtoken.Type == JTokenType.Object ) {
          JObject obj = (JObject)jtoken;
          // book.alt = "http://book.douban.com/subject/3011518/" = UriPrefix_Book_Subject + book.id
          result.Add( (string)obj["alt"], obj );
        }
      }
      return result;
    }

  }

}
