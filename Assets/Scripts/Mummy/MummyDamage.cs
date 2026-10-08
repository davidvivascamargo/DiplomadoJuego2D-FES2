using UnityEngine;

public class MummyDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;

    public int Damage => damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Debug.Log($"Mummy hit Player. Damage: {damage}");
    }
}