using Management;
using UnityEngine;
using UnityEngine.UI;

//angry bird manager
public class AngryManager : SceneManager<AngryManager> {
    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public int gameMode = 0; //0: initialization, 1: in game, 2: game over
    private GameObject _pStone, _pLineBack, _pLineFore;
    private GameObject _plank, _bird;
    private Transform posPlanks; //planks base transform
    private Text tScore, tBest, tLife;
    private float _wid = 1.6f; //판자 길이
    private int score = 0, life = 3;
    private int birdCnt = 3; //생성될 angry bird 수

    public GameObject stone;
    
    public int Life {
        get { return life; }
    }
    public int Score {
        get { return score; }
    }

    protected override void Awake() {
        base.Awake();
        SetScreenTransitionEffect("Fade", "Canvas");

        _pStone = Resources.Load("AngryBird/Stone") as GameObject;
        _pLineBack = GameObject.Find("LineBack");
        _pLineFore = GameObject.Find("LineFore");
        _plank = Resources.Load("AngryBird/Plank") as GameObject;
        _bird = Resources.Load("AngryBird/Bird") as GameObject;
        posPlanks = GameObject.Find("Planks").transform;
        tScore = GameObject.Find("txtScore").GetComponent<Text>();
        tBest = GameObject.Find("txtBest").GetComponent<Text>();
        tLife = GameObject.Find("txtLife").GetComponent<Text>();
        stone = GameObject.Find("Stone");

        Invoke("setCountDown", 1);
    }

    private void Start() {
        MakePlank();
        tBest.text = $"{PIAManager.Inst.Name}\n{PIAManager.Inst.BestA}";
    }

    //game start
    public override void GameStart() { GameObject.Find("Main Camera").SendMessage("GameReady"); }

    //game over
    public override void GameOver() {
        if (gameMode == 2) return;
        gameMode = 2;
        isGameOver = true;

        InstantiateUI("boardResult", "Canvas");
    }

    //spawn count down UI object
    private void setCountDown() { InstantiateUI("txtCount", "Canvas"); }

    //판자 랜덤 생성
    private void MakePlank() {
        int maxcol = 8;
        for (int r = 0; r <= 2; r++) {
            maxcol = Random.Range(1, 1 + maxcol);
            CreateRows(r, maxcol);
        }
    }

    //판자가 생성될 길이 랜덤 설정
    private void CreateRows(int row, int col) {
        float s = _wid * (-col / 2) - (_wid / 2) * (col % 2);
        for (int i = 0; i < col + 1; i++) CreatePlank(s, row, i, true);
        for (int i = 0; i < col; i++) CreatePlank(s + _wid / 2, row, i, false);
        GameObject o = Instantiate(_bird, transform.position, Quaternion.identity);
        o.transform.SetParent(posPlanks);
        if (row == 0) o.GetComponent<AngryBird>().init = false;
        float x = s + _wid / 2 + Random.Range(0, col) * _wid;
        float y = -0.5f + 2f * row;
        o.transform.localPosition = new Vector2(x, y);
    }

    //지정된 길이의 만큼의 판자 생성
    private void CreatePlank(float s, int r, int c, bool v) {
        GameObject o = Instantiate(_plank, transform.position, Quaternion.identity);
        o.transform.SetParent(posPlanks);
        if (v) {
            o.transform.localRotation = Quaternion.Euler(0, 0, 90);
            o.transform.localPosition = new Vector2(s + c * _wid, r * 2);
        }
        else o.transform.localPosition = new Vector2(s + c * _wid, r * 2 + 1);
    }

    //add score
    public void SetAddScore() {
        score += 10;
        --birdCnt;
        tScore.text = string.Format("Score : {0}", score);
    }

    //decrease life
    public void SetLifeDown() { tLife.text = string.Format("Life : {0}", --life); }

    //다음 턴을 위한 데이터 세팅
    public void SetNextTrun() {
        gameMode = 0;
        if (birdCnt != 0) {
            if (life <= 0) {
                GameOver();
                return;
            }
            SetLifeDown();
        }
        birdCnt = 3;
        foreach (Transform child in posPlanks) Destroy(child.gameObject);
        _pLineBack.gameObject.SetActive(true);
        _pLineFore.gameObject.SetActive(true);
        stone = Instantiate(_pStone, new Vector3(-4.23f, -1.19f, 0f), Quaternion.identity);
        MakePlank();
        GameObject.Find("Main Camera").SendMessage("GameReady");
    }

    public override void LoadScene(int sceneIdx) {
        base.LoadScene(sceneIdx);

        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().sceneIdx = sceneIdx;
        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().EndEffectFirst();
    }
}
