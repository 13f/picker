using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;
using Catel.MVVM;
using Picker.Core.Spider;
using Picker.Core.Storage;
using Picker.Core.Models;
using System.Windows.Threading;

namespace Picker.ViewModels {
  /// <summary>
  /// DoubanBook view model.
  /// </summary>
  public class DoubanViewModel : Picker.ViewModels.ApiViewModel {
    #region Fields

    IStorage store = null;
    DoubanApi api = null;
    Douban biz = null;

    /// <summary>
    /// AutoLoopInterval的毫秒数
    /// </summary>
    int auto_loop_in_miliseconds = 6000;
    AsynchronousCommand cmdPick = null;

    bool firstTime = true;
    int pageIndex = 0;

    #endregion


    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubanBookViewModel"/> class.
    /// </summary>
    public DoubanViewModel() {
      // in App.config
      string conn = System.Configuration.ConfigurationManager.ConnectionStrings["Postgresql_Douban"].ConnectionString;
      AppKey = System.Configuration.ConfigurationManager.AppSettings["DoubanAppKey"];

      store = new Picker.Postgresql.StoreContext();
      store.OpenDoubanDatabase( conn );
      api = new DoubanApi( AppKey );
      biz = new Douban( api, store, config );

      // Commands
      CmdPickUsers = new AsynchronousCommand( OnCmdPickUsersExecute, OnOnCmdPickUsersCanExecute );
      CmdPickBooks = new AsynchronousCommand( OnCmdPickBooksExecute, OnCmdPickBooksCanExecute );
      CmdPickMoviesTop250 = new AsynchronousCommand( OnCmdPickMoviesTop250Execute, OnCmdPickMoviesTop250CanExecute );
      CmdPickTravel = new AsynchronousCommand( OnCmdPickTravelExecute, OnCmdPickTravelCanExecute );
      CmdPickSpecialUser = new AsynchronousCommand( OnCmdPickSpecialUserExecute, OnCmdPickSpecialUserCanExecute );
      CmdPickItemsOfPage = new AsynchronousCommand( OnCmdPickItemsOfPageExecute, OnCmdPickItemsOfPageCanExecute );
      CmdPickOneItem = new AsynchronousCommand( OnCmdPickOneItemExecute, OnCmdPickOneItemCanExecute );

      // 获取统计数据
      refreshStatistics();
      // 继续上一次任务
      Task.Factory.StartNew( () => {
        continueLastPicking();
      } );
    }

    #endregion


    #region Properties
    /// <summary>
    /// Gets the title of the view model.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return "View model title"; } }

    /// <summary>
    /// Gets or sets the AppKey.
    /// </summary>
    public string AppKey {
      get { return GetValue<string>( AppKeyProperty ); }
      set { SetValue( AppKeyProperty, value ); }
    }

    /// <summary>
    /// Register the AppKey property so it is known in the class.
    /// </summary>
    public static readonly PropertyData AppKeyProperty = RegisterProperty( "AppKey", typeof( string ), "" );

    /// <summary>
    /// Gets or sets AutoLoop.
    /// </summary>
    public bool AutoLoop {
      get { return GetValue<bool>( AutoLoopProperty ); }
      set { SetValue( AutoLoopProperty, value ); }
    }

    /// <summary>
    /// Register the AutoLoop property so it is known in the class.
    /// </summary>
    public static readonly PropertyData AutoLoopProperty = RegisterProperty( "AutoLoop", typeof( bool ), false );

    /// <summary>
    /// Gets or sets AutoLoopInterval. 单位：秒。
    /// </summary>
    public int AutoLoopInterval {
      get { return GetValue<int>( AutoLoopIntervalProperty ); }
      set {
        SetValue( AutoLoopIntervalProperty, value );
        auto_loop_in_miliseconds = value < 6 ? 6000 : value * 1000;
      }
    }

    /// <summary>
    /// Register the AutoLoopInterval property so it is known in the class.
    /// </summary>
    public static readonly PropertyData AutoLoopIntervalProperty = RegisterProperty( "AutoLoopInterval", typeof( int ), 6 );

