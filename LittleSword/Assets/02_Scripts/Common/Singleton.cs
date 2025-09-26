using UnityEngine;

namespace LittelSword.Common
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // 싱글턴 인스턴스
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    // 씬에서 T타입의 컴포넌트를 찾아서 할당
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        GameObject singleObject = new GameObject(typeof(T).Name);
                        instance = singleObject.AddComponent<T>();
                        DontDestroyOnLoad(singleObject);
                    }
                }
                return instance;
            }
        }

        protected void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

    }
}

