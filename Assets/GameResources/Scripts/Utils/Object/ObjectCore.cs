using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtils
{
    [DisallowMultipleComponent]
    public class ObjectCore<T> : MonoBehaviour where T:ObjectCore<T>
    {
        //--------------------------------------------------
        /* Events */
        public event System.Action OnStartEvent;
        public event System.Action OnUpdateEvent;
        public event System.Action OnFixedUpdateEvent;
        public event System.Action OnDestroyedEvent;

        //--------------------------------------------------
        /* Messages */
        protected virtual void Start()
        {
            OnStartEvent?.Invoke();
        }

        protected virtual void Update()
        {
            OnUpdateEvent?.Invoke();
        }

        protected virtual void FixedUpdate()
        {
            OnFixedUpdateEvent?.Invoke();
        }

        protected virtual void OnDestroy()
        {
            OnDestroyedEvent?.Invoke();
        }
    }
}
