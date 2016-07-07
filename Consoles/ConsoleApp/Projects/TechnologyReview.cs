using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Picker.Core.Helpers;
using Picker.Core.Extensions;

namespace ConsoleApp {
  /// <summary>
  /// www.technologyreview.com
  /// </summary>
  public static class TechnologyReview {
    const string UriPrefix = "https://www.technologyreview.com";

    const string Url2016 = "https://www.technologyreview.com/lists/companies/2016/intro/";
    const string Url2015 = "https://www.technologyreview.com/lists/companies/2015/intro/";
    const string Url2014 = "https://www.technologyreview.com/lists/disruptive-companies/2014/";
    const string Url2013 = "https://www.technologyreview.com/lists/disruptive-companies/2013/";
    const string Url2012 = "https://www.technologyreview.com/lists/disruptive-companies/2012/";
    const string Url2011 = "https://www.technologyreview.com/lists/disruptive-companies/2011/";
    const string Url2010 = "https://www.technologyreview.com/lists/disruptive-companies/2010/";

    static void save( JObject jo, string path ) {
      string json = jo.ToString( Newtonsoft.Json.Formatting.Indented );
      File.WriteAllText( path, json );
    }

    public static async Task Pick2016( string path ) {
      JObject root = new JObject();
      root["title"] = "50 Smartest Companies 2016";
      root["description"] = "Technology Review editors pick the 50 companies that best combine innovative technology with an effective business model.";
      root["uri"] = Url2016;
      root["year"] = 2016;
      root["date"] = DateTime.Parse( "2016-6-21" ).Date;

      JObject joConfig = createConfig();
      root["config"] = joConfig;
      Console.WriteLine( "ready." );

      WebClient client = NetHelper.GetWebClient_UTF8();
      // get part 1
      string html = await client.DownloadStringTaskAsync( Url2016 );
      Console.WriteLine( "got html." );
      // process part 1
      var list = process( html, processCompany );
      Console.WriteLine( "converted." );
      // get part 2
      html = await client.DownloadStringTaskAsync( "https://www.technologyreview.com/lists/companies/2016/" );
      Console.WriteLine( "got html: part 2." );
      // update list by part 2
      processAbstract( html, list );
      Console.WriteLine( "list updated using part 2 data." );

      JArray items = new JArray();
      list.ForEach( i => items.Add( i ) );
      root["items"] = items;

      // save
      save( root, path );
      Console.WriteLine( "saved...over..." );
    }

    public static async Task Pick2015( string path ) {
      JObject root = new JObject();
      root["title"] = "50 Smartest Companies 2015";
      root["description"] = "Massive solar panel factories. Fertility treatments. Friendly robots. Meet the companies reshaping the technology business.";
      root["uri"] = Url2015;
      root["year"] = 2015;
      root["date"] = DateTime.Parse( "2015-6-23" ).Date;

      JObject joConfig = createConfig();
      root["config"] = joConfig;
      Console.WriteLine( "ready." );

      WebClient client = NetHelper.GetWebClient_UTF8();
      // get part 1
      string html = await client.DownloadStringTaskAsync( Url2015 );
      Console.WriteLine( "got html: part 1." );
      // process part 1
      var list = process( html, processCompany );
      Console.WriteLine( "part 1 converted." );
      // get part 2
      html = await client.DownloadStringTaskAsync( "https://www.technologyreview.com/lists/companies/2015/" );
      Console.WriteLine( "got html: part 2." );
      // update list by part 2
      processAbstract( html, list );
      Console.WriteLine( "list updated using part 2 data." );

      JArray items = new JArray();
      list.ForEach( i => items.Add( i ) );
      root["items"] = items;

      // save
      save( root, path );
      Console.WriteLine( "saved...over..." );
    }

    public static async Task Pick2014( string path ) {
      JObject root = new JObject();
      root["title"] = "50 Smartest Companies 2014";
      root["description"] = "50 Smartest Companies 2014";
      root["uri"] = Url2014;
      root["year"] = 2014;
      //root["date"] = DateTime.Parse( "" ).Date;

      JObject joConfig = createConfig();
      root["config"] = joConfig;
      Console.WriteLine( "ready." );

      WebClient client = NetHelper.GetWebClient_UTF8();
      // get
      string html = await client.DownloadStringTaskAsync( Url2014 );
      Console.WriteLine( "got html." );
      // process
      var list = processBefore2015( html, null, processCompanyBefore2015 );
      Console.WriteLine( "converted." );

      JArray items = new JArray();
      list.ForEach( i => items.Add( i ) );
      root["items"] = items;

      // save
      save( root, path );
      Console.WriteLine( "saved...over..." );
    }

