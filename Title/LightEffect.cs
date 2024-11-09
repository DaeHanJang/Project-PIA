using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightEffect : MonoBehaviour {
    private Light _light;
    private bool bright = true;
    private float intensity = 0f;

    public float degree = 0.005f;

    public bool Bright {
        get { return bright; }
    }

    private void Awake() { _light = GetComponent<Light>(); }

    private void Update() {
        if (_light != null) {
            if (bright) intensity += degree;
            else intensity -= degree;

            _light.intensity = intensity;

            if (intensity <= 0f) bright = true;
            if (intensity >= 1f) bright = false;
        }
    }
}
