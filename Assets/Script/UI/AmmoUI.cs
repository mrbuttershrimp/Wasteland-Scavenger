using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Weapon weapon;          // 탄약을 읽을 총기
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("표시 형식")]
    [SerializeField] private string format = "{0} / {1}"; // {0} 현재 탄, {1} 최대 탄
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowAmmoColor = Color.red;
    [Tooltip("이 비율 이하이면 lowAmmoColor로 표시")]
    [Range(0f, 1f)][SerializeField] private float lowAmmoRatio = 0.25f;

    private int lastAmmo = -1;
    private int lastMax = -1;

    private void Update()
    {
        if (weapon == null || weapon.Data == null || ammoText == null) return;

        int current = weapon.CurrentAmmo;
        int max = weapon.Data.magazineSize;

        // 값이 바뀔 때만 텍스트 갱신
        if (current == lastAmmo && max == lastMax) return;
        lastAmmo = current;
        lastMax = max;

        ammoText.text = string.Format(format, current, max);

        float ratio = max > 0 ? (float)current / max : 0f;
        ammoText.color = ratio <= lowAmmoRatio ? lowAmmoColor : normalColor;
    }
}
