using System.Threading;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;
using DG.Tweening;

namespace Challenges._3._GGStateMachineCharacterPhysics.Scripts.States
{
    public class WalkingState : GGStateBase
    {
        private readonly Transform _characterTransform;

        private const float SCALE_FACTOR = 0.3f;
        private const float SCALE_TIME = 1.1f;
        private const float ELASTICITY = 1f;
        private const int VIBRATO = 1;

        public WalkingState(Transform characterTransform)
        {
            _characterTransform = characterTransform;
        }

        public override void Setup()
        {
        }

        public override async UniTask Entry(CancellationToken cancellationToken)
        {
            _characterTransform.DOPunchScale(Vector3.one * SCALE_FACTOR, SCALE_TIME, VIBRATO, ELASTICITY)
                .SetLoops(-1, LoopType.Restart);
        }

        public override async UniTask Exit(CancellationToken cancellationToken)
        {
            DOTween.KillAll();
            _characterTransform.localScale = Vector3.one;
        }

        public override void CleanUp()
        {
        }
    }
}