using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picker.Core.Helpers {
  public static class TimeHelper {
    /// <summary>
    /// 判断是否在某一时间段内：
    /// 1. lastTime == null -- 未发生过，返回true；
    /// 2. interval == null -- 未设置interval，返回true；
    /// 3. now - lastTime 是否小于 interval。
    /// </summary>
    /// <param name="lastTime"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static bool IsInInterval( DateTime? lastTime, TimeSpan? interval ) {
      //return ( lastTime == null ) ||
      //  ( interval == null && lastTime != null ) ||
      //  ( interval.HasValue && lastTime.HasValue && ( DateTime.UtcNow - lastTime.Value < interval.Value ) );
      return lastTime == null ||
        interval == null ||
        ( DateTime.UtcNow - lastTime.Value < interval.Value );
    }

    /// <summary>
    /// 判断是否在某一时间段内：
    /// 1. lastTime == null -- 未发生过，返回true；
    /// 2. interval == null -- 未设置interval，返回true；
    /// 3. now - lastTime 是否小于 interval。
    /// </summary>
    /// <param name="lastTime"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static bool IsInInterval( DateTimeOffset? lastTime, TimeSpan? interval ) {
      return lastTime == null ||
        interval == null ||
        ( DateTime.UtcNow - lastTime.Value.UtcDateTime < interval.Value );
    }

    /// <summary>
    /// 判断是否在某一时间段之外：
    /// 1. lastTime == null -- 未发生过，返回true；
    /// 2. now - lastTime 是否大于等于 interval
    /// </summary>
    /// <param name="lastTime"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static bool IsOutOfInterval( DateTime? lastTime, TimeSpan? interval ) {
      return lastTime == null ||
        ( interval.HasValue && ( DateTime.UtcNow - lastTime.Value < interval.Value ) );
    }

    /// <summary>
    /// 判断是否在某一时间段之外：
    /// 1. lastTime == null -- 未发生过，返回true；
    /// 2. now - lastTime 是否大于等于 interval
    /// </summary>
    /// <param name="lastTime"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static bool IsOutOfInterval( DateTimeOffset? lastTime, TimeSpan? interval ) {
      return lastTime == null ||
        ( interval.HasValue && ( DateTime.UtcNow - lastTime.Value.UtcDateTime < interval.Value ) );
    }

  }

}
