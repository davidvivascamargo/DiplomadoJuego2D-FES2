using UnityEngine;

/// <summary>
/// Applies damage to the player when a fireball particle hits it.
/// </summary>
public class FireballDamage : MonoBehaviour
{
    [SerializeField] private int damage = 3;

    /// <summary>
    /// Called when a particle collides with another collider.
    /// </summary>
    private void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            return;
        }

        for (int i = 0; i < damage; i++)
        {
            GameManager.Instance.ReduceHealth();
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Configures the amount of damage this fireball applies.
    /// </summary>
    public void SetDamage(int amount)
    {
        damage = Mathf.Max(0, amount);
    }
}