using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;
    public GameObject[] buildingPrefabs;
    public GameObject buildingBlueprint;
    private bool buildingPlacement = false;
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
            Debug.Log("Start construction of fusion reactor");
        }
        else if (buildingIndex == 1)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new tritaniumExtractor(MapManager.instance.activePlanet));
            Debug.Log("Start construction of tritanium extractor");
        }
        else if (buildingIndex == 2)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new NanoCarbonExtruder(MapManager.instance.activePlanet));
            Debug.Log("Start construction of nanocarbon exturder");
        }
        else if (buildingIndex == 3)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new CrystalSynthesizer(MapManager.instance.activePlanet));
            Debug.Log("Start construction of crystal synthesizer");
        }
        else if (buildingIndex == 4)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new RareMetalsExtractor(MapManager.instance.activePlanet));
            Debug.Log("Start construction of rare metals extractor");
        }
        else if (buildingIndex == 5)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new ResearchFacility());
            Debug.Log("Start construction of research facility");
        }
        else if (buildingIndex == 6)
        {
            MapManager.instance.activePlanet.planetaryStructures.Add(new GasSeparator(MapManager.instance.activePlanet));
            Debug.Log("Start construction of gas separation facility");
        }
    }

    public void BuildSpaceStructure(int buildingIndex)
    {
        if (buildingIndex == 0)
        {
            buildingPlacement = true;
            buildingBlueprint = GameObject.Instantiate(buildingPrefabs[0]);
            buildingBlueprint.transform.SetParent(MapManager.instance.activePlanet.transform.parent);
            Debug.Log("Start construction of small shipyard");
        }
        else if (buildingIndex == 1)
        {
            buildingPlacement = true;
            buildingBlueprint = GameObject.Instantiate(buildingPrefabs[1]);
            buildingBlueprint.transform.SetParent(MapManager.instance.activePlanet.transform.parent);
            Debug.Log("Start construction of large shipyard");
        }
        else if (buildingIndex == 2)
        {
            buildingPlacement = true;
            buildingBlueprint = GameObject.Instantiate(buildingPrefabs[2]);
            buildingBlueprint.transform.SetParent(MapManager.instance.activePlanet.transform.parent);
            Debug.Log("Start construction of phased energy beam");
        }
        else if (buildingIndex == 3)
        {
            buildingPlacement = true;
            buildingBlueprint = GameObject.Instantiate(buildingPrefabs[3]);
            buildingBlueprint.transform.SetParent(MapManager.instance.activePlanet.transform.parent);
            Debug.Log("Start construction of torpedo launcher");
        }
        else if (buildingIndex == 4)
        {
            buildingPlacement = true;
            buildingBlueprint = GameObject.Instantiate(buildingPrefabs[4]);
            buildingBlueprint.transform.SetParent(MapManager.instance.activePlanet.transform.parent);
            Debug.Log("Start construction of railgun cannon");
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
            buildingBlueprint.GetComponent<SpaceStructure>().parentBody = MapManager.instance.activePlanet;
            MapManager.instance.activePlanet.targetables.Add(buildingBlueprint.GetComponent<SpaceStructure>());
            MapManager.instance.activePlanet.spaceStructures.Add(buildingBlueprint.GetComponent<SpaceStructure>());
            buildingPlacement = false;
            buildingBlueprint = null;
        }
    }
}