    /// <summary>
    /// Gets or sets the SpecialUserId.
    /// </summary>
    public string SpecialUserId {
      get { return GetValue<string>( SpecialUserIdProperty ); }
      set {
        string v = value != null && value.StartsWith("http://") ? api.GetUserIdByPersonalPageUri( value ) : value;
        SetValue( SpecialUserIdProperty, v );
      }
    }

    /// <summary>
    /// Register the StartingUserId property so it is known in the class.
    /// </summary>
    public static readonly PropertyData SpecialUserIdProperty = RegisterProperty( "StartingUserId", typeof( string ), "" );

    /// <summary>
    /// Gets or sets SeriePage.
    /// </summary>
    public string SeriePage {
      get { return GetValue<string>( SeriePageProperty ); }
      set { SetValue( SeriePageProperty, value ); }
    }

    /// <summary>
    /// Register the SeriePage property so it is known in the class.
    /// </summary>
    public static readonly PropertyData SeriePageProperty = RegisterProperty( "SeriePage", typeof( string ), null );

    /// <summary>
    /// Gets or sets CountPerSeriePage.
    /// </summary>
    public int CountPerSeriePage {
      get { return GetValue<int>( CountPerSeriePageProperty ); }
      set { SetValue( CountPerSeriePageProperty, value ); }
    }

    /// <summary>
    /// Register the CountPerSeriePage property so it is known in the class.
    /// </summary>
    public static readonly PropertyData CountPerSeriePageProperty = RegisterProperty( "CountPerSeriePage", typeof( int ), 25 );

    /// <summary>
    /// Gets or sets SubjectUrl.
    /// </summary>
    public string SubjectUrl {
      get { return GetValue<string>( SubjectUrlProperty ); }
      set { SetValue( SubjectUrlProperty, value ); }
    }

    /// <summary>
    /// Register the SubjectUrl property so it is known in the class.
    /// </summary>
    public static readonly PropertyData SubjectUrlProperty = RegisterProperty( "SubjectUrl", typeof( string ), null );

    /// <summary>
    /// Gets or sets PageUri.
    /// </summary>
    public string PageUri {
      get { return GetValue<string>( PageUriProperty ); }
      set {
        SetValue( PageUriProperty, value );
        if ( RefreshBrowser != null )
          RefreshBrowser();
      }
    }

    /// <summary>
    /// Register the PageUri property so it is known in the class.
    /// </summary>
    public static readonly PropertyData PageUriProperty = RegisterProperty( "PageUri", typeof( string ), null );

    /// <summary>
    /// Gets or sets HtmlDownloaded.
    /// </summary>
    public bool HtmlDownloaded {
      get { return GetValue<bool>( HtmlDownloadedProperty ); }
      set { SetValue( HtmlDownloadedProperty, value ); }
    }

    /// <summary>
    /// Register the HtmlDownloaded property so it is known in the class.
    /// </summary>
    public static readonly PropertyData HtmlDownloadedProperty = RegisterProperty( "HtmlDownloaded", typeof( bool ), true );

    /// <summary>
    /// Gets or sets CurrentHtml.
    /// </summary>
    public string CurrentHtml {
      get { return GetValue<string>( CurrentHtmlProperty ); }
      set {
        SetValue( CurrentHtmlProperty, value );
        processHtmlContent();
      }
    }

    /// <summary>
    /// Register the CurrentHtml property so it is known in the class.
    /// </summary>
    public static readonly PropertyData CurrentHtmlProperty = RegisterProperty( "CurrentHtml", typeof( string ), null );

    /// <summary>
        /// Gets or sets the PageNumber.
        /// </summary>
    public int PageNumber {
      get { return GetValue<int>( PageNumberProperty ); }
      set { SetValue( PageNumberProperty, value ); }
    }

    /// <summary>
    /// Register the PageNumber property so it is known in the class.
    /// </summary>
    public static readonly PropertyData PageNumberProperty = RegisterProperty( "PageNumber", typeof( int ), 0 );

    /// <summary>
    /// Gets or sets IsPickingUsers.
    /// </summary>
    public bool IsPickingData {
      get { return GetValue<bool>( IsPickingDataProperty ); }
      set {
        SetValue( IsPickingDataProperty, value );
        if ( value )
          Log = "抓取中……";
        // raise commands
        raiseCommandsCanExecute();
      }
    }

