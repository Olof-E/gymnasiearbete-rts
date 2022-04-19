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

    private void Update()
    {
        foreach (KeyValuePair<GameObject, Formation> fleet in fleets)
        {
            fleet.Value.Update();
        }
        // for (int i = 0; i < tempFormations.Count; i++)
        // {
        //     tempFormations[i].Update();
        // }
    }

    public void CreateFleet()
    {
        List<Unit> selectedUnits = SelectionManager.instance.selected.Cast<Unit>().ToList();
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
            Debug.Log("this is from a listener(first): " + fleetId);
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
            //             Debug.Log(UiManager.instance.fleetList.transform.GetChild(i).GetComponent<Button>().onClick.GetPersistentEventCount()
            // );
            //             UiManager.instance.fleetList.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            //             Debug.Log(UiManager.instance.fleetList.transform.GetChild(i).GetComponent<Button>().onClick.GetPersistentEventCount()
            // );
            //             Debug.Log("index:" + i);
            //             UiManager.instance.fleetList.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
            //             {
            //                 Debug.Log("this is from a listener(not first): " + i);
            //                 if (CommandManager.instance.assignOrder)
            //                 {
            //                     CommandManager.instance.GiveOrders(UiManager.instance.fleetList.transform.GetChild(i).gameObject);
            //                 }
            //                 else
            //                 {
            //                     SelectFleet(UiManager.instance.fleetList.transform.GetChild(i).gameObject);
            //                 }
            //             });
        }
        Debug.Log("--------");
        Debug.Log($"{fleets.Count} {UiManager.instance.fleetList.transform.childCount}");
        Debug.Log("--------");
        UiManager.instance.fleetList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, fleets.Count * 115);
    }

    public void SelectFleet(GameObject fleetKey)
    {
        SelectionManager.instance.selected.Clear();
        //Debug.Log($"we care about this: {fleetId}");
        for (int i = 0; i < fleets[fleetKey].units.Count; i++)
        {
            fleets[fleetKey].units[i].selected = true;
            SelectionManager.instance.selected.Add(fleets[fleetKey].units[i]);
        }
        selectedFleetKey = fleetKey;
        Debug.Log($"Selected fleet: {fleets[fleetKey].id}");
    }

    public void AssignFleet(Unit assignedUnit, GameObject fleetKey)
    {
        if (assignedUnit.fleetId == fleets[fleetKey].id)
            return;

        if (assignedUnit.fleetId != -1)
        {
            Debug.Log("Removing from previous fleet");
            fleets[fleetKey].units.Remove(assignedUnit);
        }

        fleets[fleetKey].units.Add(assignedUnit);
        assignedUnit.fleetId = fleets[fleetKey].id;
        Debug.Log(assignedUnit.fleetId);
    }
}