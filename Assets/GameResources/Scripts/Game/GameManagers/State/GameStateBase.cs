using DesignPatterns.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameController.State {
    public abstract class GameStateBase : StateBase {
        /* Fields */

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */

        //-------------------------------------------------------------------
        /* Methods */

    }

    public abstract class GameStateBaseWithTimer: GameStateBase
    {
        [SerializeField] float waitDuration = 3;

        float timer;

        protected void StateTransitionWithTimer(string nextStateName)
        {
            timer += Time.deltaTime;

            if (timer >= waitDuration) {
                timer = 0;
                StateTransition(nextStateName);
            }
        }
    }
}