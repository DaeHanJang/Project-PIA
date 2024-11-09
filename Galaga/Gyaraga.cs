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
        if (HP <= 0) { //�ı��� ���
            //���� ������ ��
            if (enemyMode) {
                GalagaManager.Inst.bKidnaping = false; //manager ��ġ �Ķ���� ����
                ownerEnemy.GetComponent<BossGalaga>().bkidnap = false; //��ġ�� boss galaga ��ġ �Ķ���� ����
                GalagaManager.Inst.SetAddScore(score); //add score
            }
            else {
                if (doubleMode) { //��� ����� ���
                    GalagaManager.Inst.bDualMode = false; //manager ��� ��� �Ķ���� ����
                    if (GalagaManager.Inst.player == gameObject) { //������ �÷��̾ ���� ���
                        GalagaManager.Inst.player = doubleObj; //manager���� ����� �÷��̾ �� �÷��̾�� ����
                        doubleObj.GetComponent<Gyaraga>().doubleObj = null; //����� �÷��̾��� ¦ ��ü ������Ʈ ����
                        doubleObj.GetComponent<Gyaraga>().doubleMode = false; //����� �÷��̾��� ���� ��� �Ķ���� ����
                    }
                    else { //����� �÷��̾ ���� ���
                        doubleObj = null; //¦ ��ü ������Ʈ ����
                        doubleMode = false; //���� ��� �Ķ���� ����
                    }
                }
                //�Ϲ� ������ ��
                else { GalagaManager.Inst.Life--; }  //manager���� ��� �ϳ� ���� 
            }

            DestroyObject();
        }

        //������ �� ����
        if (!enemyMode) { //�Ϲ� ����
            float h = Input.GetAxisRaw("Horizontal");
            Vector3 curPos = transform.position;
            Vector3 nextPos = new Vector3(h, 0, 0) * speed;
            Vector3 pos = curPos + nextPos;
            if (!doubleMode) { //��� ��� �ƴ� ��
                if (pos.x <= -4f) pos.x = -4f;
                else if (pos.x >= 4f) pos.x = 4f;
            }
            else { //��� ����� ��
                if (GalagaManager.Inst.player == gameObject) { //������ ��ü
                    if (pos.x <= -4f) pos.x = -4f;
                    else if (pos.x >= 3.5f) pos.x = 3.5f; //�����ʿ� ����� ��ü�� �����Ƿ� ������ ���� 0.5f��ŭ ����
                }
                else { //����� ��ü
                    if (pos.x <= -3.5f) pos.x = -3.5f; //������ ��ü�� �����Ƿ� ���� ���� 0.5f��ŭ ����
                    else if (pos.x >= 4f) pos.x = 4f;
                }
            }
            transform.position = pos;

            if (Input.GetKeyDown(KeyCode.Space) && bulletCnt < 3) { //�Ѿ��� �ִ� 3������ �߻� ����
                bullet[bulletCnt] = Instantiate(preBullet, transform.position, Quaternion.identity);
                bullet[bulletCnt].GetComponent<PlayerBullet>().Player = this;
                bulletCnt++;
            }
        }
        else { //���� ����� ��
            if (!ownerEnemy) { //�θ� boss galaga�� �ı����� ���
                if (ownerFollow) { //������
                    transform.position = Vector3.MoveTowards(transform.position, GalagaManager.Inst.player.transform.position + new Vector3(0.5f, 0f), speed * 1.5f);
                    SetAngle(GalagaManager.Inst.player.transform.position + new Vector3(0f, 0.5f) - transform.position);
                    if (transform.position == GalagaManager.Inst.player.transform.position + new Vector3(0.5f, 0f)) {
                        SetDoubleMode(); //double mode setting
                        GalagaManager.Inst.pause = false; //�Ͻ� ���� ����
                    }
                }
            }
            else { //�θ� boss galaga�� ������� ���
                if (!ownerFollow) { //����ٴϱ� ��� ��Ȱ��ȭ(��ġ��)
                    transform.position = Vector3.MoveTowards(transform.position, ownerEnemy.transform.position + new Vector3(0f, 0.5f), speed * 1.5f);
                    SetAngle(ownerEnemy.transform.position + new Vector3(0f, 0.5f) - transform.position);
                    if (transform.position == ownerEnemy.transform.position + new Vector3(0f, 0.5f)) {
                        ownerFollow = true;
                        GalagaManager.Inst.pause = false; //�Ͻ� ���� ����
                    }
                }
                else { //����ٴϱ� ��� Ȱ��ȭ(��ġ�Ϸ�)
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
        GalagaManager.Inst.pause = true; //�Ͻ� ����
        UnitIdx = 1; //player serial idx(1)�� ����
        score = 0; //player�� ���ƿԱ� ������ ���� ����
        SR.sprite = sprites[0]; //�Ʊ� ������ sprite�� ����
        StartCoroutine(SoundManager.Inst.PlayPlayerRescue()); //sound manager���� ���� ȿ���� ���
    }

    //��� ���� ����
    public void SetDoubleMode() {
        GalagaManager.Inst.bKidnaping = false; //manager ��ġ �Ķ���� ����
        GalagaManager.Inst.bDualMode = true; //manager ��� ��� ����
        GalagaManager.Inst.player.GetComponent<Gyaraga>().doubleObj = gameObject; //���� �÷��̾�� ����� ��ü(this) ����
        enemyMode = false; //enemy mode ����
        ownerFollow = false; //��ġ �� ��츦 �����Ͽ� ����
        doubleMode = true; //��� ��� ����
        doubleObj = GalagaManager.Inst.player; //����� ��ü�� ���� �÷��̾� ����
        doubleObj.GetComponent<Gyaraga>().doubleMode = true; //���� �÷��̾ ��� ��� ����
        speed = doubleObj.GetComponent<Gyaraga>().speed; //����� ��ü�� �ӵ��� ���� �÷��̾� �ӵ��� �����ϰ� ����
        transform.rotation = doubleObj.transform.rotation; //����� ��ü�� ���� �÷��̾�� ���� ������ ������ ����
    }

    //��ġ�Ǿ� ���� ���� ����
    public void SetEnemyMode() {
        GalagaManager.Inst.pause = true; //�Ͻ� ����
        UnitIdx = 2; //enemy serial idx(2)�� ����
        enemyMode = true; //���� ���� �Ķ���� on
        score = 1000; //���� ���¿��� �ı��� ��� ����
        foreach (var o in bullet) {
            if (o != null) o.GetComponent<Object2D>().UnitIdx = 2; //��ġ ���� �߻�� �Ѿ˵��� ���� ������ serial idx�� ����
        }
        SR.sprite = sprites[1]; //���� ���� sprite�� ����
    }

    public Quaternion SetAngle2D(Vector2 pos) {
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle - 90f);
    }
}
