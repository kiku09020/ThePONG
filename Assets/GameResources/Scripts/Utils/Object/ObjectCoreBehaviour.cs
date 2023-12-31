using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtils
{
    public class ObjectCoreBehaviour<T> : MonoBehaviour where T : ObjectCore<T>
    {
        /* Fields */
        [Header("Component Settings")]
        [SerializeField] int priority = 0;

        protected T core;

        //--------------------------------------------------
        /* Properties */
        public int Priority => priority;

        //--------------------------------------------------
        /* Messages */
        protected virtual void Awake()
        {
            // GetComponentで取得できない場合は、親から取得する
            if(!TryGetComponent(out core)) {
                core = GetComponentInParent<T>();
            }

            core.OnStartEvent += OnStart;
            core.OnUpdateEvent += OnUpdate;
            core.OnFixedUpdateEvent += OnFixedUpdate;
            core.OnDestroyedEvent += OnDestroyed;
        }

        //--------------------------------------------------
        /* Messages */
        protected virtual void OnStart() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnDestroyed() { }

        //--------------------------------------------------
        /* Methods */
    }
}
