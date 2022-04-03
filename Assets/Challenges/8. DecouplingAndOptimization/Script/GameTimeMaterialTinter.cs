using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Challenges._8._DecouplingAndOptimization.Script
{
    public struct GameTime
    {
        public readonly int Hour;
        public readonly int Seconds;

        public GameTime(int hour, int seconds)
        {
            Hour = hour;
            Seconds = seconds;
        }

        public override string ToString()
        {
            return Hour.ToString("D2") + ":" + Seconds.ToString("D2");
        }
    }

    public class GameTimeMaterialTinter : MonoBehaviour
    {
        [SerializeField]
        private Renderer targetRenderer;

        [Inject]
        private GameTimeMaterialTintSettings settings;

        private bool _isInside = false;

        private Color _baseColor;

        private int _gameHour;
        private int _gameSeconds;

        private float _builtUpTime;

        void Start()
        {
            _gameHour = settings.startHour;
            _gameSeconds = settings.startSeconds;

            if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();
            _baseColor = targetRenderer.material.GetColor("_Color");

            HandleTime();
        }

        private async UniTask HandleTime()
        {
            while (true)
            {
                _builtUpTime += Time.deltaTime;
                bool changeHappened = false;
                while (_builtUpTime >= settings.realSecondsPerGameSecond)
                {
                    IncrementTime();
                    _builtUpTime -= settings.realSecondsPerGameSecond;
                    changeHappened = true;
                }
                if (changeHappened) OnTimerUpdate(new GameTime(_gameHour, _gameSeconds));

                await UniTask.Yield();
            }
        }

        private void IncrementTime()
        {
            _gameSeconds++;
            if (_gameSeconds >= 60)
            {
                _gameSeconds = 0;
                _gameHour++;
            }

            if (_gameHour >= 24)
            {
                _gameHour = 0;
            }
        }

        private void OnTimerUpdate(GameTime time)
        {
            if (_isInside) return;

            Color color = GetColorByTime(time);
            
            targetRenderer.material.SetColor("_Color",_baseColor*color);
        }

        private Color GetColorByTime(GameTime time)
        {
            Color color;
            if (time.Hour < 6)
                return color = Color.Lerp(settings.colorAtMidnight, settings.colorAt6AM, GetColorLerpSpeed(0, time));

            if (time.Hour < 12)
                return color = Color.Lerp(settings.colorAt6AM, settings.colorAt12PM, GetColorLerpSpeed(6, time));

            if (time.Hour < 18)
                return color = Color.Lerp(settings.colorAt12PM, settings.colorAt6PM, GetColorLerpSpeed(12, time));

            color = Color.Lerp(settings.colorAt6PM, settings.colorAtMidnight, GetColorLerpSpeed(18, time));
            return color;
        }

        private float GetColorLerpSpeed(float lastHour, GameTime time)
        {
            return (60 * (time.Hour - lastHour) + time.Seconds) / (360f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == IndoorArea.IndoorAreaLayer)
            {
                _isInside = true;
                targetRenderer.material.SetColor("_Color",_baseColor);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == IndoorArea.IndoorAreaLayer)
            {
                _isInside = false;
            }
        }
    }
}
