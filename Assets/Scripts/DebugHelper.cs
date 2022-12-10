using System;
using UnityEngine;

//public static class DebugHelper
//{
//    public static void DebugLog()
//    {
//        //安卓平台
//#if UNITY_ANDROID
//            debug.log("Android");
//#endif

//#if !UNITY_EDITOR
//    return;
//#endif

//        //苹果平台
//#if UNITY_IPHONE
//             debug.log("IOS");
//#endif
//        //Windows平台
//#if UNITY_STANDALONE_WIN
//        Debug.Log("Windows");
//        #endif

//    }
//}

/// <summary>
/// 封装 Debug 调试，通过宏定义来控制是否开启 Debug 的相关打印
/// </summary>
public sealed class DebugHelper
{
    /// <summary>
    /// Debug.Log 的封装
    /// 定义了 LOG 宏，才开启打印
    /// </summary>
    /// <param name="message">打印的内容</param>
    /// <param name="context">上下文</param>
    public static void Log(object message, UnityEngine.Object context = null)
    {
#if LOG     // 定义LOG宏，才走该处
        // 添加 时间和对应的内容 Debug 打印
        Debug.Log(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")+"  " +message, context);
#endif

    }


    /// <summary>
    /// Debug.LogWarning 的封装
    /// 定义了 LOGWARNING 宏，才开启打印
    /// </summary>
    /// <param name="message">打印的内容</param>
    /// <param name="context">上下文</param>
    public static void LogWarning(object message, UnityEngine.Object context = null)
    {
#if LOGWARNING      // LOGWARNING，才走该处
        // 添加 时间和对应的内容 Debug 打印
        Debug.LogWarning(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "  " + message, context);
#endif
    }


    /// <summary>
    /// Debug.LogError 的封装
    /// 定义了 LOGERROR 宏，才开启打印
    /// </summary>
    /// <param name="message">打印的内容</param>
    /// <param name="context">上下文</param>
    public static void LogError(object message, UnityEngine.Object context = null)
    {
#if UNITY_ANDROID
        return;
#elif UNITY_IPHONE
        return;
#elif UNITY_STANDALONE_WIN
        Debug.LogError(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "  " + message, context);
#endif
//#if LOGERROR        // LOGERROR，才走该处
//        // 添加 时间和对应的内容 Debug 打印

//#endif
    }

}