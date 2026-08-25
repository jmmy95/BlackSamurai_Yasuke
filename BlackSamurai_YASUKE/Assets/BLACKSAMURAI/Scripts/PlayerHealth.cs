using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnDeath;

    public int CurrentHealth { get; private set; }

    private void Start()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth == 0)
        {
            OnDeath?.Invoke();
            Die();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Die()
    {
        PlayerController2D controller = GetComponent<PlayerController2D>();
        if (controller != null) controller.enabled = false;

        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat != null) combat.enabled = false;

        Invoke(nameof(RestartLevel), 1.5f);
    }

    private void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}
