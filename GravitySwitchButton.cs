using System.Collections;
using UnityEngine;

// 버튼/크리스탈 상호작용으로 중력을 회전시키는 스크립트
public class GravitySwitchButton2D : MonoBehaviour
{
    [Header("중력 설정")]
    [SerializeField] private float defaultGravityStrength = 9.8f;   // 기본 중력 세기
    [SerializeField] private bool rotatePlayerToMatchGravity = false; // 플레이어도 같이 회전할지

    [Header("입력 설정")]
    [SerializeField] private KeyCode interactKey = KeyCode.E; // 상호작용 키

    [Header("크리스탈 스프라이트 설정")]
    [SerializeField] private SpriteRenderer crystalRenderer; // 크리스탈 렌더러
    [SerializeField] private Sprite normalSprite;            // 기본 스프라이트
    [SerializeField] private Sprite cooldownSprite;          // 쿨타임 중 스프라이트

    [Header("쿨타임 설정")]
    [SerializeField] private float cooldownDuration = 2f;    // 재사용 대기시간

    private bool playerInRange;      // 플레이어가 범위 안에 있는지
    private Transform playerTransform; // 플레이어 Transform
    private bool isCooldown;         // 현재 쿨타임인지

    private void Awake()
    {
        // 렌더러가 비어 있으면 자식에서 자동 찾기
        if (crystalRenderer == null)
            crystalRenderer = GetComponentInChildren<SpriteRenderer>();

        // 기본 스프라이트가 없으면 현재 스프라이트를 사용
        if (crystalRenderer != null && normalSprite == null)
            normalSprite = crystalRenderer.sprite;
    }

    private void Update()
    {
        // 플레이어가 범위 안에 있고, 쿨타임이 아닐 때만 작동
        if (!playerInRange || isCooldown)
            return;

        // 상호작용 키 입력 시 버튼 실행
        if (Input.GetKeyDown(interactKey))
            StartCoroutine(ActivateButtonRoutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 들어오면 상호작용 가능 상태
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;
        playerTransform = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 나가면 상호작용 불가
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        playerTransform = null;
    }

    private IEnumerator ActivateButtonRoutine()
    {
        isCooldown = true;

        // 중력 회전
        RotateGravityClockwise();

        // 쿨타임 스프라이트로 변경
        if (crystalRenderer != null && cooldownSprite != null)
            crystalRenderer.sprite = cooldownSprite;

        yield return new WaitForSeconds(cooldownDuration);

        // 원래 스프라이트로 복구
        if (crystalRenderer != null && normalSprite != null)
            crystalRenderer.sprite = normalSprite;

        isCooldown = false;
    }

    private void RotateGravityClockwise()
    {
        Vector2 currentGravity = Physics2D.gravity;

        // 현재 중력 방향, 없으면 아래 방향 사용
        Vector2 currentDirection = currentGravity.sqrMagnitude > 0.0001f
            ? currentGravity.normalized
            : Vector2.down;

        // 현재 중력 크기 유지, 없으면 기본값 사용
        float gravityMagnitude = currentGravity.sqrMagnitude > 0.0001f
            ? currentGravity.magnitude
            : Mathf.Abs(defaultGravityStrength);

        // 시계 방향 90도 회전
        Vector2 rotatedDirection = new Vector2(-currentDirection.y, currentDirection.x);

        // 새 중력 적용
        Physics2D.gravity = rotatedDirection * gravityMagnitude;

        // 옵션이 켜져 있으면 플레이어도 회전
        if (rotatePlayerToMatchGravity && playerTransform != null)
            playerTransform.up = -rotatedDirection;
    }
}
