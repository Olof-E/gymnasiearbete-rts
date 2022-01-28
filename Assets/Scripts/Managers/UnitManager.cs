using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
    public List<Formation> fleets;
    public GameObject fleetLisInfoPrefab;

    public int selectedFleet;
    //public List<Formation> tempFormations;
    private void Awake()
    {
        if (instance == null)
        {
            fleets = new List<Formation>();
            //tempFormations = new List<Formation>();
            instance = this;
        }
        else
        {
            Debug.Log("Unit manager instance already exists...");
            Destroy(this);
        }
    }

    private void Update()
    {
        for (int i = 0; i < fleets.Count; i++)
        {
            fleets[i].Update();
        }
        // for (int i = 0; i < tempFormations.Count; i++)
        // {
        //     tempFormations[i].Update();
        // }
    }

    public void CreateFleet()
    {
        List<Unit> fleetUnits = SelectionManager.instance.selected.Cast<Unit>().ToList();
        Formation newFormation = new Formation(fleetUnits, FormationType.SQUARE);

        if (fleets.Count <= 0)
        {
            GameObject newFleetInfoPanel = GameObject.Instantiate(fleetLisInfoPrefab, Vector3.zero, Quaternion.identity);
            RectTransform panelTransform = newFleetInfoPanel.GetComponent<RectTransform>();
            newFleetInfoPanel.transform.SetParent(UiManager.instance.fleetList.transform, false);
            panelTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            GameObject newFleetInfoPanel = GameObject.Instantiate(fleetLisInfoPrefab, Vector3.zero, Quaternion.identity);
            RectTransform panelTransform = newFleetInfoPanel.GetComponent<RectTransform>();
            newFleetInfoPanel.transform.SetParent(UiManager.instance.fleetList.transform, false);
            panelTransform.anchoredPosition = new Vector2(0, -115 * (fleets.Count - 1));
            int fleetId = fleets.Count - 1;
            newFleetInfoPanel.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (CommandManager.instance.assignOrder)
                {
                    CommandManager.instance.GiveOrders(fleetId);
                }
                else
                {
                    SelectFleet(fleetId);
                }
            });
            newFleetInfoPanel.GetComponentInChildren<TMP_Text>().SetText($"fleet {fleetId}");
        }
        UiManager.instance.fleetList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, fleets.Count * 115);
    }

    public void DisbandFleet()
    {
        if (selectedFleet == -1)
        {
            return;
        }

        fleets[selectedFleet].units.ForEach((Unit unit) => { unit.fleetId = -1; });
        fleets.RemoveAt(selectedFleet);
        UiManager.instance.fleetList.transform.GetChild(selectedFleet).parent = null;
    }

    public void SelectFleet(int fleetId)
    {
        for (int i = 0; i < fleets[fleetId].units.Count; i++)
        {
            fleets[fleetId].units[i].selected = true;
            SelectionManager.instance.selected.Add(fleets[fleetId].units[i]);
        }
        selectedFleet = fleetId;
    }

    public void AssignFleet(Unit assignedUnit, int fleetId)
    {
        if (assignedUnit.fleetId == fleetId)
            return;
        fleets[fleetId].units.Add(assignedUnit);
        assignedUnit.fleetId = fleetId;
    }
}