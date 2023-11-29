using Game.Ball;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Bar {
    /// <summary> �o�[�̔��ˏ��� </summary>
    public class BarReflecter : BarComponent {
        /* Fields */
        [SerializeField] float maxReflectionAngle = 30;

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */
        protected override void OnHitBall(Collision2D collision)
        {
            base.OnHitBall(collision);

            Reflect(collision);

            print("Hit!");
        }

        //-------------------------------------------------------------------
        /* Methods */
        void Reflect(Collision2D collision)
        {
            var relativePos = collision.transform.position - core.transform.position;
            var dir = relativePos.normalized;

            // ���ˌ��Velocity���Z�b�g
            var ball = collision.gameObject.GetComponent<BallCore>();
            var rb = collision.gameObject.GetComponent<Rigidbody2D>();

            rb.velocity = dir * ball.MoveSpeed;
        }
    }
}