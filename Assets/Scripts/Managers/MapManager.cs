using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapState
{
    GALAXY_VIEW,
    SYSTEM_VIEW,
    PLANETARY_VIEW
}

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public MapState mapState;
    public int mapSeed;
    public float starDistribution = 3.5f;
    public Color lineColor;
    public Material lineMaterial;
    public GameObject starMapPrefab;
    public GameObject starPrefab;
    public GameObject planetPrefab;
    public PlanetProperties[] planetPropertiesList;
    public StarSystem activeSystem = null;
    public Planet activePlanet = null;
    public Graph mapGraph;
    public GameObject[] starMapObjs;
    private StarSystem[] starSystems;
    private GameObject starSystemsObj;
    private GameObject starMapObj;
    private GameObject mapLinesObj;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Map manager already exists...");
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mapGraph = new Graph();
        mapSeed = UnityEngine.Random.Range(-9999, 9999);
        mapGraph.Initialize(100, mapSeed);



        starMapObjs = new GameObject[mapGraph.vertices.Length];
        starSystems = new StarSystem[mapGraph.vertices.Length];
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

        starSystemsObj = new GameObject("Starsystems");

        starMapObj = new GameObject("Map Stars");
        mapLinesObj = new GameObject("Map Lines");

        starSystemsObj.transform.SetParent(transform);
        starMapObj.transform.SetParent(transform);
        mapLinesObj.transform.SetParent(transform);

        for (int i = 0; i < mapGraph.vertices.Length; i++)
        {

            starMapObjs[i] = GameObject.Instantiate(starMapPrefab, mapGraph.vertices[i].position, Quaternion.identity);
            starMapObjs[i].name = $"Star map obj {i}";

            // Text text = starMapObjs[i].AddComponent<Text>();
            // text.text = $"{mapGraph.vertices[i].connections.Count}";

            starMapObjs[i].transform.SetParent(starMapObj.transform);
            starMapObjs[i].GetComponent<MeshRenderer>().GetPropertyBlock(propBlock);

            float starTemperature = Mathf.Clamp(40000f * Mathf.Pow(UnityEngine.Random.Range(0f, 1f), starDistribution) + 1000f, 1000f, 40000f);
            propBlock.SetColor("_Color", Mathf.CorrelatedColorTemperatureToRGB(starTemperature));
            starMapObjs[i].GetComponent<MeshRenderer>().SetPropertyBlock(propBlock);

            starSystems[i] = new StarSystem(i, starTemperature);
            starSystems[i].star.transform.parent.SetParent(starSystemsObj.transform);
            starSystems[i].HideSystem(true);
        }

        for (int i = 0; i < mapGraph.edges.Length; i++)
        {
            GameObject lineRendererGo = new GameObject($"MapLineRenderer {i}");
            lineRendererGo.transform.SetParent(mapLinesObj.transform);
            LineRenderer lr = lineRendererGo.AddComponent<LineRenderer>();

            lr.positionCount = 2;
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            lr.startWidth = 0.055f;
            lr.material = lineMaterial;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;

            lr.SetPosition(0, mapGraph.edges[i].vertexA.position);
            lr.SetPosition(1, mapGraph.edges[i].vertexB.position);
        }

    }

    //Get the system object from its world representation
    public StarSystem GetSystem(GameObject systemObj)
    {
        return starSystems[Array.IndexOf(starMapObjs, systemObj)];
    }

    //Focus the view to the given system
    public void FocusSystem(StarSystem focusSystem)
    {
        starMapObj.SetActive(false);
        mapLinesObj.SetActive(false);
        activeSystem?.HideSystem(true);
        activeSystem = focusSystem;
        activeSystem?.HideSystem(false);
        if (focusSystem != null)
        {
            MapManager.instance.mapState = MapState.SYSTEM_VIEW;
        }
        CameraController.instance.FocusPosition(Vector3.zero);
    }

    //Switch to the map view above the current one
    public void SwitchMapView()
    {
        if (mapState == MapState.PLANETARY_VIEW)
        {
            mapState = MapState.SYSTEM_VIEW;
            activeSystem.HideSystem(false);
            activePlanet.Focus(false);
            activePlanet = null;
            UiManager.instance.ActivateActions(-1);
            if (BuildingManager.instance.buildingPlacement)
            {
                Destroy(BuildingManager.instance.buildingBlueprint);
                BuildingManager.instance.buildingPlacement = false;
            }

        }
        else if (mapState == MapState.SYSTEM_VIEW)
        {
            mapState = MapState.GALAXY_VIEW;
            FocusSystem(null);
            starMapObj.SetActive(true);
            mapLinesObj.SetActive(true);
        }
        Camera.main.transform.parent.position = Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        if (mapGraph != null)
        {
            foreach (Vertex vertex in mapGraph.vertices)
            {
                if (vertex.numOfCons < 2)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawSphere(vertex.position, 0.25f);
            }
            foreach (Edge edge in mapGraph.edges)
            {
                if (edge.isRandomEdge)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawLine(edge.vertexA.position, edge.vertexB.position);
            }
        }
    }
}
