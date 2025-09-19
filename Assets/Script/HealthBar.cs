using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float fillAnimationDuration = 0.5f;

    private string tweenId;

    private void Awake()
    {
        tweenId = "HealthBar" + GetInstanceID();
    }

    private void OnDisable()
    {
        DOTween.Kill(tweenId);
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    public void UpdateHealthBar(int current, int max)
    {
        if (healthFill == null || max <= 0)
        {
            return;
        }

        DOTween.Kill(tweenId);

        var percent = Mathf.Clamp01((float)current / max);
        healthFill.DOFillAmount(percent, fillAnimationDuration).SetId(tweenId);

        if (healthText != null)
        {
            healthText.text = $"{current} / {max}";
        }
    }
}
