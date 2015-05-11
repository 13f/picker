using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Picker.Core.Storage {
  public class Configuration {
    public const string Key_Douban_User = "douban_user";
    public const string Key_Douban_Book = "douban_book";
    public const string Key_Douban_Movie = "douban_movie";
    public const string Key_Douban_Music = "douban_music";
    public const string Key_Douban_Travel = "douban_travel";
    public const string Key_Douban_Page = "douban_page";

    public const string Key_LastApi = "last_api";
    public const string Key_LastPageIndex = "last_page_index";
    public const string Key_LastUserID = "last_user_id";
    public const string Key_CountPerPage = "count_per_page";
    
    /// <summary>
    /// 系统配置文件的文件名
    /// </summary>
    const string configFileName = "configration.xml";

    /// <summary>
    /// 系统可执行文件所在文件夹
    /// </summary>
    public static string AppExcuteFilePath = string.Empty;


    XElement xeRoot = null;

    public Configuration( string appExcuteFilePath ) {
      AppExcuteFilePath = appExcuteFilePath;
      string configFile = System.IO.Path.Combine( AppExcuteFilePath, configFileName );
      if ( File.Exists( configFile ) ) {
        xeRoot = XElement.Load( configFile );
      }
      else {
        xeRoot = new XElement( "configration",
          new XElement( "group", new XAttribute( "name", Key_Douban_User ) ),
          new XElement( "group", new XAttribute( "name", Key_Douban_Book ) ),
          new XElement( "group", new XAttribute( "name", Key_Douban_Movie ) ),
          new XElement( "group", new XAttribute( "name", Key_Douban_Music ) ),
          new XElement( "group", new XAttribute( "name", Key_Douban_Travel ) ),
          new XElement( "group", new XAttribute( "name", Key_Douban_Page ) )
          );
        Save(); // save default
      }
    }

    /// <summary>
    /// 保存
    /// </summary>
    public void Save() {
      string configFile = System.IO.Path.Combine( AppExcuteFilePath, configFileName );
      xeRoot.Save( configFile, SaveOptions.None );
    }

    /// <summary>
    /// 更新某个几点的数据并保存
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="lastApi"></param>
    /// <param name="lastPageIndex"></param>
    public void Save( string groupName, string lastApi, int lastPageIndex ) {
      XElement xe = xeRoot.Elements( "group" )
        .Where( i => i.Attribute( "name" ) != null && i.Attribute( "name" ).Value == groupName )
        .FirstOrDefault();
      if ( xe == null )
        return;
      xe.SetElementValue( Key_LastApi, lastApi );
      xe.SetElementValue( Key_LastPageIndex, lastPageIndex );
      Save();
    }


    /// <summary>
    /// 更新UserId并保存
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="lastUserId"></param>
    public void SaveUserId( string groupName, string lastUserId ) {
      XElement xe = xeRoot.Elements( "group" )
        .Where( i => i.Attribute( "name" ) != null && i.Attribute( "name" ).Value == groupName )
        .FirstOrDefault();
      if ( xe == null )
        return;
      xe.SetElementValue( Key_LastUserID, lastUserId );
      Save();
    }

    /// <summary>
    /// 更新CountPerPage并保存
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="countPerPage"></param>
    public void SaveCountPerPage( string groupName, int countPerPage ) {
      XElement xe = xeRoot.Elements( "group" )
        .Where( i => i.Attribute( "name" ) != null && i.Attribute( "name" ).Value == groupName )
        .FirstOrDefault();
      if ( xe == null )
        return;
      xe.SetElementValue( Key_CountPerPage, countPerPage );
      Save();
    }


    /// <summary>
    /// 读取某个Element的值，不存在则返回null
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="elementName"></param>
    /// <returns></returns>
    public string ReadElementValue( string groupName, string elementName ) {
      XElement xe = xeRoot.Elements( "group" )
        .Where( i => i.Attribute( "name" ) != null && i.Attribute( "name" ).Value == groupName )
        .FirstOrDefault();
      if ( xe == null )
        return null;
      var ele = xe.Elements( elementName ).FirstOrDefault();
      return ele == null ? null : ele.Value;
    }

    public bool HasChildren( string groupName ) {
      XElement xe = xeRoot.Elements( "group" )
        .Where( i => i.Attribute( "name" ) != null && i.Attribute( "name" ).Value == groupName )
        .FirstOrDefault();
      return xe == null ? false : xe.HasElements;
    }

    /// <summary>
    /// 移除有关LastApi和LastPageIndex的记录
    /// </summary>
    /// <param name="groupName"></param>
    public void RemoveAccessLog( string groupName ) {
      XElement xe = xeRoot.Elements( "group" )
        .Where( i => i.Attribute( "name" ) != null && i.Attribute( "name" ).Value == groupName )
        .FirstOrDefault();
      if ( xe == null )
        return;
      xe.SetElementValue( Key_LastApi, null );
      xe.SetElementValue( Key_LastPageIndex, null );
      xe.SetElementValue( Key_LastUserID, null );
      xe.SetElementValue( Key_CountPerPage, null );
      Save();
    }

    /// <summary>
    /// 获取配置信息
    /// </summary>
    /// <param name="ApplicationExcuteFilePath"></param>
    /// <returns></returns>
    public static Configuration GetConfigration( string ApplicationExcuteFilePath ) {
      Configuration config = new Configuration( ApplicationExcuteFilePath );
      return config;
    }

  }

}
