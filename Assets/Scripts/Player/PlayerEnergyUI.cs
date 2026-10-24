using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerEnergyUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerEnergy playerEnergy;
    [SerializeField] private Slider energySlider;
    [SerializeField] private TMP_Text energyPercent;

    private void Awake()
    {
        if (energySlider == null)
        {
            energySlider = GetComponent<Slider>();
        }
    }

    private void Update()
    {
        if (playerEnergy == null || energySlider == null)
        {
            return;
        }

        float normalizedEnergy = playerEnergy.EnergyNormalized;

        energySlider.value = normalizedEnergy;

        if (energyPercent != null)
        {
            int percentage = Mathf.RoundToInt(normalizedEnergy * 100f);
            energyPercent.text = percentage + "%";
        }
    }
}