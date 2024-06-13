using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar; 
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthFill();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthFill();
    }

    private void UpdateHealthFill()
    {
        healthBar.value = currentHealth / maxHealth;
    }
}
