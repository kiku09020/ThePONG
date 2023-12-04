using Game.Player.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameController.State {
    /// <summary> ゴール状態 </summary>
    public class GoaledState : GameStateBaseWithTimer {
        /* Fields */
        [SerializeField] PlayerManager　playerManager;   

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

            // ゲームセットならリザルトへ
            if (playerManager.IsGameSet) {
                StateTransitionWithTimer("Result");
            }

            // それ以外ならターン開始状態へ
            StateTransitionWithTimer("TurnStart");
        }

        public override void OnExit()
        {
            base.OnExit();

        }

        //-------------------------------------------------------------------
        /* Methods */

    }
}