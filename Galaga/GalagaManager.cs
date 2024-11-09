using Management;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//galaga manager
public class GalagaManager : SceneManager<GalagaManager> {
    [HideInInspector] public bool isGameOver = false; //게임 종료 여부
    [HideInInspector] public int gameMode = 0; //1: 대기, 2: 인플레이, 3: 게임 오버
    private GameObject prePlayer; //플레이어
    private GameObject[] preEnemy = new GameObject[3]; //적
    private GameObject[] preExEnemy = new GameObject[3]; //추가적
    private GameObject[] preChEnemy = new GameObject[3]; //첼린지 스테이지 전용 적
    private Vector2[] posEnemyCreate = new Vector2[4]; //적 생성 위치
    private Vector2[] posChEnemyCreate = new Vector2[3]; //첼린지 스테이지 전용 적 생성 위치
    private GameObject tAnnounce; //알림
    private Coroutine wave;
    private Text tScore, tBest; //점수, 최고 점수 텍스트
    private int score = 0, life = 2; //점수, 라이프

    public Transform[] formation = new Transform[40]; //적 대기 상태 위치
    public GameObject player; //현재 플레이어
    public bool bAttack = false; //공격 단계
    public bool bKidnaping = false; //납치 여부
    public bool bDualMode = false; //듀얼 모드 여부
    public bool pause = false; //게임 정지 여부
    public bool challengeStage = false; //펠린지 스테이지 여부
    public bool directAttack, doublePath; //적 생성 시 즉시 공격 여부, 두 줄 생성 여부
    public int enemyCreateCnt = 40, enemyLiveCnt = 40, enemyKillCnt = 0; //생성된 적 수, 살아남은 적 수, 죽인 적 수
    public int stage = 1; //스테이지
    public int path; //적 생성 시 공격로

    public int Life {
        get { return life; }
        set { life = value; }
    }
    public int Score {
        get { return score; }
    }

    public GameObject GetPreEnemy(int n) {
        switch (n) {
            case 0: return preEnemy[0];
            case 1: return preEnemy[1];
            case 2: return preEnemy[2];
            case 3: return preExEnemy[0];
            case 4: return preExEnemy[1];
            case 5: return preExEnemy[2];
            case 6: return preChEnemy[0];
            case 7: return preChEnemy[1];
            case 8: return preChEnemy[2];
            default: return null;
        }
    }

    protected override void Awake() {
        base.Awake();
        SetScreenTransitionEffect("Fade", "Canvas");

        prePlayer = Resources.Load("Galaga/Player") as GameObject;
        preEnemy[0] = Resources.Load("Galaga/Jaco") as GameObject;
        preEnemy[1] = Resources.Load("Galaga/GoA") as GameObject;
        preEnemy[2] = Resources.Load("Galaga/BossGalaga") as GameObject;
        preExEnemy[0] = Resources.Load("Galaga/Sasori") as GameObject;
        preExEnemy[1] = Resources.Load("Galaga/Midori") as GameObject;
        preExEnemy[2] = Resources.Load("Galaga/Galacian") as GameObject;
        preChEnemy[0] = Resources.Load("Galaga/Phantom") as GameObject;
        preChEnemy[1] = Resources.Load("Galaga/Momiji") as GameObject;
        preChEnemy[2] = Resources.Load("Galaga/Enterprise") as GameObject;
        posEnemyCreate[0] = GameObject.Find("Path1").transform.GetChild(0).position;
        posEnemyCreate[1] = GameObject.Find("Path2").transform.GetChild(0).position;
        posEnemyCreate[2] = GameObject.Find("Path3").transform.GetChild(0).position;
        posEnemyCreate[3] = GameObject.Find("Path4").transform.GetChild(0).position;
        posChEnemyCreate[0] = new Vector2(0f, 8f);
        posChEnemyCreate[1] = new Vector2(-4.5f, -6f);
        posChEnemyCreate[2] = new Vector2(4.5f, -6f);
        tAnnounce = GameObject.Find("txtAnnounce");
        tScore = GameObject.Find("txtScore").GetComponent<Text>();
        tBest = GameObject.Find("txtBest").GetComponent<Text>();

        tAnnounce.SetActive(false);

        Invoke("SetCountDown", 1);
    }

