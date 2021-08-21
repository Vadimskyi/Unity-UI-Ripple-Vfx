/* Copyright (C) 2021 Vadimskyi - All Rights Reserved
 * Github - https://github.com/Vadimskyi
 * Website - https://www.vadimskyi.com/
 * You may use, distribute and modify this code under the
 * terms of the GPL-3.0 License.
 */
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VadimskyiLab.Utils;

namespace VadimskyiLab.UiExtension
{
    public class UiRippleVfx : UIBehaviour, IUiRippleVfx , IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Color _rippleColor = Color.white;
        [SerializeField] private int _rippleSize = 100;
        [SerializeField] private float _effectDuration = 0.3f;
        [SerializeField] private float _scaleFactor = 2;
        [SerializeField] private bool _applyMask = true;
        [SerializeField] private bool _holdOnPress = true;
        [SerializeField] private bool _customRipplePosition = false;
        [SerializeField] private Vector2 _ripplePosition;

        private Mask _mask;
        private Image _rippleSprite;
        private Texture2D _generatedTexture;
        private ITweenRemoteControl _fadeTweener;
        private ITweenRemoteControl _scaleTweener;

        private const float _alphaFactorMin = 0.3f;
        private const float _alphaFactorMax = 1f;

        protected override void Awake()
        {
            base.Awake();
        }

        public void Enable(bool b)
        {
            enabled = b;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!enabled)
                return;
            OnClick();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!enabled || !_holdOnPress)
                return;
            _fadeTweener?.Kill();
            _fadeTweener = _rippleSprite.TweenAlpha(_alphaFactorMin, _effectDuration, TweenerPlayStyle.Once);
            _fadeTweener.OnComplete(ClearCache);
        }

        private void OnClick()
        {
            if (!enabled)
                return;

            CreateRippleSprite();
            GenerateRippleTexture();
            CreateMask();

            if(_customRipplePosition)
                _rippleSprite.rectTransform.anchoredPosition = _ripplePosition;
            else
                _rippleSprite.rectTransform.anchoredPosition = UiCanvasHelper.Instance.ScreenPosToGui(InputUtils.GetInputPosition());

            _rippleSprite.rectTransform.localScale = Vector3.one;
            GraphicExtend.SetAlpha(_rippleSprite, _alphaFactorMin);

            _fadeTweener?.Kill();
            _fadeTweener = _rippleSprite.TweenAlpha(_alphaFactorMax, _effectDuration, _holdOnPress ? TweenerPlayStyle.Once : TweenerPlayStyle.PingPong);
            _scaleTweener?.Kill();
            _scaleTweener = _rippleSprite.rectTransform.TweenScale2D(new Vector2(GetScaleFactor(), GetScaleFactor()), _effectDuration, TweenerPlayStyle.Once);

            if(!_holdOnPress)
                _fadeTweener.OnComplete(ClearCache);
        }

        private void CreateRippleSprite()
        {
            if (_rippleSprite != null) return;

            var go = new GameObject("ripple", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(transform, false);
            go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            go.GetComponent<CanvasRenderer>().cullTransparentMesh = true;
            _rippleSprite = go.GetComponent<Image>();
            _rippleSprite.raycastTarget = false;

            SetRippleObjectSize(_rippleSprite);
        }

        private void GenerateRippleTexture()
        {
            var texSize = GetRippleSize();

            _generatedTexture = TextureStaticFactory.CreateCircleTexture(_rippleColor, texSize, texSize, texSize / 2, texSize / 2, texSize / 2);
            _rippleSprite.sprite = Sprite.Create(
                _generatedTexture,
                new Rect(Vector2.zero, new Vector2(texSize, texSize)),
                Vector2.zero,
                UiCanvasHelper.Instance.PixelsPerUnit,
                1,
                SpriteMeshType.FullRect);
        }

        private void ClearCache()
        {
            if (_generatedTexture != null)
            {
                TextureStaticFactory.ReturnTextureRGBA32(_generatedTexture);
                _generatedTexture = null;
            }

            GraphicExtend.SetAlpha(_rippleSprite, 0);
            _rippleSprite.sprite = null;

            if (_mask != null)
            {
                Destroy(_mask);
                _mask = null;
            }
        }

        private void CreateMask()
        {
            if(!_applyMask) return;
            _mask = _mask ?? GetComponent<Mask>() ?? gameObject.AddComponent<Mask>();
            _mask.showMaskGraphic = true;
        }

        private void SetRippleObjectSize(Image rippleSprite)
        {
            var rippleObjectSize = GetRippleSize();
            rippleSprite.rectTransform.sizeDelta = new Vector2(rippleObjectSize, rippleObjectSize);
        }

        private int GetRippleSize()
            => _rippleSize;

        private float GetScaleFactor() => _scaleFactor;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _fadeTweener?.Kill();
            _scaleTweener?.Kill();
            if (_generatedTexture != null)
            {
                TextureStaticFactory.ReturnTextureRGBA32(_generatedTexture);
                _generatedTexture = null;
            }
        }
    }
}
