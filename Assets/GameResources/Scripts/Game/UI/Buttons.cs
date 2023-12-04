using GameUtils.SceneController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class Buttons : MonoBehaviour
    {
        /* Fields */

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */

        //-------------------------------------------------------------------
        /* Methods */
        public void OnClickedRestartButton()
        {
            SceneController.ReloadScene();
        }

        public void OnClickedExitButton()
        {
            //SceneController.LoadScene("Title");
        }
    }
}