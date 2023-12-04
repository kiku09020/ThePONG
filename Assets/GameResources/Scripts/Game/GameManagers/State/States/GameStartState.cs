using GameController.UI;
using GameUtils.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameController.State
{
    ///<summary> ゲーム開始状態 </summary>
    public class GameStartState : GameStateBaseWithTimer
    {
        /* Fields */
        [SerializeField] bool isSkipStartState = true;

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */
        public override void OnEnter()
        {
            base.OnEnter();

            UIManager.ShowUIGroup<GameStartUIGroup>();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            // 経過後に遷移
            if (isSkipStartState) {
                StateTransition("TurnStart");
            }
            else {
                StateTransitionWithTimer("TurnStart");
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            UIManager.HideUIGroup<GameStartUIGroup>();
            UIManager.ShowUIGroup<MainUIGroup>();
        }

        //-------------------------------------------------------------------
        /* Methods */

    }
}