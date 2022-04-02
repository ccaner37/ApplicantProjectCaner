using System;
using DG.Tweening;
using UnityEngine;

namespace Challenges._7._Abstract_DOTween.Scripts
{
    public enum DOAction
    {
        DOMove,
        DOScale
    }

    [Serializable]
    public struct CenterObjectSetting
    {
        public Vector3 NextVector;
        public float Duration;
        public bool IsJoin;
        public DOAction Action;
    }
    
    public class PickUpAnimator : DoTweenAnimation
    {
        [SerializeField]
        private CenterObjectSetting[] _centerObjectSettings;

        [Header("Shade Object Options")]
        [SerializeField]
        private Vector3 _shadeScaleVector;
        [SerializeField]
        private float _shadeScaleDuration, _shadeFadeAmount,_shadeFadeDuration;

        [Header("Floor Object Options")]
        [SerializeField]
        private Vector3 _punchScaleVector;
        [SerializeField]
        private float _punchDuration, _punchElasticity;
        [SerializeField]
        private int _punchVibrato, _loopNumber;

        public override Tween StartPreview()
        {
            var sequence = DOTween.Sequence();

            for (int i = 0; i < _centerObjectSettings.Length; i++)
            {
                CenterObjectSetting settings = _centerObjectSettings[i];
                Tween tween;

                switch (settings.Action)
                {
                    case DOAction.DOMove:
                        tween = CenterObject.transform.DOLocalMove(settings.NextVector, settings.Duration);
                        break;
                    case DOAction.DOScale:
                        tween = CenterObject.transform.DOScale(settings.NextVector, settings.Duration);
                        break;
                    default:
                        tween = null;
                        break;

                }

                if (settings.IsJoin)
                {
                    sequence.Join(tween);
                }
                else
                {
                    sequence.Append(tween);
                }
            }

            Tween floorPadTween = FloorPad.DOPunchScale(_punchScaleVector, _punchDuration, _punchVibrato, _punchElasticity).SetLoops(_loopNumber, LoopType.Incremental);

            Tween shadeTween = CenterObjectShade.DOScale(_shadeScaleVector, _shadeScaleDuration);
            Tween shadeFadeTween = CenterObjectShade.GetComponent<Renderer>().material.DOFade(_shadeFadeAmount, _shadeFadeDuration).SetEase(Ease.InExpo);

            sequence.Append(floorPadTween);
            sequence.Join(shadeTween);
            sequence.Join(shadeFadeTween);

            return sequence;
        }
    }
}
