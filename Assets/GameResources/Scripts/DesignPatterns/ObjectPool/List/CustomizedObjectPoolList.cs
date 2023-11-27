using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.ObjectPool
{
    public class CustomizedObjectPoolList<T> : ObjectPoolList<T> where T : PooledObject<T>
    {
        //-------------------------------------------------------------------
        /* Events */
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