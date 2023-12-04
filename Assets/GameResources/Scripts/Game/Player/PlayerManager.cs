using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        /* Fields */
        [SerializeField] List<Player> players;

        [SerializeField] int maxScore = 5;

        //-------------------------------------------------------------------
        /* Properties */
        public bool IsGameSet => players.Exists(player => player.Score >= maxScore);

        //-------------------------------------------------------------------
        /* Messages */

        //-------------------------------------------------------------------
        /* Methods */
        /// <summary> ‘Îí‘Šè‚ÌPlayer‚ğæ“¾ </summary>
        public Player GetOppPlayer(Player ownPlayer)
        {
            if (players.Count < 2) return null;

            return players.Find(player => player != ownPlayer);
        }
    }
}