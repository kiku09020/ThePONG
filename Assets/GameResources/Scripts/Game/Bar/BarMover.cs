using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Bar {
    public class BarMover : BarComponent {
        /* Fields */
        [Header("Parameters")]
        [SerializeField] float _speed = 1f;
        [SerializeField] float moveLimit = 13.5f;

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */
        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            Move();
        }

        //-------------------------------------------------------------------
        /* Methods */
        void Move()
        {
            if (core.InputProvider.IsInputAxisY) {
                // à⁄ìÆ
                core.transform.transform.Translate(Vector3.up * core.InputProvider.InputAxisY * _speed * Time.deltaTime);

                // à⁄ìÆêßå¿
                var pos = core.transform.position;
                pos.y = Mathf.Clamp(core.transform.position.y, -moveLimit, moveLimit);
                core.transform.position = pos;
            }
        }
    }
}