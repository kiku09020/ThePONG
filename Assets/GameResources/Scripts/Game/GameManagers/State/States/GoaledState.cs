using Game.Player.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameController.State {
    /// <summary> �S�[����� </summary>
    public class GoaledState : GameStateBaseWithTimer {
        /* Fields */
        [SerializeField] PlayerManager�@playerManager;   

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

            // �Q�[���Z�b�g�Ȃ烊�U���g��
            if (playerManager.IsGameSet) {
                StateTransitionWithTimer("Result");
            }

            // ����ȊO�Ȃ�^�[���J�n��Ԃ�
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