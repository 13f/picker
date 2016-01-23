using System.Windows;

namespace Picker.Views {
  /// <summary>
  /// MainWindow.xaml 的交互逻辑
  /// </summary>
  public partial class MainView : MahApps.Metro.Controls.MetroWindow {
    public MainView() {
      InitializeComponent();
      this.Loaded += MainView_Loaded;
    }

    void MainView_Loaded( object sender, RoutedEventArgs e ) {
    }
    
  }

}
