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
        /// Gets or sets HtmlViewOpened.
        /// </summary>
    public bool HtmlViewOpened
    {
      get { return GetValue<bool>( HtmlViewOpenedProperty ); }
      set { SetValue( HtmlViewOpenedProperty, value ); }
    }

    /// <summary>
    /// Register the HtmlViewOpened property so it is known in the class.
    /// </summary>
    public static readonly PropertyData HtmlViewOpenedProperty = RegisterProperty( "HtmlViewOpened", typeof( bool ), false );

    /// <summary>
        /// Gets or sets HtmlTableViewOpend.
        /// </summary>
    public bool HtmlTableViewOpened
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
        /// Gets or sets TabularViewOpened.
        /// </summary>
    public bool TabularViewOpened
    {
      get { return GetValue<bool>( TabularViewOpenedProperty ); }
      set { SetValue( TabularViewOpenedProperty, value ); }
    }

    /// <summary>
    /// Register the TabularViewOpened property so it is known in the class.
    /// </summary>
    public static readonly PropertyData TabularViewOpenedProperty = RegisterProperty( "TabularViewOpened", typeof( bool ), false );

    /// <summary>
        /// Gets or sets DoubanViewOpend.
        /// </summary>
    public bool DoubanViewOpened
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

    /// <summary>
    /// Gets or sets FellowPlusViewOpened.
    /// </summary>
    public bool FellowPlusViewOpened
    {
      get { return GetValue<bool>( FellowPlusViewOpenedProperty ); }
      set { SetValue( FellowPlusViewOpenedProperty, value ); }
    }

    /// <summary>
    /// Register the FellowPlusViewOpened property so it is known in the class.
    /// </summary>
    public static readonly PropertyData FellowPlusViewOpenedProperty = RegisterProperty( "FellowPlusViewOpened", typeof( bool ), false );

    public MainViewModel() {
      CmdHtml = new Command( OnCmdHtmlExecute, OnCmdHtmlCanExecute );
      CmdHtmlTable = new Command( OnCmdHtmlTableExecute, OnCmdHtmlTableCanExecute );
      CmdCSV = new Command( OnCmdCSVExecute, OnCmdCSVCanExecute );

      CmdDouban = new Command( OnCmdDoubanExecute, OnCmdDoubanCanExecute );
      CmdFellowPlus = new Command( OnCmdFellowPlusExecute, OnCmdFellowPlusCanExecute );
    }


    #region Commands

    /// <summary>
    /// Gets the CmdHtml command.
    /// </summary>
    public Command CmdHtml { get; private set; }
    
    /// <summary>
    /// Method to check whether the CmdHtml command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdHtmlCanExecute() {
      return !HtmlViewOpened;
    }

    /// <summary>
    /// Method to invoke when the CmdHtml command is executed.
    /// </summary>
    private void OnCmdHtmlExecute() {
      HtmlViewOpened = true;

      //DoubanViewModel viewmodel = new DoubanViewModel();
      Views.HtmlView view = new Views.HtmlView();
      //view.DataContext = viewmodel;
      view.Closed += HtmlView_Closed;
      view.Show();
    }

    private void HtmlView_Closed( object sender, EventArgs e ) {
      HtmlViewOpened = false;
    }

    /// <summary>
    /// Gets the CmdHtmlTable command.
    /// </summary>
    public Command CmdHtmlTable { get; private set; }
    
    /// <summary>
    /// Method to check whether the CmdHtmlTable command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdHtmlTableCanExecute() {
      return !HtmlTableViewOpened;
    }

    /// <summary>
    /// Method to invoke when the CmdHtmlTable command is executed.
    /// </summary>
    private void OnCmdHtmlTableExecute() {
      HtmlTableViewOpened = true;

      HtmlTableViewModel viewmodel = new HtmlTableViewModel();
      Views.HtmlTableView view = new Views.HtmlTableView();
      view.DataContext = viewmodel;
      view.Closed += HtmlTableView_Closed;
      view.Show();
    }

    private void HtmlTableView_Closed( object sender, EventArgs e ) {
      HtmlTableViewOpened = false;
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
      return !DoubanViewOpened;
    }

    /// <summary>
    /// Method to invoke when the CmdDouban command is executed.
    /// </summary>
    private void OnCmdDoubanExecute() {
      DoubanViewOpened = true;

      DoubanViewModel viewmodel = new DoubanViewModel();
      Views.DoubanView view = new Views.DoubanView();
      view.DataContext = viewmodel;
      view.Closed += DoubanView_Closed;
      view.Show();
    }

    /// <summary>
        /// Gets the CmdFellowPlus command.
        /// </summary>
    public Command CmdFellowPlus { get; private set; }

    /// <summary>
    /// Method to check whether the CmdFellowPlus command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdFellowPlusCanExecute() {
      return !FellowPlusViewOpened;
    }

    /// <summary>
    /// Method to invoke when the CmdFellowPlus command is executed.
    /// </summary>
    private void OnCmdFellowPlusExecute() {
      FellowPlusViewOpened = true;

      FellowPlusViewModel viewmodel = new FellowPlusViewModel();
      Views.FellowPlusView view = new Views.FellowPlusView();
      view.DataContext = viewmodel;
      view.Closed += FellowPlusView_Closed;
      view.Show();
    }

    /// <summary>
    /// Gets the CmdCSV command.
    /// </summary>
    public Command CmdCSV { get; private set; }

    /// <summary>
    /// Method to check whether the CmdCSV command can be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
    private bool OnCmdCSVCanExecute() {
      return !TabularViewOpened;
    }

    /// <summary>
    /// Method to invoke when the CmdCSV command is executed.
    /// </summary>
    private void OnCmdCSVExecute() {
      DoubanViewOpened = true;

      TabularViewModel viewmodel = new TabularViewModel();
      Views.TabularView view = new Views.TabularView();
      view.DataContext = viewmodel;
      view.Closed += TabularView_Closed;
      view.Show();
    }

    private void TabularView_Closed( object sender, EventArgs e ) {
      DoubanViewOpened = false;
    }

    private void DoubanView_Closed( object sender, EventArgs e ) {
      DoubanViewOpened = false;
    }

    private void FellowPlusView_Closed( object sender, EventArgs e ) {
      FellowPlusViewOpened = false;
    }

    #endregion Commands


    #region Methods



    #endregion Methods

  }

}
