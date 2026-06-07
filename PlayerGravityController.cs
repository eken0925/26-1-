using UnityEngine;

// 현재 중력 방향을 기준으로 플레이어를 이동시키는 스크립트
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGravityController2D : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveForce = 20f;     // 이동 힘
    [SerializeField] private float maxMoveSpeed = 6f;   // 최대 이동 속도

    [Header("리스폰 설정")]
    [SerializeField] private Transform startPoint;       // 리스폰 위치

    [Header("애니메이션 설정")]
    [SerializeField] private Animator animator;          // Animator
    [SerializeField] private float runThreshold = 0.1f; // 달리기 판정 기준 속도

    [Header("스프라이트 설정")]
    [SerializeField] private SpriteRenderer spriteRenderer; // 스프라이트 렌더러
    [SerializeField] private bool spriteFacesRightByDefault = true; // 기본 방향이 오른쪽인지

    private Rigidbody2D rb;          // 물리 이동용
    private float horizontalInput;   // 좌우 입력값

    private Quaternion initialWorldRotation; // 시작 회전값 저장
    private bool initialFlipX;              // 시작 flipX 저장

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 비어 있으면 자식에서 자동 찾기
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 초기 상태 저장
        initialWorldRotation = transform.rotation;

        if (spriteRenderer != null)
            initialFlipX = spriteRenderer.flipX;
    }

    private void Update()
    {
        // 좌우 입력 받기
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 이동 방향에 맞게 스프라이트 반전
        UpdateSpriteFlip();
    }

    private void FixedUpdate()
    {
        // 현재 중력 방향 구하기
        Vector2 gravityDir = GetGravityDirection();

        // 중력에 수직인 방향을 이동축으로 사용
        Vector2 moveAxis = new Vector2(-gravityDir.y, gravityDir.x);

        // 이동 힘 적용
        rb.AddForce(moveAxis * horizontalInput * moveForce);

        // 중력 방향 속도는 유지
        Vector2 gravityVelocity = Vector2.Dot(rb.linearVelocity, gravityDir) * gravityDir;

        // 이동축 속도만 따로 계산 후 제한
        float moveSpeed = Vector2.Dot(rb.linearVelocity, moveAxis);
        moveSpeed = Mathf.Clamp(moveSpeed, -maxMoveSpeed, maxMoveSpeed);

        Vector2 moveVelocity = moveAxis * moveSpeed;

        // 최종 속도 적용
        rb.linearVelocity = gravityVelocity + moveVelocity;

        // 애니메이션 갱신
        UpdateRunAnimation(moveSpeed);
    }

    private Vector2 GetGravityDirection()
    {
        // 현재 중력 방향 반환, 없으면 아래 방향
        return Physics2D.gravity.sqrMagnitude > 0.0001f
            ? Physics2D.gravity.normalized
            : Vector2.down;
    }

    private void UpdateRunAnimation(float moveSpeed)
    {
        if (animator == null)
            return;

        // 일정 속도 이상이면 달리기 상태
        bool isRun = Mathf.Abs(moveSpeed) > runThreshold && Mathf.Abs(horizontalInput) > 0.01f;
        animator.SetBool("isRun", isRun);
    }

    private void UpdateSpriteFlip()
    {
        // 스프라이트가 없거나 입력이 없으면 종료
        if (spriteRenderer == null || Mathf.Abs(horizontalInput) < 0.01f)
            return;

        Vector2 gravityDir = GetGravityDirection();
        Vector2 moveAxis = new Vector2(-gravityDir.y, gravityDir.x);

        // 실제 바라봐야 할 방향
        Vector2 desiredDir = moveAxis * Mathf.Sign(horizontalInput);

        // flipX가 꺼진 상태 기준 기본 방향
        Vector2 forwardNoFlip = spriteFacesRightByDefault
            ? (Vector2)transform.right
            : -(Vector2)transform.right;

        // 방향이 반대면 flipX 적용
        spriteRenderer.flipX = Vector2.Dot(forwardNoFlip, desiredDir) < 0f;
    }

    public void RespawnToStart()
    {
        if (startPoint == null)
        {
            Debug.LogWarning("Start Point가 연결되지 않았습니다.");
            return;
        }

        // 리스폰 시 입력값 초기화
        horizontalInput = 0f;

        // 리스폰 시 중력을 다시 아래 방향으로 초기화
        float gravityMagnitude = Physics2D.gravity.sqrMagnitude > 0.0001f
            ? Physics2D.gravity.magnitude
            : 9.8f;

        Physics2D.gravity = Vector2.down * gravityMagnitude;

        // 속도 초기화
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // 위치 이동
        rb.position = startPoint.position;

        // 회전 복구
        transform.rotation = initialWorldRotation;

        // 스프라이트 방향 복구
        if (spriteRenderer != null)
            spriteRenderer.flipX = initialFlipX;

        // 애니메이션 초기화
        if (animator != null)
            animator.SetBool("isRun", false);
    }
}
