using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.ObjectPool
{
    public class SimpleCustomizedObjectPool<T> : SimpleObjectPool<T> where T : PooledObject<T>
    {
        //-------------------------------------------------------------------
        /* Events */
        protected override T OnCreate(T prefab, Transform parent)
        {
            var obj = base.OnCreate(prefab, parent);
            obj.OnCreated();
            return obj;
        }

        protected override void OnGetFromPool(T obj)
        {
            obj.OnGetFromPool();
        }

        protected override void OnReleaseToPool(T obj)
        {
            obj.OnReleasedToPool();
        }

        protected override void OnDestroyObject(T obj)
        {
            obj.OnDestroyed();
        }
    }
}