using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField, Min(1f)] private float maxEnergy = 100f;
    [SerializeField, Min(0f)] private float chargeRate = 25f;

    [SerializeField] private float currentEnergy;
    private bool isCharging;

    public float CurrentEnergy => currentEnergy;
    public float MaxEnergy => maxEnergy;
    public float EnergyNormalized => currentEnergy / maxEnergy;
    public bool IsCharging => isCharging;

    public bool CanFire => currentEnergy >= maxEnergy;

    private void Update()
    {
        if (!isCharging)
        {
            return;
        }

        currentEnergy = Mathf.MoveTowards(
            currentEnergy,
            maxEnergy,
            chargeRate * Time.deltaTime
        );
    }

    public void SetCharging(bool charging)
    {
        isCharging = charging;
    }

    public bool TryConsumeFireballEnergy()
    {
        if (!CanFire)
        {
            return false;
        }

        currentEnergy = 0f;
        return true;
    }

    private void OnDisable()
    {
        isCharging = false;
    }
}