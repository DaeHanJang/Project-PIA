using UnityEngine;

//stone object
public class Bomb : MonoBehaviour {
    private Rigidbody2D rb;
    private SpringJoint2D sj;
    private LineRenderer _lineback, _linefore;
    private AudioSource _as;
    private Ray _rayToCatapult; //투석기부터 돌까지의 ray
    private Vector2 _prev_velocity; //한 프레임 앞 속력
    private bool clickedOn = false; //클릭 중인지에 대한 여부
    private bool _isShowLine = true; //투석기 줄 노출 여부
    private float _maxLength = 3f; //투석기로 부터 최대 떨어질 수 있는 거리

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
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //mouse가 가리키는 world 좌표
            
            Vector2 _newVector = mouseWorldPoint - _zeroPoint.position; //투석기부터 mouse 방향의 벡터
            if (_newVector.sqrMagnitude > _maxLength * _maxLength) { //제한 거리보다 멀리 있을 경우
                _rayToCatapult.direction = _newVector; //ray 방향 설정
                mouseWorldPoint = _rayToCatapult.GetPoint(_maxLength); //제한 거리로 설정
            }
            mouseWorldPoint.z = 0f;

            transform.position = mouseWorldPoint;
        }

        if (sj != null) {
            if (_prev_velocity.sqrMagnitude > rb.velocity.sqrMagnitude) { //함 프레임 앞 속력이 현제 속력보다 클경우 즉, spring joint를 통해 중앙 지점을 지나 감속할 경우
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

    //던져지고 난 이후 처리
    private void ThrowStone() {
        AngryManager.Inst.SetNextTrun();
        Destroy(gameObject);
    }

    private void PlayClip(int n) {
        _as.clip = ac[n];
        _as.Play();
    }
}
