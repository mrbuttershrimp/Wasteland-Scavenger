using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ExtractionUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RunDirector director;
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerController2D player;

    [Header("확인 팝업")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("로비 씬 이름")]
    [SerializeField] private string lobbySceneName = "Lobby";

    private bool open;

    private void Awake()
    {
        panel.SetActive(false);
        yesButton.onClick.AddListener(OnYes);
        noButton.onClick.AddListener(Close);
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;

        // 팝업이 열린 상태에서 ESC는 계속 진행(닫기)
        if (open)
        {
            Close();
            return;
        }

        if (GamePause.IsPaused) return;
        if (director == null || director.CurrentPreset == null) return;

        switch (director.CurrentPreset.roomType)
        {
            case RoomType.Combat:
                CombatRoom room = FindFirstObjectByType<CombatRoom>();
                if (room != null && room.IsCleared)
                    Open();
                break;

            case RoomType.Rest:
            case RoomType.Shop:
                // 휴식방, 상인방은 확인 없이 바로 귀환
                RunExtraction.Extract(inventory, lobbySceneName);
                break;
        }
    }

    private void Open()
    {
        open = true;
        panel.SetActive(true);
        GamePause.Push();
        if (player != null) player.SetInputEnabled(false);
    }

    private void Close()
    {
        open = false;
        panel.SetActive(false);
        GamePause.Pop();
        if (player != null) player.SetInputEnabled(true);
    }

    private void OnYes()
    {
        open = false;
        panel.SetActive(false);
        RunExtraction.Extract(inventory, lobbySceneName);
    }
}
