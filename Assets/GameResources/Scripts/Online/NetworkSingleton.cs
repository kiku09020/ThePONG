using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Network {
    public class NetworkSingleton<T> : NetworkBehaviour where T : Component {
        static T instance;

        public static T Instance {
            get {
                if (!instance) {
                    // 既存のインスタンス検索
                    instance = FindFirstObjectByType<T>();

                    // 無ければ新規作成
                    if (!instance) {
                        SetupInstance();
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            RemoveDuplicates();
        }

        // 新規作成
        static void SetupInstance()
        {
            var obj = new GameObject();
            obj.name = typeof(T).Name;

            instance = obj.AddComponent<T>();
            DontDestroyOnLoad(obj);
        }

        // インスタンスの重複削除
        void RemoveDuplicates()
        {
            // シーン上に無ければ新規作成
            if (!instance) {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }

            // 既にあれば自身を削除
            else {
                Debug.LogError($"*{gameObject} was destroyed.");
                Destroy(gameObject);
            }
        }
    }
}