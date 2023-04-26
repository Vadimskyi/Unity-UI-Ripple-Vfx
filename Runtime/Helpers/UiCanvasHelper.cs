/* Copyright (C) 2021 Vadimskyi - All Rights Reserved
 * Github - https://github.com/Vadimskyi
 * Website - https://www.vadimskyi.com/
 * You may use, distribute and modify this code under the
 * terms of the GPL-3.0 License.
 */
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VadimskyiLab.Android;
using VadimskyiLab.Utils;
using Object = UnityEngine.Object;

namespace VadimskyiLab.UiExtension
{
    [RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
    public class UiCanvasHelper : MonoBehaviour
    {
        public static UiCanvasHelper Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = Object.FindObjectOfType<CanvasScaler>().gameObject.AddComponent<UiCanvasHelper>();
                _instance.Awake();
                _instance.Start();
                return _instance;
            }
        }

        public float PixelsPerUnit => CanvasScaler.referencePixelsPerUnit;

        public RectTransform ScreenRect
        {
            get { return _screenRect = _screenRect ?? GetComponent<RectTransform>(); }
            private set { _screenRect = value; }
        }

        public CanvasScaler CanvasScaler
        {
            get { return _canvasScaler = _canvasScaler ?? GetComponent<CanvasScaler>(); }
            private set { _canvasScaler = value; }
        }

        public GraphicRaycaster GuiReycaster
        {
            get { return _guiReycaster = _guiReycaster ?? GetComponent<GraphicRaycaster>(); }
            private set { _guiReycaster = value; }
        }

        public Vector2 ScreenSize => ScreenRect.sizeDelta;

        public Vector2 ScreenToGuiAspect => new Vector2(
            (float)Screen.width / UICanvasSize.x,
            (float)Screen.height / UICanvasSize.y);

        public Vector2 UICanvasSize =>
            new Vector2(
                ScreenRect.rect.width,
                ScreenRect.rect.height);

        public ApplicationOrientation CanvasOrientation => Screen.width > Screen.height
            ? ApplicationOrientation.Landscape
            : ApplicationOrientation.Portrait;

        public EventSystem EventSystem;

        private static UiCanvasHelper _instance;
        private GraphicRaycaster _guiReycaster;
        private CanvasScaler _canvasScaler;
        private RectTransform _screenRect;
        private Vector2 _guiOffset;
        private Vector2 _uICanvasSize;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _screenRect = GetComponent<RectTransform>();
            _guiOffset = new Vector2( _screenRect.sizeDelta.x / 2f,  _screenRect.sizeDelta.y / 2f);
        }

        public Vector2 ScreenPosToGui(RectTransform rect, Vector2 screenPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, Camera.main, out var localPoint);
            return localPoint;
        }

        public Vector2 ScreenPosToGui()
        {
            return ScreenPosToGui(ScreenRect, InputUtils.GetInputPosition());
        }

        public Vector2 ScreenPosToWorld(Vector2 screenPos)
        {
            return Camera.main.ScreenToWorldPoint(screenPos);
        }

        public Vector2 VirtualScreenPosToGui(Vector2 virtualScreenPos)
        {
            return (virtualScreenPos - _guiOffset);
        }

        public Vector2 GuiPosToScreen(Vector2 guiPos)
        {
#if UNITY_EDITOR
            return new Vector2(
                guiPos.x + _guiOffset.x,
                -guiPos.y + _guiOffset.y);
#else
            return new Vector2(
                guiPos.x + _guiOffset.x,
                guiPos.y + _guiOffset.y);
#endif
        }

        public bool IsInputInRect(Vector2 screenPos, RectTransform rect)
        {
            var guiPos = ScreenPosToGui(ScreenRect, screenPos);
            var size = GetRectSize(rect);
            var bound = new Bounds(GetRectCenter(rect), size);
            return bound.Contains(guiPos);
        }

        public Vector2 GetRectCenter(RectTransform rect)
        {
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);


            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = ScreenRect.InverseTransformPoint(corners[i]);
            }

            var size = GetRectSize(rect);

            return new Vector2(
                corners[0].x + size.x / 2,
                corners[0].y + size.y / 2);
        }

        public Vector2 GetRectSize(RectTransform rect)
        {
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);


            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = ScreenRect.InverseTransformPoint(corners[i]);
            }

            return new Vector2(
                Math.Abs(corners[0].x - corners[2].x),
                Math.Abs(corners[0].y - corners[2].y));
        }

        /// <summary>
        /// Normalize gui-rect position on screen so it contains in the ScreenRect bounds
        /// </summary>
        public void NormalizeGuiContent(RectTransform rectTransform, Vector2 customOffset)
        {
            Vector2 normalizedPos = rectTransform.anchoredPosition;

            var halfScreenSize = new Vector2(ScreenRect.sizeDelta.x / 2 - customOffset.x, ScreenRect.sizeDelta.y / 2 - customOffset.y);

            var rectCenter = GetRectCenter(rectTransform);
            Vector2 rectSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

            var halfRectScreen = new Vector2(rectSize.x / 2, rectSize.y / 2);

            if (rectCenter.y + halfRectScreen.y > halfScreenSize.y)
            {
                normalizedPos.y = normalizedPos.y - (halfRectScreen.y + rectCenter.y - halfScreenSize.y);
            }

            if (rectCenter.x - halfRectScreen.x < -halfScreenSize.x)
            {
                normalizedPos.x = normalizedPos.x + ((rectCenter.x - halfRectScreen.x) * -1 - halfScreenSize.x);
            }
            else if (rectCenter.x + halfRectScreen.x > halfScreenSize.x)
            {
                normalizedPos.x = normalizedPos.x - (halfRectScreen.x + rectCenter.x - halfScreenSize.x);
            }

            rectTransform.anchoredPosition = normalizedPos;
        }

        public Vector2 GetPointBetweenFingers_Gui()
        {
            if(Input.touches == null || Input.touches.Length == 0) return ScreenPosToGui(ScreenRect, InputUtils.GetInputPosition());
            if (Input.touches.Length < 2) return ScreenPosToGui(ScreenRect, Input.GetTouch(0).position);
            var poin1 = Input.GetTouch(0).position;
            var poin2 = Input.GetTouch(1).position;
            return ScreenPosToGui(ScreenRect, (poin1 + poin2) / 2);
        }

        public float PhysicalSizeInInches_ToPixels(float inches)
        {
            return AndroidMetrics.ScreenDpi * inches;
        }

        public float PhysicalSizeInMm_ToPixels(float mm)
        {
            return PhysicalSizeInInches_ToPixels(mm) / 25.4f;
        }

        public Vector2 ConvertScreenValuesToGui(Vector2 value)
        {
            if(ScreenToGuiAspect.x < 1 && ScreenToGuiAspect.y < 1) return value / ScreenToGuiAspect;
            return value * ScreenToGuiAspect;
        }
    }

    public enum ApplicationOrientation
    {
        Portrait,
        Landscape
    }
}