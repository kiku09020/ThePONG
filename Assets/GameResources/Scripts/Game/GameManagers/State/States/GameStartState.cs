using GameController.UI;
using GameUtils.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameController.State
{
    ///<summary> �Q�[���J�n��� </summary>
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

            // �o�ߌ�ɑJ��
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