    private void Start() { tBest.text = $"{PIAManager.Inst.Name}\n{PIAManager.Inst.BestG}"; }

    private void Update() {
        if (life < 0) GameOver();

        if (gameMode == 1) {
            if (enemyCreateCnt == 40 && enemyLiveCnt == 0) {
                ShowAnnounce($"Stage {stage}");
                Invoke("CloseAnnounce", 2);
                wave = StartCoroutine(StartWave());
            }
        }
        else if (gameMode == 2) {
            if (player == null) ReproducePlayer();

            if (enemyCreateCnt == 40 && enemyLiveCnt == 0) {
                gameMode = 0;
                stage++;
                if (!challengeStage) StartCoroutine(ShowDelayAnnounce("Ready..", 2f));
                else {
                    challengeStage = false;
                    if (enemyKillCnt == 40) life++;
                    SetAddScore(enemyKillCnt * 100);
                    StartCoroutine(ShowDelayAnnounce($"Number of hits {enemyKillCnt}\nBonus {enemyKillCnt * 100}", 5f));
                }
            }
        }
    }

    //game start
    public override void GameStart() {
        InitData();
        CreatePlayer();
    }

    //game over
    public override void GameOver() {
        if (gameMode == 3) return;
        gameMode = 3;
        isGameOver = true;
        pause = true;
        StopCoroutine(wave);
        life = 0;
        SetAddScore(0);

        InstantiateUI("boardResult", "Canvas");
    }

    //spawn count down UI object
    private void SetCountDown() { InstantiateUI("txtCount", "Canvas"); }

    //init data
    private void InitData(int _stage = 1) {
        gameMode = 1;
        enemyCreateCnt = 40;
        enemyLiveCnt = 0;
        stage = _stage;
    }

    //spawn player object
    private void CreatePlayer() { player = Instantiate(prePlayer, new Vector2(0, -7), Quaternion.identity); }

    //show UI object
    public void ShowAnnounce(string str) {
        gameMode = 1;
        pause = true;
        tAnnounce.SetActive(true);
        tAnnounce.GetComponent<Text>().text = str;
    }

    //close UI object
    private void CloseAnnounce() {
        gameMode = 2;
        pause = false;
        tAnnounce.SetActive(false);
    }

    //close UI object(delay)
    private IEnumerator ShowDelayAnnounce(string str, float delay) {
        gameMode = 0;
        pause = true;
        tAnnounce.SetActive(true);
        tAnnounce.GetComponent<Text>().text = str;
        yield return new WaitForSeconds(delay);
        gameMode = 1;
    }

    //player reproduction
    public void ReproducePlayer() {
        gameMode = 0;
        ShowAnnounce("Ready..");
        Invoke("CreatePlayer", 2);
        Invoke("CloseAnnounce", 2.5f);
    }

    //add score
    public void SetAddScore(int n) {
        score += n;
        tScore.text = string.Format("Life : {0}\nScore : {1}", life, score);
    }

