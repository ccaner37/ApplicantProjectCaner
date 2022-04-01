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
        private readonly Transform _headTransform;
        private float _strength;
        private float _time;

        public IdleState(Transform characterTransform, Transform headTransform)
        {
            _characterTransform = characterTransform;
            _headTransform = headTransform;
        }

        public override void Setup()
        {
        }

        public override async UniTask Entry(CancellationToken cancellationToken)
        {
            _characterTransform.DOShakeScale(1f, 0.5f, 5, 50);
        }

        public override async UniTask Exit(CancellationToken cancellationToken)
        {
            DOTween.KillAll();
            _characterTransform.DOScale(Vector3.one, 0.5f);
        }

        public override void CleanUp()
        {
        }
    }
}
