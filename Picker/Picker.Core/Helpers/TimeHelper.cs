using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picker.Core.Helpers {
  public static class TimeHelper {
    ///// <summary>
    ///// 判断是否在某一时间段内：
    ///// 1. lastTime == null -- 未发生过，返回true；
    ///// 2. interval == null -- 未设置interval，返回true；
    ///// 3. now - lastTime 是否小于 interval。
    ///// </summary>
    ///// <param name="lastTime"></param>
    ///// <param name="interval"></param>
    ///// <returns></returns>
    //public static bool IsInInterval( DateTime? lastTime, TimeSpan? interval ) {
    //  //return ( lastTime == null ) ||
    //  //  ( interval == null && lastTime != null ) ||
    //  //  ( interval.HasValue && lastTime.HasValue && ( DateTime.UtcNow - lastTime.Value < interval.Value ) );
    //  return lastTime == null ||
    //    interval == null ||
    //    ( DateTime.UtcNow - lastTime.Value.ToUniversalTime() < interval.Value );
    //}



    ///// <summary>
    ///// 判断是否在某一时间段内：
    ///// 1. lastTime == null -- 未发生过，返回true；
    ///// 2. interval == null -- 未设置interval，返回true；
    ///// 3. now - lastTime 是否小于 interval。
    ///// </summary>
    ///// <param name="lastTime"></param>
    ///// <param name="interval"></param>
    ///// <returns></returns>
    //public static bool IsInInterval( DateTimeOffset? lastTime, TimeSpan? interval ) {
    //  return lastTime == null ||
    //    interval == null ||
    //    ( DateTime.UtcNow - lastTime.Value.UtcDateTime < interval.Value );
    //}


    /// <summary>
    /// 判断一个任务是否已经完成
    /// </summary>
    /// <param name="lastTime"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static bool IsCompleted( DateTime? lastTime, TimeSpan? interval ) {
      if ( lastTime == null )
        return false;
      else if ( interval == null )
        return true;
      // both is not null
      return ( DateTime.UtcNow - lastTime.Value.ToUniversalTime() < interval.Value );
    }

    /// <summary>
    /// 判断一个任务是否已经完成
    /// </summary>
    /// <param name="lastTime"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static bool IsCompleted( DateTimeOffset? lastTime, TimeSpan? interval ) {
      if ( lastTime == null )
        return false;
      else if ( interval == null )
        return true;
      // both is not null
      return ( DateTime.UtcNow - lastTime.Value.UtcDateTime < interval.Value );
    }


    ///// <summary>
    ///// 判断是否在某一时间段之外：
    ///// 1. lastTime == null -- 未发生过，返回true；
    ///// 2. now - lastTime 是否大于等于 interval
    ///// </summary>
    ///// <param name="lastTime"></param>
    ///// <param name="interval"></param>
    ///// <returns></returns>
    //public static bool IsOutOfInterval( DateTime? lastTime, TimeSpan? interval ) {
    //  return lastTime == null ||
    //    ( interval.HasValue && ( DateTime.UtcNow - lastTime.Value < interval.Value ) );
    //}

    ///// <summary>
    ///// 判断是否在某一时间段之外：
    ///// 1. lastTime == null -- 未发生过，返回true；
    ///// 2. now - lastTime 是否大于等于 interval
    ///// </summary>
    ///// <param name="lastTime"></param>
    ///// <param name="interval"></param>
    ///// <returns></returns>
    //public static bool IsOutOfInterval( DateTimeOffset? lastTime, TimeSpan? interval ) {
    //  return lastTime == null ||
    //    ( interval.HasValue && ( DateTime.UtcNow - lastTime.Value.UtcDateTime < interval.Value ) );
    //}


    public static DateTime? GetDateTime(string data ) {
      if ( string.IsNullOrWhiteSpace( data ) )
        return null;
      DateTime dtTmp = DateTime.MinValue;
      try {
        dtTmp = DateTime.Parse( data );
        return dtTmp;
      }
      catch {
        var dt = GetDateTimeFromChinese( data );
        return dt;
      }
    }

    public static DateTime? GetDateTimeFromChinese( string data ) {
      try {
        int start = data.IndexOf( " " );
        data = data.Substring( 0, start );

        int days = 0;
        int hours = 0;
        int minutes = 0;
        DateTime dtTmp = DateTime.MinValue;
        // 6天前
        start = data.IndexOf( "天" );
        if ( start > 0 ) {
          string strDays = data.Substring( 0, start );
          days = Int32.Parse( strDays );
          dtTmp = DateTime.Now.AddDays( days * -1 );
          return dtTmp.ToUniversalTime();
        }
        // 23时13分钟前
        // hour
        string keyHour = "小时";
        start = data.IndexOf( keyHour );
        if ( start < 0 ) {
          keyHour = "时";
          start = data.IndexOf( keyHour );
        }
        if ( start > 0 ) {
          string strHours = data.Substring( 0, start );
          if ( !string.IsNullOrWhiteSpace( strHours ) )
            hours = Int32.Parse( strHours );
        }

        // minute
        if ( start >= 0 )
          data = data.Substring( start + keyHour.Length );

        string keyMinute = "分钟";
        start = data.IndexOf( keyMinute );
        if ( start < 0 ) {
          keyMinute = "分";
          start = data.IndexOf( keyMinute );
        }
        if ( start > 0 ) {
          string strMinutes = data.Substring( 0, start );
          if ( !string.IsNullOrWhiteSpace( strMinutes ) )
            minutes = Int32.Parse( strMinutes );
        }
        dtTmp = DateTime.Now.AddHours( hours * -1 ).AddMinutes( minutes * -1 );
        return dtTmp.ToUniversalTime();
      }
      catch {
        return null;
      }
    }

  }

}
