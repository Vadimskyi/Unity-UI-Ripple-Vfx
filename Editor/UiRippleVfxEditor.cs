/* Copyright (C) 2021 Vadimskyi - All Rights Reserved
 * Github - https://github.com/Vadimskyi
 * Website - https://www.vadimskyi.com/
 * You may use, distribute and modify this code under the
 * terms of the GPL-3.0 License.
 */
using UnityEditor;

namespace VadimskyiLab.UiExtension
{
    [CustomEditor(typeof(UiRippleVfx), true)]
    [CanEditMultipleObjects]
    public class UiRippleVfxEditor : Editor
    {
        SerializedProperty RippleColor;
        SerializedProperty RippleSize;
        SerializedProperty EffectDuration;
        SerializedProperty ScaleFactor;
        SerializedProperty ApplyMask;

        protected void OnEnable()
        {
            RippleColor = serializedObject.FindProperty("_rippleColor");
            RippleSize = serializedObject.FindProperty("_rippleSize");
            EffectDuration = serializedObject.FindProperty("_effectDuration");
            ScaleFactor = serializedObject.FindProperty("_scaleFactor");
            ApplyMask = serializedObject.FindProperty("_applyMask");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(RippleColor);
            EditorGUILayout.PropertyField(RippleSize);
            EditorGUILayout.PropertyField(EffectDuration);
            EditorGUILayout.PropertyField(ScaleFactor);
            EditorGUILayout.PropertyField(ApplyMask);

            serializedObject.ApplyModifiedProperties();
        }
    }
}