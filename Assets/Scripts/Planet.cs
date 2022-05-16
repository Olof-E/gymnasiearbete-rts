using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
M - Habitable (Earthlike)
L - habitable but somewhat harsh
F - Barely Unhabitable (Require artifical env)
O - Uninhabitale (Require artifical env)
Y - Very uninhabitable (Hard to colonize)
H - Gas giant (unable to colonize)
*/
public enum PlanetType
{
    CLASS_M,
    CLASS_L,
    CLASS_F,
    CLASS_O,
    CLASS_Y,
    CLASS_H
}

public class Planet : MonoBehaviour, ISelectable
{
    public int playerId;
    public List<PlanetaryStructure> planetaryStructures;
    public List<SpaceStructure> spaceStructures;
    public PlanetType planetType;
    public PlanetProperties planetProperties;
    private float orbitalSpeed;
    private float orbitalProgress = 0f;
    public float orbitalRadius;
    public float scaledOrbitalRadius;
    public bool selected { get; set; } = false;
    public bool focused = false;
    public StarSystem parentSystem;
    public BoxCollider selectionCollider { get; set; }
    private int shaderPlanetPosId;
    public MeshRenderer meshRenderer;
    private MaterialPropertyBlock mpb;
    public List<Targetable> targetables;
    public List<ISelectable> selectables;
    public SpriteRenderer selectedSprite { get; set; }
    public bool isOrderable { get; set; } = false;
    public Vector3 selectablePosition { get; set; }
    [field: SerializeField]
    public Renderer boundsRenderer { get; set; }
    public Vector3 planetSize;

    //Initialize planet properties
    public void Initialize(int index, StarSystem _parentSystem)
    {
        orbitalProgress = Random.Range(0f, 1f);
        orbitalSpeed = Random.Range(0.0005f, 0.001f) * 5;
        transform.localScale = planetSize;
        parentSystem = _parentSystem;

        selectedSprite = transform.Find("SelectedSprite").GetComponent<SpriteRenderer>();

        shaderPlanetPosId = Shader.PropertyToID("_PlanetPosWS");
        mpb = new MaterialPropertyBlock();

        //Determine what type of planet this is using orbital radius
        // if (orbitalRadius > 0f && orbitalRadius <= 2.5f)
        // {
        //     planetType = PlanetType.CLASS_Y;
        // }
        // else if (orbitalRadius > 2.5f && orbitalRadius <= 4f)
        // {
        //     planetType = PlanetType.CLASS_O;
        // }

        // else if (orbitalRadius > 4f && orbitalRadius <= 5f)
        // {
        //     planetType = Random.Range(0f, 1f) > 0.7f ? PlanetType.CLASS_M : PlanetType.CLASS_L;
        // }

        // else if (orbitalRadius > 5f && orbitalRadius <= 7.5f)
        // {
        //     planetType = Random.Range(0f, 1f) > 0.4f ? PlanetType.CLASS_M : PlanetType.CLASS_L;
        // }

        // else if (orbitalRadius > 7.5f && orbitalRadius <= 10f)
        // {
        //     planetType = Random.Range(0f, 1f) > 0.4f ? PlanetType.CLASS_O : PlanetType.CLASS_O;
        // }

        // else if (orbitalRadius > 10f && orbitalRadius <= 12f)
        // {
        //     planetType = Random.Range(0f, 1f) > 0.6f ? PlanetType.CLASS_O : PlanetType.CLASS_O;
        // }
        planetType = (PlanetType)Mathf.FloorToInt(Random.value * 5f);

        planetProperties = MapManager.instance.planetPropertiesList[(int)planetType];

        meshRenderer.GetPropertyBlock(mpb, 0);

        mpb.SetTexture("_MainTex", planetProperties.diffuseMap);
        mpb.SetTexture("_NormalMap", planetProperties.normalMap);
        mpb.SetTexture("_RoughnessMap", planetProperties.roughnessMap);
        mpb.SetTexture("_SpecularMap", planetProperties.specularMap);
        if (planetProperties.emissionMap != null)
        {
            mpb.SetTexture("_EmissionMap", planetProperties.emissionMap);
        }

        meshRenderer.SetPropertyBlock(mpb, 0);
        mpb.Clear();

        meshRenderer.GetPropertyBlock(mpb, 1);

        mpb.SetColor("_Color", planetProperties.atmosphereColor);

        meshRenderer.SetPropertyBlock(mpb, 1);
        mpb.Clear();

        planetaryStructures = new List<PlanetaryStructure>();
        spaceStructures = new List<SpaceStructure>();
        targetables = new List<Targetable>();
        selectables = new List<ISelectable>();
        selectables.Add(this);
        selectionCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        orbitalProgress += Time.deltaTime * orbitalSpeed;
        orbitalProgress %= 1f;

        if (!focused && MapManager.instance.activeSystem == parentSystem)
        {
            transform.position = EvaluateOrbitalPos(orbitalRadius);
            selectablePosition = transform.position;
        }

        for (int i = 0; i < planetaryStructures.Count; i++)
        {
            planetaryStructures[i].Execute();
        }


        if (focused || MapManager.instance.activeSystem == parentSystem)
        {
            if (selected)
            {
                if (!selectedSprite.gameObject.activeInHierarchy)
                {
                    selectedSprite.gameObject.SetActive(true);
                }
            }
            else
            {
                if (selectedSprite.gameObject.activeInHierarchy)
                {
                    selectedSprite.gameObject.SetActive(false);
                }
            }

            if (selected && !UiManager.instance.actionsActive && !UiManager.instance.actions[0].activeInHierarchy)
            {
                UiManager.instance.ActivateActions(-1);
                UiManager.instance.ActivateActions(0);
            }
            else if (!selected && !UiManager.instance.actionsActive && !UiManager.instance.actions[1].activeInHierarchy)
            {
                UiManager.instance.ActivateActions(-1);
                UiManager.instance.ActivateActions(1);
            }

            meshRenderer.GetPropertyBlock(mpb, 0);

            mpb.SetVector(shaderPlanetPosId, transform.position);

            meshRenderer.SetPropertyBlock(mpb, 0);


            meshRenderer.GetPropertyBlock(mpb, 1);

            mpb.SetVector(shaderPlanetPosId, transform.position);

            meshRenderer.SetPropertyBlock(mpb, 1);
        }
    }

