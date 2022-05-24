using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;
    public GameObject[] buildingPrefabs;
    public GameObject buildingBlueprint;
    public bool buildingPlacement = false;
    private Camera mainCamera;
    private static Plane XZPlane = new Plane(Vector3.up, Vector3.zero);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            mainCamera = Camera.main;
        }
        else
        {
            Debug.Log("Building manager already exists...");
            Destroy(this);
        }
    }

    private void Update()
    {
        if (buildingPlacement)
        {
            buildingBlueprint.transform.position = GetMousePositionOnXZPlane();
        }
    }

    public void BuildPlanetaryStructure(int buildingIndex)
    {
        if (buildingIndex == 0)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new FusionReactor());
        }
        else if (buildingIndex == 1)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new tritaniumExtractor(MapManager.instance.activePlanet));
        }
        else if (buildingIndex == 2)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new NanoCarbonExtruder(MapManager.instance.activePlanet));
        }
        else if (buildingIndex == 3)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new CrystalSynthesizer(MapManager.instance.activePlanet));
        }
        else if (buildingIndex == 4)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new RareMetalsExtractor(MapManager.instance.activePlanet));
        }
        else if (buildingIndex == 5)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new ResearchFacility());
        }
        else if (buildingIndex == 6)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new GasSeparator(MapManager.instance.activePlanet));
        }
    }

    public void BuildSpaceStructure(int buildingIndex)
    {
        if (buildingBlueprint != null)
        {
            Destroy(buildingBlueprint);
            buildingPlacement = false;
        }
        if (buildingIndex == 0)
        {
            buildingPlacement = true;
            buildingBlueprint = GameObject.Instantiate(buildingPrefabs[0]);
            buildingBlueprint.transform.SetParent(MapManager.instance.activePlanet.transform);
        }
        else if (buildingIndex == 1)
        {
            buildingPlacement = true;
            buildingBlueprint = GameObject.Instantiate(buildingPrefabs[1]);
            buildingBlueprint.transform.SetParent(MapManager.instance.activePlanet.transform);
        }
        else if (buildingIndex == 2)
        {
            buildingPlacement = true;
            buildingBlueprint = GameObject.Instantiate(buildingPrefabs[2]);
            buildingBlueprint.transform.SetParent(MapManager.instance.activePlanet.transform);
        }
        else if (buildingIndex == 3)
        {
            buildingPlacement = true;
            buildingBlueprint = GameObject.Instantiate(buildingPrefabs[3]);
            buildingBlueprint.transform.SetParent(MapManager.instance.activePlanet.transform);
        }
        else if (buildingIndex == 4)
        {
            buildingPlacement = true;
            buildingBlueprint = GameObject.Instantiate(buildingPrefabs[4]);
            buildingBlueprint.transform.SetParent(MapManager.instance.activePlanet.transform);
        }
    }

    public static Vector3 GetMousePositionOnXZPlane()
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (XZPlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            //Ensure y position is exactly zero
            hitPoint.y = 0;
            return hitPoint;
        }
        return Vector3.zero;
    }

    public void HandleMouseInput(MouseEventArgs e)
    {
        if (!buildingPlacement) { return; }

        if (e.mouseBtn == 0 && !e.doubleClick)
        {
            SpaceStructure spaceStructure = buildingBlueprint.GetComponent<SpaceStructure>();
            ((ISelectable)spaceStructure).selectablePosition = spaceStructure.transform.position;
            spaceStructure.parentBody = MapManager.instance.activePlanet;
            MapManager.instance.activePlanet.targetables.Add(spaceStructure);
            MapManager.instance.activePlanet.selectables.Add(spaceStructure);
            MapManager.instance.activePlanet.spaceStructures.Add(spaceStructure);
            buildingPlacement = false;
            buildingBlueprint = null;
        }
    }
}