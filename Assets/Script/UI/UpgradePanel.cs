using TMPro;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    public TextMeshProUGUI upgradeName;
    public TextMeshProUGUI upgradeDescription;
    SO_Upgrade actualUpgrade;

    public void Initialize (SO_Upgrade upgrade)
    {
        actualUpgrade = upgrade;
        upgradeName.text = upgrade.name;
        upgradeDescription.text = upgrade.BuildDescription();
    } 

    public void UpgradeClicked ()
    {
        GameManager.Instance.UpgradeSelected(actualUpgrade);
    }
}
