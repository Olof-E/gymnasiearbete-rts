using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    public List<Formation> fleets;
    public List<Formation> tempFormations;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Unit manager instance already exists...");
            Destroy(this);
        }
    }

    private void Start()
    {
        fleets = new List<Formation>();
        tempFormations = new List<Formation>();
    }

    private void Update()
    {
        for (int i = 0; i < fleets.Count; i++)
        {
            fleets[i].Update();
        }
        for (int i = 0; i < tempFormations.Count; i++)
        {
            tempFormations[i].Update();
        }
    }

    public void CreateFleet()
    {
        List<Unit> fleetUnits = SelectionManager.instance.selected.Cast<Unit>().ToList();
        new Formation(fleetUnits, FormationType.SQUARE, false);
    }

    public void AssignFleet(Unit assignedUnit, int fleetId)
    {
        fleets[fleetId].units.Add(assignedUnit);
        assignedUnit.fleetId = fleetId;
    }
}