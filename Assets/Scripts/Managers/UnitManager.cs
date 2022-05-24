using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
    public Dictionary<GameObject, Formation> fleets;
    public GameObject fleetLisInfoPrefab;

    public GameObject selectedFleetKey;
    //public List<Formation> tempFormations;
    private void Awake()
    {
        if (instance == null)
        {
            fleets = new Dictionary<GameObject, Formation>();
            //tempFormations = new List<Formation>();
            instance = this;
        }
        else
        {
            Debug.Log("Unit manager instance already exists...");
            Destroy(this);
        }
    }

    public void CreateFleet()
    {
        List<ISelectable> selection = SelectionManager.instance.selected;
        if (!selection.TrueForAll((ISelectable a) => { return a.GetType().IsSubclassOf(typeof(Unit)); }))
        {
            return;
        }
        List<Unit> selectedUnits = selection.Cast<Unit>().ToList();
        if (selectedUnits.Count <= 0)
        {
            return;
        }
        Formation newFormation = new Formation(new List<Unit>(), FormationType.SQUARE);
        GameObject newFleetInfoPanel = GameObject.Instantiate(fleetLisInfoPrefab, Vector3.zero, Quaternion.identity);

        fleets.Add(newFleetInfoPanel, newFormation);
        int fleetId = fleets.Count - 1;

        RectTransform panelTransform = newFleetInfoPanel.GetComponent<RectTransform>();
        newFleetInfoPanel.transform.SetParent(UiManager.instance.fleetList.transform, false);

        panelTransform.anchoredPosition = new Vector2(0, -115 * fleetId);
        newFleetInfoPanel.GetComponentInChildren<TMP_Text>().SetText($"fleet {fleetId}");

        newFleetInfoPanel.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (CommandManager.instance.assignOrder)
            {
                CommandManager.instance.GiveOrders(newFleetInfoPanel);
            }
            else
            {
                SelectFleet(newFleetInfoPanel);
            }
        });

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            AssignFleet(selectedUnits[i], newFleetInfoPanel);
        }

        UiManager.instance.fleetList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, fleets.Count * 115);
    }

    public void DisbandFleet()
    {
        if (selectedFleetKey == null)
        {
            return;
        }

        fleets[selectedFleetKey].units.ForEach((Unit unit) => { unit.fleetId = -1; });
        fleets.Remove(selectedFleetKey);
        selectedFleetKey.transform.SetParent(null, false);
        for (int i = 0; i < fleets.Count; i++)
        {
            UiManager.instance.fleetList.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -115 * i);
        }
        UiManager.instance.fleetList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, fleets.Count * 115);
    }

    public void SelectFleet(GameObject fleetKey)
    {
        SelectionManager.instance.ClearSelection();
        for (int i = 0; i < fleets[fleetKey].units.Count; i++)
        {
            fleets[fleetKey].units[i].selected = true;
            SelectionManager.instance.selected.Add(fleets[fleetKey].units[i]);
        }
        selectedFleetKey = fleetKey;
    }

    public void AssignFleet(Unit assignedUnit, GameObject fleetKey)
    {
        if (assignedUnit.fleetId == fleets[fleetKey].id)
            return;

        if (assignedUnit.fleetId != -1)
        {
            fleets[fleetKey].units.Remove(assignedUnit);
        }

        fleets[fleetKey].units.Add(assignedUnit);
        assignedUnit.fleetId = fleets[fleetKey].id;
    }
}