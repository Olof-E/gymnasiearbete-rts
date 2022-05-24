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
    public TMP_Text tritaniumText;
    public TMP_Text crystalText;
    public TMP_Text nanoCarbonText;
    public TMP_Text availablePowerText;
    public TMP_Text RareMetalText;
    public TMP_Text gasText;
    public TMP_Text selectionInfoText;
    public Button switchMapView;
    public Canvas fleetListCanvas;
    public Canvas selectedListCanvas;
    public GameObject fleetList;
    public GameObject selectedList;
    public bool actionsActive { get; private set; } = true;
    public Image toggleFleetBtn;
    public Image toggleSelectedBtn;

    //Create singelton instance of ui manager
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Ui manager already exists...");
            Destroy(this);
        }
    }

    public void BuildPlanStrucClick(int index)
    {
        BuildingManager.instance.BuildPlanetaryStructure(index);
    }

    public void BuildSpaceStrucClick(int index)
    {
        BuildingManager.instance.BuildSpaceStructure(index);
    }

    public void BuildSmallShip(int index)
    {
        ((SmallShipyard)SelectionManager.instance.selected[0]).BuildShip(index);
    }

    public void BuildLargeShip(int index)
    {
        //((LargeShipyard)SelectionManager.instance.selected[0]).BuildShip(index);
    }

    public void CreateNewFleet()
    {
        UnitManager.instance.CreateFleet();
    }

    public void DisbandFleet()
    {
        UnitManager.instance.DisbandFleet();
    }

    private void FixedUpdate()
    {
        //fpsCounterTxt.SetText($"Fps: {Mathf.Round(1f / Time.unscaledDeltaTime)}");
        creditsText.SetText($"¢: {Mathf.RoundToInt(Player.instance.totalCredits)}");
        scienceText.SetText($"S: {Mathf.RoundToInt(Player.instance.totalScience)}");
        tritaniumText.SetText($"T: {Mathf.RoundToInt(Player.instance.totalTritanium)}");
        crystalText.SetText($"C: {Mathf.RoundToInt(Player.instance.totalCrystals)}");
        nanoCarbonText.SetText($"NC: {Mathf.RoundToInt(Player.instance.totalNanoCarbon)}");
        availablePowerText.SetText($"P: {Player.instance.availablePower}");
        RareMetalText.SetText($"R: {Mathf.RoundToInt(Player.instance.totalRareMetals)}");
        gasText.SetText($"G: {Mathf.RoundToInt(Player.instance.totalGas)}");

        //selectionInfoText.SetText($"Selection: {SelectionManager.instance.selected.Count}");
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
        //Debug.Log("this was called");
        if (actionsId == -1)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].SetActive(false);
            }
            actionsActive = false;
        }
        else
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].SetActive(false);
            }
            actions[actionsId].SetActive(true);
            if (actionsId != 1)
            {
                actionsActive = true;
            }
        }
    }

    /*
    0 - Fleets info
    1 - Selected objects info
    */
    public void SwitchSidePanel(int panelId)
    {
        if (panelId == 0 && !fleetListCanvas.enabled)
        {
            toggleFleetBtn.color *= 0.5f;
            toggleSelectedBtn.color /= 0.5f;
        }

        if (panelId == 1 && !selectedListCanvas.enabled)
        {
            toggleFleetBtn.color /= 0.5f;
            toggleSelectedBtn.color *= 0.5f;

        }
        fleetListCanvas.enabled = panelId == 0 ? true : false;
        selectedListCanvas.enabled = panelId == 1 ? true : false;

        if (panelId == 1)
        {
            SelectionManager.instance.UpdateSelectedList();
        }
    }
}