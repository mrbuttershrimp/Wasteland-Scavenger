using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WeaponUpgradeUI : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private WeaponUpgradeConfig config;
    [SerializeField] private WeaponData baseWeapon;      // 표시용 기준 무기
    [SerializeField] private string weaponId = "AR";
    [SerializeField] private string lobbySceneName = "Lobby";

    [Header("탄창")]
    [SerializeField] private Button magButton;
    [SerializeField] private TextMeshProUGUI magLabel;

    [Header("데미지")]
    [SerializeField] private Button damageButton;
    [SerializeField] private TextMeshProUGUI damageLabel;

    [Header("발사 속도")]
    [SerializeField] private Button rateButton;
    [SerializeField] private TextMeshProUGUI rateLabel;

    [Header("기타")]
    [SerializeField] private Button backButton;

    private void Start()
    {
        magButton.onClick.AddListener(() => Upgrade(WeaponStat.MagSize));
        damageButton.onClick.AddListener(() => Upgrade(WeaponStat.Damage));
        rateButton.onClick.AddListener(() => Upgrade(WeaponStat.FireRate));
        backButton.onClick.AddListener(() => SceneManager.LoadScene(lobbySceneName));
        Refresh();
    }

    private void Upgrade(WeaponStat stat)
    {
        WeaponUpgradeProfile.TryUpgrade(weaponId, stat, config.Get(stat));
        Refresh();
    }

    private void Refresh()
    {
        RefreshStat(WeaponStat.MagSize, magButton, magLabel, $"{baseWeapon.magazineSize}");
        RefreshStat(WeaponStat.Damage, damageButton, damageLabel, $"{baseWeapon.damage}");
        RefreshStat(WeaponStat.FireRate, rateButton, rateLabel, $"{baseWeapon.fireRate:F1}");
    }

    private void RefreshStat(WeaponStat stat, Button button, TextMeshProUGUI label, string baseValueText)
    {
        WeaponUpgradeConfig.Stat def = config.Get(stat);
        int level = WeaponUpgradeProfile.GetLevel(weaponId, stat);

        if (level >= def.maxLevel)
        {
            label.text = $"{def.displayName}  {baseValueText}\nLv {level}/{def.maxLevel} (MAX)";
            button.interactable = false;
            return;
        }

        int cost = def.CostAt(level);
        int have = MetaProfile.GetStash(def.costItem);
        string itemName = def.costItem != null ? def.costItem.itemName : "-";

        label.text = $"{def.displayName}  {baseValueText}{def.unit}  Lv {level}/{def.maxLevel}\n" +
                     $"+{def.valuePerLevel}{def.unit}  |  {itemName} {cost} ({have})";
        button.interactable = def.costItem != null && have >= cost;
    }
}
