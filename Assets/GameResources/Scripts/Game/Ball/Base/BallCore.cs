using GameUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ball
{
    public class BallCore : ObjectCore<BallCore>
    {
        /* Fields */
        [SerializeField] float moveSpeed = 30;

        //-------------------------------------------------------------------
        /* Properties */
        public float MoveSpeed => moveSpeed;

        //--------------------------------------------------
        /* Events */
        public event System.Action<Collision2D> OnHit;

        //-------------------------------------------------------------------
        /* Messages */
        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnHit?.Invoke(collision);
        }

        //-------------------------------------------------------------------
        /* Methods */

    }
}