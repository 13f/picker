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
  public class FellowPlusViewModel: Picker.ViewModels.ViewModelBase {
    #region Fields

    IStorage store = null;
    FellowPlus biz = null;

    /// <summary>
    /// AutoLoopInterval的毫秒数
    /// </summary>
    int auto_loop_in_miliseconds = 6000;
    AsynchronousCommand cmdPick = null;

    bool firstTime = true;
    DispatcherTimer timer = null;
    Random random = null;

    #endregion


    #region Properties

    /// <summary>
    /// Gets or sets UserId.
    /// </summary>
    public string UserId
    {
      get { return GetValue<string>( UserIdProperty ); }
      set { SetValue( UserIdProperty, value ); }
    }

    /// <summary>
    /// Register the UserId property so it is known in the class.
    /// </summary>
    public static readonly PropertyData UserIdProperty = RegisterProperty( "UserId", typeof( string ), "7967" );

    /// <summary>
        /// Gets or sets Token.
        /// </summary>
    public string Token
    {
      get { return GetValue<string>( TokenProperty ); }
      set { SetValue( TokenProperty, value ); }
    }

    /// <summary>
    /// Register the Token property so it is known in the class.
    /// </summary>
    public static readonly PropertyData TokenProperty = RegisterProperty( "Token", typeof( string ), "sCtcTUDKT%2B2IKN6r532jADl8jpCTvTouLvMkixXK1a0%3D" );

    /// <summary>
    /// Gets or sets AutoLoopInterval. 单位：秒。
    /// </summary>
    public int AutoLoopInterval
    {
      get { return GetValue<int>( AutoLoopIntervalProperty ); }
      set
      {
        SetValue( AutoLoopIntervalProperty, value );
        auto_loop_in_miliseconds = value < 6 ? 6000 : value * 1000;
      }
    }

    /// <summary>
    /// Register the AutoLoopInterval property so it is known in the class.
    /// </summary>
    public static readonly PropertyData AutoLoopIntervalProperty = RegisterProperty( "AutoLoopInterval", typeof( int ), 6 );

    /// <summary>
    /// Gets or sets Skip.
    /// </summary>
    public long Skip
    {
      get { return GetValue<long>( SkipProperty ); }
      set { SetValue( SkipProperty, value ); }
    }

    /// <summary>
    /// Register the Skip property so it is known in the class.
    /// </summary>
    public static readonly PropertyData SkipProperty = RegisterProperty( "Skip", typeof( long ), 0 );

    /// <summary>
    /// Gets or sets IsPickingUsers.
    /// </summary>
    public bool IsPickingData
    {
      get { return GetValue<bool>( IsPickingDataProperty ); }
      set
      {
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
    /// Gets or sets SkipRandomItemsWhenGetProjectPreviewNotProcessed.
    /// </summary>
    public bool SkipRandomItemsWhenGetProjectPreviewNotProcessed
    {
      get { return GetValue<bool>( SkipRandomItemsWhenGetProjectPreviewNotProcessedProperty ); }
      set { SetValue( SkipRandomItemsWhenGetProjectPreviewNotProcessedProperty, value ); }
    }

    /// <summary>
    /// Register the SkipRandomItemsWhenGetProjectPreviewNotProcessed property so it is known in the class.
    /// </summary>
    public static readonly PropertyData SkipRandomItemsWhenGetProjectPreviewNotProcessedProperty = RegisterProperty( "SkipRandomItemsWhenGetProjectPreviewNotProcessed", typeof( bool ), false );

    /// <summary>
    /// Gets or sets ProjectId.
    /// </summary>
    public string ProjectId
    {
      get { return GetValue<string>( ProjectIdProperty ); }
      set { SetValue( ProjectIdProperty, value ); }
    }

    /// <summary>
    /// Register the ProjectId property so it is known in the class.
    /// </summary>
    public static readonly PropertyData ProjectIdProperty = RegisterProperty( "ProjectId", typeof( string ), null );

    /// <summary>
    /// Gets or sets StatisticsInfo.
    /// </summary>
    public ObservableCollection<StatisticsItem> StatisticsInfo
    {
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
    public string Log
    {
      get { return GetValue<string>( LogProperty ); }
      set { SetValue( LogProperty, value ); }
    }

    /// <summary>
    /// Register the Log property so it is known in the class.
    /// </summary>
    public static readonly PropertyData LogProperty = RegisterProperty( "Log", typeof( string ), null );

    #endregion Properties


    public FellowPlusViewModel() {
      // in App.config
      string conn = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServer_FellowPlus"].ConnectionString;
      store = new Picker.Postgresql.StoreContext();
      store.OpenFellowPlusDatabase( conn );
      biz = new FellowPlus( store );

      CmdPickProjectsList = new AsynchronousCommand( OnCmdPickProjectsListExecute, OnCmdPickProjectsListCanExecute );
      CmdPickProjects = new AsynchronousCommand( OnCmdPickProjectsExecute, OnCmdPickProjectsCanExecute );
      CmdPickProjectById = new AsynchronousCommand( OnCmdPickProjectByIdExecute, OnCmdPickProjectByIdCanExecute );

      random = new Random( DateTime.Now.Millisecond );

      timer = new DispatcherTimer();
      timer.Tick += Timer_Tick;
      init();
    }


    #region Commands

    /// <summary>
    /// Gets the CmdPickProjectsList command.
    /// </summary>
    public AsynchronousCommand CmdPickProjectsList { get; private set; }

    /// <summary>
    /// Method to check whether the CmdPickProjectsList command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPickProjectsListCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdPickProjectsList command is executed.
    /// </summary>
    private async void OnCmdPickProjectsListExecute() {
      IsPickingData = true;
      cmdPick = CmdPickProjectsList;
      try {
        TimeSpan tsInterval = new TimeSpan( 0, 0, AutoLoopInterval );
        await biz.LoopPickProjectList( tsInterval, UserId, Token, Skip );
      }
      catch ( Exception ex ) {
        Log = ex.Message;
      }
      finally {
        IsPickingData = false;
        // 更新统计数据
        refreshStatistics();
      }
    }

    /// <summary>
    /// Gets the CmdPickProjects command.
    /// </summary>
    public AsynchronousCommand CmdPickProjects { get; private set; }

    /// <summary>
    /// Method to check whether the CmdPickProjects command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPickProjectsCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdPickProjects command is executed.
    /// </summary>
    private async void OnCmdPickProjectsExecute() {
      IsPickingData = true;
      cmdPick = CmdPickProjects;
      timer.Interval = new TimeSpan( 0, 0, AutoLoopInterval );
      timer.Start();
    }

    /// <summary>
    /// Gets the CmdPickProjectById command.
    /// </summary>
    public AsynchronousCommand CmdPickProjectById { get; private set; }

    /// <summary>
    /// Method to check whether the CmdPickProjectById command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPickProjectByIdCanExecute() {
      return !IsPickingData && !string.IsNullOrWhiteSpace( ProjectId );
    }

    /// <summary>
    /// Method to invoke when the CmdPickProjectById command is executed.
    /// </summary>
    private async void OnCmdPickProjectByIdExecute() {
      IsPickingData = true;
      try {
        await biz.PickProject( UserId, Token, ProjectId );
      }
      catch ( Exception ex ) {
        Log = ex.Message;
      }
      finally {
        IsPickingData = false;
        // 更新统计数据
        refreshStatistics();
      }
    }

    #endregion Commands


    #region Methods

    async Task init() {
      refreshStatistics();

      var siProjectsPreview = StatisticsInfo.Where( i => i.Key == "projects-preview" ).FirstOrDefault();
      Skip = siProjectsPreview.Count;
    }

    void raiseCommandsCanExecute() {
      CmdPickProjectsList.RaiseCanExecuteChanged();
    }

    void refreshStatistics() {
      // 抓取数据的时候进行查询，可能造成异步方法和同步方法同时对A表进行操作，会抛出异常
      if ( store == null || IsPickingData )
        return;
      try {
        App.Current.Dispatcher.Invoke( () => {
          List<StatisticsItem> data = store.FellowPlus_LoadStatistics();
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

    private async void Timer_Tick( object sender, EventArgs e ) {
      timer.Stop();
      try {
        int skipRandomItems = 0;
        if ( SkipRandomItemsWhenGetProjectPreviewNotProcessed )
          skipRandomItems = random.Next( 1, 10 );
        int r = await biz.PickProject( UserId, Token, skipRandomItems );
        if ( r > 0 )
          timer.Start();
        else {
          IsPickingData = false;
          // 更新统计数据
          refreshStatistics();
        }
      }
      catch ( Exception ex ) {
        Log = ex.Message;
        IsPickingData = false;
        // 更新统计数据
        refreshStatistics();
      }
    }


    #endregion Methods

  }

}
