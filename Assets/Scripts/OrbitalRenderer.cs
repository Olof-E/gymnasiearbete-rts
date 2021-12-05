using UnityEngine;

public class OrbitalRenderer : MonoBehaviour
{
    public int segments { get; private set; }
    public float radius { get; private set; }
    private LineRenderer orbitRenderer;

    public void Initialize(int _segments, float _radius)
    {
        segments = _segments;
        radius = _radius;

        orbitRenderer = gameObject.AddComponent<LineRenderer>();

        Vector3[] orbitalPoints = new Vector3[segments];

        float angle = Mathf.Deg2Rad * 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Sin(angle * i) * radius;
            float z = Mathf.Cos(angle * i) * radius;

            orbitalPoints[i] = new Vector3(x, 0f, z);
        }
        orbitRenderer.positionCount = segments;
        orbitRenderer.startColor = new Color(0.125f, 0.5f, 0.65f, 1);
        orbitRenderer.endColor = new Color(0.125f, 0.5f, 0.65f, 1);
        orbitRenderer.startWidth = 0.05f;
        orbitRenderer.material = MapManager.instance.lineMaterial;
        orbitRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        orbitRenderer.receiveShadows = false;
        orbitRenderer.loop = true;

        orbitRenderer.SetPositions(orbitalPoints);
    }
}