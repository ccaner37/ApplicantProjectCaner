using System;
using Challenges._3._GGStateMachineCharacterPhysics.Scripts.States;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using GGPlugins.GGStateMachine.Scripts.Data;
using GGPlugins.GGStateMachine.Scripts.Installers;
using UnityEditor;
using UnityEngine;
using Zenject;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Challenges._3._GGStateMachineCharacterPhysics.Scripts.MonoBehaviours
{
    [Serializable]
    public class CharacterMovementConfig
    {
        [SerializeField,Range(0,3)]
        private float characterRadius;
        [SerializeField,Range(0,8)]
        private float characterHeight;
        // u/s^2 = units per seconds squared
        [SerializeField,Range(0,20)][Tooltip("How quickly the character speeds up: u/s^2")]
        private float accelerationByTime;
        [SerializeField,Range(0,10)][Tooltip("Maximum speed: u/s")]
        private float maxSpeed;
        [SerializeField,Range(0.9f,1)][Tooltip("Multiplied with speed every frame ")]
        private float generalVelocityDamping;
        [SerializeField,Range(0.9f,1)][Tooltip("Multiplied with speed every frame only when there's input")]
        private float withInputVelocityDamping;
        [SerializeField,Range(0.9f,1)][Tooltip("Multiplied with speed every frame only when there's no input")]
        private float noInputVelocityDamping;
        [SerializeField,Range(0.9f,1)][Tooltip("Multiplied with speed every frame only when the character is above ground")]
        private float midAirXZVelocityDamping;
        [SerializeField,Min(0)][Tooltip("u/s^2")]
        private float gravity;

        
        
        public float Gravity => gravity;

        public float MidAirXZVelocityDamping => midAirXZVelocityDamping;

        public float NoInputVelocityDamping => noInputVelocityDamping;

        public float WithInputVelocityDamping => withInputVelocityDamping;

        public float GeneralVelocityDamping => generalVelocityDamping;

        public float MAXSpeed => maxSpeed;

        public float AccelerationByTime => accelerationByTime;

        public float CharacterHeight => characterHeight;

        public float CharacterRadius => characterRadius;
    }
    [ExecuteAlways]
    public class CharacterController : MonoBehaviour, IInputListener
    {
        [SerializeField]
        private CharacterMovementConfig characterMovementConfig;
        [SerializeField]
        private Transform headTransform;
        
        private GGStateMachineFactory _ggStateMachineFactory;
        private IGGStateMachine _stateMachine;

        [Inject]
        public void Inject(GGStateMachineFactory ggStateMachineFactory)
        {
            _ggStateMachineFactory = ggStateMachineFactory;
        }
        void Start()
        {
            if (!Application.isPlaying) return;
            CreateStateMachine();
            SetupStateMachineStates();
            foreach (var stateMachineUser in transform.GetComponentsInChildren<IStateMachineUser>())
            {
                stateMachineUser.SetStateMachine(_stateMachine);
            }
            _stateMachine.StartStateMachine<IdleState>();
        }

        private void OnDestroy()
        {
            if(Application.isPlaying)
                _stateMachine.RequestExit();
        }

        private void CreateStateMachine()
        {
            _stateMachine = _ggStateMachineFactory.Create();
            //We don't want the machine to leave a state and re-enter it.
            _stateMachine.SetSettings(new StateMachineSettings(true));
            _stateMachine.RegisterUniqueState(new FlowerEarnedState(transform, headTransform));
        }

        #region EDIT
        // You should only need to edit in this region, you can add any variables you wish.



        //Add your states under this function
        private void SetupStateMachineStates()
        {
            _stateMachine.RegisterUniqueState(new ExampleState()).RegisterUniqueState(new ExampleParametrizedState(5f));

            _stateMachine.RegisterUniqueState(new IdleState(transform));
            _stateMachine.RegisterUniqueState(new WalkingState(transform));
        }
        
        //Feel free to remove this
        private void ExampleStateSwitching()
        {
            _stateMachine.EnqueueState<ExampleParametrizedState,float>(1f);
            _stateMachine.EnqueueState<ExampleState>();
            // EnqueueState will queue up the states
            
            
            _stateMachine.SwitchToState<ExampleParametrizedState,float>(1f);
            _stateMachine.SwitchToState<ExampleState>();
            // SwitchToState will clear the current queue and add the input as next state (after the currently active one ends)
        }

        private bool _isGrounded;

        private SphereCollider _sphereCollider;

        private float _gravityVelocity;

        private float _heightToRayFactor = 0.4f;

        private float _radiusToRayFactor = 0.4f;

        private Vector3 _rayPoint;

        private Vector3 _collisionVelocity;

        private Vector3 _velocity;

        [SerializeField]
        private LayerMask _rayLayer;

        private void Awake()
        {
            _rayPoint = new Vector3(0, characterMovementConfig.CharacterHeight * _heightToRayFactor, 0);
            _sphereCollider = transform.GetComponent<SphereCollider>();
        }

        // CharacterInput.cs will call this function every frame in Update. xzPlaneMovementVector specifies the current input.
        // Ex:
        // (W is pressed) -> (0,1) ;
        // (W and D) -> (1,1) ;
        // (W and S) -> (0,0) ;
        // (A and S) -> (-1,-1) ;
        // (A) -> (-1,0)
        public void SetCurrentMovement(Vector2 xzPlaneMovementVector)
        {
            xzPlaneMovementVector *= characterMovementConfig.MAXSpeed;
            _velocity = new Vector3(xzPlaneMovementVector.x, -_gravityVelocity, xzPlaneMovementVector.y) + _collisionVelocity;
            transform.position += _velocity * Time.deltaTime;

            _collisionVelocity = Vector3.zero;

            CheckGround();
            HandleGravity();
            HandleCollision();
            HandleStates();
        }

        private async UniTaskVoid CheckGround()
        {
            Ray ray = new Ray(transform.TransformPoint(_rayPoint), Vector3.down);
            RaycastHit hit;

            if (Physics.SphereCast(ray, characterMovementConfig.CharacterRadius * _radiusToRayFactor, out hit, 5f, _rayLayer))
            {
                _isGrounded = true;

                Vector3 nextPosition = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, nextPosition, characterMovementConfig.Gravity * Time.deltaTime);

                return;
            }

            _isGrounded = false;
        }

        private async UniTaskVoid HandleGravity()
        {
            if (_isGrounded)
                _gravityVelocity = 0;
            else
                _gravityVelocity = characterMovementConfig.Gravity;
        }

        private async UniTaskVoid HandleCollision()
        {
            Collider[] overlaps = new Collider[4];
            int num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(_sphereCollider.center), _sphereCollider.radius, overlaps, _rayLayer);
            for (int i = 0; i < num; i++)
            {
                Transform collidedTransform = overlaps[i].transform;
                Vector3 direction;
                float distance;

                if (Physics.ComputePenetration(_sphereCollider, transform.position, transform.rotation, overlaps[i], 
                    collidedTransform.position, collidedTransform.rotation, out direction, out distance))
                {
                    Vector3 penetrationVector = direction * distance;
                    Vector3 velocityProjected = Vector3.Project(_velocity, -direction);
                    transform.position += penetrationVector;
                    _collisionVelocity -= velocityProjected;
                }
            }
        }

        private async UniTaskVoid HandleStates()
        {
            if (_velocity == Vector3.zero)
            {
                if (_stateMachine.CheckCurrentState<IdleState>()) return;
                _stateMachine.SwitchToState<IdleState>();
            }
            else
            {
                if (_stateMachine.CheckCurrentState<WalkingState>()) return;
                _stateMachine.SwitchToState<WalkingState>();
            }
        }
        #endregion


        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(transform.position,transform.up,characterMovementConfig.CharacterRadius);
            Handles.DrawWireDisc(transform.position+(transform.up*characterMovementConfig.CharacterHeight),transform.up,characterMovementConfig.CharacterRadius);
            for (int i = 0; i < 10; i++)
            {
                var angle = Mathf.PI*2 * ((i + 0f) / 10f);
                var x = Mathf.Cos(angle);
                var y = Mathf.Sin(angle);
                var localPos = new Vector3(x, 0, y)*characterMovementConfig.CharacterRadius;
                var localTargetPos = localPos + Vector3.up * characterMovementConfig.CharacterHeight;
                Handles.DrawLine(transform.TransformPoint(localPos),transform.TransformPoint(localTargetPos));
            }
          
        }
    }
}