    /// <summary>
    /// Register the IsPickingUsers property so it is known in the class.
    /// </summary>
    public static readonly PropertyData IsPickingDataProperty = RegisterProperty( "IsPickingUsers", typeof( bool ), false );

    /// <summary>
    /// Gets or sets StatisticsInfo.
    /// </summary>
    public ObservableCollection<StatisticsItem> StatisticsInfo {
      get { return GetValue<ObservableCollection<StatisticsItem>>( StatisticsInfoProperty ); }
      set { SetValue( StatisticsInfoProperty, value ); }
    }

    /// <summary>
    /// Register the StatisticsInfo property so it is known in the class.
    /// </summary>
    public static readonly PropertyData StatisticsInfoProperty = RegisterProperty( "StatisticsInfo", typeof( ObservableCollection<StatisticsItem> ), null );

    /// <summary>
    /// Gets or sets Log.
    /// </summary>
    public string Log {
      get { return GetValue<string>( LogProperty ); }
      set { SetValue( LogProperty, value ); }
    }

    /// <summary>
    /// Register the Log property so it is known in the class.
    /// </summary>
    public static readonly PropertyData LogProperty = RegisterProperty( "Log", typeof( string ), null );

    /// <summary>
    /// 刷新浏览器
    /// </summary>
    public Action RefreshBrowser { get; set; }

    #endregion


    #region Commands

    /// <summary>
    /// Gets the CmdPickUsers command.
    /// </summary>
    public AsynchronousCommand CmdPickUsers { get; private set; }

    private bool OnOnCmdPickUsersCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdPickUsers command is executed.
    /// </summary>
    private async void OnCmdPickUsersExecute() {
      cmdPick = CmdPickUsers;
      IsPickingData = true;
      try {
        await biz.StartUserTask( null, null, false );
      }
      catch ( Exception ex ) {
        Log = ex.Message;
      }
      finally {
        IsPickingData = false;
      }
      // 更新统计数据
      refreshStatistics();
      // 是否自动继续
      await autoLoop();
    }

    /// <summary>
    /// Gets the CmdPickBooks command.
    /// </summary>
    public AsynchronousCommand CmdPickBooks { get; private set; }

    /// <summary>
    /// Method to check whether the CmdPickBooks command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPickBooksCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdPickBooks command is executed.
    /// </summary>
    private async void OnCmdPickBooksExecute() {
      cmdPick = CmdPickBooks;
      IsPickingData = true;
      try {
        await biz.StartBookTask( null, false );
      }
      catch ( Exception ex ) {
        Log = ex.Message;
      }
      finally {
        IsPickingData = false;
      }
      // 更新统计数据
      refreshStatistics();
      // 是否自动继续
      await autoLoop();
    }

    /// <summary>
    /// Gets the CmdPickMoviesTop250 command.
    /// </summary>
    public AsynchronousCommand CmdPickMoviesTop250 { get; private set; }

    /// <summary>
    /// Method to check whether the CmdPickMoviesTop250 command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPickMoviesTop250CanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdPickMoviesTop250 command is executed.
    /// </summary>
    private async void OnCmdPickMoviesTop250Execute() {
      cmdPick = null; // CmdPickMoviesTop250;
      IsPickingData = true;
      bool hasError = false;
      try {
        await biz.StartMovieTask_Top250( false );
      }
      catch ( Exception ex ) {
        Log = ex.Message;
        hasError = true;
      }
      finally {
        IsPickingData = false;
        if ( !hasError )
          Log = "完成。";
      }
      // 更新统计数据
      refreshStatistics();
      // 是否自动继续
      //await autoLoop();
    }

    /// <summary>
    /// Gets the CmdPickTravel command.
    /// </summary>
    public AsynchronousCommand CmdPickTravel { get; private set; }

    /// <summary>
    /// Method to check whether the CmdPickTravel command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPickTravelCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdPickTravel command is executed.
    /// </summary>
    private async void OnCmdPickTravelExecute() {
      cmdPick = CmdPickTravel;
      IsPickingData = true;
      try {
        await biz.StartTravelTask( null, false );
      }
      catch ( Exception ex ) {
        Log = ex.Message;
      }
      finally {
        IsPickingData = false;
      }
      // 更新统计数据
      refreshStatistics();
      // 是否自动继续
      await autoLoop();
    }

