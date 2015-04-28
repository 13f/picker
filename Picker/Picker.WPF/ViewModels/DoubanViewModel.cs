using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;
using Catel.MVVM;
using Picker.Core.Spider;
using Picker.Core.Storage;

namespace Picker.ViewModels {
  /// <summary>
  /// DoubanBook view model.
  /// </summary>
  public class DoubanViewModel : Picker.ViewModels.ApiViewModel {
    #region Fields

    IStorage store = null;
    DoubanApi api = null;
    Douban biz = null;

    #endregion


    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubanBookViewModel"/> class.
    /// </summary>
    public DoubanViewModel() {
      // in App.config
      string conn = System.Configuration.ConfigurationManager.ConnectionStrings["Postgresql_Douban"].ConnectionString;
      string appkey = System.Configuration.ConfigurationManager.AppSettings["DoubanAppKey"];

      store = new Picker.Postgresql.StoreContext( conn );
      api = new DoubanApi( appkey );
      biz = new Douban( api, store, config );

      // Commands
      CmdPickUsers = new AsynchronousCommand( OnCmdPickUsersExecute );
    }

    #endregion


    #region Properties
    /// <summary>
    /// Gets the title of the view model.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return "View model title"; } }

    /// <summary>
    /// Gets or sets IsPickingUsers.
    /// </summary>
    public bool IsPickingUsers {
      get { return GetValue<bool>( IsPickingUsersProperty ); }
      set {
        SetValue( IsPickingUsersProperty, value );
        CmdPickUsers.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Register the IsPickingUsers property so it is known in the class.
    /// </summary>
    public static readonly PropertyData IsPickingUsersProperty = RegisterProperty( "IsPickingUsers", typeof( bool ), false );

    #endregion


    #region Commands

    /// <summary>
    /// Gets the CmdPickUsers command.
    /// </summary>
    public AsynchronousCommand CmdPickUsers { get; private set; }

    /// <summary>
    /// Method to invoke when the CmdPickUsers command is executed.
    /// </summary>
    private async void OnCmdPickUsersExecute() {
      IsPickingUsers = true;
      try {
        await biz.StartUserTask( "taurenshaman", false );
      }
      finally {
        IsPickingUsers = false;
      }
    }

    private bool OnOnCmdPickUsersCanExecute() {
      return !IsPickingUsers;
    }

    #endregion


    #region Methods

    #endregion

  }

}
