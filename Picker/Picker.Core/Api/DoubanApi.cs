using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
    public const string ApiPrefix_Music = ApiPrefix + "music/";
    public const string ApiPrefix_Travel = ApiPrefix + "travel/";
    public const string UriPrefix_Book_Subject = "http://book.douban.com/subject/";
    public const string UriPrefix_Movie_Subject = "http://movie.douban.com/subject/";

    // === 用户 ====
    public const string Api_UserInfo = ApiPrefix + "user/{0}?apikey={1}";
    /// <summary>
    /// 我关注的人
    /// </summary>
    public const string Api_MyFollowing = ApiPrefix_Shuo + "users/{0}/following?start={1}&apikey={2}";

    // === 书籍 ====
    public const string Api_MyBookCollections = ApiPrefix_Book + "user/{0}/collections?start={1}&apikey={2}";
    public const string Api_BooksOfSerie = ApiPrefix_Book + "series/{0}/books?start={1}&apikey={1}";
    public const string Api_BookById = ApiPrefix_Book + "{0}?apikey={1}";
    public const string Api_BookByIsbn = ApiPrefix_Book + "isbn/{0}?apikey={1}";

    // === 电影 ====
    /// <summary>
    /// 正在热映，默认city=北京，count=50（无法更改）。已测试，无法使用
    /// </summary>
    public const string Api_MovieNowPlaying = ApiPrefix_Movie + "nowplaying?start={0}&apikey={1}";
    /// <summary>
    /// Top250
    /// </summary>
    public const string Api_MovieTop250 = ApiPrefix_Movie + "top250?start={0}&apikey={1}";
    /// <summary>
    /// 即将上映，默认每次查询20条。已测试，无法使用
    /// </summary>
    public const string Api_MovieComming = ApiPrefix_Movie + "coming?start={0}&apikey={1}";
    public const string Api_MovieById = ApiPrefix_Movie + "subject/{0}?apikey={1}";
    /// <summary>
    /// 影人/电影名人
    /// </summary>
    public const string Api_MovieCelebrityById = ApiPrefix_Movie + "celebrity/{0}?apikey={1}";

    // === 音乐 ====
    public const string Api_MusicById = ApiPrefix_Music + "{0}?apikey={1}";

    // === 旅行/去过 ====
    public const string Api_MyTravelCollections = ApiPrefix_Travel + "user/{0}/collections?start={1}&apikey={2}";
    public const string Api_TravelPlaceById = ApiPrefix_Travel + "place/{0}?apikey={1}";


    public string AppKey { get; set; }
    WebClient client = null;

    public DoubanApi( string _appKey ) {
      AppKey = _appKey == null ? "" : _appKey;
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
      int start = pageIndex * CountPerPage;
      string uri = string.Format( Api_MyBookCollections, username, start, AppKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return getItems( (JArray)obj["collections"], "book", "alt" );
    }

    /// <summary>
    /// 获取某个系列的书籍：bookApi + series/{0}/books?start={1}
    /// </summary>
    /// <param name="username"></param>
    public async Task<Dictionary<string, JObject>> GetBooksOfSerie( string serieId, int pageIndex ) {
      int start = pageIndex * CountPerPage;
      string uri = string.Format( Api_BooksOfSerie, serieId, start, AppKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return getItems( (JArray)obj["books"], "alt" );
    }

    /// <summary>
    /// bookApi + id（在豆瓣的ID）
    /// </summary>
    /// <param name="username"></param>
    public async Task<JObject> GetBookById( string id ) {
      string uri = string.Format( Api_BookById, id, AppKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return obj;
    }

    /// <summary>
    /// bookApi + isbn（10位或13位，注意某些书可能没有ISBN或者ISBN错误）
    /// </summary>
    /// <param name="username"></param>
    public async Task<JObject> GetBookByIsbn( string isbn ) {
      string uri = string.Format( Api_BookByIsbn, isbn, AppKey );
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

    /// <summary>
    /// 通过网址获取ID
    /// </summary>
    /// <param name="subjectUrl"></param>
    /// <returns></returns>
    public string GetBookId( string subjectUrl ) {
      if ( string.IsNullOrWhiteSpace( subjectUrl ) || !subjectUrl.StartsWith( DoubanApi.UriPrefix_Book_Subject ) )
        return null;
      return subjectUrl.Replace( DoubanApi.UriPrefix_Book_Subject, "" )
        .Replace( "/", "" ); // 网址最后可能是/符号
    }

    #endregion book


    #region Travel

    /// <summary>
    /// 获取某个用户收藏的地方：travelApi + user/{0}/collections?start={1}
    /// </summary>
    /// <param name="username"></param>
    public async Task<Dictionary<string, JObject>> GetMyTravelCollections( string username, int pageIndex ) {
      int start = pageIndex * CountPerPage;
      string uri = string.Format( Api_MyTravelCollections, username, start, AppKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return getItems( (JArray)obj["places"], "place", "url" );
    }

    /// <summary>
    /// travelApi + id（在豆瓣的ID）
    /// </summary>
    /// <param name="username"></param>
    public async Task<JObject> GetTravelPlaceById( string id ) {
      string uri = string.Format( Api_TravelPlaceById, id, AppKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return obj;
    }

    #endregion Travel


    #region Movies

    public async Task<Dictionary<string, JObject>> GetMovies_Top250( int start ) {
      string uri = string.Format( Api_MovieTop250, start, AppKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return getItems( (JArray)obj["subjects"], "alt" );
    }

    /// <summary>
    /// movieApi + id（在豆瓣的ID）
    /// </summary>
    /// <param name="username"></param>
    public async Task<JObject> GetMovieById( string id ) {
      string uri = string.Format( Api_MovieById, id, AppKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return obj;
    }

    /// <summary>
    /// 通过网址获取ID
    /// </summary>
    /// <param name="subjectUrl"></param>
    /// <returns></returns>
    public string GetMovieId( string subjectUrl ) {
      if ( string.IsNullOrWhiteSpace( subjectUrl ) || !subjectUrl.StartsWith( DoubanApi.UriPrefix_Movie_Subject ) )
        return null;
      return subjectUrl.Replace( DoubanApi.UriPrefix_Movie_Subject, "" )
        .Replace( "/", "" ); // 网址最后可能是/符号
    }

    #endregion Movies


    public async Task<JObject> GetUserInfo( string username ) {
      string uri = string.Format( Api_UserInfo, username, AppKey );
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
    public async Task<List<Tuple<string, string, JObject>>> GetFollowings( string username, int pageIndex ) {
      int start = pageIndex * CountPerPage;
      string uri = string.Format( Api_MyFollowing, username, start, AppKey );
      string json = await client.DownloadStringTaskAsync( uri );
      var array = JArray.Parse( json );
      List<Tuple<string, string, JObject>> result = new List<Tuple<string, string, JObject>>();
      foreach ( var obj in array ) {
        var t = Tuple.Create( (string)obj["id"], (string)obj["uid"], (JObject)obj );
        result.Add( t );
      }
      return result;
    }


    /// <summary>
    /// 获取一个页面中的所有条目。目前只支持Book和Movie。
    /// </summary>
    /// <param name="pageUrl"></param>
    /// <param name="start"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, JObject>> GetItemsOfPage( string pageUrl, int start ) {
      Dictionary<string, JObject> result = new Dictionary<string, JObject>();
      string uri = string.Format( pageUrl + "?start={0}&apikey={1}", start, AppKey );
      string html = await client.DownloadStringTaskAsync( uri );
      var links = getSubjectsLinks( html );
      foreach ( string link in links ) {
        JObject data = null;
        if ( link.StartsWith( DoubanApi.UriPrefix_Book_Subject ) ) {
          string id = id = GetBookId( link );
          if ( string.IsNullOrWhiteSpace( id ) )
            continue;
          data = await GetBookById( id );
        }
        else if ( link.StartsWith( DoubanApi.UriPrefix_Movie_Subject ) ) {
          string id = GetMovieId( link );
          if ( string.IsNullOrWhiteSpace( id ) )
            continue;
          data = await GetMovieById( id );
        }
        if ( data != null )
          result.Add( link, data );
      }
      return result;
    }


    /// <summary>
    /// data["count"]
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int GetCount( JObject data ) {
      return (int)data["count"];
    }

    /// <summary>
    /// data["total"]
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int GetTotal( JObject data ) {
      return (int)data["total"];
    }

    /// <summary>
    /// data["start"]
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int GetStart( JObject data ) {
      return (int)data["start"];
    }



    /// <summary>
    /// 返回(keyUri, JObject)集合
    /// </summary>
    /// <param name="collections"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    Dictionary<string, JObject> getItems( JArray collections, string keyPropertyName ) {
      Dictionary<string, JObject> result = new Dictionary<string, JObject>();
      foreach ( JObject obj in collections ) {
        // book.alt = "http://book.douban.com/subject/3011518/" = UriPrefix_Book_Subject + book.id
        result.Add( (string)obj[keyPropertyName], obj );
      }
      return result;
    }

    /// <summary>
    /// 返回(keyUri, JObject)集合
    /// </summary>
    /// <param name="collections"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    Dictionary<string, JObject> getItems( JArray collections, string itemPropertyName, string keyPropertyName ) {
      Dictionary<string, JObject> result = new Dictionary<string, JObject>();
      foreach ( JObject tmpItem in collections ) {
        var jtoken = tmpItem[itemPropertyName];
        if ( jtoken != null && jtoken.Type == JTokenType.Object ) {
          JObject obj = (JObject)jtoken;
          // book.alt = "http://book.douban.com/subject/3011518/" = UriPrefix_Book_Subject + book.id
          result.Add( (string)obj[keyPropertyName], obj );
        }
      }
      return result;
    }

    /// <summary>
    /// 解析出HTML中的所有条目链接
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    List<string> getSubjectsLinks( string html ) {
      List<string> result = new List<string>();
      Regex regex = new Regex( @"http://(book|movie)\.douban\.com/subject/\d+/", RegexOptions.IgnoreCase );
      var matches = regex.Matches( html );
      foreach ( Match m in matches ) {
        result.Add( m.Value );
      }
      return result;
    }

  }

}
