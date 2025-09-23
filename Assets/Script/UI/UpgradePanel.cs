using TMPro;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI upgradeDescription;

    private SO_Upgrade upgrade;

    public void Initialize(SO_Upgrade newUpgrade)
    {
        upgrade = newUpgrade;

        if (upgradeName != null)
        {
            upgradeName.text = upgrade != null ? upgrade.name : string.Empty;
        }

        if (upgradeDescription != null)
        {
            upgradeDescription.text = upgrade != null ? upgrade.BuildDescription() : string.Empty;
        }
    }

    public void UpgradeClicked()
    {
        if (upgrade == null)
        {
            return;
        }

        GameManager.Instance.UpgradeSelected(upgrade);
    }
}
