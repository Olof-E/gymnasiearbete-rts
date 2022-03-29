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
    public SpriteRenderer selectedSprite { get; set; }
    public bool isOrderable { get; set; } = false;

    private Vector3 planetSize;
    public void Initialize(float _orbitalRadius, StarSystem _parentSystem)
    {
        orbitalRadius = _orbitalRadius;
        scaledOrbitalRadius = orbitalRadius * 8f;
        orbitalSpeed = Random.Range(0.0005f, 0.001f) * 5;
        planetSize = Vector3.one * Random.Range(1.25f, 4f);
        transform.localScale = planetSize;
        parentSystem = _parentSystem;

        shaderPlanetPosId = Shader.PropertyToID("_PlanetPosWS");
        mpb = new MaterialPropertyBlock();

        //Determine what type of planet this is using orbital radius
        if (orbitalRadius > 0f && orbitalRadius <= 15f)
        {
            planetType = PlanetType.CLASS_Y;
        }
        else if (orbitalRadius > 15f && orbitalRadius <= 30f)
        {
            planetType = PlanetType.CLASS_O;
        }

        else if (orbitalRadius > 30f && orbitalRadius <= 45f)
        {
            planetType = Random.Range(0f, 1f) > 0.7f ? PlanetType.CLASS_M : PlanetType.CLASS_L;
        }

        else if (orbitalRadius > 45f && orbitalRadius <= 60f)
        {
            planetType = Random.Range(0f, 1f) > 0.4f ? PlanetType.CLASS_M : PlanetType.CLASS_L;
        }

        else if (orbitalRadius > 60f && orbitalRadius <= 75f)
        {
            planetType = Random.Range(0f, 1f) > 0.4f ? PlanetType.CLASS_O : PlanetType.CLASS_H;
        }

        else if (orbitalRadius > 75f && orbitalRadius <= 90f)
        {
            planetType = Random.Range(0f, 1f) > 0.6f ? PlanetType.CLASS_O : PlanetType.CLASS_H;
        }

        planetProperties = MapManager.instance.planetPropertiesList[(int)planetType];
        meshRenderer.material = planetProperties.material;

        planetaryStructures = new List<PlanetaryStructure>();
        spaceStructures = new List<SpaceStructure>();
        targetables = new List<Targetable>();
        selectionCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        orbitalProgress += Time.deltaTime * orbitalSpeed;
        orbitalProgress %= 1f;

        if (!focused)
        {
            transform.position = EvaluateOrbitalPos(orbitalRadius);
        }

        for (int i = 0; i < planetaryStructures.Count; i++)
        {
            planetaryStructures[i].Execute();
        }

        if (selected && !UiManager.instance.actions[0].activeInHierarchy && focused)
        {
            UiManager.instance.ActivateActions(-1);
            UiManager.instance.ActivateActions(0);
        }
        else if (!selected && !UiManager.instance.actions[1].activeInHierarchy && focused)
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
    //Evaluate the planets orbital position 
    public Vector3 EvaluateOrbitalPos(float radius)
    {
        float angle = Mathf.Deg2Rad * 360f * orbitalProgress;
        float x = Mathf.Sin(angle) * radius;
        float z = Mathf.Cos(angle) * radius;

        return new Vector3(x, 0f, z);
    }

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

    public void Focus(bool _focused)
    {
        focused = _focused;
        transform.parent.GetComponent<LineRenderer>().enabled = !focused;
        if (focused)
        {
            transform.localScale = planetSize * 7.5f;

            transform.position = EvaluateOrbitalPos(scaledOrbitalRadius);
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
            MeshRenderer[] renderers = spaceStructures[i].GetComponentsInChildren<MeshRenderer>();
            for (int j = 0; j < renderers.Length; j++)
            {
                renderers[j].enabled = focused;
            }
            spaceStructures[i].selectionCollider.enabled = focused;
            spaceStructures[i].selectedSprite.enabled = focused;
        }

        foreach (Unit unit in targetables.FindAll((Targetable target) => target.GetType().IsSubclassOf(typeof(Unit))))
        {
            MeshRenderer[] renderers = unit.GetComponentsInChildren<MeshRenderer>();
            for (int j = 0; j < renderers.Length; j++)
            {
                renderers[j].enabled = focused;
            }

            unit.selectionCollider.enabled = focused;
            unit.selectedSprite.enabled = focused;
        }
    }
}
