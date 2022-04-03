using Challenges._1._Basic_Progress_Bar.Scripts;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Challenges._5._Complex_Loading_Bar.Scripts
{
    /// <summary>
    /// Uses the basic progress bar to provide an interface of a loading bar with inherent thresholds.
    /// You can imagine this like a player level bar, say your experience thresholds are [0,150,400,1500,8000]
    /// If you jump from 90XP to 1800XP, you would expect the progress bar to loop multiple times until it reaches the desired percentage
    ///
    /// The previous and next threshold texts should be update depending on where the progress currently is,
    /// if the progress bar needs to loop several times, the threshold text should be updated as it passes through each threshold.
    ///
    /// </summary>
    public class LoopableProgressBar : MonoBehaviour, ILoopableProgressBar
    {
        [SerializeField] private ProgressBar basicProgressBar;
        [SerializeField] private int[] initialThresholds;
        [SerializeField] private TMP_Text previousThresholdText;
        [SerializeField] private TMP_Text nextThresholdText;

        private void Start()
        {
            if(basicProgressBar==null) Debug.LogError("Basic progress bar is missing");
            if(previousThresholdText==null) Debug.LogError("Previous Threshold Text is missing");
            if(nextThresholdText==null) Debug.LogError("Next Threshold Text is missing");
            //Fallback
            if (initialThresholds.Length < 2)
            {
                Debug.LogWarning("Initial threshold size was less than 2, replacing it with [0,10]");
                initialThresholds = new int[] {0, 10};
            }
            SetThresholds(initialThresholds);
            ForceValue(initialThresholds[0]);
        }

        #region Editable Area

        private int[] thresholds;
        private int currentThreshold = 0;
        private int nextThreshold = 1;

        public void SetThresholds(int[] thresholds)
        {
            this.thresholds = thresholds;
            UpdateTexts();
        }

        public void ForceValue(int value)
        {
            Debug.Log($"Exp set to: {value}");
            float progress = Mathf.InverseLerp(thresholds[currentThreshold], thresholds[nextThreshold], value);
            basicProgressBar.ForceValue(progress);
            if (progress >= 1)
            {
                OnLevelUp();
                ForceValue(value);
            }
        }

        public void SetTargetValue(int value, float? speedOverride = null)
        {
            Debug.Log($"Exp set to: {value}");
            float progress = Mathf.InverseLerp(thresholds[currentThreshold], thresholds[nextThreshold], value);
            basicProgressBar.SetTargetValue(progress);
            if (progress >= 1)
            {
                basicProgressBar.fillBarScaleTween.OnComplete(() =>
                {
                    OnLevelUp();
                    SetTargetValue(value);
                });
            }
        }

        private void OnLevelUp()
        {
            currentThreshold++;
            nextThreshold++;
            UpdateTexts();
            basicProgressBar.ResetProgressBar();
        }

        private void UpdateTexts()
        {
            previousThresholdText.text = thresholds[currentThreshold].ToString();
            nextThresholdText.text = thresholds[nextThreshold].ToString();
        }

        #endregion
    }
}