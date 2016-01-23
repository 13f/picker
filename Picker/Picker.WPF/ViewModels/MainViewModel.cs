using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;
using Catel.MVVM;

namespace Picker.ViewModels {
  public class MainViewModel : ViewModelBase {

    /// <summary>
        /// Gets or sets HtmlTableViewOpend.
        /// </summary>
    public bool HtmlTableViewOpend
    {
      get { return GetValue<bool>( HtmlTableViewOpendProperty ); }
      set {
        SetValue( HtmlTableViewOpendProperty, value );
        if ( CmdHtmlTable != null )
          CmdHtmlTable.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Register the HtmlTableViewOpend property so it is known in the class.
    /// </summary>
    public static readonly PropertyData HtmlTableViewOpendProperty = RegisterProperty( "HtmlTableViewOpend", typeof( bool ), false );

    /// <summary>
        /// Gets or sets DoubanViewOpend.
        /// </summary>
    public bool DoubanViewOpend
    {
      get { return GetValue<bool>( DoubanViewOpendProperty ); }
      set {
        SetValue( DoubanViewOpendProperty, value );
        if ( CmdDouban != null )
          CmdDouban.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Register the DoubanViewOpend property so it is known in the class.
    /// </summary>
    public static readonly PropertyData DoubanViewOpendProperty = RegisterProperty( "DoubanViewOpend", typeof( bool ), false );

    public MainViewModel() {
      CmdHtmlTable = new Command( OnCmdHtmlTableExecute, OnCmdHtmlTableCanExecute );

      CmdDouban = new Command( OnCmdDoubanExecute, OnCmdDoubanCanExecute );
    }


    #region Commands

    /// <summary>
    /// Gets the CmdHtmlTable command.
    /// </summary>
    public Command CmdHtmlTable { get; private set; }
    
    /// <summary>
    /// Method to check whether the CmdHtmlTable command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdHtmlTableCanExecute() {
      return !HtmlTableViewOpend;
    }

    /// <summary>
    /// Method to invoke when the CmdHtmlTable command is executed.
    /// </summary>
    private void OnCmdHtmlTableExecute() {
      HtmlTableViewOpend = true;

      HtmlTableViewModel viewmodel = new HtmlTableViewModel();
      Views.HtmlTableView view = new Views.HtmlTableView();
      view.DataContext = viewmodel;
      view.Closed += HtmlTableView_Closed;
      view.Show();
    }

    private void HtmlTableView_Closed( object sender, EventArgs e ) {
      HtmlTableViewOpend = false;
    }

    /// <summary>
    /// Gets the CmdDouban command.
    /// </summary>
    public Command CmdDouban { get; private set; }

    /// <summary>
    /// Method to check whether the CmdDouban command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdDoubanCanExecute() {
      return !DoubanViewOpend;
    }

    /// <summary>
    /// Method to invoke when the CmdDouban command is executed.
    /// </summary>
    private void OnCmdDoubanExecute() {
      DoubanViewOpend = true;

      DoubanViewModel viewmodel = new DoubanViewModel();
      Views.MainView view = new Views.MainView();
      view.DataContext = viewmodel;
      view.Closed += DoubanView_Closed;
      view.Show();
    }

    private void DoubanView_Closed( object sender, EventArgs e ) {
      DoubanViewOpend = false;
    }

    #endregion Commands


    #region Methods



    #endregion Methods

  }

}
