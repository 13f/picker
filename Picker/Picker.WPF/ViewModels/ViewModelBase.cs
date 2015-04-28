using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Picker.Core.Storage;

namespace Picker.ViewModels {
  public class ViewModelBase : Catel.MVVM.ViewModelBase {
    protected static Configuration config;
    public ViewModelBase() {
      if ( config == null ) {
        string executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        string executableFolder = System.IO.Path.GetDirectoryName( executablePath );
        config = new Configuration( executableFolder );
      }
    }

  }

}
