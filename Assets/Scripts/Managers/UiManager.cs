using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    public GameObject[] actions;
    public TMP_Text fpsCounterTxt;

    public TMP_Text creditsText;
    public TMP_Text scienceText;
    public TMP_Text foodText;
    public TMP_Text tritaniumText;
    public TMP_Text crystalText;
    public TMP_Text nanoCarbonText;
    public TMP_Text availablePowerText;
    public TMP_Text RareMetalText;
    public TMP_Text gasText;

    public Button switchMapView;
    public Button buildFusionReactor;
    public Button buildTritaniumExtractor;
    public Button buildNanoCarbonExtruder;
    public Button buildCrystalSynthesizer;
    public Button buildResearchFacility;
    public Button buildGasSepartor;
    public Button buildSmallShipyard;
    public Button buildEnergyBeam;
    public Button buildTorpedLauncher;
    public Button buildRailgunCannon;

    //Create singelton instance of ui manager
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            switchMapView.onClick.AddListener(() => { MapManager.instance.SwitchMapView(); });
            buildFusionReactor.onClick.AddListener(() => { BuildingManager.instance.BuildPlanetaryStructure(0); });
            buildTritaniumExtractor.onClick.AddListener(() => { BuildingManager.instance.BuildPlanetaryStructure(1); });
            buildNanoCarbonExtruder.onClick.AddListener(() => { BuildingManager.instance.BuildPlanetaryStructure(2); });
            buildCrystalSynthesizer.onClick.AddListener(() => { BuildingManager.instance.BuildPlanetaryStructure(3); });
            buildResearchFacility.onClick.AddListener(() => { BuildingManager.instance.BuildPlanetaryStructure(5); });
            buildGasSepartor.onClick.AddListener(() => { BuildingManager.instance.BuildPlanetaryStructure(6); });
            buildSmallShipyard.onClick.AddListener(() => { BuildingManager.instance.BuildSpaceStructure(0); });
            buildEnergyBeam.onClick.AddListener(() => { BuildingManager.instance.BuildSpaceStructure(2); });
            buildTorpedLauncher.onClick.AddListener(() => { BuildingManager.instance.BuildSpaceStructure(3); });
            buildRailgunCannon.onClick.AddListener(() => { BuildingManager.instance.BuildSpaceStructure(4); });
        }
        else
        {
            Debug.Log("Ui manager already exists...");
            Destroy(this);
        }
    }

    private void Update()
    {
        fpsCounterTxt.SetText($"Fps: {Mathf.Round(1f / Time.unscaledDeltaTime)}");

        creditsText.SetText($"¢: {Player.instance.totalCredits}");
        scienceText.SetText($"S: {Player.instance.totalScience}");
        tritaniumText.SetText($"T: {Player.instance.totalTritanium}");
        crystalText.SetText($"C: {Player.instance.totalCrystals}");
        nanoCarbonText.SetText($"NC: {Player.instance.totalNanoCarbon}");
        availablePowerText.SetText($"P: {Player.instance.availablePower}");
        RareMetalText.SetText($"R: {Player.instance.totalRareMetals}");
        gasText.SetText($"G: {Player.instance.totalGas}");
    }

    /*
    Activate the actions button corresponding to what is needed
    0 - Actions concerning planetary structures
    1 - Actions concerning space structures
    2 - Actions concerning  small shipyards
    3 - Actions concerning large shipyards
    4 - Actions concerning units
    */
    public void ActivateActions(int actionsId)
    {
        if (actionsId == -1)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].SetActive(false);
            }
            actions[actionsId].SetActive(true);
        }
    }
}