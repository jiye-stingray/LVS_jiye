using UnityEngine;
using LVS.UI;

namespace LVS.Player
{
    /// <summary>
    /// 플레이어 이동 컨트롤러. VirtualJoystick 입력을 받아 Rigidbody2D로 이동.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VirtualJoystick _joystick;

        [Header("Stats")]
        [SerializeField] private float _moveSpeed = 5f;

        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private Vector2 _moveDirection;

        // Animator parameter hash (문자열 비교 비용 절감)
        private static readonly int ANIM_SPEED  = Animator.StringToHash("Speed");
        private static readonly int ANIM_MOVE_X = Animator.StringToHash("MoveX");
        private static readonly int ANIM_MOVE_Y = Animator.StringToHash("MoveY");

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator  = GetComponent<Animator>();
        }

        private void Update()
        {
            GatherInput();
            UpdateAnimation();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void GatherInput()
        {
            if (_joystick == null)
            {
                // 조이스틱 미연결 시 키보드 폴백 (에디터 테스트용)
                _moveDirection = new Vector2(
                    Input.GetAxisRaw("Horizontal"),
                    Input.GetAxisRaw("Vertical")
                ).normalized;
            }
            else
            {
                _moveDirection = _joystick.Direction;
            }
        }

        private void Move()
        {
            Vector2 nextPosition = _rigidbody.position + _moveDirection * (_moveSpeed * Time.fixedDeltaTime);
            _rigidbody.MovePosition(nextPosition);
        }

        private void UpdateAnimation()
        {
            float speed = _moveDirection.magnitude;
            _animator.SetFloat(ANIM_SPEED, speed);

            if (speed > 0.01f)
            {
                _animator.SetFloat(ANIM_MOVE_X, _moveDirection.x);
                _animator.SetFloat(ANIM_MOVE_Y, _moveDirection.y);
            }
        }

        /// <summary>외부(스탯 시스템)에서 이동속도 설정</summary>
        public void SetMoveSpeed(float speed) => _moveSpeed = speed;
    }
}
