using GameController.UI;
using GameUtils.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameController.State
{
    /// <summary> åãâ èÛë‘ </summary>
    public class ResultState : GameStateBase
    {
        /* Fields */

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */
        public override void OnEnter()
        {
            base.OnEnter();

            UIManager.ShowUIGroup<ResultUIGroup>();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
        //-------------------------------------------------------------------
        /* Methods */

    }
}