    /// <summary>
    /// Gets the CmdPickSpecialUser command.
    /// </summary>
    public AsynchronousCommand CmdPickSpecialUser { get; private set; }

    /// <summary>
    /// Method to check whether the CmdPickSpecialUser command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPickSpecialUserCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdPickSpecialUser command is executed.
    /// </summary>
    private async void OnCmdPickSpecialUserExecute() {
      cmdPick = null;
      IsPickingData = true;
      bool hasError = false;
      try {
        await biz.StartTask( null, SpecialUserId );
      }
      catch ( Exception ex ) {
        Log = ex.Message;
        hasError = true;
      }
      finally {
        IsPickingData = false;
        if ( !hasError )
          Log = "完成。";
      }
      // 更新统计数据
      refreshStatistics();
    }

    /// <summary>
    /// Gets the CmdPickItemsOfPage command.
    /// </summary>
    public AsynchronousCommand CmdPickItemsOfPage { get; private set; }

    /// <summary>
    /// Method to check whether the CmdPickItemsOfPage command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPickItemsOfPageCanExecute() {
      return !IsPickingData && HtmlDownloaded && !string.IsNullOrWhiteSpace( SeriePage );
    }

    /// <summary>
    /// Method to invoke when the CmdPickItemsOfPage command is executed.
    /// </summary>
    private async void OnCmdPickItemsOfPageExecute() {
      cmdPick = null;
      IsPickingData = true;
      try {
        // 1
        //await biz.PickItemsOfPage( SeriePage, CountPerSeriePage, false );

        // 2
        pageIndex = 0;
        PageNumber = pageIndex + 1;
        updatePageUri();
      }
      catch ( Exception ex ) {
        Log = ex.Message;
        IsPickingData = false;
        HtmlDownloaded = true; // enable the button
        CurrentHtml = null;
      }
    }

    /// <summary>
    /// Gets the PickOneItem command.
    /// </summary>
    public AsynchronousCommand CmdPickOneItem { get; private set; }

    /// <summary>
    /// Method to check whether the PickOneItem command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPickOneItemCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the PickOneItem command is executed.
    /// </summary>
    private async void OnCmdPickOneItemExecute() {
      cmdPick = null;
      IsPickingData = true;
      bool hasError = false;
      try {
        await biz.PickOneItem( SubjectUrl, false );
      }
      catch ( Exception ex ) {
        Log = ex.Message;
        hasError = true;
      }
      finally {
        IsPickingData = false;
        if ( !hasError )
          Log = "完成。";
      }
      // 更新统计数据
      refreshStatistics();
    }

    #endregion


    #region Methods

    void raiseCommandsCanExecute() {
      CmdPickUsers.RaiseCanExecuteChanged();
      CmdPickBooks.RaiseCanExecuteChanged();
      CmdPickMoviesTop250.RaiseCanExecuteChanged();
      CmdPickTravel.RaiseCanExecuteChanged();
      CmdPickSpecialUser.RaiseCanExecuteChanged();
      CmdPickItemsOfPage.RaiseCanExecuteChanged();
      CmdPickOneItem.RaiseCanExecuteChanged();
    }

    void refreshStatistics() {
      // 抓取数据的时候进行查询，可能造成异步方法和同步方法同时对A表进行操作，会抛出异常
      if ( store == null || IsPickingData )
        return;
      try {
        App.Current.Dispatcher.Invoke( () => {
          List<StatisticsItem> data = store.Douban_LoadStatistics();
          if ( StatisticsInfo != null )
            StatisticsInfo.Clear();
          if ( data != null )
            StatisticsInfo = new ObservableCollection<StatisticsItem>( data );
        } );
      }
      catch ( Exception ex ) {
        Log = "获取统计数据时出错，稍后重试：" + ex.Message;
      }     
    }

    async Task autoLoop() {
      if ( cmdPick == null || !AutoLoop || !cmdPick.CanExecute() || cmdPick.IsExecuting ) {
        Log = "完成。";
        return;
      }
      await Task.Delay( auto_loop_in_miliseconds );
      cmdPick.Execute();
    }

