using Game.Player;
using GameController.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Player.Manager;

namespace Game.Goal
{
    public class Goal : MonoBehaviour
    {
        /* Fields */
        [SerializeField] Player.Player player;

        GameStateMachine stateMachine;
        PlayerManager playerManager;

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */
        private void Awake()
        {
            stateMachine = FindFirstObjectByType<GameStateMachine>();
            playerManager = FindFirstObjectByType<PlayerManager>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ball")) {
                playerManager.GetOppPlayer(player).AddScore();

                stateMachine.StateTransition<GoaledState>();
            }
        }

        //-------------------------------------------------------------------
        /* Methods */

    }
}