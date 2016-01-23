using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Catel.Data;
using Catel.MVVM;
using Newtonsoft.Json.Linq;

namespace Picker.ViewModels {
  public class HtmlTableViewModel : ViewModelBase {

    /// <summary>
    /// Gets or sets TableXml.
    /// </summary>
    public string TableXml
    {
      get { return GetValue<string>( TableXmlProperty ); }
      set { SetValue( TableXmlProperty, value ); }
    }

    /// <summary>
    /// Register the TableXml property so it is known in the class.
    /// </summary>
    public static readonly PropertyData TableXmlProperty = RegisterProperty( "TableXml", typeof( string ), null );

    /// <summary>
    /// Gets or sets Json.
    /// </summary>
    public string Json
    {
      get { return GetValue<string>( JsonProperty ); }
      set { SetValue( JsonProperty, value ); }
    }

    /// <summary>
    /// Register the Json property so it is known in the class.
    /// </summary>
    public static readonly PropertyData JsonProperty = RegisterProperty( "Json", typeof( string ), null );

    #region replace

    /// <summary>
    /// Gets or sets XmlOldString.
    /// </summary>
    public string XmlOldString
    {
      get { return GetValue<string>( XmlOldStringProperty ); }
      set { SetValue( XmlOldStringProperty, value ); }
    }

    /// <summary>
    /// Register the XmlOldString property so it is known in the class.
    /// </summary>
    public static readonly PropertyData XmlOldStringProperty = RegisterProperty( "XmlOldString", typeof( string ), null );

    /// <summary>
    /// Gets or sets XmlNewString.
    /// </summary>
    public string XmlNewString
    {
      get { return GetValue<string>( XmlNewStringProperty ); }
      set { SetValue( XmlNewStringProperty, value ); }
    }

    /// <summary>
    /// Register the XmlNewString property so it is known in the class.
    /// </summary>
    public static readonly PropertyData XmlNewStringProperty = RegisterProperty( "XmlNewString", typeof( string ), null );

    #endregion replace


    public HtmlTableViewModel() {
      CmdFormatXml = new Command( OnCmdFormatXmlExecute );
      CmdFormatJson = new Command( OnCmdFormatJsonExecute );

      CmdReplaceXml = new Command( OnCmdReplaceXmlExecute );

      CmdProcess = new Command( OnCmdProcessExecute );
    }

    #region Commands

    #region Xml

    /// <summary>
    /// Gets the CmdFormatXml command.
    /// </summary>
    public Command CmdFormatXml { get; private set; }

    /// <summary>
    /// Method to invoke when the CmdFormatXml command is executed.
    /// </summary>
    private void OnCmdFormatXmlExecute() {
      if ( string.IsNullOrWhiteSpace( TableXml ) )
        return;
      try {
        XElement xml = XElement.Parse( TableXml );
        TableXml = xml.ToString( SaveOptions.None );
      }
      catch { }
    }

    /// <summary>
    /// Gets the CmdReplaceXml command.
    /// </summary>
    public Command CmdReplaceXml { get; private set; }

    /// <summary>
    /// Method to invoke when the CmdReplaceXml command is executed.
    /// </summary>
    private void OnCmdReplaceXmlExecute() {
      if ( string.IsNullOrWhiteSpace( TableXml ) )
        return;
      TableXml = TableXml.Replace( XmlOldString, XmlNewString );
    }

    #endregion Xml


    #region Json

    /// <summary>
    /// Gets the CmdFormatJson command.
    /// </summary>
    public Command CmdFormatJson { get; private set; }
    
    /// <summary>
    /// Method to invoke when the CmdFormatJson command is executed.
    /// </summary>
    private void OnCmdFormatJsonExecute() {
      if ( string.IsNullOrWhiteSpace( Json ) )
        return;
      try {
        JToken token = JToken.Parse( Json );
        Json = token.ToString( Newtonsoft.Json.Formatting.Indented );
      }
      catch { }
    }

    #endregion Json

    /// <summary>
    /// Gets the CmdProcess command.
    /// </summary>
    public Command CmdProcess { get; private set; }
    
    /// <summary>
    /// Method to invoke when the CmdProcess command is executed.
    /// </summary>
    private void OnCmdProcessExecute() {
      
    }

    #endregion Commands


    #region Methods



    #endregion Methods


  }

}
