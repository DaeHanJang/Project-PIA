using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//camera ray
public class CameraRay : MonoBehaviour {
    private Camera cam;

    void Awake() { cam = GetComponent<Camera>(); }

    void Update() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f)) hit.transform.gameObject.layer = 7;
    }
}
