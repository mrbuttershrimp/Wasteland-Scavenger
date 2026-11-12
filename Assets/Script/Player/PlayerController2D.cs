using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 40f;
    [Range(0f, 1f)]
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    [Header("점프 설정")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;   // 바닥 + 천장(일어설 때 막힐 대상)

    [Header("앉기 설정")]
    [Tooltip("앉았을 때 콜라이더 높이 비율 (0.5 = 절반)")]
    [SerializeField] private float crouchHeightRatio = 0.5f;

    [Header("화면 제한 / 맵 제한")]
    [SerializeField] private bool clampToMap = true;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Vector2 mapPadding = new Vector2(0.5f, 0f);

    [Header("상체 연동")]
    [SerializeField] private Transform upperRoot;          // UpperBody + 어깨 + 팔 + 무기를 묶은 부모
    [Tooltip("1 = 콜라이더 줄어든 만큼 상체도 내림")]
    [SerializeField] private float upperCrouchFollow = 1f;

    [Header("하체 정렬")]
    [SerializeField] private SpriteRenderer lowerBodyRenderer; // LowerBody의 SpriteRenderer

    private Vector3 lowerStartLocalPos;
    private Vector3 upperStartLocalPos;
    private Vector3 upperRootStartPos;


    private Rigidbody2D rb;
    private BoxCollider2D box;

    private float moveInput;
    private bool jumpPressed;
    private bool crouchHeld;
    private bool isGrounded;
    private bool isCrouching;
    private bool inputEnabled = true;

    private Vector2 originalSize;
    private Vector2 originalOffset;
    private Vector2 mapMin;
    private Vector2 mapMax;

    // 애니메이터 등 외부에서 읽는 상태
    public bool IsGrounded => isGrounded;
    public bool IsCrouching => isCrouching;
    public float VerticalVelocity => rb.linearVelocity.y;

    public void SetInputEnabled(bool on)
    {
        inputEnabled = on;
        if (!on)
        {
            moveInput = 0f;
            jumpPressed = false;
            crouchHeld = false;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }


    public void EnterMap(SpriteRenderer newBackground, Vector2 spawnPos)
    {
        background = newBackground;
        if (background != null)
        {
            Bounds b = background.bounds;
            mapMin = b.min;
            mapMax = b.max;
        }

        // 속도와 위치를 초기화해서 스폰 지점에 배치
        rb.linearVelocity = Vector2.zero;
        rb.position = spawnPos;
        transform.position = spawnPos;
        jumpPressed = false;

        if (isCrouching) Stand();
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();

        rb.gravityScale = 1f;
        rb.freezeRotation = true;

        originalSize = box.size;
        originalOffset = box.offset;

        if (lowerBodyRenderer != null)
            lowerStartLocalPos = lowerBodyRenderer.transform.localPosition;
        if (upperRoot != null)
            upperStartLocalPos = upperRoot.localPosition;


    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null || !inputEnabled) return;

        moveInput = 0f;
        if (kb.aKey.isPressed) moveInput -= 1f;
        if (kb.dKey.isPressed) moveInput += 1f;

        if (kb.wKey.wasPressedThisFrame)
            jumpPressed = true;

        crouchHeld = kb.sKey.isPressed;
    }

    private void FixedUpdate()
    {
        isGrounded = groundCheck != null &&
                     Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        HandleCrouch();

        // 좌우 이동 (앉으면 느리게)
        float speed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        float targetVelX = moveInput * speed;
        float newVelX = Mathf.MoveTowards(rb.linearVelocity.x, targetVelX, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newVelX, rb.linearVelocity.y);

        // 점프 (바닥에 있고 앉지 않은 상태)
        if (jumpPressed && isGrounded && !isCrouching)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpPressed = false;

        if (clampToMap && background != null)
        {
            Vector2 pos = rb.position;
            pos.x = Mathf.Clamp(pos.x, mapMin.x + mapPadding.x, mapMax.x - mapPadding.x);
            pos.y = Mathf.Clamp(pos.y, mapMin.y + mapPadding.y, mapMax.y - mapPadding.y);
            rb.position = pos;
        }
    }

    private void HandleCrouch()
    {
        if (crouchHeld && isGrounded)
        {
            // S를 누르고 있는 동안만 앉기
            if (!isCrouching)
                Crouch();
        }
        else if (isCrouching && CanStand())
        {
            // S를 떼면 머리 위가 비어 있을 때 일어나기
            Stand();
        }
    }

    private void Crouch()
    {
        isCrouching = true;
        float newHeight = originalSize.y * crouchHeightRatio;
        box.size = new Vector2(originalSize.x, newHeight);
        // 발 위치는 고정하고 위쪽에서 줄어들도록 offset 이동
        box.offset = new Vector2(originalOffset.x,
            originalOffset.y - (originalSize.y - newHeight) * 0.5f);
    }

    private void Stand()
    {
        isCrouching = false;
        box.size = originalSize;
        box.offset = originalOffset;
    }

    // 앉은 콜라이더 머리 위에 서 있을 때 높이만큼의 빈 공간이 있는지 확인
    private bool CanStand()
    {
        float crouchedHeight = originalSize.y * crouchHeightRatio;
        float gapHeight = originalSize.y - crouchedHeight;

        // 앉은 상태의 콜라이더 윗면 (로컬 Y)
        float crouchedTopY = originalOffset.y - (originalSize.y - crouchedHeight) * 0.5f + crouchedHeight * 0.5f;

        // 일어섰을 때 늘어나는 부분(gap)만 검사
        Vector2 center = (Vector2)transform.position +
                         new Vector2(originalOffset.x, crouchedTopY + gapHeight * 0.5f);
        Vector2 size = new Vector2(originalSize.x * 0.9f, gapHeight * 0.9f);

        return Physics2D.OverlapBox(center, size, 0f, groundLayer) == null;
    }


    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        BoxCollider2D bc = box != null ? box : GetComponent<BoxCollider2D>();
        if (bc == null) return;

        // 일어설 때 필요한 빈 공간 (빨간 박스)
        Vector2 originalSizeLocal = Application.isPlaying ? originalSize : bc.size;
        float crouchedHeight = originalSizeLocal.y * crouchHeightRatio;
        float gapHeight = originalSizeLocal.y - crouchedHeight;
        float crouchedTopY = bc.offset.y - (originalSizeLocal.y - crouchedHeight) * 0.5f + crouchedHeight * 0.5f;

        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position +
                         new Vector2(bc.offset.x, crouchedTopY + gapHeight * 0.5f);
        Gizmos.DrawWireCube(center, new Vector2(originalSizeLocal.x * 0.9f, gapHeight * 0.9f));
    }
    private void LateUpdate()
    {
        if (lowerBodyRenderer == null) return;

        Transform lowerT = lowerBodyRenderer.transform;

        // 매 프레임 시작 위치로 되돌린 뒤 다시 계산
        lowerT.localPosition = lowerStartLocalPos;
        if (upperRoot != null)
            upperRoot.localPosition = upperStartLocalPos;

        if (!isCrouching || groundCheck == null) return;

        // 하체: 스프라이트 맨 아래를 GroundCheck에 맞춤
        float delta = groundCheck.position.y - lowerBodyRenderer.bounds.min.y;
        lowerT.position += Vector3.up * delta;

        // 상체: 하체와 같은 양만큼 내리되 upperCrouchFollow로 연결 비율 조정
        if (upperRoot != null)
            upperRoot.position += Vector3.up * delta * upperCrouchFollow;
    }

}
