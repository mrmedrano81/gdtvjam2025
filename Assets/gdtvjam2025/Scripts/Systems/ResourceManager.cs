using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Resource Settings")]
    public int startingMoney = 1000;
    public int startingOre = 1000;
    public int currentMoney { get; private set; }
    public int currentOre { get; private set; }

    private float currentOrePerSecond = 0f;

    [Header("Gatherer Settings")]
    public float gathererOrePerSecond = 1f;

    public float GetcurrentOrePerSecond()
    {
        return 0f;
    }

    private void SetcurrentOrePerSecond(float value)
    {
        currentOrePerSecond = value;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
    }

    public void RemoveMoney(int amount)
    {
        currentMoney -= amount;

        if (currentMoney < 0)
        {
            currentMoney = 0;
        }
    }

    public void AddOre(int amount)
    {
        currentOre += amount;
    }

    public void RemoveOre(int amount)
    {
        currentOre -= amount;

        if (currentOre < 0)
        {
            currentOre = 0;
        }
    }

}
