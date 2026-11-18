using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button lobbyButton;

    [Header("로비 씬 이름")]
    [SerializeField] private string lobbySceneName = "Lobby";

    private void Awake()
    {
        panel.SetActive(false);
        lobbyButton.onClick.AddListener(GoToLobby);
    }

    public void Show()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;   // 화면 정지
    }

    private void GoToLobby()
    {
        RunExtraction.Die(lobbySceneName);
    }



}
