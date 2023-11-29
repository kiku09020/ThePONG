using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ball {
    public class PooledBall : PooledObject<PooledBall> {
		/* Fields */

		//-------------------------------------------------------------------
		/* Properties */

		//-------------------------------------------------------------------
		/* Messages */
		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnGetFromPool()
		{
			base.OnGetFromPool();
		}

		public override void OnReleasedToPool()
		{
			base.OnReleasedToPool();
		}

		public override void OnDestroyed()
		{
			base.OnDestroyed();
		}

		//-------------------------------------------------------------------
		/* Methods */

	}
}