//if (Physics.SphereCast(ray, characterMovementConfig.CharacterRadius, out hit, 20f, _rayLayer))
//{
//    //float currentSlope = Vector3.Angle(hit.normal, Vector3.up);
//    Collider[] colliders = new Collider[3];
//    int num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(_groundCheckPoint), characterMovementConfig.CharacterRadius, colliders, _rayLayer);
//    _isGrounded = false;

//    for (int i = 0; i < num; i++)
//    {
//        if (colliders[i].transform == hit.transform)
//        {
//            _groundHit = hit;
//            _isGrounded = true;

//            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, _groundHit.point.y + characterMovementConfig.CharacterHeight / 6f, transform.position.z), characterMovementConfig.Gravity * Time.deltaTime);
//        }

//        break;

//        if (num <= 1 && hit.distance <= 3.1f)
//        {
//            if (colliders[0] != null)
//            {
//                Ray ray2 = new Ray(transform.TransformPoint(_liftPoint), Vector3.down);
//                RaycastHit hit2;
//                if (Physics.Raycast(ray, out hit2, 3.1f, _rayLayer))
//                {
//                    if (hit.transform != colliders[0].transform)
//                    {
//                        _isGrounded = false;
//                        return;
//                    }
//                }
//            }
//        }
//    }
//}
//else
//{
//    _isGrounded = false;
//}