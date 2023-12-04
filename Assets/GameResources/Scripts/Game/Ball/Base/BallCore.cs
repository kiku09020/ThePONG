using GameUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ball
{
    public class BallCore : ObjectCore<BallCore>
    {
        /* Fields */
        [SerializeField] float shotSpeed = 20;
        [SerializeField] float moveSpeedBase = 30;

        float currentMoveSpeed;

        //-------------------------------------------------------------------
        /* Properties */
        public bool IsMovable { get; private set; }
        public float MoveSpeed => currentMoveSpeed;
        public Vector2 Direction { get; private set; }

        //--------------------------------------------------
        /* Events */
        public event System.Action OnShot;
        public event System.Action OnGoaled;
        public event System.Action<Collision2D> OnHit;

        //-------------------------------------------------------------------
        /* Messages */

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnHit?.Invoke(collision);
            currentMoveSpeed = moveSpeedBase;
        }

        //-------------------------------------------------------------------
        /* Methods */
        public void Shot(Vector2 dir)
        {
            SetDirection(dir);

            currentMoveSpeed = shotSpeed;

            OnShot?.Invoke();
            IsMovable = true;
        }

        public void Goaled()
        {
            OnGoaled?.Invoke();
            IsMovable = false;
        }

        public void SetDirection(Vector2 dir)
        {
            if(dir == Vector2.zero) return;

            Direction = dir;
        }
    }
}