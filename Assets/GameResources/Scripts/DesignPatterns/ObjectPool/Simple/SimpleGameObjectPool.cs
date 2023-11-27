using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.ObjectPool
{
    /// <summary> 自作のPooledObjectを生成するオブジェクトプール </summary>
    public class SimpleGameObjectPool : SimpleObjectPool<GameObject> 
    {
        [SerializeField,Tooltip("生成時に有効化するか")] 
        bool setActiveOnCreate = true;
        //-------------------------------------------------------------------
        /* Events */
        protected override GameObject OnCreate(GameObject prefab, Transform parent)
        {
            var obj = base.OnCreate(prefab, parent);
            obj.SetActive(setActiveOnCreate);
            return obj;
        }

        protected override void OnGetFromPool(GameObject obj)
        {
            obj.SetActive(true);
        }

        protected override void OnReleaseToPool(GameObject obj)
        {
           obj.SetActive(false);
        }

        protected override void OnDestroyObject(GameObject obj)
        {
            Destroy(obj);
        }
    }
}