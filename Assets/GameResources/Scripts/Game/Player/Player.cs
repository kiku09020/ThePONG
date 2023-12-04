using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class Player : MonoBehaviour
    {
        /* Fields */

        //-------------------------------------------------------------------
        /* Properties */
        public int Score { get; private set; }

        //-------------------------------------------------------------------
        /* Events */
        public event System.Action<int> OnScoreChanged;

        //-------------------------------------------------------------------
        /* Methods */
        public void AddScore()
        {
            Score++;
            OnScoreChanged?.Invoke(Score);
        }
    }
}