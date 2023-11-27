using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace DesignPatterns.ObjectPool {
    public class PooledObject<T> : MonoBehaviour where T : Component {
        /// <summary> プールから取得する処理(プール初期化時に追加される) </summary>
        public event System.Func<T> GetFromPoolEvent;

        /// <summary> プールに戻す処理(プール初期化時に追加される) </summary>
        public event System.Action ReleaseToPoolEvent;

        [Header("PooledObject Parameters")]
        [SerializeField,Tooltip("生成時に取得されたときの処理を実行するか")]
        bool doGetActionOnCreated;

        //--------------------------------------------------
        /// <summary> 作成されるときの処理 </summary>
        public virtual void OnCreated()
        {
            // 取得処理
            if (doGetActionOnCreated) {
                OnGetFromPool();
            }
        }

        /// <summary> プールから取得されたときの処理 </summary>
        public virtual void OnGetFromPool()
        {
            gameObject.SetActive(true);
        }

        /// <summary> プールに戻されるときの処理 </summary>
        public virtual void OnReleasedToPool()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnDestroyed()
        {
            Destroy(gameObject);
        }

        //--------------------------------------------------
        /// <summary> 複製 </summary>
        public virtual T Duplicate()
        {
            var obj = GetFromPoolEvent?.Invoke();

            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.transform.localScale = transform.localScale;

            return obj;
        }

        /// <summary> 自身をプールに戻す </summary>
        public void Release()
        {
            ReleaseToPoolEvent?.Invoke();
        }
    }
}