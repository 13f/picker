﻿using System;
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

namespace Picker.Views {
  /// <summary>
  /// DoubanView.xaml 的交互逻辑
  /// </summary>
  public partial class DoubanView : MahApps.Metro.Controls.MetroWindow {
    public DoubanView() {
      InitializeComponent();
      this.Loaded += DoubanView_Loaded;
    }

    void DoubanView_Loaded( object sender, RoutedEventArgs e ) {
      var vm = this.DataContext as ViewModels.DoubanViewModel;
      if ( vm == null )
        return;
      vm.RefreshBrowser = () => {
        webBrowser.Url = new Uri( vm.PageUri );
      };
    }

    //private void btnGo_Click( object sender, RoutedEventArgs e ) {
    //  var vm = this.DataContext as ViewModels.DoubanViewModel;
    //  if ( vm == null )
    //    return;
    //  vm.HtmlDownloaded = false;
    //  vm.CurrentHtml = null;
    //  webBrowser.Navigate( vm.SeriePage );
    //}

    private void webBrowser_DocumentCompleted( object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e ) {
      var vm = this.DataContext as ViewModels.DoubanViewModel;
      if ( vm == null )
        return;
      vm.HtmlDownloaded = true;
      vm.CurrentHtml = webBrowser.DocumentText;
    }

  }

}
