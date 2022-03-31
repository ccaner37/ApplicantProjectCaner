using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

namespace Challenges._1._Basic_Progress_Bar.Scripts
{
    /// <summary>
    /// Edit this script for the ProgressBar challenge.
    /// </summary>
    public class ProgressBar : MonoBehaviour, IProgressBar
    {
        /// <summary>
        /// You can add more options
        /// </summary>
        private enum ProgressSnapOptions
        {
            SnapToLowerValue,
            SnapToHigherValue,
            DontSnap
        }
        
        /// <summary>
        /// You can add more options
        /// </summary>
        private enum TextPosition
        {
            BarCenter,
            Progress,
            NoText
        }
        
        /// <summary>
        /// These settings below must function
        /// </summary>
        [Header("Options")]
        [SerializeField] [Range(0.1f, 3)]
        private float baseSpeed;
        [SerializeField]
        private ProgressSnapOptions snapOptions;
        [SerializeField]
        private TextPosition textPosition;
        [SerializeField]
        private int percentageMultiplier = 100;
        [SerializeField] [Range(430, 440)]
        private int distanceBetweenBarAndText = 435;

        [Header("Bar Objects")]
        [SerializeField]
        private Transform fillBar;
        [SerializeField]
        private TextMeshProUGUI barText;

        private void Awake()
        {
            CheckTextOptions();
        }

        private void CheckTextOptions()
        {
            Transform textTransform = barText.transform;

            switch (textPosition)
            {
                case TextPosition.BarCenter:
                    textTransform.localPosition = Vector3.zero;
                    break;
                case TextPosition.Progress:
                    barText.alignment = TextAlignmentOptions.Left;
                    break;
                case TextPosition.NoText:
                    textTransform.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the progress bar to the given normalized value instantly.
        /// </summary>
        /// <param name="value">Must be in range [0,1]</param>
        public void ForceValue(float value)
        {
            SetBarText(value);
            SetFillBarScale(value, true);
        }

        /// <summary>
        /// The progress bar will move to the given value
        /// </summary>
        /// <param name="value">Must be in range [0,1]</param>
        /// <param name="speedOverride">Will override the base speed if one is given</param>
        public void SetTargetValue(float value, float? speedOverride = null)
        {
            SetBarText(value);
            SetFillBarScale(value, false);
        }

        /// <summary>
        /// Sets progress bar text according to value and bars percentageMultiplier (default 100)
        /// </summary>
        /// <param name="value">Must be in range[0,1]</param>
        private void SetBarText(float value)
        {
            int percentage = (int)(value * percentageMultiplier);
            string percentageString = percentage.ToString();
            barText.text = $"{percentageString}%";
        }

        private void SetBarTextPosition(Vector3 nextScale, bool isSmooth)
        {
            if (textPosition != TextPosition.Progress) return;

            Transform barTextTransform = barText.transform;
            Vector3 nextPosition = Vector3.zero;
            nextPosition.x = nextScale.x * 435;

            if (isSmooth)
            {
                barTextTransform.DOLocalMove(nextPosition, baseSpeed);
                return;
            }
            barTextTransform.localPosition = nextPosition;
        }

        private void SetFillBarScale(float value, bool isForced)
        {
            Vector3 nextFillBarScale = Vector3.one;
            nextFillBarScale.x = value;

            if (isForced)
            {
                ScaleFillBarInstant(nextFillBarScale);
                return;
            }

            CheckSnapOptions(nextFillBarScale);
        }

        private void CheckSnapOptions(Vector3 nextScale)
        {
            Vector3 currentScale = fillBar.localScale;

            bool isNextScaleLower = nextScale.x < currentScale.x;

            switch (snapOptions)
            {
                case ProgressSnapOptions.SnapToLowerValue:
                    if(isNextScaleLower)
                    {
                        ScaleFillBarInstant(nextScale);
                        return;
                    }
                    ScaleFillBarSmooth(nextScale);
                    break;
                case ProgressSnapOptions.SnapToHigherValue:
                    if (!isNextScaleLower)
                    {
                        ScaleFillBarInstant(nextScale);
                        return;
                    }
                    ScaleFillBarSmooth(nextScale);
                    break;
                case ProgressSnapOptions.DontSnap:
                    ScaleFillBarSmooth(nextScale);
                    break;
                default:
                    break;
            }
        }

        private void ScaleFillBarSmooth(Vector3 nextScale)
        {
            fillBar.DOScale(nextScale, baseSpeed);
            SetBarTextPosition(nextScale, true);
        }

        private void ScaleFillBarInstant(Vector3 nextScale)
        {
            DOTween.KillAll();
            fillBar.localScale = nextScale;
            SetBarTextPosition(nextScale, false);
        }
    }
}
