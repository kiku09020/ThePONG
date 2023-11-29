using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Bar {
    /// <summary> �o�[���̓v���o�C�_�[ </summary>
    public class BarInputProvider : MonoBehaviour {
        /* Fields */

        //-------------------------------------------------------------------
        /* Properties */
        /// <summary> �㉺�ړ����͒l </summary>
        public float InputAxisY { get; private set; }
        public bool IsInputAxisY => InputAxisY != 0f;

        //-------------------------------------------------------------------
        /* Methods */

        public void SetMoveInputData(InputAction.CallbackContext ctx)
        {
            InputAxisY = ctx.ReadValue<float>();

            print($"provider: {InputAxisY}");
        }

        public void ResetMoveInputData(InputAction.CallbackContext ctx)
        {
            if (ctx.canceled) {
                InputAxisY = 0f;
            }
        }
    }
}