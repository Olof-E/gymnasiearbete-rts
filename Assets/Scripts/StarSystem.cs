using System;
using UnityEngine;
using UnityEngine.VFX;

// O (hottest) --> M (coolest)
public enum StarType
{
    CLASS_O,
    CLASS_B,
    CLASS_A,
    CLASS_F,
    CLASS_G,
    CLASS_K,
    CLASS_M
}

public class StarSystem
{
    //Properties
    public int id { get; private set; }
    public GameObject star { get; private set; }
    public StarType starType { get; private set; }
    public Planet[] planets;
    public static float AU = 1.495f;
    private float starRadius;
    private float starTemperature;


    public StarSystem(int systemId, float _starTemperature)
    {
        id = systemId;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        //Create the star
        GameObject starSystemObj = new GameObject($"Starsystem {systemId}");
        starType = (StarType)Mathf.FloorToInt(7 - (_starTemperature % 1450));
        starTemperature = _starTemperature;
        starRadius = UnityEngine.Random.Range(12f, 16f);
        star = GameObject.Instantiate(MapManager.instance.starPrefab, Vector3.zero, Quaternion.identity);
        star.transform.localScale = Vector3.one * starRadius;
        star.transform.SetParent(starSystemObj.transform);

        //Set star color depending on its temprature
        star.GetComponent<MeshRenderer>().GetPropertyBlock(mpb);
        mpb.SetColor("_PrimaryColor", Mathf.CorrelatedColorTemperatureToRGB(starTemperature) * 32.5f);
        float hue;
        float saturation;
        float intensity;
        Color.RGBToHSV(Mathf.CorrelatedColorTemperatureToRGB(starTemperature), out hue, out saturation, out intensity);
        mpb.SetColor("_SecondaryColor", Color.HSVToRGB(hue + UnityEngine.Random.Range(-0.3f, 0.3f), saturation, intensity, true) / 5f);
        star.GetComponent<MeshRenderer>().SetPropertyBlock(mpb);
        star.transform.GetChild(0).GetComponent<VisualEffect>().SetVector4("_MainColor", Mathf.CorrelatedColorTemperatureToRGB(starTemperature) * 6.5f);


        mpb.Clear();
        mpb = null;

        //Initialize all planets with random values
        int planetCount = UnityEngine.Random.Range(1, 9);
        planets = new Planet[planetCount];

        float prevOrbitRadius = 0f;
        for (int i = 0; i < planets.Length; i++)
        {
            GameObject orbitObj = new GameObject($"Orbit {i}");
            orbitObj.transform.SetParent(starSystemObj.transform);

            planets[i] = GameObject.Instantiate(MapManager.instance.planetPrefab).GetComponent<Planet>();
            planets[i].parentSystem = this;
            planets[i].gameObject.name = $"Planet {i}";
            planets[i].transform.SetParent(orbitObj.transform);
            planets[i].planetSize = Vector3.one * UnityEngine.Random.Range(0.8f, 3f);
            float orbitRadius = Mathf.Clamp((AU + starRadius / 2f + planets[i].planetSize.x) * UnityEngine.Random.Range(1f * (i + 1), 2f * (i + 1)), prevOrbitRadius * 1.2f, Mathf.Infinity);
            prevOrbitRadius = orbitRadius + planets[i].planetSize.x;
            planets[i].orbitalRadius = orbitRadius;
            planets[i].scaledOrbitalRadius = orbitRadius * 8f;
            planets[i].Initialize(i, this);
            orbitObj.AddComponent<OrbitalRenderer>();
            orbitObj.GetComponent<OrbitalRenderer>().Initialize(50, orbitRadius);
        }
    }

    //Hide all renderers in the system and its children
    public void HideSystem(bool hide)
    {
        star.transform.position = Vector3.zero;
        star.SetActive(!hide);
        for (int i = 0; i < planets.Length; i++)
        {
            planets[i].Hide(hide);
        }
    }

    //Focus camera on specific planet
    public void FocusPlanet(GameObject planetObj)
    {
        Planet focusedPlanet = planets[Array.IndexOf(planets, planetObj.GetComponent<Planet>())];
        for (int i = 0; i < planets.Length; i++)
        {
            if (planets[i] != focusedPlanet)
            {
                planets[i].Hide(true);
            }
        }
        focusedPlanet.Focus(true);
        MapManager.instance.mapState = MapState.PLANETARY_VIEW;
        star.transform.position = Vector3.Normalize(star.transform.position - focusedPlanet.transform.position) * 250f;
        UiManager.instance.ActivateActions(0);
    }

    public override int GetHashCode()
    {
        return planets.GetHashCode();
    }
}
