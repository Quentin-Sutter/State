using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public SO_Upgrade[] upgradeAvaibleAfterWave;

    public SO_Upgrade[] GetUpgradeAfterWave (int amount)
    {
        if (upgradeAvaibleAfterWave.Length < amount)
        {
            Debug.LogWarning("Not enough upgrade to pull");
        }

        SO_Upgrade[] upgrades = new SO_Upgrade[amount];

        if (upgradeAvaibleAfterWave.Length == amount)
        {
            return upgradeAvaibleAfterWave;
        }

        List<SO_Upgrade> avaibleUpgrade = upgradeAvaibleAfterWave.ToList ();

        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, avaibleUpgrade.Count);
            SO_Upgrade upgrade = avaibleUpgrade[index];
            avaibleUpgrade.RemoveAt(index);
            upgrades[i] = upgrade;
        } 
        return upgrades;
    }
}
