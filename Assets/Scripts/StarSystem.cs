using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    public int id { get; private set; }
    public GameObject star { get; private set; }
    private float starRadius;
    public StarType starType { get; private set; }
    public float starTemperature { get; private set; }
    public Planet[] planets;
    public static float AU = 1.495f;
    //public static int maxPlanetCount = 9;

    public StarSystem(int systemId, float _starTemperature)
    {
        starType = (StarType)Mathf.FloorToInt(7 - (_starTemperature % 1450));
        starTemperature = _starTemperature;
        starRadius = UnityEngine.Random.Range(2f, 6f);

        GameObject starSystemObj = new GameObject($"Starsystem {systemId}");
        star = GameObject.Instantiate(MapManager.instance.starPrefab, Vector3.zero, Quaternion.identity);
        star.transform.SetParent(starSystemObj.transform);

        int planetCount = UnityEngine.Random.Range(1, 9);
        planets = new Planet[planetCount];

        for (int i = 0; i < planets.Length; i++)
        {
            GameObject orbitObj = new GameObject($"Orbit {i}");
            orbitObj.transform.SetParent(starSystemObj.transform);

            planets[i] = GameObject.Instantiate(MapManager.instance.planetPrefab).GetComponent<Planet>();
            planets[i].gameObject.name = $"Planet {i}";
            planets[i].transform.SetParent(orbitObj.transform);
            float orbitRadius = (AU + starRadius) * UnityEngine.Random.Range(0.1f * (i + 1), 10f * (i + 1));
            planets[i].Initialize(orbitRadius);
            orbitObj.AddComponent<OrbitalRenderer>();
            orbitObj.GetComponent<OrbitalRenderer>().Initialize(50, orbitRadius);
        }
    }

    public void HideSystem(bool hide)
    {
        star.transform.parent.gameObject.SetActive(!hide);
    }

    public void FocusPlanet(GameObject planetObj)
    {
        Planet focusedPlanet = planets[Array.IndexOf(planets, planetObj.GetComponent<Planet>())];
        for (int i = 0; i < planets.Length; i++)
        {
            if (planets[i] != focusedPlanet)
            {
                planets[i].transform.parent.gameObject.SetActive(false);
            }
        }
        focusedPlanet.Focus(true);
        MapManager.instance.mapState = MapState.PLANETARY_VIEW;
        UiManager.instance.ActivateActions(0);
    }

    public override int GetHashCode()
    {
        return planets.GetHashCode();
    }
}
