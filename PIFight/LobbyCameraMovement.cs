using UnityEngine;

public class LobbyCameraMovement : MonoBehaviour {
    [SerializeField]
    private float angle = 0f; //����

    public float radius = 0f; //������
    public float speed = 0f; //ȸ�� �ӵ�

    private void Update() {
        angle += Time.deltaTime * speed;

        if (angle < 360f) {
            var x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            var z = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            transform.position = new Vector3(x, 1f, z);
            transform.rotation = Quaternion.Euler(0f, -angle - 90f, 0f);
        }
        else angle = 0f;
    }
}
