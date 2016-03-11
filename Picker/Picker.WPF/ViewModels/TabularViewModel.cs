using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Catel.Data;
using Catel.MVVM;
using Newtonsoft.Json.Linq;
using KBCsv;
using System.IO;

namespace Picker.ViewModels {
  public class TabularViewModel : ViewModelBase {
    string filePath = null;

    /// <summary>
    /// Gets or sets SourceString.
    /// </summary>
    public string SourceString
    {
      get { return GetValue<string>( SourceStringProperty ); }
      set { SetValue( SourceStringProperty, value ); }
    }

    /// <summary>
    /// Register the SourceString property so it is known in the class.
    /// </summary>
    public static readonly PropertyData SourceStringProperty = RegisterProperty( "SourceString", typeof( string ), null );

    /// <summary>
    /// Gets or sets IdsString.
    /// </summary>
    public string IdsString
    {
      get { return GetValue<string>( IdsStringProperty ); }
      set { SetValue( IdsStringProperty, value ); }
    }

    /// <summary>
    /// Register the IdsString property so it is known in the class.
    /// </summary>
    public static readonly PropertyData IdsStringProperty = RegisterProperty( "IdsString", typeof( string ), null );

    /// <summary>
        /// Gets or sets IdsCount.
        /// </summary>
    public int IdsCount
    {
      get { return GetValue<int>( IdsCountProperty ); }
      set { SetValue( IdsCountProperty, value ); }
    }

    /// <summary>
    /// Register the IdsCount property so it is known in the class.
    /// </summary>
    public static readonly PropertyData IdsCountProperty = RegisterProperty( "IdsCount", typeof( int ), 0 );

    /// <summary>
    /// Gets or sets Ids.
    /// </summary>
    public List<string> Ids
    {
      get { return GetValue<List<string>>( IdsProperty ); }
      set { SetValue( IdsProperty, value ); }
    }

    /// <summary>
    /// Register the Ids property so it is known in the class.
    /// </summary>
    public static readonly PropertyData IdsProperty = RegisterProperty( "Ids", typeof( List<string> ), null );

    /// <summary>
    /// Gets or sets IdsTitlesString.
    /// </summary>
    public string IdsTitlesString
    {
      get { return GetValue<string>( IdsTitlesStringProperty ); }
      set { SetValue( IdsTitlesStringProperty, value ); }
    }

    /// <summary>
    /// Register the IdsTitlesString property so it is known in the class.
    /// </summary>
    public static readonly PropertyData IdsTitlesStringProperty = RegisterProperty( "IdsTitlesString", typeof( string ), null );

    /// <summary>
    /// Gets or sets IdsTitles.
    /// </summary>
    public List<string> IdsTitles
    {
      get { return GetValue<List<string>>( IdsTitlesProperty ); }
      set { SetValue( IdsTitlesProperty, value ); }
    }

    /// <summary>
    /// Register the IdsTitles property so it is known in the class.
    /// </summary>
    public static readonly PropertyData IdsTitlesProperty = RegisterProperty( "IdsTitles", typeof( List<string> ), null );

    /// <summary>
        /// Gets or sets ReadCsvHeader.
        /// </summary>
    public bool ReadCsvHeader
    {
      get { return GetValue<bool>( ReadCsvHeaderProperty ); }
      set { SetValue( ReadCsvHeaderProperty, value ); }
    }

    /// <summary>
    /// Register the ReadCsvHeader property so it is known in the class.
    /// </summary>
    public static readonly PropertyData ReadCsvHeaderProperty = RegisterProperty( "ReadCsvHeader", typeof( bool ), false );

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
    

    public TabularViewModel() {
      Ids = new List<string>();
      IdsTitles = new List<string>();

      CmdOpenFile = new Command( OnCmdOpenFileExecute );
      CmdSave = new Command( OnCmdSaveExecute );
    }

    #region Commands

    /// <summary>
    /// Gets the CmdOpenFile command.
    /// </summary>
    public Command CmdOpenFile { get; private set; }
    
    /// <summary>
    /// Method to invoke when the CmdOpenFile command is executed.
    /// </summary>
    private void OnCmdOpenFileExecute() {
      System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
      ofd.Multiselect = false;
      ofd.Filter = "CSV|*.csv|All|*.*";

      var r = ofd.ShowDialog();
      if(r == System.Windows.Forms.DialogResult.OK ) {
        filePath = ofd.FileName;
        SourceString = System.IO.File.ReadAllText( filePath );
      }
    }
    
    /// <summary>
    /// Gets the CmdProcess command.
    /// </summary>
    public Command CmdSave { get; private set; }
    
    /// <summary>
    /// Method to invoke when the CmdProcess command is executed.
    /// </summary>
    private void OnCmdSaveExecute() {
      // save config
      System.IO.File.WriteAllText( filePath + ".config",
        "==== Id(s) list ====" + Environment.NewLine + IdsString + Environment.NewLine + Environment.NewLine + "==== Id(s) Titles list ====" + Environment.NewLine + IdsTitlesString,
        System.Text.Encoding.UTF8 );
      // save result
      System.IO.File.WriteAllText( filePath + ".json", Json, System.Text.Encoding.UTF8 );
    }

    public void Process() {
      JObject joRoot = new JObject();
      joRoot["title"] = "";
      joRoot["description"] = "";
      // ids
      JArray jaIds = new JArray();
      for( int i = 0; i < IdsCount; i++ ) {
        JObject jo = new JObject();
        jo["id"] = Ids[i];
        jo["title"] = IdsTitles[i];
        jaIds.Add( jo );
      }
      joRoot["config"] = jaIds;

      // content
      JArray jaItems = new JArray();
      using ( var reader = CsvReader.FromCsvString( SourceString ) ) {
        var tmp = reader.ToEnumerable( readHeader: ReadCsvHeader );
        //var results =
        //    from record in tmp
        //    select record["Name"] + " is " + record["Age"] + " years olds.";
        while ( reader.HasMoreRecords ) {
          var dataRecord = reader.ReadDataRecord();
          JObject item = new JObject();
          //Console.WriteLine( "{0} is {1} years old.", dataRecord[0], dataRecord[1] );
          for (int i = 0; i< IdsCount; i++ ) {
            string id = Ids[i];
            item[id] = dataRecord[i];
          }
          jaItems.Add( item );
        }
      }
      joRoot["items"] = jaItems;

      // to UI
      Json = joRoot.ToString( Newtonsoft.Json.Formatting.Indented );
    }

    #endregion Commands


    #region Methods



    #endregion Methods


  }

}
