using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private UpgradeConfig config;
    [SerializeField] private string runSceneName = "Game";

    [Header("이동속도")]
    [SerializeField] private Button moveButton;
    [SerializeField] private TextMeshProUGUI moveLabel;

    [Header("최대 체력")]
    [SerializeField] private Button healthButton;
    [SerializeField] private TextMeshProUGUI healthLabel;

    [Header("최대 무게")]
    [SerializeField] private Button weightButton;
    [SerializeField] private TextMeshProUGUI weightLabel;

    [Header("기타")]
    [SerializeField] private Button startButton;

    [Header("데이터 초기화")]
    [SerializeField] private Button resetButton;
    [SerializeField] private LobbyInventoryUI inventoryUI;   // 창고 목록 갱신용 (선택)
                                                             // LobbyUI 등에 추가

    [Header("무기고")]
    [SerializeField] private Button armoryButton;
    [SerializeField] private string armorySceneName = "Armory";
    
    



    private void Start()
    {
        moveButton.onClick.AddListener(() => Upgrade(MetaStat.MoveSpeed));
        healthButton.onClick.AddListener(() => Upgrade(MetaStat.MaxHealth));
        weightButton.onClick.AddListener(() => Upgrade(MetaStat.MaxWeight));
        startButton.onClick.AddListener(StartRun);
        resetButton.onClick.AddListener(OnResetClicked);
        armoryButton.onClick.AddListener(GoToArmory);

        Refresh();
    }

    private void OnResetClicked()
    {
        MetaProfile.ResetAll();
        Refresh();

        if (inventoryUI != null)
            inventoryUI.Refresh();

        Debug.Log("[Lobby] 저장 데이터 초기화 완료");
    }


    private void Upgrade(MetaStat stat)
    {
        MetaProfile.TryUpgrade(stat, config.Get(stat));
        Refresh();
    }

    public void GoToArmory()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(armorySceneName);
    }

    private void Refresh()
    {
        RefreshStat(MetaStat.MoveSpeed, moveButton, moveLabel);
        RefreshStat(MetaStat.MaxHealth, healthButton, healthLabel);
        RefreshStat(MetaStat.MaxWeight, weightButton, weightLabel);
    }

    private void RefreshStat(MetaStat stat, Button button, TextMeshProUGUI label)
    {
        UpgradeConfig.Stat def = config.Get(stat);
        int level = MetaProfile.GetLevel(stat);

        if (level >= def.maxLevel)
        {
            label.text = $"{def.displayName}\nLv {level}/{def.maxLevel} (MAX)";
            button.interactable = false;
            return;
        }

        int cost = def.CostAt(level);
        string itemName = def.costItem != null ? def.costItem.itemName : "-";
        int have = MetaProfile.GetStash(def.costItem);

        label.text = $"{def.displayName}  Lv {level}/{def.maxLevel}\n+{def.valuePerLevel}{def.unit}  |  {itemName} {cost} ({have})";
        button.interactable = def.costItem != null && have >= cost;
    }

    public void StartRun()
    {
        SceneManager.LoadScene(runSceneName);
    }
}
