using UnityEngine;

//enemy formation position
public class Formation : MonoBehaviour {
    private float h = 1f;

    public float speed; //formation movement speed

    private void Start() { speed = 0.5f * Time.deltaTime; }

    private void Update() {
        if (transform.position.x <= -1) h = 1.5f;
        else if (transform.position.x >= 1) h = -1.5f;
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, 0, 0) * speed;
        transform.position = curPos + nextPos;
    }
}
