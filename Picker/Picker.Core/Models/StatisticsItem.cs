using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picker.Core.Models {
  public class StatisticsItem {
    /// <summary>
    /// can search by key
    /// </summary>
    public string Key { get; set; }

    public string Title { get; set; }
    public long Count { get; set; }
    /// <summary>
    /// 条目总数
    /// </summary>
    public long TaskCount { get; set; }
    /// <summary>
    /// 已处理的条目数
    /// </summary>
    public long ProcessedCount { get; set; }

  }

}
