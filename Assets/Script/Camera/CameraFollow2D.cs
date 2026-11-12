using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraFollow2D : MonoBehaviour
{
    [Header("대상")]
    [SerializeField] private Transform target;          // Player
    [SerializeField] private Rigidbody2D targetRb;      // 비워두면 자동 탐색

    [Header("추적 방식")]
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Vector2 offset = Vector2.zero;

    [Header("룩어헤드 (이동 방향으로 카메라 선행)")]
    [SerializeField] private bool useLookAhead = true;
    [SerializeField] private float lookAheadDistance = 3f;
    [SerializeField] private float lookAheadSmoothTime = 0.4f;
    [SerializeField] private float maxSpeedForLookAhead = 6f;

    [Header("정조준 (우클릭 홀드)")]
    [Tooltip("카메라가 마우스 방향으로 최대 이동하는 거리 (유닛)")]
    [SerializeField] private float aimOffsetDistance = 4f;
    [Tooltip("마우스까지 거리 중 카메라가 따라가는 비율 (0.5 = 절반)")]
    [Range(0f, 1f)][SerializeField] private float aimFollowRatio = 0.5f;
    [Tooltip("1보다 작을수록 줌인 (예: 0.85)")]
    [SerializeField] private float aimZoomMultiplier = 0.85f;
    [SerializeField] private float aimSmoothTime = 0.2f;

    [Header("데드존 (세로 방향 완충)")]
    [SerializeField] private Vector2 deadZone = new Vector2(0.5f, 0.3f);

    [Header("Y축 추적")]
    [SerializeField] private bool followY = true;

    [Header("월드 경계 (선택)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds = new Vector2(-100f, -100f);
    [SerializeField] private Vector2 maxBounds = new Vector2(100f, 100f);

    private Vector3 velocity;
    private float lookAheadCurrent;
    private float lookAheadVelocity;

    private Vector2 aimOffsetCurrent;
    private Vector2 aimOffsetVelocity;
    private float aimAmount;          // 0 = 평상시, 1 = 정조준 완료
    private float aimAmountVelocity;

    private float baseOrthoSize;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        baseOrthoSize = cam.orthographicSize;

        if (targetRb == null && target != null)
            targetRb = target.GetComponent<Rigidbody2D>();
    }

    public void SetBounds(Vector2 min, Vector2 max)
    {
        useBounds = true;
        minBounds = min;
        maxBounds = max;
    }

    public void Snap()
    {
        if (target == null) return;

        Vector3 p = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
        transform.position = p;
        velocity = Vector3.zero;
        lookAheadCurrent = 0f;
        lookAheadVelocity = 0f;
    }


    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = transform.position;

        // 1. 룩어헤드
        float lookAheadTarget = 0f;
        if (useLookAhead && targetRb != null)
        {
            float vx = targetRb.linearVelocity.x;
            float ratio = Mathf.Clamp(vx / maxSpeedForLookAhead, -1f, 1f);
            lookAheadTarget = ratio * lookAheadDistance;
        }
        lookAheadCurrent = Mathf.SmoothDamp(lookAheadCurrent, lookAheadTarget,
                                            ref lookAheadVelocity, lookAheadSmoothTime);

        // 2. 정조준 (우클릭 홀드 여부)
        bool aiming = Mouse.current != null && Mouse.current.rightButton.isPressed;

        aimAmount = Mathf.SmoothDamp(aimAmount, aiming ? 1f : 0f,
                                     ref aimAmountVelocity, aimSmoothTime);

        Vector2 aimOffsetTarget = Vector2.zero;
        if (aiming)
        {
            Vector2 mouseWorld = GetMouseWorld();
            Vector2 toMouse = mouseWorld - (Vector2)target.position;
            aimOffsetTarget = Vector2.ClampMagnitude(toMouse * aimFollowRatio, aimOffsetDistance);
        }
        aimOffsetCurrent = Vector2.SmoothDamp(aimOffsetCurrent, aimOffsetTarget,
                                              ref aimOffsetVelocity, aimSmoothTime);

        // 3. 추적 목표 = 플레이어 + 오프셋 + 룩어헤드 + 정조준 오프셋
        Vector2 targetPos = (Vector2)target.position + offset + aimOffsetCurrent;
        targetPos.x += lookAheadCurrent;

        // 4. 데드존 (세로)
        float desiredX = targetPos.x;
        float desiredY = pos.y;
        if (followY)
        {
            float dy = targetPos.y - pos.y;
            if (Mathf.Abs(dy) > deadZone.y)
                desiredY = targetPos.y - Mathf.Sign(dy) * deadZone.y;
        }

        Vector3 desired = new Vector3(desiredX, desiredY, pos.z);

        // 5. 부드러운 추적
        Vector3 newPos = Vector3.SmoothDamp(pos, desired, ref velocity, smoothTime);

        // 6. 정조준 줌 (부드럽게 보간)
        cam.orthographicSize = Mathf.Lerp(baseOrthoSize,
                                          baseOrthoSize * aimZoomMultiplier,
                                          aimAmount);

        // 7. 월드 경계 (줌 변화 반영해서 매 프레임 계산)
        if (useBounds)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;
            newPos.x = Mathf.Clamp(newPos.x, minBounds.x + halfW, maxBounds.x - halfW);
            newPos.y = Mathf.Clamp(newPos.y, minBounds.y + halfH, maxBounds.y - halfH);
        }

        transform.position = newPos;
    }

    private Vector2 GetMouseWorld()
    {
        Vector3 screen = new Vector3(
            Mouse.current.position.ReadValue().x,
            Mouse.current.position.ReadValue().y,
            -cam.transform.position.z);
        return cam.ScreenToWorldPoint(screen);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZone.x * 2f, deadZone.y * 2f, 0f));

        if (useLookAhead)
        {
            Gizmos.color = Color.green;
            Vector3 c = transform.position;
            Gizmos.DrawLine(c + Vector3.left * lookAheadDistance, c + Vector3.right * lookAheadDistance);
        }

        if (useBounds)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) * 0.5f, (minBounds.y + maxBounds.y) * 0.5f, 0f);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
