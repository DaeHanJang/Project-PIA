using UnityEngine;

public class BossGalaga : Enemy {
    private GameObject preKidnapEffect, objKidnapEffect;
    private GameObject kidnapPlayer;
    private Animator anim;
    public bool bkidnap = false; //납치 된 상태

    public bool bKidnapping = false; //납치 진행중

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
            //납치 파라미터가 false이고 듀얼 모드가 false일 경우 납치 공격할지 선택
            if (!GalagaManager.Inst.bKidnaping && !GalagaManager.Inst.bDualMode) {
                if (Random.Range(0, 10) < 2) {
                    GalagaManager.Inst.bKidnaping = true;
                    bKidnapping = true;
                }
            }

            if (bKidnapping) { //납치 공격
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
                else if (attackState == 2) { //납치 위치 도달 상태
                    attackState = 3;
                    objKidnapEffect = Instantiate(preKidnapEffect, transform.position + unitVectorY * -2, Quaternion.identity); //납치 레이저 발사
                    objKidnapEffect.GetComponent<KidnapEffect>().Owner = gameObject;
                }
                else if (attackState == 3) { //납치 레이저 발사 종료 상태
                    if (!objKidnapEffect) {
                        attackState = 6;
                        RB.velocity = unitVectorY * -1 * speed;
                        bKidnapping = false;
                        if (!bkidnap) GalagaManager.Inst.bKidnaping = false;
                    }
                }
            }
            else { //일반 공격
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
        if (bKidnapping) GalagaManager.Inst.bKidnaping = false; //납치 공격 도중 죽었을 경우 GalagaManager의 납치 파라미터 보정
        StartCoroutine(SoundManager.Inst.PlayEnemyDestory(2));
        base.DestroyObject();
    }
}
