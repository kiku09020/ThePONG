using GameUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Bar {
    public class BarCore : ObjectCore<BarCore> {
        /* Fields */
        [SerializeField] BarInputProvider inputProvider;

        //-------------------------------------------------------------------
        /* Properties */
        public BarInputProvider InputProvider => inputProvider;

        //--------------------------------------------------
        /* Events */
        public event System.Action<Collision2D> OnHitBall;

        //-------------------------------------------------------------------
        /* Messages */
        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ball")) {
                OnHitBall?.Invoke(collision);
            }
        }

        //-------------------------------------------------------------------
        /* Methods */

    }
}