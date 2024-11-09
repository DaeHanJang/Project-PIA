using ObjectTools;
using UnityEngine;

//player gunship(Gyaraga)
public class Gyaraga : Object2D {
    private GameObject[] bullet = new GameObject[3];
    private GameObject preBullet = null;
    private GameObject ownerEnemy = null;
    private GameObject doubleObj = null;
    private SpriteRenderer SR = null;
    private bool enemyMode = false;
    private bool ownerFollow = false;
    private bool doubleMode = false;

    public Sprite[] sprites = new Sprite[2];
    public int bulletCnt = 0;
    public int score = 0;

    public GameObject OwnerEnemy {
        get { return OwnerEnemy; }
        set { ownerEnemy = value; }
    }

    public GameObject DoubleObj {
        get { return doubleObj; }
        set { doubleObj = value; }
    }

    public bool DoubleMode {
        get { return doubleMode; }
        set { doubleMode = value; }
    }

    private void Awake() {
        preBullet = Resources.Load("Galaga/Bullet") as GameObject;
        SR = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        HP = 1;
        UnitIdx = 1;
        speed = 5f * Time.deltaTime;
    }

    private void Update() {
        if (HP <= 0) { //파괴될 경우
            //적군 상태일 때
            if (enemyMode) {
                GalagaManager.Inst.bKidnaping = false; //manager 납치 파라미터 해제
                ownerEnemy.GetComponent<BossGalaga>().bkidnap = false; //납치한 boss galaga 납치 파라미터 해제
                GalagaManager.Inst.SetAddScore(score); //add score
            }
            else {
                if (doubleMode) { //듀얼 모드일 경우
                    GalagaManager.Inst.bDualMode = false; //manager 듀얼 모드 파라미터 해제
                    if (GalagaManager.Inst.player == gameObject) { //구출한 플레이어가 죽을 경우
                        GalagaManager.Inst.player = doubleObj; //manager에게 구출된 플레이어를 주 플레이어로 설정
                        doubleObj.GetComponent<Gyaraga>().doubleObj = null; //구출된 플레이어의 짝 기체 오브젝트 제거
                        doubleObj.GetComponent<Gyaraga>().doubleMode = false; //구출된 플레이어의 더블 모드 파라미터 해제
                    }
                    else { //구출된 플레이어가 죽을 경우
                        doubleObj = null; //짝 기체 오브젝트 제거
                        doubleMode = false; //더블 모드 파라미터 해제
                    }
                }
                //일반 상태일 때
                else { GalagaManager.Inst.Life--; }  //manager에게 목숨 하나 제거 
            }

            DestroyObject();
        }

        //움직임 및 조작
        if (!enemyMode) { //일반 상태
            float h = Input.GetAxisRaw("Horizontal");
            Vector3 curPos = transform.position;
            Vector3 nextPos = new Vector3(h, 0, 0) * speed;
            Vector3 pos = curPos + nextPos;
            if (!doubleMode) { //듀얼 모드 아닐 시
                if (pos.x <= -4f) pos.x = -4f;
                else if (pos.x >= 4f) pos.x = 4f;
            }
            else { //듀얼 모드일 시
                if (GalagaManager.Inst.player == gameObject) { //구출한 기체
                    if (pos.x <= -4f) pos.x = -4f;
                    else if (pos.x >= 3.5f) pos.x = 3.5f; //오른쪽에 구출된 기체가 있으므로 오른쪽 벽에 0.5f만큼 덜감
                }
                else { //구출된 기체
                    if (pos.x <= -3.5f) pos.x = -3.5f; //구출한 기체가 있으므로 왼쪽 벽에 0.5f만큼 덜감
                    else if (pos.x >= 4f) pos.x = 4f;
                }
            }
            transform.position = pos;

            if (Input.GetKeyDown(KeyCode.Space) && bulletCnt < 3) { //총알은 최대 3개까지 발사 가능
                bullet[bulletCnt] = Instantiate(preBullet, transform.position, Quaternion.identity);
                bullet[bulletCnt].GetComponent<PlayerBullet>().Player = this;
                bulletCnt++;
            }
        }
        else { //적군 모드일 시
            if (!ownerEnemy) { //부모 boss galaga가 파괴됐을 경우
                if (ownerFollow) { //구출중
                    transform.position = Vector3.MoveTowards(transform.position, GalagaManager.Inst.player.transform.position + new Vector3(0.5f, 0f), speed * 1.5f);
                    SetAngle(GalagaManager.Inst.player.transform.position + new Vector3(0f, 0.5f) - transform.position);
                    if (transform.position == GalagaManager.Inst.player.transform.position + new Vector3(0.5f, 0f)) {
                        SetDoubleMode(); //double mode setting
                        GalagaManager.Inst.pause = false; //일시 정지 해제
                    }
                }
            }
            else { //부모 boss galaga가 살아있을 경우
                if (!ownerFollow) { //따라다니기 모드 비활성화(납치중)
                    transform.position = Vector3.MoveTowards(transform.position, ownerEnemy.transform.position + new Vector3(0f, 0.5f), speed * 1.5f);
                    SetAngle(ownerEnemy.transform.position + new Vector3(0f, 0.5f) - transform.position);
                    if (transform.position == ownerEnemy.transform.position + new Vector3(0f, 0.5f)) {
                        ownerFollow = true;
                        GalagaManager.Inst.pause = false; //일시 정지 해제
                    }
                }
                else { //따라다니기 모드 활성화(납치완료)
                    transform.position = ownerEnemy.transform.position + new Vector3(0f, 0.5f);
                    transform.rotation = ownerEnemy.transform.rotation;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Object2D>() != null) {
            if (collision.gameObject.GetComponent<Object2D>().UnitIdx != UnitIdx) HP--;
        }
    }

    //destroy
    protected override void DestroyObject() {
        StartCoroutine(SoundManager.Inst.PlayPlayerDestroy());
        base.DestroyObject();
    }

    //rescue
    public void Rescue() {
        GalagaManager.Inst.pause = true; //일시 정지
        UnitIdx = 1; //player serial idx(1)로 변경
        score = 0; //player로 돌아왔기 때문에 점수 제거
        SR.sprite = sprites[0]; //아군 상태의 sprite로 변경
        StartCoroutine(SoundManager.Inst.PlayPlayerRescue()); //sound manager에서 구출 효과음 재생
    }

    //듀얼 보드 설정
    public void SetDoubleMode() {
        GalagaManager.Inst.bKidnaping = false; //manager 납치 파라미터 해제
        GalagaManager.Inst.bDualMode = true; //manager 듀얼 모드 설정
        GalagaManager.Inst.player.GetComponent<Gyaraga>().doubleObj = gameObject; //현재 플레이어에게 구출된 기체(this) 적재
        enemyMode = false; //enemy mode 해제
        ownerFollow = false; //납치 될 경우를 생각하여 해제
        doubleMode = true; //듀얼 모드 설정
        doubleObj = GalagaManager.Inst.player; //구출된 기체에 현재 플레이어 적재
        doubleObj.GetComponent<Gyaraga>().doubleMode = true; //현재 플레이어에 듀얼 모드 설정
        speed = doubleObj.GetComponent<Gyaraga>().speed; //구출된 기체의 속도를 현재 플레이어 속도와 동일하게 맞춤
        transform.rotation = doubleObj.transform.rotation; //구출된 기체가 현재 플레이어와 같은 방향을 보도록 설정
    }

    //납치되어 적군 모드로 변경
    public void SetEnemyMode() {
        GalagaManager.Inst.pause = true; //일시 정지
        UnitIdx = 2; //enemy serial idx(2)로 변경
        enemyMode = true; //적군 상태 파라미터 on
        score = 1000; //적군 상태에서 파괴될 경우 점수
        foreach (var o in bullet) {
            if (o != null) o.GetComponent<Object2D>().UnitIdx = 2; //납치 직전 발사된 총알들을 적군 상태의 serial idx로 변경
        }
        SR.sprite = sprites[1]; //적군 상태 sprite로 변경
    }

    public Quaternion SetAngle2D(Vector2 pos) {
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle - 90f);
    }
}
