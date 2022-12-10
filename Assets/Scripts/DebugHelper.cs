using System;
using UnityEngine;

//public static class DebugHelper
//{
//    public static void DebugLog()
//    {
//        //��׿ƽ̨
//#if UNITY_ANDROID
//            debug.log("Android");
//#endif

//#if !UNITY_EDITOR
//    return;
//#endif

//        //ƻ��ƽ̨
//#if UNITY_IPHONE
//             debug.log("IOS");
//#endif
//        //Windowsƽ̨
//#if UNITY_STANDALONE_WIN
//        Debug.Log("Windows");
//        #endif

//    }
//}

/// <summary>
/// ��װ Debug ���ԣ�ͨ���궨���������Ƿ��� Debug ����ش�ӡ
/// </summary>
public sealed class DebugHelper
{
    /// <summary>
    /// Debug.Log �ķ�װ
    /// ������ LOG �꣬�ſ�����ӡ
    /// </summary>
    /// <param name="message">��ӡ������</param>
    /// <param name="context">������</param>
    public static void Log(object message, UnityEngine.Object context = null)
    {
#if LOG     // ����LOG�꣬���߸ô�
        // ��� ʱ��Ͷ�Ӧ������ Debug ��ӡ
        Debug.Log(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")+"  " +message, context);
#endif

    }


    /// <summary>
    /// Debug.LogWarning �ķ�װ
    /// ������ LOGWARNING �꣬�ſ�����ӡ
    /// </summary>
    /// <param name="message">��ӡ������</param>
    /// <param name="context">������</param>
    public static void LogWarning(object message, UnityEngine.Object context = null)
    {
#if LOGWARNING      // LOGWARNING�����߸ô�
        // ��� ʱ��Ͷ�Ӧ������ Debug ��ӡ
        Debug.LogWarning(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "  " + message, context);
#endif
    }


    /// <summary>
    /// Debug.LogError �ķ�װ
    /// ������ LOGERROR �꣬�ſ�����ӡ
    /// </summary>
    /// <param name="message">��ӡ������</param>
    /// <param name="context">������</param>
    public static void LogError(object message, UnityEngine.Object context = null)
    {
#if UNITY_ANDROID
        return;
#elif UNITY_IPHONE
        return;
#elif UNITY_STANDALONE_WIN
        Debug.LogError(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "  " + message, context);
#endif
//#if LOGERROR        // LOGERROR�����߸ô�
//        // ��� ʱ��Ͷ�Ӧ������ Debug ��ӡ

//#endif
    }

}