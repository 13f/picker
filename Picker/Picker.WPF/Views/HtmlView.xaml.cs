using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HtmlAgilityPack;
using Picker.Core.Extensions;

namespace Picker.Views {
  /// <summary>
  /// Interaction logic for HtmlView.xaml
  /// </summary>
  public partial class HtmlView : MahApps.Metro.Controls.MetroWindow {
    public HtmlView() {
      InitializeComponent();
    }

    private void tbGo_Click( object sender, RoutedEventArgs e ) {
      try {
        webBrowser.Navigate( tbUri.Text.Trim() );
      }
      catch(Exception ex ) {
        MessageBox.Show( ex.Message, "Error" );
      }
    }

    private void webBrowser_DocumentCompleted( object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e ) {
      tbClearHtml.Visibility = Visibility.Visible;

      HtmlDocument doc = new HtmlDocument();
      try {
        doc.LoadHtml( webBrowser.DocumentText );
        doc.RemoveAttributes();
        doc.RemoveComments();
        doc.RemoveScripts();
        doc.RemoveStyles();
        tbHtml.Text = doc.GetHtmlString().RemoveNbsp().RemoveBlankLines();
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message, "Error" );
      }
    }

    private void tbClearHtml_Click( object sender, RoutedEventArgs e ) {
      HtmlDocument doc = new HtmlDocument();
      try {
        doc.LoadHtml( webBrowser.DocumentText );
        doc.RemoveAttributes();
        doc.RemoveComments();
        doc.RemoveScripts();
        doc.RemoveStyles();
        tbHtml.Text = doc.GetHtmlString().RemoveNbsp().RemoveBlankLines();
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message, "Error" );
      }
    }

  }

}
