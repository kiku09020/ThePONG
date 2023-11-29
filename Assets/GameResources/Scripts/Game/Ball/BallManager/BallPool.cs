using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ball.Management {
    public class BallPool : SimpleCustomizedObjectPool<PooledBall> {
		/* Fields */

		//-------------------------------------------------------------------
		/* Properties */

		//-------------------------------------------------------------------
		/* Messages */
		protected override PooledBall OnCreate(PooledBall prefab, Transform parent)
		{
			return base.OnCreate(prefab, parent);
		}

		protected override void OnGetFromPool(PooledBall obj)
		{
			base.OnGetFromPool(obj);
		}

		protected override void OnReleaseToPool(PooledBall obj)
		{
			base.OnReleaseToPool(obj);
		}

		//-------------------------------------------------------------------
		/* Methods */

	}
}