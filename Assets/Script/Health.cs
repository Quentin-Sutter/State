using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private HealthBar healthBar;

    [SerializeField] private UnityEvent<int, int> onHealthChanged = new UnityEvent<int, int>();
    [SerializeField] private UnityEvent onDeath = new UnityEvent();

    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }

    public UnityEvent<int, int> OnHealthChanged => onHealthChanged;
    public UnityEvent OnDeath => onDeath;

    public void Initialize(int max)
    {
        MaxHealth = Mathf.Max(1, max);
        CurrentHealth = MaxHealth;

        if (healthBar != null)
        {
            onHealthChanged.AddListener(healthBar.UpdateHealthBar);
        }

        NotifyHealthChanged();
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - Mathf.Abs(amount), 0, MaxHealth);
        NotifyHealthChanged();

        if (CurrentHealth == 0)
        {
            onDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + Mathf.Abs(amount), 0, MaxHealth);
        NotifyHealthChanged();
    }

    private void NotifyHealthChanged()
    {
        onHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}
