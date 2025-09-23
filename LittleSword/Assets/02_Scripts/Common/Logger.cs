
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace LittelSword.Common
{
    public static class Logger
    {
        [Conditional("DEVELOP_MODE")]
        [Conditional("UNITY_EDITOR")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        [Conditional("DEVELOP_MODE")]
        [Conditional("UNITY_EDITOR")]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        [Conditional("DEVELOP_MODE")]
        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }
    }

}
