using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimController : MonoBehaviour
{
    [System.Serializable]
    public class ArmPart
    {
        public Transform shoulder;          // 회전 피벗 (빈 오브젝트)
        public SpriteRenderer armRenderer;  // 팔 스프라이트
        public SpriteRenderer weaponRenderer; // 무기 (없으면 비워 둠)
    }

    [Header("비주얼 루트 (좌우 반전 대상)")]
    [SerializeField] private Transform visualRoot;

    [Header("팔 목록 (0: 뒷팔, 1: 앞팔 등)")]
    [SerializeField] private ArmPart[] arms;

    [Header("옵션")]
    [SerializeField] private float centerOffset = 0f;

    public bool IsFacingLeft { get; private set; }

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || cam == null) return;

        Vector3 mouseWorld = GetMouseWorldPosition(mouse.position.ReadValue());

        // 1. 좌우 판정 및 VisualRoot 미러링
        IsFacingLeft = mouseWorld.x < transform.position.x + centerOffset;
        visualRoot.localScale = new Vector3(IsFacingLeft ? -1f : 1f, 1f, 1f);

        // 2. 각 어깨마다 마우스를 향해 회전
        foreach (ArmPart part in arms)
        {
            if (part.shoulder == null) continue;

            Vector2 aimDir = (Vector2)mouseWorld - (Vector2)part.shoulder.position;
            float localX = IsFacingLeft ? -aimDir.x : aimDir.x;
            float localAngle = Mathf.Atan2(aimDir.y, localX) * Mathf.Rad2Deg;

            part.shoulder.localRotation = Quaternion.Euler(0f, 0f, localAngle);

            if (part.armRenderer != null) part.armRenderer.flipY = false;
            if (part.weaponRenderer != null) part.weaponRenderer.flipY = false;
        }
    }

    private Vector3 GetMouseWorldPosition(Vector2 screenPos)
    {
        Vector3 screen = new Vector3(screenPos.x, screenPos.y, -cam.transform.position.z);
        return cam.ScreenToWorldPoint(screen);
    }
}
