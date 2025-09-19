using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Image healthFill; // Drag & drop ton Image de barre de vie 
    public TextMeshProUGUI number;

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    public void UpdateHealthBar(int current, int max)
    {
        DOTween.Kill("HealthBar" + gameObject.GetInstanceID());
        float percent = (float)current / (float)max;
        healthFill.DOFillAmount(percent, 0.5f).SetId("HealthBar" + gameObject.GetInstanceID());
        number.text = current + " / " + max;
    }
}
