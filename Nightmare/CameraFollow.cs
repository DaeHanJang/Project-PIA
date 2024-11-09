using UnityEngine;

//main camera movement
public class CameraFollow : MonoBehaviour {
    private Transform target;
    private Vector3 offset;

    public float smooting = 5f; //카메라 이동 속도

    private void Awake() { target = GameObject.Find("Player").transform; }

    private void Start() { offset = transform.position - target.position; }

    private void FixedUpdate() {
        Vector3 newPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, newPos, smooting * Time.deltaTime);
    }

    //main camera 변경 시 위치 초기화
    public void CameraInit() { transform.position = target.position; }
}
