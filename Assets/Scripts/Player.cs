using UnityEngine;

public struct ResourceConsumtion
{
    public float usedCredits;
    public float usedScience;
    public float usedFood;
    public float usedTritanium;
    public float usedCrystals;
    public float usedNanoCarbon;
    public float usedPower;
    public float usedRareMetals;
    public float usedGas;
}

public class Player : MonoBehaviour
{
    public static Player instance;
    public float totalCredits;
    public float totalScience;
    public float totalTritanium;
    public float totalCrystals;
    public float totalNanoCarbon;
    public float totalPower;
    public float availablePower;
    public float usedPower;
    public float totalRareMetals;
    public float totalGas;
    public Client tcpClient { get; private set; }
    public int playerId { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Player instance already exists...");
            Destroy(this);
        }


    }
    private void Start()
    {
        // tcpClient = new Client();
        // tcpClient.StartConnection();
    }

    //Generic function to consume the players resources (ex. when building) 
    public void UseResources(ResourceConsumtion usedResources)
    {
        if (totalCredits >= usedResources.usedCredits) { totalCredits -= usedResources.usedCredits; }

        if (totalScience >= usedResources.usedScience) { totalScience -= usedResources.usedScience; }

        if (totalTritanium >= usedResources.usedTritanium) { totalTritanium -= usedResources.usedTritanium; }

        if (totalCrystals >= usedResources.usedCrystals) { totalCrystals -= usedResources.usedCrystals; }

        if (totalNanoCarbon >= usedResources.usedNanoCarbon) { totalNanoCarbon -= usedResources.usedNanoCarbon; }

        usedPower += usedResources.usedPower;
        availablePower = totalPower - usedPower;

        if (totalRareMetals >= usedResources.usedRareMetals) { totalRareMetals -= usedResources.usedRareMetals; }

        if (totalGas >= usedResources.usedGas) { totalGas -= usedResources.usedGas; }

    }

    private void OnApplicationQuit()
    {
        // tcpClient.Disconnect();
        // tcpClient = null;
        // Debug.Log("Connection to server has been terminated");
    }
}