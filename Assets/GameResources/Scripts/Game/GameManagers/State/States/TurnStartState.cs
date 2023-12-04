using Game.Ball.Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameController.State
{
    /// <summary> ターン開始状態 </summary>
    public class TurnStartState : GameStateBaseWithTimer
    {
        /* Fields */
        [SerializeField] BallManager ballManager;
        

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            StateTransitionWithTimer("GamePlaying");
        }

        public override void OnExit()
        {
            base.OnExit();

            var ball = ballManager.GenerateBall();
            ballManager.ShotBall(ball);
        }

        //-------------------------------------------------------------------
        /* Methods */

    }
}