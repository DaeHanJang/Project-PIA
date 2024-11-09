using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow))]
public class ShadowEffect : MonoBehaviour {
    private Shadow shadow;
    private LightEffect le;

    public float distance = 5f;
    public float degree = 0.02f;

    private void Awake() {
        shadow = GetComponent<Shadow>();
        le = GameObject.Find("Point Light").GetComponent<LightEffect>();
    }

    private void Start() { shadow.effectDistance = new Vector2(distance, -distance); }

    private void Update() {
        if (shadow != null) {
            if (le.Bright) shadow.effectDistance += new Vector2(degree, -degree);
            else shadow.effectDistance -= new Vector2(degree, -degree);
        }
    }
}
