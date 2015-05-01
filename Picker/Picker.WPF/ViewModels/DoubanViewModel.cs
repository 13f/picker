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

    DispatcherTimer timer = null;

    #endregion


    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubanBookViewModel"/> class.
    /// </summary>
    public DoubanViewModel() {
      // in App.config
      string conn = System.Configuration.ConfigurationManager.ConnectionStrings["Postgresql_Douban"].ConnectionString;
      AppKey = System.Configuration.ConfigurationManager.AppSettings["DoubanAppKey"];

      store = new Picker.Postgresql.StoreContext( conn );
      api = new DoubanApi( AppKey );
      biz = new Douban( api, store, config );

      timer = new DispatcherTimer( DispatcherPriority.Normal );
      timer.Interval = new TimeSpan( 0, 0, 3 );
      timer.Tick += timer_Tick;
      timer.Start();

      // Commands
      CmdPickUsers = new AsynchronousCommand( OnCmdPickUsersExecute, OnOnCmdPickUsersCanExecute );
      CmdPickBooks = new AsynchronousCommand( OnCmdPickBooksExecute, OnCmdPickBooksCanExecute );
      CmdPickMoviesTop250 = new AsynchronousCommand( OnCmdPickMoviesTop250Execute, OnCmdPickMoviesTop250CanExecute );
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
    /// Gets or sets the StartingUserId.
    /// </summary>
    public string StartingUserId {
      get { return GetValue<string>( StartingUserIdProperty ); }
      set { SetValue( StartingUserIdProperty, value ); }
    }

    /// <summary>
    /// Register the StartingUserId property so it is known in the class.
    /// </summary>
    public static readonly PropertyData StartingUserIdProperty = RegisterProperty( "StartingUserId", typeof( string ), "" );

    /// <summary>
    /// Gets or sets IsPickingUsers.
    /// </summary>
    public bool IsPickingData {
      get { return GetValue<bool>( IsPickingDataProperty ); }
      set {
        SetValue( IsPickingDataProperty, value );
        CmdPickUsers.RaiseCanExecuteChanged();
        CmdPickBooks.RaiseCanExecuteChanged();
        CmdPickMoviesTop250.RaiseCanExecuteChanged();
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
      IsPickingData = true;
      try {
        await biz.StartUserTask( null, StartingUserId, false );
        //var task = biz.StartUserTask( null, StartingUserId, false );
      }
      finally {
        IsPickingData = false;
      }
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
      IsPickingData = true;
      try {
        await biz.StartBookTask( null, false );
      }
      finally {
        IsPickingData = false;
      }
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
      IsPickingData = true;
      try {
        await biz.StartMovieTask_Top250( false );
      }
      finally {
        IsPickingData = false;
      }
    }

    #endregion


    #region Methods

    void timer_Tick( object sender, EventArgs e ) {
      refreshStatistics();
    }

    void refreshStatistics() {
      // 抓取数据的时候进行查询，可能造成异步方法和同步方法同时对A表进行操作，会抛出异常
      if ( store == null || IsPickingData )
        return;

      if ( StatisticsInfo != null )
        StatisticsInfo.Clear();
      var data = store.LoadStatistics();
      StatisticsInfo = new ObservableCollection<StatisticsItem>( data );
    }

    #endregion

  }

}