    //Evaluate the planets orbital position 
    public Vector3 EvaluateOrbitalPos(float radius)
    {
        float angle = Mathf.Deg2Rad * 360f * orbitalProgress;
        float x = Mathf.Sin(angle) * radius;
        float z = Mathf.Cos(angle) * radius;

        return new Vector3(x, 0f, z);
    }

    //Toggle all renderers of the planet 
    public void Hide(bool hide)
    {
        transform.parent.GetComponent<LineRenderer>().enabled = !hide;
        MeshRenderer[] planetRenderers = this.GetComponentsInChildren<MeshRenderer>();
        for (int j = 0; j < planetRenderers.Length; j++)
        {
            planetRenderers[j].enabled = !hide;
        }

        selectionCollider.enabled = !hide;
    }

    //Focus this planet and toggle all interactables
    public void Focus(bool _focused)
    {
        focused = _focused;
        transform.parent.GetComponent<LineRenderer>().enabled = !focused;
        if (focused)
        {
            transform.localScale = planetSize * 10f;

            transform.position = EvaluateOrbitalPos(orbitalRadius);
            CameraController.instance.FocusPosition(transform.position);

            MapManager.instance.activePlanet = this;
        }
        else
        {
            transform.localScale = planetSize;
            transform.position = EvaluateOrbitalPos(orbitalRadius);
            CameraController.instance.FocusPosition(Vector3.zero);

            UiManager.instance.ActivateActions(-1);
        }


        for (int i = 0; i < spaceStructures.Count; i++)
        {
            spaceStructures[i].Hide(!focused);
        }

        foreach (Unit unit in targetables.FindAll((Targetable target) => target.GetType().IsSubclassOf(typeof(Unit))))
        {
            unit.Hide(!focused);
        }
    }
}