    //start stage
    private IEnumerator StartWave() {
        bAttack = false;
        enemyCreateCnt = 0;
        enemyLiveCnt = 40;
        enemyKillCnt = 0;
        yield return new WaitForSeconds(3.5f);
        int enemyExCnt = (stage > 6) ? 8 : (stage - 1) * 2;
        if (stage % 8 == 1 || stage % 8 == 6) {
            for (int i = 0; i < 4 + enemyExCnt / 2; i++) {
                CreateEnemy(0, preEnemy[0], posEnemyCreate[0], enemyCreateCnt >= 8);
                if (enemyCreateCnt < 8) enemyCreateCnt++;
                CreateEnemy(1, preEnemy[1], posEnemyCreate[1], enemyCreateCnt >= 8);
                if (enemyCreateCnt < 8) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 8 + enemyExCnt; i++) {
                if (i % 2 == 0) CreateEnemy(2, preEnemy[1], posEnemyCreate[2], enemyCreateCnt >= 16);
                else CreateEnemy(2, preEnemy[2], posEnemyCreate[2], enemyCreateCnt >= 16);
                if (enemyCreateCnt < 16) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 8 + enemyExCnt; i++) {
                CreateEnemy(3, preEnemy[1], posEnemyCreate[3], enemyCreateCnt >= 24);
                if (enemyCreateCnt < 24) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 8 + enemyExCnt; i++) {
                CreateEnemy(0, preEnemy[0], posEnemyCreate[0], enemyCreateCnt >= 32);
                if (enemyCreateCnt < 32) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 8 + enemyExCnt; i++) {
                CreateEnemy(1, preEnemy[0], posEnemyCreate[1], enemyCreateCnt >= 40);
                if (enemyCreateCnt < 40) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            directAttack = false; //jaco가 분열 후 되돌아 왔을 때 enemyLiveCnt가 카운트 되도록 마지막에 false 해줌
            yield return new WaitForSeconds(3f);
            bAttack = true;
        }
        else if (stage % 8 == 2 || stage % 8 == 5) {
            for (int i = 0; i < 4 + enemyExCnt / 2; i++) {
                CreateEnemy(0, preEnemy[0], posEnemyCreate[0], enemyCreateCnt >= 8);
                if (enemyCreateCnt < 8) enemyCreateCnt++;
                CreateEnemy(1, preEnemy[1], posEnemyCreate[1], enemyCreateCnt >= 8);
                if (enemyCreateCnt < 8) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 4 + enemyExCnt / 2; i++) {
                CreateEnemy(2, preEnemy[1], posEnemyCreate[2], enemyCreateCnt >= 16);
                if (enemyCreateCnt < 16) enemyCreateCnt++;
                CreateEnemy(2, preEnemy[2], posEnemyCreate[2] + new Vector2(0, -1), enemyCreateCnt >= 16, true);
                if (enemyCreateCnt < 16) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 4 + enemyExCnt / 2; i++) {
                CreateEnemy(3, preEnemy[1], posEnemyCreate[3], enemyCreateCnt >= 24);
                if (enemyCreateCnt < 24) enemyCreateCnt++;
                CreateEnemy(3, preEnemy[1], posEnemyCreate[3] + new Vector2(0, -1), enemyCreateCnt >= 24, true);
                if (enemyCreateCnt < 24) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 4 + enemyExCnt / 2; i++) {
                CreateEnemy(0, preEnemy[0], posEnemyCreate[0], enemyCreateCnt >= 32);
                if (enemyCreateCnt < 32) enemyCreateCnt++;
                CreateEnemy(0, preEnemy[0], posEnemyCreate[0] + new Vector2(1, 0), enemyCreateCnt >= 32, true);
                if (enemyCreateCnt < 32) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 4 + enemyExCnt / 2; i++) {
                CreateEnemy(1, preEnemy[0], posEnemyCreate[1], enemyCreateCnt >= 40);
                if (enemyCreateCnt < 40) enemyCreateCnt++;
                CreateEnemy(1, preEnemy[0], posEnemyCreate[1] + new Vector2(-1, 0), enemyCreateCnt >= 40, true);
                if (enemyCreateCnt < 40) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            directAttack = false;
            yield return new WaitForSeconds(3f);
            bAttack = true;
        }
        else if (stage % 4 == 0) {
            for (int i = 0; i < 4 + enemyExCnt / 2; i++) {
                CreateEnemy(0, preEnemy[0], posEnemyCreate[0], enemyCreateCnt >= 8);
                if (enemyCreateCnt < 8) enemyCreateCnt++;
                CreateEnemy(1, preEnemy[1], posEnemyCreate[1], enemyCreateCnt >= 8);
                if (enemyCreateCnt < 8) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 4 + enemyExCnt / 2; i++) {
                CreateEnemy(2, preEnemy[1], posEnemyCreate[2], enemyCreateCnt >= 16);
                if (enemyCreateCnt < 16) enemyCreateCnt++;
                CreateEnemy(3, preEnemy[2], posEnemyCreate[3], enemyCreateCnt >= 16);
                if (enemyCreateCnt < 16) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 4 + enemyExCnt / 2; i++) {
                CreateEnemy(2, preEnemy[1], posEnemyCreate[2], enemyCreateCnt >= 24);
                if (enemyCreateCnt < 24) enemyCreateCnt++;
                CreateEnemy(3, preEnemy[1], posEnemyCreate[3], enemyCreateCnt >= 24);
                if (enemyCreateCnt < 24) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 4 + enemyExCnt / 2; i++) {
                CreateEnemy(0, preEnemy[0], posEnemyCreate[0], enemyCreateCnt >= 32);
                if (enemyCreateCnt < 32) enemyCreateCnt++;
                CreateEnemy(1, preEnemy[0], posEnemyCreate[1], enemyCreateCnt >= 32);
                if (enemyCreateCnt < 32) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 4 + enemyExCnt / 2; i++) {
                CreateEnemy(0, preEnemy[0], posEnemyCreate[0], enemyCreateCnt >= 40);
                if (enemyCreateCnt < 40) enemyCreateCnt++;
                CreateEnemy(1, preEnemy[0], posEnemyCreate[1], enemyCreateCnt >= 40);
                if (enemyCreateCnt < 40) enemyCreateCnt++;
                yield return new WaitForSeconds(0.2f);
            }
            directAttack = false;
            yield return new WaitForSeconds(3f);
            bAttack = true;
        }
        else {
            challengeStage = true;
            if (stage == 3) {
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(0, preEnemy[0], posEnemyCreate[0]);
                    CreateEnemy(1, preEnemy[0], posEnemyCreate[1]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4.5f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(2, preEnemy[0], posChEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                    CreateEnemy(2, preEnemy[2], posChEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4.5f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(3, preEnemy[0], posChEnemyCreate[2]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4.5f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(0, preEnemy[0], posEnemyCreate[0]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4.5f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(0, preEnemy[0], posEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
            }
            else if (stage == 7) {
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(0, preEnemy[1], posEnemyCreate[0]);
                    CreateEnemy(1, preEnemy[1], posEnemyCreate[1]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4.5f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(2, preEnemy[2], posChEnemyCreate[1]);
                    CreateEnemy(3, preEnemy[1], posChEnemyCreate[2]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(5f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(2, preEnemy[1], posChEnemyCreate[1]);
                    CreateEnemy(3, preEnemy[1], posChEnemyCreate[2]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(5f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(0, preEnemy[1], posEnemyCreate[0]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4.5f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(1, preEnemy[1], posEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
            }
            else if (stage == 11) {
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(0, preChEnemy[0], posChEnemyCreate[0]);
                    CreateEnemy(1, preChEnemy[0], posChEnemyCreate[0]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4.5f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(2, preEnemy[2], posChEnemyCreate[1]);
                    CreateEnemy(3, preChEnemy[0], posChEnemyCreate[2]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4.5f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(2, preChEnemy[0], posChEnemyCreate[0]);
                    CreateEnemy(3, preChEnemy[0], posChEnemyCreate[0]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(5f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(0, preChEnemy[0], posChEnemyCreate[0]);
                    CreateEnemy(1, preChEnemy[0], posChEnemyCreate[0]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(5f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(0, preChEnemy[0], posChEnemyCreate[0]);
                    CreateEnemy(1, preChEnemy[0], posChEnemyCreate[0]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
            }
            else if (stage == 15) {
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(0, preExEnemy[0], posChEnemyCreate[0]);
                    CreateEnemy(1, preExEnemy[0], posChEnemyCreate[0]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(5f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(2, preEnemy[2], posChEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                    CreateEnemy(2, preExEnemy[0], posChEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(6f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(3, preExEnemy[0], posChEnemyCreate[2]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                    CreateEnemy(3, preExEnemy[0], posChEnemyCreate[2]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(6f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(0, preExEnemy[0], posChEnemyCreate[0]);
                    CreateEnemy(1, preExEnemy[0], posChEnemyCreate[0]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(5f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(0, preExEnemy[0], posChEnemyCreate[0]);
                    CreateEnemy(1, preExEnemy[0], posChEnemyCreate[0]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
            }
            else if (stage == 19) {
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(0, preChEnemy[1], posEnemyCreate[0]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4.5f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(2, preEnemy[2], posChEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                    CreateEnemy(2, preChEnemy[1], posChEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(6f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(3, preChEnemy[1], posChEnemyCreate[2]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                    CreateEnemy(3, preChEnemy[1], posChEnemyCreate[2]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(6f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(0, preChEnemy[1], posEnemyCreate[0]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4.5f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(1, preChEnemy[1], posEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
            }
            else if (stage == 23) {
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(0, preExEnemy[1], posChEnemyCreate[0] + new Vector2(-4f, 0));
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(2, preEnemy[2], posChEnemyCreate[1]);
                    CreateEnemy(3, preExEnemy[1], posChEnemyCreate[2]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(2, preExEnemy[1], posChEnemyCreate[1]);
                    CreateEnemy(3, preExEnemy[1], posChEnemyCreate[2]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(1, preExEnemy[1], posChEnemyCreate[0] + new Vector2(4f, 0));
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(0, preExEnemy[1], posChEnemyCreate[0] + new Vector2(-4f, 0));
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
            }
            else if (stage == 27) {
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(0, preExEnemy[2], posEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(2, preEnemy[2], posChEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                    CreateEnemy(2, preExEnemy[2], posChEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(5f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(3, preExEnemy[2], posChEnemyCreate[2]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                    CreateEnemy(3, preExEnemy[2], posChEnemyCreate[2]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(5f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(1, preExEnemy[2], posEnemyCreate[0]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(0, preExEnemy[2], posEnemyCreate[1]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
            }
            else if (stage == 31) {
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(0, preChEnemy[2], posEnemyCreate[0]);
                    CreateEnemy(1, preChEnemy[2], posEnemyCreate[1]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(2, preEnemy[2], posChEnemyCreate[1]);
                    yield return new WaitForSeconds(0.1f);
                    CreateEnemy(2, preChEnemy[2], posChEnemyCreate[1]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4f);
                for (int i = 0; i < 8; i++) {
                    CreateEnemy(3, preChEnemy[2], posChEnemyCreate[2]);
                    enemyCreateCnt++;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(0, preChEnemy[2], posEnemyCreate[0]);
                    CreateEnemy(1, preChEnemy[2], posEnemyCreate[1]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(4f);
                for (int i = 0; i < 4; i++) {
                    CreateEnemy(0, preChEnemy[2], posEnemyCreate[0]);
                    CreateEnemy(1, preChEnemy[2], posEnemyCreate[1]);
                    enemyCreateCnt += 2;
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3f);
            }
        }
    }

    //spanw enemy object
    private void CreateEnemy(int _path, GameObject obj, Vector2 pos, bool _directAttack = false, bool _doublePath = false) {
        path = _path;
        directAttack = _directAttack;
        doublePath = _doublePath;
        Instantiate(obj, pos, Quaternion.identity);
    }

    public override void LoadScene(int sceneIdx) {
        base.LoadScene(sceneIdx);

        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().sceneIdx = sceneIdx;
        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().EndEffectFirst();
    }
}
