using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private string format = "{0} / {1}";

    private void Start()
    {
        if (playerHealth == null) return;
        playerHealth.Changed += Refresh;
        Refresh(playerHealth.Current, playerHealth.Max);
    }

    private void OnDestroy()
    {
        if (playerHealth != null) playerHealth.Changed -= Refresh;
    }

    private void Refresh(int current, int max)
    {
        if (healthText != null)
            healthText.text = string.Format(format, current, max);
    }
}
