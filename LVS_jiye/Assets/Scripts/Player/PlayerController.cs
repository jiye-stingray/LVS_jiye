using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : UnitBase
{
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _moveDirection;

    private static readonly int ANIM_IS_MOVING = Animator.StringToHash("IsMoving");
    private static readonly int ANIM_MOVE_X    = Animator.StringToHash("MoveX");
    private static readonly int ANIM_MOVE_Y    = Animator.StringToHash("MoveY");

    public Transform _skillHolder;


    protected override void Awake()
    {
        base.Awake();
        _rigidbody      = GetComponent<Rigidbody2D>();
        _animator       = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        Manager.Instance.InitPlayerController(this);
    }

    protected override void Die()
    {
        // TODO: 게임 오버 처리
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
        VirtualJoystick joystick = Manager.Instance.UI.Joystick;
        if (joystick == null)
        {
            _moveDirection = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
        }
        else
        {
            _moveDirection = joystick.Direction;
        }
    }

    private void Move()
    {
        Vector2 nextPosition = _rigidbody.position + _moveDirection * (_stat.MoveSpeed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(nextPosition);
    }

    private void UpdateAnimation()
    {
        bool isMoving = _moveDirection.sqrMagnitude > 0.01f;
        _animator.SetBool(ANIM_IS_MOVING, isMoving);

        if (!isMoving) return;

        _animator.SetFloat(ANIM_MOVE_X, _moveDirection.x);
        _animator.SetFloat(ANIM_MOVE_Y, _moveDirection.y);

        // side 방향일 때만 좌우 반전
        if (Mathf.Abs(_moveDirection.x) > Mathf.Abs(_moveDirection.y))
            _spriteRenderer.flipX = _moveDirection.x > 0;
    }

}
