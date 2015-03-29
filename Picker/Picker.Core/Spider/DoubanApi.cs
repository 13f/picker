using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Picker.Core.Spider {
  public class DoubanApi {
    const int CountPerPage = 20;
    const string ApiPrefix = "https://api.douban.com/v2/";
    const string ApiPrefix_Book = ApiPrefix + "book/";
    const string ApiPrefix_Movie = ApiPrefix + "movie/";
    const string ApiPrefix_music = ApiPrefix + "music/";

    const string Api_MyBookCollections = ApiPrefix_Book + "user/{0}/collections?start={1}";
    const string Api_BookById = ApiPrefix_Book + "{0}";
    const string Api_BookByIsbn = ApiPrefix_Book + "isbn/{0}";

    const string UriPrefix_Book_Subject = "http://book.douban.com/subject/";

    string key = null;
    WebClient client = null;

    public DoubanApi() {
      client = new WebClient();
      client.Encoding = System.Text.Encoding.UTF8;
      client.UseDefaultCredentials = true;
    }

    /// <summary>
    /// bookApi + user/{0}/collections?start={1}
    /// </summary>
    /// <param name="username"></param>
    public async Task<Dictionary<string, JObject>> GetMyBookCollections( string username, int pageIndex ) {
      string uri = string.Format( Api_MyBookCollections, username, pageIndex );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return getItems( (JArray)obj["collections"], "book" );
    }

    /// <summary>
    /// bookApi + id（在豆瓣的ID）
    /// </summary>
    /// <param name="username"></param>
    public async Task<JObject> GetBookById( string id ) {
      string uri = string.Format( Api_BookById, id );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return obj;
    }

    /// <summary>
    /// bookApi + isbn（10位或13位，注意某些书可能没有ISBN或者ISBN错误）
    /// </summary>
    /// <param name="username"></param>
    public async Task<JObject> GetBookByIsbn( string isbn ) {
      string uri = string.Format( Api_BookByIsbn, isbn );
      string json = await client.DownloadStringTaskAsync( uri );
      var obj = JObject.Parse( json );
      return obj;
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
