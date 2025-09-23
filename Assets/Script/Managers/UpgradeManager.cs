using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradeManager : MonoBehaviour
{
    [FormerlySerializedAs("upgradeAvaibleAfterWave")]
    [SerializeField] private SO_Upgrade[] upgradesAvailableAfterWave;

    public SO_Upgrade[] GetUpgradeAfterWave(int amount)
    {
        if (upgradesAvailableAfterWave == null || upgradesAvailableAfterWave.Length == 0)
        {
            Debug.LogWarning("No upgrades configured in UpgradeManager.");
            return System.Array.Empty<SO_Upgrade>();
        }

        amount = Mathf.Clamp(amount, 0, upgradesAvailableAfterWave.Length);

        if (upgradesAvailableAfterWave.Length == amount)
        {
            return (SO_Upgrade[])upgradesAvailableAfterWave.Clone();
        }

        var availableUpgrades = new List<SO_Upgrade>(upgradesAvailableAfterWave);
        var selection = new SO_Upgrade[amount];

        for (var i = 0; i < amount; i++)
        {
            var index = Random.Range(0, availableUpgrades.Count);
            selection[i] = availableUpgrades[index];
            availableUpgrades.RemoveAt(index);
        }

        return selection;
    }
}
