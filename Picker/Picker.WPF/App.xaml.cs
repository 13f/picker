using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Picker {
  /// <summary>
  /// App.xaml 的交互逻辑
  /// </summary>
  public partial class App : Application {
    protected override void OnStartup( StartupEventArgs e ) {
      ViewModels.MainViewModel viewmodel = new ViewModels.MainViewModel();
      Views.MainView view = new Views.MainView();
      view.DataContext = viewmodel;
      view.Show();
    }

  }

}
