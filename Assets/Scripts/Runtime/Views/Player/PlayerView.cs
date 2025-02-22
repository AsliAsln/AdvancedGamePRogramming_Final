using DG.Tweening;
using Rich.Base.Runtime.Abstract.View;
using Runtime.Data.ValueObject;
using Runtime.Enums;
using Runtime.Key;
using Runtime.Signals;
using Runtime.Views.Stack;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Views.Player
{
    public class PlayerView : RichView
    {
        #region Self Variables

        #region Public Variables

        public UnityAction<GameObject> onStackCollectableAction = delegate { };

        public UnityAction<GameObject> onCollectableInteract = delegate { };
        public UnityAction onReset = delegate { };
        public UnityAction<Transform, Transform> onStageAreaEntered = delegate { };
        public UnityAction onFinishAreaEntered = delegate { };

        public UnityAction<Vector2> onSetPosAction = delegate { };

        public UnityAction onInteractionObstacle = delegate { };

            #endregion

        #region Serialized Variables

        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Renderer renderer;
        [SerializeField] private TextMeshPro scaleText;
        [SerializeField] private ParticleSystem confettiParticle;

        #endregion

        #region Private Variables

        [ShowInInspector] private bool _isReadyToMove, _isReadyToPlay;
        [ShowInInspector] private float _xValue;

        private float2 _clampValues;
        [ShowInInspector] private PlayerData _playerData;

        private readonly string _collectable = "Collectable";

        private readonly string _stageArea = "StageArea";

        private readonly string _groundObstacle = "GroundObstacle";

        private readonly string _groundRed = "GroundRed";
        private readonly string _groundYellow = "GroundYellow";
        private readonly string _groundBlue = "GroundBlue";


        private readonly string _gate = "Gate";

        private readonly string _finish = "FinishArea";
        private readonly string _miniGame = "MiniGameArea";

        private readonly string _obstacle = "Obstacle";

        [ShowInInspector] private PlayerColorTypes _colorType;

        #endregion

        #endregion

        public void SetPlayerData(PlayerData playerData)
        {
            _playerData = playerData;
        }

        internal void SetStackPos()
        {
            var position = transform.position;
            Vector2 pos = new Vector2(position.x, position.z);
            onSetPosAction?.Invoke(pos);
        }

        public void OnInputDragged(HorizontalInputParams horizontalInputParams)
        {
            _xValue = horizontalInputParams.HorizontalValue;
            _clampValues = horizontalInputParams.ClampValues;
        }

        public void OnInputReleased()
        {
            IsReadyToMove(false);
        }

        public void OnInputTaken()
        {
            IsReadyToMove(true);
        }

        private void FixedUpdate()
        {
            if (_isReadyToPlay)
            {
                SetStackPos();
            }

            if (!_isReadyToPlay)
            {
                StopPlayer();
                return;
            }

            if (_isReadyToMove)
            {
                MovePlayer();
            }
            else
            {
                StopPlayerHorizontally();
            }
        }

        private void StopPlayer()
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        private void StopPlayerHorizontally()
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, _playerData.MovementData.ForwardSpeed);
            rigidbody.angularVelocity = Vector3.zero;
        }

        private void MovePlayer()
        {
            var velocity = rigidbody.velocity;
            velocity = new Vector3(_xValue * _playerData.MovementData.SidewaysSpeed, velocity.y,
                _playerData.MovementData.ForwardSpeed);
            rigidbody.velocity = velocity;
            var position1 = rigidbody.position;
            Vector3 position;
            position = new Vector3(Mathf.Clamp(position1.x, _clampValues.x, _clampValues.y),
                (position = rigidbody.position).y, position.z);
            rigidbody.position = position;
        }

        internal void IsReadyToPlay(bool condition)
        {
            _isReadyToPlay = condition;
        }

        public void IsReadyToMove(bool condition)
        {
            _isReadyToMove = condition;
        }

        private void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.name == _collectable)
            {
                onStackCollectableAction?.Invoke(other.transform.gameObject);
            }

            if (other.CompareTag(_obstacle))
            {
                onInteractionObstacle?.Invoke();
            }

        }
        private void SetPlayerColor(PlayerColorTypes color)
        {
            switch (color)
            {
                case PlayerColorTypes.Blue:
                    // Blue color logic
                    renderer.materials[0].color = Color.blue;
                    break;
                case PlayerColorTypes.Yellow:
                    // Yellow color logic
                    renderer.materials[0].color = Color.yellow;
                    break;
                case PlayerColorTypes.Red:
                    // Red color logic
                    renderer.materials[0].color = Color.red;
                    break;
            }
        }


        internal void ScaleUpPlayer()
        {
            renderer.gameObject.transform.DOScaleX(_playerData.MeshData.ScaleCounter, 1).SetEase(Ease.Flash);
        }

        internal void ShowUpText()
        {
            scaleText.gameObject.SetActive(true);
            scaleText.DOFade(1, 0f).SetEase(Ease.Flash).OnComplete(() => scaleText.DOFade(0, 0).SetDelay(.65f));
            scaleText.rectTransform.DOAnchorPosY(.85f, .65f).SetRelative(true).SetEase(Ease.OutBounce).OnComplete(() =>
                scaleText.rectTransform.DOAnchorPosY(-.85f, .65f).SetRelative(true));
        }

        internal void PlayConfettiParticle()
        {
            confettiParticle.Play();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var transform1 = transform;
            var position1 = transform1.position;

            Gizmos.DrawSphere(new Vector3(position1.x, position1.y - 1f, position1.z + .9f), 1.7f);
        }

        internal void OnReset()
        {
            onReset?.Invoke();
            StopPlayer();
            _isReadyToMove = false;
            _isReadyToPlay = false;
            renderer.gameObject.transform.DOScaleX(1, 1).SetEase(Ease.Linear);
        }
    }
}