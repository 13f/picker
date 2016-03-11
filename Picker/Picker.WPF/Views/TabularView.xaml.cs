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
using Picker.ViewModels;

namespace Picker.Views {
  /// <summary>
  /// Interaction logic for TabularView.xaml
  /// </summary>
  public partial class TabularView : MahApps.Metro.Controls.MetroWindow {
    public TabularView() {
      InitializeComponent();
    }

    private void btnProcess_Click( object sender, RoutedEventArgs e ) {
      if(tbIds.LineCount != tbIdsTitles.LineCount) {
        MessageBox.Show( "Ids.length != IdsTitles.length", "Error" );
        return;
      }

      TabularViewModel vm = this.DataContext as TabularViewModel;
      vm.IdsCount = tbIds.LineCount;

      vm.Ids.Clear();
      vm.IdsTitles.Clear();

      for ( int i = 0; i < vm.IdsCount; i++ ) {
        string id = tbIds.GetLineText( i );
        vm.Ids.Add( id.Replace( "\r\n", "" ) );

        string title = tbIdsTitles.GetLineText( i );
        vm.IdsTitles.Add( title.Replace( "\r\n", "" ) );
      }

      vm.Process();
    }

  }

}
