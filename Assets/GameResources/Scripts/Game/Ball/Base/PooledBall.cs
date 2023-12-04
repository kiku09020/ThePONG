using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ball {
    public class PooledBall : PooledObject<PooledBall> {
		/* Fields */
		[SerializeField] BallCore core;

		//-------------------------------------------------------------------
		/* Properties */
		public BallCore Core => core;

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