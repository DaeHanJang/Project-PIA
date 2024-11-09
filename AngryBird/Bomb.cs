using UnityEngine;

//stone object
public class Bomb : MonoBehaviour {
    private Rigidbody2D rb;
    private SpringJoint2D sj;
    private LineRenderer _lineback, _linefore;
    private AudioSource _as;
    private Ray _rayToCatapult; //��������� �������� ray
    private Vector2 _prev_velocity; //�� ������ �� �ӷ�
    private bool clickedOn = false; //Ŭ�� �������� ���� ����
    private bool _isShowLine = true; //������ �� ���� ����
    private float _maxLength = 3f; //������� ���� �ִ� ������ �� �ִ� �Ÿ�

    public AudioClip[] ac = new AudioClip[2];
    public Transform _zeroPoint; //default transform

    private void Awake() { _zeroPoint = GameObject.Find("CatapultPosition").transform; }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        sj = GetComponent<SpringJoint2D>();
        _rayToCatapult = new Ray(_zeroPoint.position, Vector3.zero);
        _lineback = GameObject.Find("LineBack").GetComponent<LineRenderer>();
        _linefore = GameObject.Find("LineFore").GetComponent<LineRenderer>();
        _as = GetComponent<AudioSource>();
    }

    private void Update() {
        if (clickedOn) {
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //mouse�� ����Ű�� world ��ǥ
            
            Vector2 _newVector = mouseWorldPoint - _zeroPoint.position; //��������� mouse ������ ����
            if (_newVector.sqrMagnitude > _maxLength * _maxLength) { //���� �Ÿ����� �ָ� ���� ���
                _rayToCatapult.direction = _newVector; //ray ���� ����
                mouseWorldPoint = _rayToCatapult.GetPoint(_maxLength); //���� �Ÿ��� ����
            }
            mouseWorldPoint.z = 0f;

            transform.position = mouseWorldPoint;
        }

        if (sj != null) {
            if (_prev_velocity.sqrMagnitude > rb.velocity.sqrMagnitude) { //�� ������ �� �ӷ��� ���� �ӷº��� Ŭ��� ��, spring joint�� ���� �߾� ������ ���� ������ ���
                Destroy(sj);
                DeleteLine();
                rb.velocity = _prev_velocity;
            }

            if (clickedOn == false) _prev_velocity = rb.velocity;
        }
        UpdateLine();
    }

    private void OnMouseDown() {
        if (AngryManager.Inst.gameMode != 1) return;
        PlayClip(0);
        clickedOn = true;
    }

    private void OnMouseUp() {
        if (AngryManager.Inst.gameMode != 1) return;
        PlayClip(1);
        clickedOn = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        Invoke("ThrowStone", 7f);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<AngryBird>() != null) collision.gameObject.GetComponent<Animator>().enabled = true;
    }

    //update line
    private void UpdateLine() {
        if (!_isShowLine) return;

        _lineback.SetPosition(1, transform.position);
        _linefore.SetPosition(1, transform.position);
    }

    //delete line
    private void DeleteLine() {
        _isShowLine = false;
        _lineback.gameObject.SetActive(false);
        _linefore.gameObject.SetActive(false);
    }

    //�������� �� ���� ó��
    private void ThrowStone() {
        AngryManager.Inst.SetNextTrun();
        Destroy(gameObject);
    }

    private void PlayClip(int n) {
        _as.clip = ac[n];
        _as.Play();
    }
}
