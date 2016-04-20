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
  public class QichachaViewModel : Picker.ViewModels.ViewModelBase {
    #region Fields

    IStorage store = null;
    Qichacha biz = null;

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
    /// Gets or sets CookieAuth.
    /// </summary>
    public string CookieAuth
    {
      get { return GetValue<string>( CookieAuthProperty ); }
      set {
        SetValue( CookieAuthProperty, value );
        if ( !string.IsNullOrWhiteSpace( value ) && biz != null )
          biz.SetAuthCookie( value );
      }
    }

    /// <summary>
    /// Register the CookieAuth property so it is known in the class.
    /// </summary>
    public static readonly PropertyData CookieAuthProperty = RegisterProperty( "CookieAuth", typeof( string ), ";pspt=" );

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
    /// Gets or sets AlbumPage.
    /// </summary>
    public string AlbumPage
    {
      get { return GetValue<string>( AlbumPageProperty ); }
      set { SetValue( AlbumPageProperty, value ); }
    }

    /// <summary>
    /// Register the name property so it is known in the class.
    /// </summary>
    public static readonly PropertyData AlbumPageProperty = RegisterProperty( "AlbumPage", typeof( string ), "http://qichacha.com/album" );

    /// <summary>
    /// Gets or sets AlbumUri.
    /// </summary>
    public string AlbumUri
    {
      get { return GetValue<string>( AlbumUriProperty ); }
      set { SetValue( AlbumUriProperty, value ); }
    }

    /// <summary>
    /// Register the name property so it is known in the class.
    /// </summary>
    public static readonly PropertyData AlbumUriProperty = RegisterProperty( "AlbumUri", typeof( string ), null );

    /// <summary>
        /// Gets or sets QueryKey.
        /// </summary>
    public string QueryKey
    {
      get { return GetValue<string>( QueryKeyProperty ); }
      set { SetValue( QueryKeyProperty, value ); }
    }

    /// <summary>
    /// Register the QueryKey property so it is known in the class.
    /// </summary>
    public static readonly PropertyData QueryKeyProperty = RegisterProperty( "QueryKey", typeof( string ), null );

    /// <summary>
    /// Gets or sets LastPage.
    /// </summary>
    public int LastPage
    {
      get { return GetValue<int>( LastPageProperty ); }
      set { SetValue( LastPageProperty, value ); }
    }

    /// <summary>
    /// Register the LastPage property so it is known in the class.
    /// </summary>
    public static readonly PropertyData LastPageProperty = RegisterProperty( "LastPage", typeof( int ), 0 );

    #endregion Properties


    public QichachaViewModel() {
      // in App.config
      string conn = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServer_Qichacha"].ConnectionString;
      store = new Picker.Postgresql.StoreContext();
      store.OpenQichachaDatabase( conn );
      biz = new Qichacha( store );

      CmdPickAlbums = new Command( OnCmdPickAlbumsExecute, OnCmdPickAlbumsCanExecute );
      // company list
      CmdPickCompanyListByAlbum = new Command( OnCmdPickCompanyListByAlbumExecute, OnCmdPickCompanyListByAlbumCanExecute );
      CmdSearchCompany = new Command( OnCmdSearchCompanyExecute, OnCmdSearchCompanyCanExecute );
      // assets
      CmdTrademark = new Command( OnCmdTrademarkExecute, OnCmdTrademarkCanExecute );
      CmdWebsite = new Command( OnCmdWebsiteExecute, OnCmdWebsiteCanExecute );
      CmdPatent = new Command( OnCmdPatentExecute, OnCmdPatentCanExecute );
      CmdCertificate = new Command( OnCmdCertificateExecute, OnCmdCertificateCanExecute );
      CmdCopyright = new Command( OnCmdCopyrightExecute, OnCmdCopyrightCanExecute );
      CmdSoftwareCopyright = new Command( OnCmdSoftwareCopyrightExecute, OnCmdSoftwareCopyrightCanExecute );

      refresh();
    }


    #region Commands

    /// <summary>
        /// Gets the CmdPickAlbums command.
        /// </summary>
    public Command CmdPickAlbums { get; private set; }
    
    /// <summary>
    /// Method to check whether the CmdPickAlbums command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPickAlbumsCanExecute() {
      return !IsPickingData && !string.IsNullOrWhiteSpace( AlbumPage );
    }

    /// <summary>
    /// Method to invoke when the CmdPickAlbums command is executed.
    /// </summary>
    private async void OnCmdPickAlbumsExecute() {
      IsPickingData = true;
      try {
        TimeSpan tsInterval = new TimeSpan( 0, 0, AutoLoopInterval );
        await biz.PickAlbumList( tsInterval, AlbumPage );
      }
      catch ( Exception ex ) {
        Log = ex.Message;
      }
      finally {
        IsPickingData = false;
        // 更新数据
        refresh();
      }
    }

    #region Company List

    /// <summary>
    /// Gets the CmdPickCompanyListByAlbum command.
    /// </summary>
    public Command CmdPickCompanyListByAlbum { get; private set; }

    /// <summary>
    /// Method to check whether the CmdPickCompanyListByAlbum command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPickCompanyListByAlbumCanExecute() {
      return !IsPickingData && !string.IsNullOrWhiteSpace( AlbumUri );
    }

    /// <summary>
    /// Method to invoke when the CmdPickCompanyListByAlbum command is executed.
    /// </summary>
    private async void OnCmdPickCompanyListByAlbumExecute() {
      IsPickingData = true;
      try {
        TimeSpan tsInterval = new TimeSpan( 0, 0, AutoLoopInterval );
        await biz.PickCompanyListByAlbum( tsInterval, AlbumUri );
      }
      catch ( Exception ex ) {
        Log = ex.Message;
      }
      finally {
        IsPickingData = false;
        // 更新数据
        refresh();
      }
    }

    /// <summary>
    /// Gets the CmdSearchCompany command.
    /// </summary>
    public Command CmdSearchCompany { get; private set; }
    
    /// <summary>
    /// Method to check whether the CmdSearchCompany command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdSearchCompanyCanExecute() {
      return !IsPickingData && !string.IsNullOrWhiteSpace( QueryKey );
    }

    /// <summary>
    /// Method to invoke when the CmdSearchCompany command is executed.
    /// </summary>
    private async void OnCmdSearchCompanyExecute() {
      IsPickingData = true;
      try {
        TimeSpan tsInterval = new TimeSpan( 0, 0, AutoLoopInterval );
        await biz.PickCompanyListBySearch( tsInterval, QueryKey, LastPage + 1 );
      }
      catch ( Exception ex ) {
        Log = ex.Message;
      }
      finally {
        IsPickingData = false;
        // 更新数据
        refresh();

        // continue?
        if ( !string.IsNullOrWhiteSpace( QueryKey ) ) {
          await Task.Delay( auto_loop_in_miliseconds );
          OnCmdSearchCompanyExecute();
        }
      }
    }

    #endregion

    #region Assets

    /// <summary>
    /// Gets the CmdTrademark command.
    /// </summary>
    public Command CmdTrademark { get; private set; }
    
    /// <summary>
    /// Method to check whether the CmdTrademark command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdTrademarkCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdTrademark command is executed.
    /// </summary>
    private void OnCmdTrademarkExecute() {
      
    }

    /// <summary>
    /// Gets the CmdWebsite command.
    /// </summary>
    public Command CmdWebsite { get; private set; }
    
    /// <summary>
    /// Method to check whether the CmdWebsite command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdWebsiteCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdWebsite command is executed.
    /// </summary>
    private void OnCmdWebsiteExecute() {
      
    }

    /// <summary>
    /// Gets the CmdPatent command.
    /// </summary>
    public Command CmdPatent { get; private set; }
    
    /// <summary>
    /// Method to check whether the CmdPatent command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdPatentCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdPatent command is executed.
    /// </summary>
    private void OnCmdPatentExecute() {
      
    }

    /// <summary>
    /// Gets the CmdCertificate command.
    /// </summary>
    public Command CmdCertificate { get; private set; }
    
    /// <summary>
    /// Method to check whether the CmdCertificate command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdCertificateCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdCertificate command is executed.
    /// </summary>
    private void OnCmdCertificateExecute() {
      
    }

    /// <summary>
    /// Gets the CmdCopyright command.
    /// </summary>
    public Command CmdCopyright { get; private set; }
    
    /// <summary>
    /// Method to check whether the CmdCopyright command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdCopyrightCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdCopyright command is executed.
    /// </summary>
    private void OnCmdCopyrightExecute() {
      
    }

    /// <summary>
    /// Gets the CmdSoftwareCopyright command.
    /// </summary>
    public Command CmdSoftwareCopyright { get; private set; }
    
    /// <summary>
    /// Method to check whether the CmdSoftwareCopyright command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdSoftwareCopyrightCanExecute() {
      return !IsPickingData;
    }

    /// <summary>
    /// Method to invoke when the CmdSoftwareCopyright command is executed.
    /// </summary>
    private void OnCmdSoftwareCopyrightExecute() {
      
    }

    #endregion Assets

    #endregion Commands


    #region Methods

    void refresh() {
      // 统计数据
      var tmp = store.Qichacha_LoadStatistics();
      StatisticsInfo = new ObservableCollection<StatisticsItem>( tmp );
      // album
      AlbumUri = store.Qichacha_GetAlbumUri_NeedToProcess();
      // search
      var kvp = store.Qichacha_GetLast_CompanySearch();
      if(!string.IsNullOrWhiteSpace( kvp.Key ) && kvp.Value >= 0 ) {
        QueryKey = kvp.Key;
        LastPage = kvp.Value;
      }
      else {
        QueryKey = null;
        LastPage = 0;
      }
    }

    void raiseCommandsCanExecute() {
      CmdPickAlbums.RaiseCanExecuteChanged();
      CmdTrademark.RaiseCanExecuteChanged();
      CmdWebsite.RaiseCanExecuteChanged();
      CmdPatent.RaiseCanExecuteChanged();
      CmdCertificate.RaiseCanExecuteChanged();
      CmdCopyright.RaiseCanExecuteChanged();
      CmdSoftwareCopyright.RaiseCanExecuteChanged();
    }

    #endregion Methods

  }

}
