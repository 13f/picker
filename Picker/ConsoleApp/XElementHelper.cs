using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp {
  public static class XElementHelper {
    /// <summary>
    /// Create a XElement: (item key="" value="" /)
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static XElement CreateItemElement( string key, string value ) {
      XElement item = new XElement( "item",
          new XAttribute( "key", key ),
          new XAttribute( "value", value )
          );
      return item;
    }

    public static void XmlToJson( string xmlFile, string jsonFile, bool omitRootObject ) {
      //string xml = System.IO.File.ReadAllText( xmlFile, System.Text.Encoding.UTF8 );
      System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
      xmlDoc.Load( xmlFile );
      string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode( xmlDoc, Newtonsoft.Json.Formatting.Indented, omitRootObject );
      System.IO.File.WriteAllText( jsonFile, json, System.Text.Encoding.UTF8 );
    }

  }

}
