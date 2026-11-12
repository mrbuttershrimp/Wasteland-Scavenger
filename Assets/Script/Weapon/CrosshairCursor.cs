using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CrosshairCursor : MonoBehaviour
{
    [Header("크로스헤어 모양")]
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float lineLength = 12f;   // 선 길이 (px)
    [SerializeField] private float lineThickness = 2f; // 선 두께 (px)
    [SerializeField] private float gap = 6f;           // 중앙에서 떨어진 거리 (px)
    [SerializeField] private bool showCenterDot = true;

    [Header("옵션")]
    [SerializeField] private bool confineCursorToWindow = true; // 커서가 창 밖으로 나가지 않도록
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape; // 사용 안 함 (Input System 키로 처리)

    private RectTransform rect;
    private bool cursorMode; // true면 일반 커서 표시

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        BuildCrosshair();
    }

    private void OnEnable()
    {
        ApplyCursorState(false);
    }

    private void OnDisable()
    {
        // 씬 종료나 비활성화 시 커서 복구
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            ApplyCursorState(!cursorMode);

        if (Mouse.current == null) return;

        // 커서 위치를 따라감 (Screen Space - Overlay 캔버스 기준 픽셀 좌표)
        if (!cursorMode)
            rect.position = Mouse.current.position.ReadValue();
    }

    private void ApplyCursorState(bool showCursor)
    {
        cursorMode = showCursor;
        Cursor.visible = showCursor;
        Cursor.lockState = showCursor
            ? CursorLockMode.None
            : (confineCursorToWindow ? CursorLockMode.Confined : CursorLockMode.None);

        foreach (Transform child in transform)
            child.gameObject.SetActive(!showCursor);
    }

    private void BuildCrosshair()
    {
        float half = gap + lineLength * 0.5f;

        CreateBar("Top", new Vector2(0f, half), new Vector2(lineThickness, lineLength));
        CreateBar("Bottom", new Vector2(0f, -half), new Vector2(lineThickness, lineLength));
        CreateBar("Left", new Vector2(-half, 0f), new Vector2(lineLength, lineThickness));
        CreateBar("Right", new Vector2(half, 0f), new Vector2(lineLength, lineThickness));

        if (showCenterDot)
            CreateBar("Dot", Vector2.zero, new Vector2(lineThickness, lineThickness));
    }

    private void CreateBar(string barName, Vector2 localPos, Vector2 size)
    {
        GameObject go = new GameObject(barName, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);

        RectTransform r = go.GetComponent<RectTransform>();
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = localPos;
        r.sizeDelta = size;

        Image img = go.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = false; // 클릭 막지 않도록
    }
}