    public static async Task Pick2013( string path ) {
      JObject root = new JObject();
      root["title"] = "50 Disruptive Companies 2013";
      root["description"] = "50 Disruptive Companies 2013";
      root["uri"] = Url2013;
      root["year"] = 2013;
      //root["date"] = DateTime.Parse( "" ).Date;

      JObject joConfig = createConfig();
      root["config"] = joConfig;
      Console.WriteLine( "ready." );

      WebClient client = NetHelper.GetWebClient_UTF8();
      // get
      string html = await client.DownloadStringTaskAsync( Url2013 );
      Console.WriteLine( "got html." );
      // process
      var list = processBefore2015_NoRank( html, null, processCompanyBefore2015_NoRank );
      Console.WriteLine( "converted." );

      JArray items = new JArray();
      list.ForEach( i => items.Add( i ) );
      root["items"] = items;

      // save
      save( root, path );
      Console.WriteLine( "saved...over..." );
    }

    public static async Task Pick2012( string path ) {
      JObject root = new JObject();
      root["title"] = "53 Disruptive Companies 2012";
      root["description"] = "What is a disruptive company? It is a business whose innovations force other businesses to alter their strategic course. The list is compiled by MIT Technology Review’s editors, who look for companies that over the previous year have demonstrated original and valuable technology, are bringing that technology to market at a significant scale, and are clearly influencing their competitors.";
      root["uri"] = Url2012;
      root["year"] = 2012;
      //root["date"] = DateTime.Parse( "" ).Date;

      JObject joConfig = createConfig();
      root["config"] = joConfig;
      Console.WriteLine( "ready." );

      WebClient client = NetHelper.GetWebClient_UTF8();
      // get
      string html = await client.DownloadStringTaskAsync( Url2012 );
      Console.WriteLine( "got html." );
      // process
      var list = processBefore2015_NoRank( html, "tr50-info", processCompany2012 );
      Console.WriteLine( "converted." );

      JArray items = new JArray();
      list.ForEach( i => items.Add( i ) );
      root["items"] = items;

      // save
      save( root, path );
      Console.WriteLine( "saved...over..." );
    }

    public static async Task Pick2011( string path ) {
      JObject root = new JObject();
      root["title"] = "The 53 Most Innovative Companies 2011";
      root["description"] = "The 53 Most Innovative Companies 2011";
      root["uri"] = Url2011;
      root["year"] = 2011;
      //root["date"] = DateTime.Parse( "" ).Date;

      JObject joConfig = createConfig();
      root["config"] = joConfig;
      Console.WriteLine( "ready." );

      WebClient client = NetHelper.GetWebClient_UTF8();
      // get
      string html = await client.DownloadStringTaskAsync( Url2011 );
      Console.WriteLine( "got html." );

      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );
      var companies = doc.DocumentNode
        .SelectNodes( "//div[@class='tr50-entry']" );

      // process
      JArray items = new JArray();
      foreach ( var company in companies ) {
        var tmp = processCompany2011( company );
        if ( tmp != null )
          items.Add( tmp );
      }
      root["items"] = items;

      // save
      save( root, path );
      Console.WriteLine( "saved...over..." );
    }

    static List<JObject> process( string html, Func<HtmlNode, JObject> funcProcessCompany ) {
      List<JObject> result = new List<JObject>();

      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );
      var companies = doc.DocumentNode
        .SelectNodes( "//div[@class='company']" );
      foreach ( var company in companies ) {
        var tmp = funcProcessCompany( company );
        if ( tmp != null )
          result.Add( tmp );
      }

