using UnityEngine;

namespace LittelSword.Common
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // �̱��� �ν��Ͻ�
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    // ������ TŸ���� ������Ʈ�� ã�Ƽ� �Ҵ�
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

