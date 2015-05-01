using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picker.Core.Models {
  public class StatisticsItem {
    public string Title { get; set; }
    /// <summary>
    /// 条目总数
    /// </summary>
    public long TotalCount { get; set; }
    /// <summary>
    /// 已处理的条目数
    /// </summary>
    public long ProcessedCount { get; set; }

  }

}
