using ObjectTools;
using System.Collections;
using UnityEngine;

public class Enemy : Object2D {
    protected enum Mode { //적 오브젝트 상태 값
        Flight,
        Standby,
        Attack,
        Challenge
    };

    private Vector3[,] pathPoint = new Vector3[4, 2]; //Flight모드 이동 경로 포인트
    private int movementState = 0; //Flight모드 시 진행 상황
    private int idxCh = 0; //첼린지스테이지 인덱스

    protected GameObject player; //현제 플레이어
    protected GameObject bullet; //총알
    protected Rigidbody2D RB; //리지드 바디
    protected Transform posFormation; //Stanby모드 시 위치
    protected Vector3 posTemp = Vector3.zero; //임시 저장 위치
    protected Vector3 posBezier; //베지어 곡선 적용 후 위치 값
    protected Vector3 posAttack; //공격 좌표
    protected Vector3 unitVectorX = new Vector3(1f, 0f); //비행 계산에 사용되는 x좌표 단위 벡터
    protected Vector3 unitVectorY = new Vector3(0f, 1f); //비힝 계산에 사용되는 y좌표 단위 벡터
    protected Mode mode = Mode.Flight;
    protected bool directAttack; //Flight모드에서 공격하는 오브젝트인지
    protected bool doublePath; //Flight모드에서 옆으로 생성되는 오브젝트인지
    protected int path; //Flight모드 경로(0 ~ 3)
    protected int score; //점수
    protected int attackState = 0; //Attack모드 시 진행 상황
    protected float bezierValue = 0f; //베지어 곡선 값(0 ~ 1)

    public GameObject Player {
        get { return player; }
        set { player = value; }
    }
    public Transform PosFormation {
        get { return posFormation; }
        set { posFormation = value; }
    }
    public bool DirectAttack {
        get { return directAttack; }
        set { directAttack = value; }
    }
    public bool DoublePath {
        get { return doublePath; }
        set { doublePath = value; }
    }
    public int Path {
        get { return path; }
        set { path = value; }
    }
    public int AttackState {
        get { return attackState; }
        set { attackState = value; }
    }

    protected virtual void Awake() {
        pathPoint[0, 0] = GameObject.Find("Path1").transform.GetChild(1).position;
        pathPoint[0, 1] = GameObject.Find("Path1").transform.GetChild(2).position;
        pathPoint[1, 0] = GameObject.Find("Path2").transform.GetChild(1).position;
        pathPoint[1, 1] = GameObject.Find("Path2").transform.GetChild(2).position;
        pathPoint[2, 0] = GameObject.Find("Path3").transform.GetChild(1).position;
        pathPoint[2, 1] = GameObject.Find("Path3").transform.GetChild(2).position;
        pathPoint[3, 0] = GameObject.Find("Path4").transform.GetChild(1).position;
        pathPoint[3, 1] = GameObject.Find("Path4").transform.GetChild(2).position;
        player = GalagaManager.Inst.player;
        bullet = Resources.Load("Galaga/EnemyBullet") as GameObject;
        RB = GetComponent<Rigidbody2D>();
        posFormation = GalagaManager.Inst.formation[GalagaManager.Inst.enemyCreateCnt < 40 ? GalagaManager.Inst.enemyCreateCnt : 0];
        path = GalagaManager.Inst.path;
        directAttack = GalagaManager.Inst.directAttack;
        doublePath = GalagaManager.Inst.doublePath;
        if (GalagaManager.Inst.challengeStage) mode = Mode.Challenge;
    }

    protected virtual void Start() {
        UnitIdx = 2;
        speed = 5f;
        curveSpeed = 2f;
    }

    protected virtual void Update() {
        if (HP <= 0) {
            GalagaManager.Inst.SetAddScore(score);
            if (!directAttack) GalagaManager.Inst.enemyLiveCnt--;
            if (mode == Mode.Challenge) GalagaManager.Inst.enemyKillCnt++;
            DestroyObject();
        }

        if (mode == Mode.Flight || mode == Mode.Challenge) SetPath();
        else if (mode == Mode.Standby) {
            RB.velocity = Vector3.zero;
            unitVectorX = new Vector3(1f, 0f);
            unitVectorY = new Vector3(0f, 1f);
            posTemp = Vector3.zero;
            bezierValue = 0;
            attackState = 0;

            transform.position = Vector3.MoveTowards(transform.position, posFormation.position, speed * Time.deltaTime);
            SetAngle(posFormation.position);

            if (GalagaManager.Inst.bAttack && !GalagaManager.Inst.pause) {
                if (GalagaManager.Inst.enemyLiveCnt < 4 && Random.Range(0, 1000) < GalagaManager.Inst.stage * 2) mode = Mode.Attack;
                else if (Random.Range(0, 10000) < GalagaManager.Inst.stage * 2) mode = Mode.Attack;
            }
        }

        if (mode == Mode.Attack && transform.position.y <= -11) {
            RB.velocity = Vector3.zero;
            transform.position = new Vector3(transform.position.x, 9f);
            mode = Mode.Standby;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Object2D>() != null) {
            if (collision.gameObject.GetComponent<Object2D>().UnitIdx != UnitIdx) HP--;
        }
    }

