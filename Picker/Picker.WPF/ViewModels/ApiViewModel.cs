using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Picker.Core.Spider;
using Picker.Core.Storage;

namespace Picker.ViewModels {
  public class ApiViewModel : Picker.ViewModels.ViewModelBase {
    

    /// <summary>
    /// 上一次访问的API地址
    /// </summary>
    protected string lastApi = null;
    /// <summary>
    /// 上一次查询的页面索引
    /// </summary>
    protected int? lastPageIndex = null;

    //public ApiViewModel() {
    //}

  }

}