      return result;
    }

    /// <summary>
    /// 2010--2014
    /// </summary>
    /// <param name="html"></param>
    /// <param name="funcProcessCompany"></param>
    /// <returns></returns>
    static List<JObject> processBefore2015( string html, string classNameOfCompany, Func<HtmlNode, int, JObject> funcProcessCompany ) {
      List<JObject> result = new List<JObject>();

      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );
      // <div id="tr50-content"
      var container = doc.DocumentNode
        .SelectSingleNode( "//div[@id='tr50-content']" );
      var companies = string.IsNullOrWhiteSpace( classNameOfCompany) ? container.SelectNodes( "./div" ) : container.SelectNodes( "./div[@class='" + classNameOfCompany + "']" );

      int order = 0;
      foreach ( var company in companies ) {
        order++;
        var tmp = funcProcessCompany( company, order );
        if ( tmp != null )
          result.Add( tmp );
      }

      return result;
    }

    /// <summary>
    /// 2010--2013
    /// </summary>
    /// <param name="html"></param>
    /// <param name="funcProcessCompany"></param>
    /// <returns></returns>
    static List<JObject> processBefore2015_NoRank( string html, string classNameOfCompany, Func<HtmlNode, JObject> funcProcessCompany ) {
      List<JObject> result = new List<JObject>();

      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );
      // <div id="tr50-content"
      var container = doc.DocumentNode
        .SelectSingleNode( "//div[@id='tr50-content']" );
      var companies = string.IsNullOrWhiteSpace( classNameOfCompany ) ? container.SelectNodes( "./div" ) : container.SelectNodes( "./div[@class='" + classNameOfCompany + "']" );

      foreach ( var company in companies ) {
        var tmp = funcProcessCompany( company );
        if ( tmp != null )
          result.Add( tmp );
      }

      return result;
    }

    static void processAbstract( string html, List<JObject> items ) {
      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml( html );


      var companies = doc.DocumentNode
        .SelectNodes( "//li[@class='company']" );

      foreach ( var company in companies ) {
        string id = company.SelectSingleNode( "./span" )
          .Attributes["id"].Value;
        JObject jo = items.Where( i => i["id"].Value<string>() == id )
          .FirstOrDefault();
        if ( jo == null )
          continue;

        JObject joReason = new JObject();
        var datapart = company.SelectSingleNode( "./div[@class='company-data']" );
        var summary = datapart.SelectSingleNode( "./p[@class='company-data__text']" );
        joReason["summary"] = summary.InnerText
          .Trim( '\t', '\r', '\n' )
          .Trim();

        var headerNext = summary.NextSibling( "h3", "class", "company-data__header" );
        if ( headerNext != null ) {
          joReason["data"] = headerNext.InnerText;

          var context = headerNext.NextSibling( "p", "class", "company-data__text" );
          if ( context != null ) {
            joReason["data_context"] = context.InnerText;
          }
        }

        jo["reason"] = joReason;
      }
    }

    static JObject processCompany( HtmlNode hn ) {
      if ( hn == null )
        return null;

      JObject jo = new JObject();
      // id
      jo["id"] = hn.Attributes["id"].Value;
      // rank
      var rank = hn.SelectSingleNode( "./p[@class='company__rank']" );
      if ( rank != null )
        jo["rank"] = int.Parse( rank.InnerText );
      // title
      var title = hn.SelectSingleNode( "./h1[@class='company__title']" );
      if ( title != null )
        jo["title"] = title.InnerText;
      // stats
      var stats = hn.SelectSingleNode( "./ul[@class='company__stats']" )
        .SelectNodes( "./li" );
      int index = 1;
      foreach ( var li in stats ) {
        if(index == 1 ) {
          jo["headquarters"] = li.InnerHtml
            .Replace( "<strong>Headquarters</strong>", "" )
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( index == 2 ) {
          jo["industry"] = li.InnerHtml
            .Replace( "<strong>Industry</strong>", "" )
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( index == 3 ) {
          jo["status"] = li.InnerHtml
            .Replace( "<strong>Status</strong>", "" )
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        else if ( index == 4 ) {
          jo["valuation"] = li.InnerHtml
            .Replace( "<strong>Valuation</strong>", "" )
            .Trim( '\r', '\n', '\t' )
            .Trim();
        }
        index++;
      }
      // summary, state
      var company_body = hn.SelectSingleNode( "./div[@class='company__body']" );
      if ( company_body != null ) {
        var summary = company_body.SelectSingleNode( "./p[@class='company__body__item company__body__summary']" );
        if ( summary != null )
          jo["summary"] = summary.InnerHtml;

        var stat = company_body.SelectSingleNode( "./p[@class='company__body__item company__body__stat']" );
        if ( stat != null )
          jo["state"] = stat.InnerHtml;
      }
      // related stories
      var ulRelated = hn.SelectSingleNode( "./ul[@class='company__related']" );
      if(ulRelated != null ) {
        var lilist = ulRelated.SelectNodes( "./li" );
        JArray items = new JArray();
        foreach ( var li in lilist ) {
          var a = li.SelectSingleNode( "./a" );
          if ( a == null )
            continue;

          JObject joRelated = new JObject();
          joRelated["article_title"] = a.InnerText;
          joRelated["url"] = UriPrefix + a.Attributes["href"].Value;
          items.Add( joRelated );
        }
        jo["related_stories"] = items;
      }

      return jo;
    }

    /// <summary>
    /// 2013, 2014
    /// </summary>
    /// <param name="hn"></param>
    /// <param name="rank"></param>
    /// <returns></returns>
    static JObject processCompanyBefore2015( HtmlNode hn, int rank ) {
      if ( hn == null )
        return null;

      JObject jo = new JObject();
      // id
      jo["id"] = hn.Attributes["id"].Value;
      // rank
      jo["rank"] = hn.Attributes["data-rank"] == null ? rank : int.Parse( hn.Attributes["data-rank"].Value );
      // title
      jo["title"] = hn.Attributes["data-name"].Value;
      // industry/channel
      jo["industry"] = hn.Attributes["data-channel"].Value;
      // headquarters
      jo["headquarters"] = hn.Attributes["data-headquarters"].Value;
      // publicity/status
      jo["status"] = hn.Attributes["data-publicity"].Value;
      // valuation都是空字符串
      jo["valuation"] = string.IsNullOrWhiteSpace( hn.Attributes["data-valuation"].Value) ? null : hn.Attributes["data-valuation"].Value;
      // founded都是空字符串
      jo["founded"] = string.IsNullOrWhiteSpace( hn.Attributes["data-founded"].Value) ? null : hn.Attributes["data-founded"].Value;
      // employees都是空字符串
      jo["employees"] = string.IsNullOrWhiteSpace( hn.Attributes["data-employees"].Value) ? null : hn.Attributes["data-employees"].Value;

      // company info
      var company_info = hn.SelectSingleNode( "./div[@class='company-info']" );
      // summary
      jo["summary"] = company_info.SelectSingleNode( "./div[@class='summary']" )
        .SelectSingleNode( "./p" )
        .InnerText;
      // data_point
      var joDataPoint = company_info.SelectSingleNode( "./div[@class='data-point']" );
      if(joDataPoint != null ) {
        jo["data_point"] = company_info.SelectSingleNode( "./div[@class='data-point']" )
        .InnerHtml
        .Replace( "<b>", "" )
        .Replace( "</b><br />", " " )
        .Replace( "</b><br/>", " " )
        .Replace( "</b><br>", " " )
        .Replace( "\r\n", "" );
      }
      
      // related stories
      var ulRelated = company_info.SelectSingleNode( "./ul[@class='related-articles']" );
      if ( ulRelated != null ) {
        var lilist = ulRelated.SelectNodes( "./li[@class='story']" );
        JArray items = new JArray();
        foreach ( var li in lilist ) {
          var a = li.SelectSingleNode( "./a" );
          if ( a == null )
            continue;

          JObject joRelated = new JObject();
          joRelated["article_title"] = a.InnerText;
          joRelated["url"] = a.Attributes["href"].Value;
          items.Add( joRelated );
        }
        jo["related_stories"] = items;
      }

      return jo;
    }

    /// <summary>
    /// 2013, 2014
    /// </summary>
    /// <param name="hn"></param>
    /// <returns></returns>
    static JObject processCompanyBefore2015_NoRank( HtmlNode hn ) {
      if ( hn == null )
        return null;

      JObject jo = new JObject();
      // id
      jo["id"] = hn.Attributes["id"].Value;
      // title
      jo["title"] = hn.Attributes["data-name"].Value;
      // industry/channel
      jo["industry"] = hn.Attributes["data-channel"].Value;
      // headquarters
      jo["headquarters"] = hn.Attributes["data-headquarters"].Value;
      // publicity/status
      jo["status"] = hn.Attributes["data-publicity"].Value;
      // valuation都是空字符串
      jo["valuation"] = string.IsNullOrWhiteSpace( hn.Attributes["data-valuation"].Value ) ? null : hn.Attributes["data-valuation"].Value;
      // founded都是空字符串
      jo["founded"] = string.IsNullOrWhiteSpace( hn.Attributes["data-founded"].Value ) ? null : hn.Attributes["data-founded"].Value;
      // employees都是空字符串
      jo["employees"] = string.IsNullOrWhiteSpace( hn.Attributes["data-employees"].Value ) ? null : hn.Attributes["data-employees"].Value;

      // company info
      var company_info = hn.SelectSingleNode( "./div[@class='company-info']" );
      // summary
      jo["summary"] = company_info.SelectSingleNode( "./div[@class='summary']" )
        .SelectSingleNode( "./p" )
        .InnerText;
      // data_point
      var joDataPoint = company_info.SelectSingleNode( "./div[@class='data-point']" );
      if ( joDataPoint != null ) {
        jo["data_point"] = company_info.SelectSingleNode( "./div[@class='data-point']" )
        .InnerHtml
        .Replace( "<b>", "" )
        .Replace( "</b><br />", " " )
        .Replace( "</b><br/>", " " )
        .Replace( "</b><br>", " " )
        .Replace( "\r\n", "" );
      }

      // related stories
      var ulRelated = company_info.SelectSingleNode( "./ul[@class='related-articles']" );
      if ( ulRelated != null ) {
        var lilist = ulRelated.SelectNodes( "./li[@class='story']" );
        JArray items = new JArray();
        foreach ( var li in lilist ) {
          var a = li.SelectSingleNode( "./a" );
          if ( a == null )
            continue;

          JObject joRelated = new JObject();
          joRelated["article_title"] = a.InnerText;
          joRelated["url"] = a.Attributes["href"].Value;
          items.Add( joRelated );
        }
        jo["related_stories"] = items;
      }

      return jo;
    }

    static JObject processCompany2012( HtmlNode hn ) {
      if ( hn == null )
        return null;

      JObject jo = new JObject();
      // id
      jo["id"] = hn.Attributes["id"].Value.Replace( "-info", "" );

      var divlist = hn.SelectNodes( "./div" );
      foreach(var div in divlist ) {
        string className = div.Attributes["class"].Value;
        if(className == "name" ) {
          jo["title"] = div.InnerText;
        }
        else if ( className == "channel" ) {
          jo["industry"] = div.InnerText;
        }
        else if ( className == "publicity" ) {
          jo["status"] = div.InnerText;
        }
        else if ( className == "summary" ) {
          jo["summary"] = div.InnerHtml
            .Trim( '\t', '\r', '\n' )
            .Trim();
        }
        else if ( className == "article" ) {
          var a = div.SelectSingleNode( "./div" ) // company
            .SelectSingleNode( "./div" ) // maincontent
            .SelectSingleNode( "./div" ) // related
            .SelectSingleNode( "./div" ) // story
            .SelectSingleNode( "./h5" )
            .SelectSingleNode( "./a" );
          JArray items = new JArray();
          JObject joRelated = new JObject();
          joRelated["article_title"] = a.InnerText;
          joRelated["url"] = a.Attributes["href"].Value;
          items.Add( joRelated );
        }
      }
      return jo;
    }

    static JObject processCompany2011( HtmlNode hn ) {
      if ( hn == null )
        return null;

      JObject jo = new JObject();

      var a = hn.SelectSingleNode( "./h5" )
        .SelectSingleNode( "./a" );

      // id
      jo["id"] = a.Attributes["href"].Value
        .Replace( "/tr50/", "" )
        .Replace( "/", "" );

      jo["title"] = a.InnerText
        .Trim();

      var p = hn.SelectSingleNode( "./p[@class='summary']" );
      if ( p != null ) {
        jo["summary"] = p.InnerText
          .Trim( '\t', '\r', '\n' )
          .Trim();
      }

      return jo;
    }

    static JObject createConfig() {
      JObject joConfig = new JObject();
      joConfig.Add( "id", "company id" );
      joConfig.Add( "rank", "company rank" );
      joConfig.Add( "title", "company title" );

      joConfig.Add( "reason", "why/reason" );
      joConfig.Add( "data", "some data" );
      joConfig.Add( "data_context", "context of the data" );
      joConfig.Add( "data_point", "data point" );

      joConfig.Add( "headquarters", "headquarters" );
      joConfig.Add( "industry", "industry" );
      joConfig.Add( "status", "status" );
      joConfig.Add( "valuation", "valuation" );

      joConfig.Add( "summary", "summary" );
      joConfig.Add( "state", "state" );
      joConfig.Add( "related_stories", "related stories" );
      return joConfig;
    }

  }

}
