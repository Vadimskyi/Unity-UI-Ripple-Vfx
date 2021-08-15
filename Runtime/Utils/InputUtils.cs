/* Copyright (C) 2021 Vadimskyi - All Rights Reserved
 * Github - https://github.com/Vadimskyi
 * Website - https://www.vadimskyi.com/
 * You may use, distribute and modify this code under the
 * terms of the GPL-3.0 License.
 */
using UnityEngine;

namespace VadimskyiLab.Utils
{
    public static class InputUtils
    {
        public static Vector2 GetInputPosition()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Input.mousePosition;
#else
            if(Input.touchCount > 0)
                return Input.GetTouch(0).position;
            return Vector2.zero;
#endif
        }

        public static bool IsInputPressed()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Input.GetMouseButton(0);
#else
            return Input.touchCount > 0;
#endif
        }
    }
}
