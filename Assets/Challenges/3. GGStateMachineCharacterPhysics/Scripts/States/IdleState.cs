using System.Threading;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;
using DG.Tweening;

namespace Challenges._3._GGStateMachineCharacterPhysics.Scripts.States
{
    /// <summary>
    /// You can edit this
    /// </summary>
    public class IdleState : GGStateBase
    {
        private readonly Transform _characterTransform;

        private const float SCALE_FACTOR = 0.7f;
        private const float SCALE_TIME = 1.8f;

        public IdleState(Transform characterTransform)
        {
            _characterTransform = characterTransform;
        }

        public override void Setup()
        {
        }

        public override async UniTask Entry(CancellationToken cancellationToken)
        {
            _characterTransform.DOScale(_characterTransform.localScale * SCALE_FACTOR, SCALE_TIME)
                .SetLoops(-1, LoopType.Yoyo);
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
