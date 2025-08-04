using UnityEngine;
using UnityEngine.UI;

public class PetEnergy : MonoBehaviour
{
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float drainRate = 5f; // energy drained per second
    public Slider energySlider; // assign in inspector

    void Start()
    {
        currentEnergy = maxEnergy;
        if (energySlider != null)
            energySlider.maxValue = maxEnergy;
    }

    void Update()
    {
        // Drain energy over time
        currentEnergy -= drainRate * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        if (energySlider != null)
            energySlider.value = currentEnergy;
    }

    // Call this to refill energy
    public void PlayWithPet(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    public void DebugCheck()
    {
        Debug.Log("Current energy: " + currentEnergy);
    }
}