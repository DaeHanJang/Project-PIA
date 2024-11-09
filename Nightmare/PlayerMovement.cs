using UnityEngine;

//player movement
public class PlayerMovement : MonoBehaviour {
    private Rigidbody rb;
    private Animator anim;
    private Vector3 movement;
    private float camRayLength = 100f; //������ �� �ִ� �ִ� ����
    private float rotationX, rotationY;
    private int floorMask; //������ �� �ִ� ����

    public float speed = 6f;
    public float fpsSpeed = 50f; //1��Ī ȸ�� �ӵ�

    private void Awake() {
        floorMask = LayerMask.GetMask("Floor");
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move(h, v);
        Turning();
        Animating(h, v);
    }

    //movement
    void Move(float h, float v) {
        movement.Set(h, 0, v);
        //3��Ī�϶� �̵�
        if (NightmareManager.Inst.cam[0].activeSelf) movement = movement.normalized * speed * Time.deltaTime;
        //1��Ī�϶� �̵�
        else movement = transform.rotation * movement.normalized * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }

    //angle
    void Turning() {
        if (NightmareManager.Inst.cam[0].activeSelf) { //3��Ī�϶� ȸ��
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit floorHit;
            if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask)) {
                Vector3 playerToMouse = floorHit.point - transform.position;
                playerToMouse.y = 0f;

                Quaternion newPos = Quaternion.LookRotation(playerToMouse);
                rb.MoveRotation(newPos);
            }
        }
        else { //1��Ī�϶� ȸ��
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            rotationX += y * fpsSpeed * Time.deltaTime;
            rotationY += x * fpsSpeed * Time.deltaTime;

            if (rotationX > 30f) rotationX = 30f;
            if (rotationX < -30f) rotationX = -30f;

            transform.eulerAngles = new Vector3(-rotationX, rotationY, 0);
        }
    }
    
    //animation
    void Animating(float h, float v) {
        bool walking = (h != 0f) || (v != 0f);
        anim.SetBool("IsWalking", walking);
    }
}