    protected void SetPath() {
        Vector3 doublePathVector = Vector3.zero;
        if (mode == Mode.Flight) {
            if (path == 0 || path == 1) {
                if (doublePath) doublePathVector = new Vector3(((path == 0) ? 1f : -1f), 0f);
                if (movementState == 0) {
                    transform.position = Vector3.MoveTowards(transform.position, pathPoint[path, 0] + doublePathVector, speed * Time.deltaTime);
                    SetAngle(pathPoint[path, 0] + doublePathVector);
                    if (transform.position == pathPoint[path, 0] + doublePathVector) movementState = 1;
                }
                else if (movementState == 1) {
                    bezierValue += curveSpeed * Time.deltaTime;
                    posBezier = BezierCurve(pathPoint[path, 0] + doublePathVector, pathPoint[path, 0] + doublePathVector + unitVectorY * -1, pathPoint[path, 1] + doublePathVector + unitVectorY * -1, pathPoint[path, 1] + doublePathVector, bezierValue);
                    SetAngle(posBezier);
                    transform.position = posBezier;
                    if (bezierValue >= 0.5 && directAttack) {
                        movementState = 2;
                        if (GalagaManager.Inst.player) posAttack = GalagaManager.Inst.player.transform.position - transform.position;
                        else posAttack = new Vector3(0f, -7f) - posBezier;
                        ShootBullet();
                    }
                    if (transform.position == pathPoint[path, 1] + doublePathVector) movementState = 3;
                }
                else if (movementState == 2) {
                    RB.velocity = posAttack / posAttack.magnitude * speed;
                    SetSigleAngle(posAttack);

                    if (transform.position.y <= -9) Destroy(gameObject);
                }
                else mode = Mode.Standby;
            }
            else {
                if (doublePath) doublePathVector = new Vector3(0f, -1f);
                if (path == 3) unitVectorX = new Vector3(-1f, 0f);
                if (movementState == 0) {
                    transform.position = Vector3.MoveTowards(transform.position, pathPoint[path, 0] + doublePathVector, speed * Time.deltaTime);
                    SetAngle(pathPoint[path, 0] + doublePathVector);
                    if (transform.position == pathPoint[path, 0] + doublePathVector) movementState = 1;
                }
                else if (movementState == 1) {
                    bezierValue += curveSpeed * Time.deltaTime;
                    posBezier = BezierCurve(pathPoint[path, 0] + doublePathVector, pathPoint[path, 0] + doublePathVector + unitVectorX, pathPoint[path, 1] + doublePathVector + unitVectorX, pathPoint[path, 1] + doublePathVector, bezierValue);
                    SetAngle(posBezier);
                    transform.position = posBezier;
                    if (transform.position == pathPoint[path, 1] + doublePathVector) {
                        movementState = 2;
                        bezierValue = 0;
                    }
                }
                else if (movementState == 2) {
                    bezierValue += curveSpeed * Time.deltaTime;
                    posBezier = BezierCurve(pathPoint[path, 1] + doublePathVector, pathPoint[path, 1] + doublePathVector + unitVectorX * -1, pathPoint[path, 0] + doublePathVector + unitVectorX * -1, pathPoint[path, 0] + doublePathVector, bezierValue);
                    SetAngle(posBezier);
                    transform.position = posBezier;
                    if (bezierValue >= 0.5 && directAttack) {
                        movementState = 3;
                        if (GalagaManager.Inst.player) posAttack = GalagaManager.Inst.player.transform.position - transform.position;
                        else posAttack = new Vector3(0f, -7f) - posBezier;
                        ShootBullet();
                    }
                    if (transform.position == pathPoint[path, 0] + doublePathVector) movementState = 4;
                }
                else if (movementState == 3) {
                    RB.velocity = posAttack / posAttack.magnitude * speed;
                    SetSigleAngle(posAttack);

                    if (transform.position.y <= -9) Destroy(gameObject);
                }
                else mode = Mode.Standby;
            }
        }
        if (mode == Mode.Challenge) {
            if (GalagaManager.Inst.stage == 3) {
                if (path == 0 || path == 1) {
                    if (path == 1) unitVectorX = new Vector3(-1f, 0f);
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX, speed * Time.deltaTime);
                        SetAngle(unitVectorX);
                        if (transform.position.y <= 0) {
                            movementState = 1;
                            posTemp = transform.position;
                        }
                    }
                    else if (movementState == 1) {
                        bezierValue += curveSpeed * Time.deltaTime / 3;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * -3, posTemp + unitVectorX * -4 + unitVectorY * -3, posTemp + unitVectorX * -4, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorX * -4) {
                            movementState = 2;
                            posTemp = transform.position;
                            bezierValue = 0f;
                        }
                    }
                    else if (movementState == 2) {
                        bezierValue += curveSpeed * Time.deltaTime / 3;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * 3, posTemp + unitVectorX * 4 + unitVectorY * 3, posTemp + unitVectorX * 4, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (bezierValue >= 0.25) movementState = 3;
                    }
                    else if (movementState == 3) {
                        movementState = 4;
                        posAttack = unitVectorX * 5 + unitVectorY * 7;
                        RB.velocity = posAttack / posAttack.magnitude * speed;
                        SetAngle(posAttack);
                    }
                    else {
                        if ((path == 0 && transform.position.x >= 5f) || (path == 1 && transform.position.x <= -5f)) {
                            GalagaManager.Inst.enemyLiveCnt--;
                            Destroy(gameObject);
                        }
                    }
                }
                else {
                    if (path == 3) unitVectorX = new Vector3(-1f, 0f);
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX + unitVectorY * -3, speed * Time.deltaTime);
                        SetAngle(unitVectorX + unitVectorY * -3);
                        if (transform.position == unitVectorX + unitVectorY * -3) movementState = 1;
                    }
                    else if (movementState == 1) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX + unitVectorY * 3, speed * Time.deltaTime);
                        SetAngle(unitVectorX + unitVectorY * 3);
                        if (transform.position == unitVectorX + unitVectorY * 3) movementState = 2;
                    }
                    else if (movementState == 2) {
                        bezierValue += curveSpeed * Time.deltaTime;
                        posBezier = BezierCurve(unitVectorX + unitVectorY * 3, unitVectorX + unitVectorY * 5, unitVectorX * -1 + unitVectorY * 5, unitVectorX * -1 + unitVectorY * 3, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == unitVectorX * -1 + unitVectorY * 3) {
                            movementState = 3;
                            bezierValue = 0;
                        }
                    }
                    else if (movementState == 3) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * -1 + unitVectorY * -3, speed * Time.deltaTime);
                        SetAngle(unitVectorX * -1 + unitVectorY * -3);
                        if (transform.position == unitVectorX * -1 + unitVectorY * -3) movementState = 4;
                    }
                    else if (movementState == 4) {
                        bezierValue += curveSpeed * Time.deltaTime;
                        posBezier = BezierCurve(unitVectorX * -1 + unitVectorY * -3, unitVectorX * -1 + unitVectorY * -5, unitVectorX + unitVectorY * -5, unitVectorX + unitVectorY * -3, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == unitVectorX + unitVectorY * -3) movementState = 5;
                    }
                    else if (movementState == 5) {
                        movementState = 6;
                        posAttack = unitVectorX * 5 + unitVectorY * 7;
                        RB.velocity = posAttack / posAttack.magnitude * speed;
                        SetAngle(posAttack);
                    }
                    else {
                        if ((path == 2 && transform.position.x >= 5f) || (path == 3 && transform.position.x <= -5f)) {
                            GalagaManager.Inst.enemyLiveCnt--;
                            Destroy(gameObject);
                        }
                    }
                }
            }
            else if (GalagaManager.Inst.stage == 7) {
                if (path == 0 || path == 1) {
                    if (path == 1) unitVectorX = new Vector3(-1f, 0f);
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX, speed * Time.deltaTime);
                        SetAngle(unitVectorX);
                        if (transform.position.y <= 0) {
                            movementState = 1;
                            posTemp = transform.position;
                        }
                    }
                    else if (movementState == 1) {
                        bezierValue += curveSpeed * Time.deltaTime / 3;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * -3, posTemp + unitVectorX * -4 + unitVectorY * -3, posTemp + unitVectorX * -4, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorX * -4) {
                            movementState = 2;
                            posTemp = transform.position;
                            bezierValue = 0f;
                        }
                    }
                    else if (movementState == 2) {
                        bezierValue += curveSpeed * Time.deltaTime / 3;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * 3, posTemp + unitVectorX * 4 + unitVectorY * 3, posTemp + unitVectorX * 4, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (bezierValue >= 0.25) movementState = 3;
                    }
                    else if (movementState == 3) {
                        movementState = 4;
                        posAttack = unitVectorX * 5 + unitVectorY * 7;
                        RB.velocity = posAttack / posAttack.magnitude * speed;
                        SetAngle(posAttack);
                    }
                    else {
                        if ((path == 0 && transform.position.x >= 5f) || (path == 1 && transform.position.x <= -5f)) {
                            GalagaManager.Inst.enemyLiveCnt--;
                            Destroy(gameObject);
                        }
                    }
                }
                else {
                    if (path == 3) unitVectorX = new Vector3(-1f, 0f);
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorY * -6);
                        if (transform.position == unitVectorY * -6) {
                            movementState = 1;
                            posTemp = transform.position;
                        }
                    }
                    else if (movementState == 1) {
                        bezierValue += curveSpeed * Time.deltaTime / 4;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorX * 4, posTemp + unitVectorX * 4 + unitVectorY * 6, posTemp + unitVectorY * 6, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorY * 6) {
                            movementState = 2;
                            posTemp = transform.position;
                            bezierValue = 0;
                        }
                    }
                    else if (movementState == 2) {
                        bezierValue += curveSpeed * Time.deltaTime / 4;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorX * -4, posTemp + unitVectorX * -4 + unitVectorY * -6, posTemp + unitVectorY * -6, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorY * -6) movementState = 3;
                    }
                    else if (movementState == 3) {
                        movementState = 4;
                        posAttack = unitVectorX * 5;
                        RB.velocity = posAttack / posAttack.magnitude * speed;
                        SetAngle(posAttack);
                    }
                    else {
                        if ((path == 2 && transform.position.x >= 5f) || (path == 3 && transform.position.x <= -5f)) {
                            GalagaManager.Inst.enemyLiveCnt--;
                            Destroy(gameObject);
                        }
                    }
                }
            }
            else if (GalagaManager.Inst.stage == 11) {
                if (path == 0 || path == 1) {
                    if (path == 1) unitVectorX = new Vector3(-1f, 0f);
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);
                        SetAngle(Vector3.zero);
                        if (transform.position == Vector3.zero) movementState = 1;
                    }
                    else if (movementState == 1) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * 4 + unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorX * 4 + unitVectorY * -6);
                        if (transform.position == unitVectorX * 4 + unitVectorY * -6) movementState = 2;
                    }
                    else if (movementState == 2) {
                        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);
                        SetAngle(Vector3.zero);
                        if (transform.position == Vector3.zero) movementState = 3;
                    }
                    else if (movementState == 3) {
                        movementState = 4;
                        posTemp = unitVectorY * 9;
                        RB.velocity = posTemp / posTemp.magnitude * speed;
                        SetAngle(posTemp);
                    }
                    else {
                        if (transform.position.y >= 9f) {
                            GalagaManager.Inst.enemyLiveCnt--;
                            Destroy(gameObject);
                        }
                    }
                }
                else {
                    if (path == 2) unitVectorX = new Vector3(-1f, 0f);
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorY * -6);
                        if (transform.position == unitVectorY * -6) {
                            movementState = 1;
                            posTemp = transform.position;
                        }
                    }
                    else if (movementState == 1) {
                        bezierValue += curveSpeed * Time.deltaTime / 4;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorX * 4, posTemp + unitVectorX * 4 + unitVectorY * 12, posTemp + unitVectorY * 12, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorY * 12) movementState = 2;
                    }
                    else if (movementState == 2) {
                        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);
                        SetAngle(Vector3.zero);
                        if (transform.position == Vector3.zero) movementState = 3;
                    }
                    else if (movementState == 3) {
                        movementState = 4;
                        posTemp = unitVectorX * 5;
                        RB.velocity = posTemp / posTemp.magnitude * speed;
                        SetSigleAngle(posTemp);
                    }
                    else {
                        if ((path == 2 && transform.position.x >= 5f) || (path == 3 && transform.position.x <= -5f)) {
                            GalagaManager.Inst.enemyLiveCnt--;
                            Destroy(gameObject);
                        }
                    }
                }
            }
            else if (GalagaManager.Inst.stage == 15) {
                if (path == 0 || path == 1) {
                    if (posTemp == Vector3.zero) {
                        posTemp = transform.position;
                        if (path == 0) unitVectorX = new Vector3(-1f, 0f);
                    }
                    if (movementState == 0) {
                        bezierValue += curveSpeed * Time.deltaTime / 1.5f;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorX * 2, posTemp + unitVectorX * 2 + unitVectorY * -3, posTemp + unitVectorY * -3, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorY * -3) {
                            posTemp = transform.position;
                            bezierValue = 0;
                            unitVectorX *= -1;
                            if (++idxCh == 4) movementState = 1;
                        }
                    }
                    else if (movementState == 1) {
                        bezierValue += curveSpeed * Time.deltaTime / 1.5f;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorX * 2, posTemp + unitVectorX * 2 + unitVectorY * 3, posTemp + unitVectorY * 3, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorY * 3) {
                            posTemp = transform.position;
                            bezierValue = 0;
                            unitVectorX *= -1;
                            if (++idxCh == 8) movementState = 2;
                        }
                    }
                    else {
                        GalagaManager.Inst.enemyLiveCnt--;
                        Destroy(gameObject);
                    }
                }
                else {
                    if (path == 3) unitVectorX = new Vector3(-1f, 0f);
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * -3 + unitVectorY * -4, speed * Time.deltaTime);
                        SetAngle(unitVectorX * -3 + unitVectorY * -4);
                        if (transform.position == unitVectorX * -3 + unitVectorY * -4) {
                            posTemp = transform.position;
                            movementState = 1;
                        }
                    }
                    else if (movementState == 1) {
                        transform.position = Vector3.MoveTowards(transform.position, posTemp + unitVectorX, speed * Time.deltaTime);
                        SetAngle(posTemp + unitVectorX);
                        if (transform.position == posTemp + unitVectorX) {
                            if (idxCh >= 5) movementState = 4;
                            else movementState = 2;
                            posTemp = transform.position;
                        }
                    }
                    else if (movementState == 2) {
                        bezierValue += curveSpeed * Time.deltaTime;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorX, posTemp + unitVectorX + unitVectorY * -2, posTemp + unitVectorY * -2, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorY * -2) {
                            movementState = 3;
                            posTemp = transform.position;
                            bezierValue = 0;
                        }
                    }
                    else if (movementState == 3) {
                        bezierValue += curveSpeed * Time.deltaTime;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorX * -1, posTemp + unitVectorX * -1 + unitVectorY * 2, posTemp + unitVectorY * 2, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorY * 2) {
                            if (idxCh <= 5) movementState = 1;
                            posTemp = transform.position;
                            bezierValue = 0;
                            idxCh++;
                        }
                    }
                    else if (movementState == 4) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * 4.5f + unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorX * 4.5f + unitVectorY * -6);
                        if (transform.position == unitVectorX * 4.5f + unitVectorY * -6) movementState = 5;
                    }
                    else {
                        GalagaManager.Inst.enemyLiveCnt--;
                        Destroy(gameObject);
                    }
                }
            }
            else if (GalagaManager.Inst.stage == 19) {
                if (path == 0 || path == 1) {
                    if (posTemp == Vector3.zero) {
                        if (path == 0) unitVectorX = new Vector3(-1f, 0f);
                        unitVectorX *= 5;
                        posTemp = transform.position;
                    }
                    if (movementState == 0) {
                        bezierValue += curveSpeed * Time.deltaTime / (-0.5f * idxCh + 3);
                        if (path == 0) posBezier = BezierCurve(posTemp, posTemp + new Vector3(-(5 - idxCh), 0), posTemp + new Vector3(-(5 - idxCh), -(14 - idxCh * 2)), posTemp + new Vector3(0, -(14 - idxCh * 2)), bezierValue);
                        else posBezier = BezierCurve(posTemp, posTemp + new Vector3(5 - idxCh, 0), posTemp + new Vector3(5 - idxCh, -(14 - idxCh * 2)), posTemp + new Vector3(0, -(14 - idxCh * 2)), bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + new Vector3(0, -(14 - idxCh * 2))) {
                            if (idxCh == 4) movementState = 2;
                            else movementState = 1;
                            posTemp = transform.position;
                            bezierValue = 0;
                            idxCh++;
                        }
                    }
                    else if (movementState == 1) {
                        bezierValue += curveSpeed * Time.deltaTime / (-0.5f * idxCh + 3);
                        if (path == 0) posBezier = BezierCurve(posTemp, posTemp + new Vector3(4 - idxCh, 0), posTemp + new Vector3(4 - idxCh, 14 - idxCh * 2), posTemp + new Vector3(0, 14 - idxCh * 2), bezierValue);
                        else posBezier = BezierCurve(posTemp, posTemp + new Vector3(-(4 - idxCh), 0), posTemp + new Vector3(-(4 - idxCh), 14 - idxCh * 2), posTemp + new Vector3(0, 14 - idxCh * 2), bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + new Vector3(0, 14 - idxCh * 2)) {
                            movementState = 0;
                            posTemp = transform.position;
                            bezierValue = 0;
                            idxCh++;
                        }
                    }
                    else if (movementState == 2) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX + unitVectorY * 6, speed * Time.deltaTime);
                        SetAngle(unitVectorX + unitVectorY * 6);
                        if (transform.position == unitVectorX + unitVectorY * 6) movementState = 3;
                    }
                    else {
                        GalagaManager.Inst.enemyLiveCnt--;
                        Destroy(gameObject);
                    }
                }
                else {
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);
                        SetAngle(Vector3.zero);
                        if (transform.position == Vector3.zero) {
                            movementState = 1;
                            posTemp = transform.position;
                            if (idxCh == 0) unitVectorX = new Vector3(-1f, 0f);
                        }
                    }
                    else if (movementState == 1) {
                        bezierValue += curveSpeed * Time.deltaTime / 2;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * 2, posTemp + unitVectorX * 4 + unitVectorY * 2, posTemp + unitVectorX * 4, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorX * 4) {
                            movementState = 2;
                            posTemp = transform.position;
                            bezierValue = 0;
                        }
                    }
                    else if (movementState == 2) {
                        bezierValue += curveSpeed * Time.deltaTime / 2;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * -2, posTemp + unitVectorX * -4 + unitVectorY * -2, posTemp + unitVectorX * -4, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorX * -4) {
                            if (idxCh == 1) movementState = 3;
                            else movementState = 0;
                            posTemp = transform.position;
                            bezierValue = 0;
                            unitVectorX *= -1;
                            idxCh++;
                        }
                    }
                    else if (movementState == 3) {
                        bezierValue += curveSpeed * Time.deltaTime / 2;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorX * 2, posTemp + unitVectorX * 2 + unitVectorY * 4, posTemp + unitVectorY * 4, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorY * 4) {
                            movementState = 4;
                            posTemp = transform.position;
                            bezierValue = 0;
                        }
                    }
                    else if (movementState == 4) {
                        bezierValue += curveSpeed * Time.deltaTime / 2;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorX * -2, posTemp + unitVectorX * -2 + unitVectorY * -4, posTemp + unitVectorY * -4, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorY * -4) {
                            movementState = 5;
                            posTemp = transform.position;
                            bezierValue = 0;
                        }
                    }
                    else if (movementState == 5) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * 4.5f + unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorX * 4.5f + unitVectorY * -6);
                        if (transform.position == unitVectorX * 4.5f + unitVectorY * -6) movementState = 6;
                    }
                    else {
                        GalagaManager.Inst.enemyLiveCnt--;
                        Destroy(gameObject);
                    }
                }
            }
            else if (GalagaManager.Inst.stage == 23) {
                if (path == 0 || path == 1) {
                    if (posTemp == Vector3.zero) {
                        if (path == 1) unitVectorX = new Vector3(-1f, 0f);
                        posTemp = transform.position;
                    }
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, posTemp + unitVectorY * -3, speed * Time.deltaTime);
                        SetAngle(posTemp + unitVectorY * -3);
                        if (transform.position == posTemp + unitVectorY * -3) {
                            movementState = 1;
                            posTemp = transform.position;
                            idxCh++;
                        }
                    }
                    else if (movementState == 1) {
                        transform.position = Vector3.MoveTowards(transform.position, posTemp + unitVectorX * 3, speed * Time.deltaTime);
                        SetAngle(posTemp + unitVectorX * 3);
                        if (transform.position == posTemp + unitVectorX * 3) {
                            if (idxCh <= 5) movementState = 0;
                            else movementState = 2;
                            posTemp = transform.position;
                            idxCh++;
                        }
                    }
                    else {
                        GalagaManager.Inst.enemyLiveCnt--;
                        Destroy(gameObject);
                    }
                }
                else {
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorY * -6);
                        if (transform.position == unitVectorY * -6) {
                            movementState = 1;
                            posTemp = transform.position;
                            if (path == 2) unitVectorX = new Vector3(-1f, 0f);
                        }
                    }
                    else if (movementState == 1) {
                        bezierValue += curveSpeed * Time.deltaTime;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorX * -3.5f, posTemp + unitVectorX * -3.5f + unitVectorY * 3.5f, posTemp + unitVectorY * 3.5f, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (idxCh == 2 && bezierValue >= 0.5f) movementState = 2;
                        if (transform.position == posTemp + unitVectorY * 3.5f) {
                            posTemp = transform.position;
                            bezierValue = 0;
                            idxCh++;
                            if (idxCh % 2 != 0) unitVectorX *= -1;
                        }
                    }
                    else if (movementState == 2) {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 8f), speed * Time.deltaTime);
                        SetAngle(new Vector3(transform.position.x, 8f));
                        if (transform.position == new Vector3(transform.position.x, 8f)) movementState = 3;
                    }
                    else {
                        GalagaManager.Inst.enemyLiveCnt--;
                        Destroy(gameObject);
                    }
                }
            }
            else if (GalagaManager.Inst.stage == 27) {
                if (path == 0 || path == 1) {
                    if (path == 0) unitVectorX = new Vector3(-1f, 0f);
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX + unitVectorY * 5, speed * Time.deltaTime);
                        SetAngle(unitVectorX + unitVectorY * 5);
                        if (transform.position == unitVectorX + unitVectorY * 5) movementState = 1;
                    }
                    else if (movementState == 1) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * 4 + unitVectorY * 5, speed * Time.deltaTime);
                        SetAngle(unitVectorX * 4 + unitVectorY * 5);
                        if (transform.position == unitVectorX * 4 + unitVectorY * 5) movementState = 2;
                    }
                    else if (movementState == 2) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * -4.5f + unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorX * -4.5f + unitVectorY * -6);
                        if (transform.position == unitVectorX * -4.5f + unitVectorY * -6) movementState = 3;
                    }
                    else {
                        GalagaManager.Inst.enemyLiveCnt--;
                        Destroy(gameObject);
                    }
                }
                else {
                    if (path == 2) unitVectorX = new Vector3(-1f, 0f);
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * 3 + unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorX * 3 + unitVectorY * -6);
                        if (transform.position == unitVectorX * 3 + unitVectorY * -6) movementState = 1;
                    }
                    else if (movementState == 1) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * 3 + unitVectorY * 4, speed * Time.deltaTime);
                        SetAngle(unitVectorX * 3 + unitVectorY * 4);
                        if (transform.position == unitVectorX * 3 + unitVectorY * 4) movementState = 2;
                    }
                    else if (movementState == 2) {
                        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);
                        SetAngle(Vector3.zero);
                        if (transform.position == Vector3.zero) movementState = 3;
                    }
                    else if (movementState == 3) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * -3 + unitVectorY * 4, speed * Time.deltaTime);
                        SetAngle(unitVectorX * -3 + unitVectorY * 4);
                        if (transform.position == unitVectorX * -3 + unitVectorY * 4) movementState = 4;
                    }
                    else if (movementState == 4) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * -3 + unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorX * -3 + unitVectorY * -6);
                        if (transform.position == unitVectorX * -3 + unitVectorY * -6) movementState = 5;
                    }
                    else if (movementState == 5) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * -4.5f + unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorX * -4.5f + unitVectorY * -6);
                        if (transform.position == unitVectorX * -4.5f + unitVectorY * -6) movementState = 6;
                    }
                    else {
                        GalagaManager.Inst.enemyLiveCnt--;
                        Destroy(gameObject);
                    }
                }
            }
            else if (GalagaManager.Inst.stage == 31) {
                if (path == 0 || path == 1) {
                    if (path == 0) unitVectorX = new Vector3(-1f, 0f);
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX + unitVectorY * 3, speed * Time.deltaTime);
                        SetAngle(unitVectorX + unitVectorY * 3);
                        if (transform.position == unitVectorX + unitVectorY * 3) {
                            movementState = 1;
                            posTemp = transform.position;
                        }
                    }
                    else if (movementState == 1) {
                        bezierValue += curveSpeed * Time.deltaTime;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * -2, posTemp + unitVectorX * 2 + unitVectorY * -2, posTemp + unitVectorX * 2, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorX * 2) {
                            movementState = 2;
                            posTemp = transform.position;
                            bezierValue = 0;
                        }
                    }
                    else if (movementState == 2) {
                        bezierValue += curveSpeed * Time.deltaTime;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * 2, posTemp + unitVectorX * -2 + unitVectorY * 2, posTemp + unitVectorX * -2, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorX * -2) movementState = 3;
                    }
                    else if (movementState == 3) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * -2 + unitVectorY * 2, speed * Time.deltaTime);
                        SetAngle(unitVectorX * -2 + unitVectorY * 2);
                        if (transform.position == unitVectorX * -2 + unitVectorY * 2) {
                            movementState = 4;
                            posTemp = transform.position;
                            bezierValue = 0;
                        }
                    }
                    else if (movementState == 4) {
                        bezierValue += curveSpeed * Time.deltaTime;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * -2, posTemp + unitVectorX * -2 + unitVectorY * -2, posTemp + unitVectorX * -2, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorX * -2) {
                            movementState = 5;
                            posTemp = transform.position;
                            bezierValue = 0;
                        }
                    }
                    else if (movementState == 5) {
                        bezierValue += curveSpeed * Time.deltaTime;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * 2, posTemp + unitVectorX * 2 + unitVectorY * 2, posTemp + unitVectorX * 2, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorX * 2) movementState = 6;
                    }
                    else if (movementState == 6) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * -4.5f + unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorX * -4.5f + unitVectorY * -6);
                        if (transform.position == unitVectorX * -4.5f + unitVectorY * -6) movementState = 7;
                    }
                    else {
                        GalagaManager.Inst.enemyLiveCnt--;
                        Destroy(gameObject);
                    }
                }
                else {
                    if (path == 2) unitVectorX = new Vector3(-1f, 0f);
                    if (movementState == 0) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX + unitVectorY * -6, speed * Time.deltaTime);
                        SetAngle(unitVectorX + unitVectorY * -6);
                        if (transform.position == unitVectorX + unitVectorY * -6) movementState = 1;
                    }
                    else if (movementState == 1) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * -3, speed * Time.deltaTime);
                        SetAngle(unitVectorX * -3);
                        if (transform.position == unitVectorX * -3) {
                            movementState = 2;
                            posTemp = transform.position;
                        }
                    }
                    else if (movementState == 2) {
                        bezierValue += curveSpeed * Time.deltaTime;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * 3, posTemp + unitVectorX * 3 + unitVectorY * 3, posTemp + unitVectorX * 3, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorX * 3) {
                            movementState = 3;
                            posTemp = transform.position;
                            bezierValue = 0;
                        }
                    }
                    else if (movementState == 3) {
                        bezierValue += curveSpeed * Time.deltaTime;
                        posBezier = BezierCurve(posTemp, posTemp + unitVectorY * -3, posTemp + unitVectorX * -3 + unitVectorY * -3, posTemp + unitVectorX * -3, bezierValue);
                        SetAngle(posBezier);
                        transform.position = posBezier;
                        if (transform.position == posTemp + unitVectorX * -3) movementState = 4;
                    }
                    else if (movementState == 4) {
                        transform.position = Vector3.MoveTowards(transform.position, unitVectorX * -4 + unitVectorY * 8, speed * Time.deltaTime);
                        SetAngle(unitVectorX * -4 + unitVectorY * 8);
                        if (transform.position == unitVectorX * -4 + unitVectorY * 8) movementState = 5;
                    }
                    else {
                        GalagaManager.Inst.enemyLiveCnt--;
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    protected void ShootBullet() {
        if (GalagaManager.Inst.stage > 4 && Random.Range(0, 100) < GalagaManager.Inst.stage + 10) StartCoroutine(CreateBullet());
    }

    private IEnumerator CreateBullet() {
        for (int i = 0; i < Random.Range(1, 4); i++) {
            Instantiate(bullet, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.15f);
        }
    }

    public int GetMode() {
        switch (mode) {
            case Mode.Flight: return 0;
            case Mode.Standby: return 1;
            case Mode.Attack: return 2;
            case Mode.Challenge: return 3;
        }
        return -1;
    }
    public void SetMode(int n) {
        switch (n) {
            case 0: mode = Mode.Flight; break;
            case 1: mode = Mode.Standby; break;
            case 2: mode = Mode.Attack; break;
            default: mode = Mode.Challenge; break;
        }
    }

    public Quaternion SetAngle2D(Vector2 pos) {
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle - 90f);
    }
}