    /// <summary>
    /// 是否继续上一次的查询。一次性的，不会进入循环模式。
    /// </summary>
    async Task continueLastPicking() {
      string group = null;
      if ( config.HasChildren( Configuration.Key_Douban_Book ) )
        group = Configuration.Key_Douban_Book;
      else if ( config.HasChildren( Configuration.Key_Douban_Movie ) )
        group = Configuration.Key_Douban_Movie;
      else if ( config.HasChildren( Configuration.Key_Douban_Music ) )
        group = Configuration.Key_Douban_Music;
      else if ( config.HasChildren( Configuration.Key_Douban_Travel ) )
        group = Configuration.Key_Douban_Travel;
      else if ( config.HasChildren( Configuration.Key_Douban_User ) )
        group = Configuration.Key_Douban_User;
      else if ( config.HasChildren( Configuration.Key_Douban_Page ) )
        group = Configuration.Key_Douban_Page;

      if ( string.IsNullOrWhiteSpace( group ) )
        return;

      string lastApi = config.ReadElementValue( group, Configuration.Key_LastApi );
      string lastPageIndexString = config.ReadElementValue( group, Configuration.Key_LastPageIndex );
      string lastUserId = config.ReadElementValue( group, Configuration.Key_LastUserID );
      string countPerPageString = config.ReadElementValue( group, Configuration.Key_CountPerPage );

      int lastPageIndex = 0;
      int.TryParse( lastPageIndexString, out lastPageIndex );
      if ( lastPageIndex < 0 )
        lastPageIndex = 0;

      IsPickingData = true;
      bool hasError = false;
      try {
        if ( group == Configuration.Key_Douban_Page ) {
          int countPerPage = 25;
          int.TryParse( countPerPageString, out countPerPage );
          if ( countPerPage > 0 )
            CountPerSeriePage = countPerPage;

          SeriePage = lastApi;
          pageIndex = lastPageIndex;
          PageNumber = pageIndex + 1;
          updatePageUri();
        }
        else {
          await biz.ContinueLastTask( group, lastApi, lastPageIndex, lastUserId, CountPerSeriePage );
        }
      }
      catch ( Exception ex ) {
        Log = ex.Message;
        hasError = true;
      }
      finally {
        if ( group != Configuration.Key_Douban_Page )
          IsPickingData = false;
        if ( !hasError )
          Log = "完成。";
      }
      if ( group != Configuration.Key_Douban_Page ) {// 更新统计数据
        refreshStatistics();
        tryAutoLoop( group );
      }
    }

    /// <summary>
    /// 更新PageUri，UI会自动刷新WebBrowser
    /// </summary>
    private void updatePageUri() {
      HtmlDownloaded = false;
      CurrentHtml = null;

      int start = pageIndex * CountPerSeriePage;
      PageUri = string.Format( SeriePage + "?start={0}&apikey={1}", start, api.AppKey );
    }

    async Task processHtmlContent() {
      if ( !IsPickingData || string.IsNullOrWhiteSpace( CurrentHtml ) )
        return;
      int count = 0;
      //bool hasError = false;
      try {
        count = await biz.PickItemsOfPage( SeriePage, CurrentHtml, CountPerSeriePage, false, pageIndex );
        // continue?
        bool hasMore = ( count >= CountPerSeriePage );
        if ( hasMore ) {
          // wait 2 seconds
          await Task.Delay( 2000 );
          // update and refresh
          pageIndex++;
          PageNumber = pageIndex + 1;
          updatePageUri();
        }
        else {
          IsPickingData = false;
          Log = "完成。";
          // 更新统计数据
          refreshStatistics();
        }
      }
      catch ( Exception ex ) {
        Log = ex.Message;
        //hasError = true;
        IsPickingData = false;
      }
    }

    /// <summary>
    /// 尝试自动进入循环模式
    /// </summary>
    /// <param name="group"></param>
    void tryAutoLoop( string group ) {
      if ( string.IsNullOrWhiteSpace( group ) )
        return;
      switch ( group ) {
        case Configuration.Key_Douban_Book:
          if ( CmdPickBooks.CanExecute() )
            CmdPickBooks.Execute();
          break;
        case Configuration.Key_Douban_User:
          if ( CmdPickUsers.CanExecute() )
            CmdPickUsers.Execute();
          break;
        case Configuration.Key_Douban_Travel:
          if ( CmdPickTravel.CanExecute() )
            CmdPickTravel.Execute();
          break;
      }
    }

    #endregion

  }

}
