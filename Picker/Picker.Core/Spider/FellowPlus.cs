using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Picker.Core.Storage;

namespace Picker.Core.Spider {
  public class FellowPlus {
    IStorage store = null;
    WebClient client = null;

    int countPerPage = 20;

    // %7B%7D  {}
    public const string Api_ProjectList = "http://dataapi.fellowplus.com/project/list?_id={0}&_token={1}&filter_content=%7B%7D&limit=20&order_by=-complete&start={2}";
    // http://dataapi.fellowplus.com/project?_id=7967&_token=sCtcTUDKT%2B2IKN6r532jADl8jpCTvTouLvMkixXK1a0%3D&id=K_M36KR_PROJECT:4871
    public const string Api_Project = "http://dataapi.fellowplus.com/project?_id={0}&_token={1}&id={2}";
    public const string Api_Website = "http://dataapi.fellowplus.com/project/website?_id={0}&_token={1}&id={2}";
    public const string Api_Weibo = "http://dataapi.fellowplus.com/project/weibo?_id={0}&_token={1}&id={2}";
    public const string Api_Weixin = "http://dataapi.fellowplus.com/project/weixin?_id={0}&_token={1}&id={2}";
    public const string Api_Company = "http://dataapi.fellowplus.com/project/company?_id={0}&_token={1}&id={2}";
    public const string Api_Invest = "http://dataapi.fellowplus.com/project/invest?_id={0}&_token={1}&id={2}";
    public const string Api_News = "http://dataapi.fellowplus.com/project/news?_id={0}&_token={1}&id={2}";

    public FellowPlus( IStorage _store ) {
      store = _store;
      client = Helpers.NetHelper.GetWebClient_UTF8();
    }

    public async Task<string> PickProjectList( string userId, string token, long start = 0 ) {
      string url = String.Format( Api_ProjectList, userId, token, start );
      string jsonList = await client.DownloadStringTaskAsync( url );
      return jsonList;
    }

    public async Task LoopPickProjectList( TimeSpan? interval, string userId, string token, long start = 0 ) {
      long skip = start >= 0 ? start : 0;
      while ( true ) {
        try {
          string list = await PickProjectList( userId, token, skip );
          // process
          var obj = JObject.Parse( list );
          int count = await store.FellowPlus_SaveProjectPreviewList( obj, true, true );
          if ( count < countPerPage )
            break;
          // continue
          if ( interval != null )
            await Task.Delay( interval.Value );
          skip += countPerPage;
        }
        catch(Exception ex ) {
          throw ex;
        }
      }
    }

    public async Task<int> PickProject( string userId, string token, string projectId )      {
      try {
        string urlProject = String.Format( Api_Project, userId, token, projectId );
        string jsonProject = await client.DownloadStringTaskAsync( urlProject );
        JToken project = JToken.Parse( jsonProject );

        await Task.Delay( 1000 );
        string urlWebsite = String.Format( Api_Website, userId, token, projectId );
        string jsonWebsite = await client.DownloadStringTaskAsync( urlWebsite );
        JToken website = JToken.Parse( jsonWebsite );

        await Task.Delay( 1000 );
        string urlCompany = String.Format( Api_Company, userId, token, projectId );
        string jsonCompany = await client.DownloadStringTaskAsync( urlCompany );
        JToken company = JToken.Parse( jsonCompany );

        await Task.Delay( 1000 );
        string urlInvest = String.Format( Api_Invest, userId, token, projectId );
        string jsonInvestt = await client.DownloadStringTaskAsync( urlInvest );
        JToken invest = JToken.Parse( jsonInvestt );

        await Task.Delay( 1000 );
        string urlNews = String.Format( Api_News, userId, token, projectId );
        string jsonNews = await client.DownloadStringTaskAsync( urlNews );
        JToken news = JToken.Parse( jsonNews );

        await Task.Delay( 1000 );
        string urlWeibo = String.Format( Api_Weibo, userId, token, projectId );
        string jsonWeibo = await client.DownloadStringTaskAsync( urlWeibo );
        JToken weibo = JToken.Parse( jsonWeibo );

        await Task.Delay( 1000 );
        string urlWeixin = String.Format( Api_Weixin, userId, token, projectId );
        string jsonWeixin = await client.DownloadStringTaskAsync( urlWeixin );
        JToken weixin = JToken.Parse( jsonWeixin );

        // save
        await store.FellowPlus_SaveProjectInfo( projectId, project, company, invest, website, weibo, weixin, news, true, false );
        await store.FellowPlus_UpdateTag_ProjectPreview( projectId, false );
        int r = await store.FellowPlus_SaveChanges();
        return r;
      }
      catch(Exception ex) {
        Console.WriteLine( "Project ID: " + projectId );
        throw ex;
      }
    }

    public async Task<int> PickProject( string userId, string token, int skipRandomItems ) {
      string projectId = store.FellowPlus_SelectId_ProjectPreview_NotProcessed( skipRandomItems );
      if ( string.IsNullOrWhiteSpace( projectId ) )
        return 0;
      return await PickProject( userId, token, projectId );
    }

  }

}
