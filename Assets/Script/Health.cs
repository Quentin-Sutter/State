using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Scripts")]
    public HealthBar healthBar;

    int maxHealth;
    public int currentHealth;

    // Évènements publics
    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnDeath;
     

    public void Initialize (int max)
    {
        maxHealth = max;
        currentHealth = maxHealth;
        OnHealthChanged.AddListener(healthBar.UpdateHealthBar);
        HealthChanged();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        HealthChanged();

        if (currentHealth == 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        HealthChanged();
    } 

    public void HealthChanged ()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    } 
}
