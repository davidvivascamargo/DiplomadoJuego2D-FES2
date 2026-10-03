using UnityEngine;

public class ElectricWaterDamage : MonoBehaviour
{
    [Header("Electric Damage")]
    [SerializeField] private float damageInterval = 1f;

    private int _playerColliderCount;
    private float _nextDamageTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        _playerColliderCount++;

        if (_playerColliderCount == 1)
        {
            ApplyDamage();

            _nextDamageTime = Time.time + damageInterval;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        _playerColliderCount--;

        _playerColliderCount = Mathf.Max(
            _playerColliderCount,
            0
        );
    }

    private void Update()
    {
        if (_playerColliderCount <= 0)
        {
            return;
        }

        if (Time.time >= _nextDamageTime)
        {
            ApplyDamage();

            _nextDamageTime = Time.time + damageInterval;
        }
    }

    private void ApplyDamage()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.ReduceHealth();
    }
}