using GameUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Bar
{
    public abstract class BarComponent : ObjectCoreBehaviour<BarCore>
    {
        /* Fields */

        //-------------------------------------------------------------------
        /* Properties */

        //--------------------------------------------------
        /* Events */

        //-------------------------------------------------------------------
        /* Messages */
        protected override void Awake()
        {
            base.Awake();

            core.OnHitBall += OnHitBall;
        }

        protected virtual void OnHitBall(Collision2D collision)
        {

        }

        //-------------------------------------------------------------------
        /* Methods */

    }
}