using UnityEngine;

public class BossGalaga : Enemy {
    private GameObject preKidnapEffect, objKidnapEffect;
    private GameObject kidnapPlayer;
    private Animator anim;
    public bool bkidnap = false; //��ġ �� ����

    public bool bKidnapping = false; //��ġ ������

    public GameObject KidnapPlayer {
        get { return KidnapPlayer; }
        set { kidnapPlayer = value; }
    }
    public bool Kidnap {
        get { return bkidnap; }
        set { bkidnap = value; }
    }

    protected override void Awake() {
        base.Awake();
        preKidnapEffect = Resources.Load("Galaga/KidnapEffect") as GameObject;
        anim = GetComponent<Animator>();
    }

    protected override void Start() {
        base.Start();
        HP = 2;
    }

    protected override void Update() {
        if (mode == Mode.Attack) score = 400;
        else score = 150;

        if (GalagaManager.Inst.pause && mode != Mode.Flight) {
            if (!bkidnap || !bKidnapping) mode = Mode.Standby;
        }

        base.Update();

        if (mode == Mode.Attack) {
            //��ġ �Ķ���Ͱ� false�̰� ��� ��尡 false�� ��� ��ġ �������� ����
            if (!GalagaManager.Inst.bKidnaping && !GalagaManager.Inst.bDualMode) {
                if (Random.Range(0, 10) < 2) {
                    GalagaManager.Inst.bKidnaping = true;
                    bKidnapping = true;
                }
            }

            if (bKidnapping) { //��ġ ����
                if (posTemp == Vector3.zero) {
                    posTemp = transform.position;
                    if (transform.position.x <= 0) unitVectorX = new Vector3(-1f, 0f);
                }
                if (attackState == 0) {
                    bezierValue += curveSpeed * Time.deltaTime;
                    posBezier = BezierCurve(posTemp, posTemp + unitVectorY, posTemp + unitVectorX + unitVectorY, posTemp + unitVectorX, bezierValue);
                    SetAngle(posBezier);
                    transform.position = posBezier;
                    if (transform.position == posTemp + unitVectorX) attackState = 1;
                }
                else if (attackState == 1) {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, -4f), speed * Time.deltaTime);

                    if (transform.position == new Vector3(transform.position.x, -4f)) attackState = 2;
                }
                else if (attackState == 2) { //��ġ ��ġ ���� ����
                    attackState = 3;
                    objKidnapEffect = Instantiate(preKidnapEffect, transform.position + unitVectorY * -2, Quaternion.identity); //��ġ ������ �߻�
                    objKidnapEffect.GetComponent<KidnapEffect>().Owner = gameObject;
                }
                else if (attackState == 3) { //��ġ ������ �߻� ���� ����
                    if (!objKidnapEffect) {
                        attackState = 6;
                        RB.velocity = unitVectorY * -1 * speed;
                        bKidnapping = false;
                        if (!bkidnap) GalagaManager.Inst.bKidnaping = false;
                    }
                }
            }
            else { //�Ϲ� ����
                if (posTemp == Vector3.zero) {
                    posTemp = transform.position;
                    if (transform.position.x <= 0) unitVectorX = new Vector3(-1f, 0f);
                }
                if (attackState == 0) {
                    bezierValue += curveSpeed * Time.deltaTime;
                    posBezier = BezierCurve(posTemp, posTemp + unitVectorY, posTemp + unitVectorX + unitVectorY, posTemp + unitVectorX, bezierValue);
                    SetAngle(posBezier);
                    transform.position = posBezier;
                    if (transform.position == posTemp + unitVectorX) {
                        attackState = 1;
                        posTemp = transform.position;
                        bezierValue = 0;
                    }
                }
                else if (attackState == 1) {
                    attackState = 2;
                    if (GalagaManager.Inst.player) {
                        posAttack = GalagaManager.Inst.player.transform.position - transform.position;
                        RB.velocity = posAttack / posAttack.magnitude * speed;
                        SetAngle(posAttack);
                    }
                    else RB.velocity = transform.position / transform.position.magnitude * speed;
                }
                else if (attackState == 2) {
                    if (transform.position.y <= 3f) {
                        attackState = 3;
                        RB.velocity = Vector3.zero;
                        posTemp = transform.position;
                        bezierValue = 0;
                        ShootBullet();
                    }
                }
                else if (attackState == 3) {
                    bezierValue += curveSpeed * Time.deltaTime;
                    posBezier = BezierCurve(posTemp, posTemp + unitVectorY * -2, posTemp + unitVectorX * 2 + unitVectorY * -2, posTemp + unitVectorX * 2, bezierValue);
                    SetAngle(posBezier);
                    transform.position = posBezier;
                    if (transform.position == posTemp + unitVectorX * 2) {
                        attackState = 4;
                        bezierValue = 0;
                    }
                }
                else if (attackState == 4) {
                    bezierValue += curveSpeed * Time.deltaTime;
                    posBezier = BezierCurve(posTemp + unitVectorX * 2, posTemp + unitVectorX * 2 + unitVectorY * 2, posTemp + unitVectorY * 2, posTemp, bezierValue);
                    SetAngle(posBezier);
                    transform.position = posBezier;
                    if (transform.position == posTemp) attackState = 5;
                }
                else if (attackState == 5) {
                    attackState = 6;
                    if (GalagaManager.Inst.player) {
                        posAttack = GalagaManager.Inst.player.transform.position - transform.position;
                        RB.velocity = posAttack / posAttack.magnitude * speed;
                        SetAngle(posAttack);
                    }
                    else RB.velocity = transform.position / transform.position.magnitude * speed;
                }
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);
        if (HP <= 1) anim.SetTrigger("HalfHP");
        if (HP <= 0 && bkidnap) {
            kidnapPlayer.GetComponent<Gyaraga>().OwnerEnemy = null;
            kidnapPlayer.GetComponent<Gyaraga>().Rescue();
        }
    }

    protected override void DestroyObject() {
        if (bKidnapping) GalagaManager.Inst.bKidnaping = false; //��ġ ���� ���� �׾��� ��� GalagaManager�� ��ġ �Ķ���� ����
        StartCoroutine(SoundManager.Inst.PlayEnemyDestory(2));
        base.DestroyObject();
    }
}
