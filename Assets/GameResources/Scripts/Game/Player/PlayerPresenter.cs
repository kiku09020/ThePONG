using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Player
{
    public class PlayerPresenter : MonoBehaviour
    {
        /* Fields */
        [Header("View")]
        [SerializeField] TextMeshProUGUI scoreText;

        [Header("Model")]
        [SerializeField] Player player;

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */
        void Awake()
        {
            player.OnScoreChanged += SetScoreText;
        }

        //-------------------------------------------------------------------
        /* Methods */
        void SetScoreText(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}