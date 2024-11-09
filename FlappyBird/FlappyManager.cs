using Management;
using UnityEngine;
using UnityEngine.UI;

//flappy bird manager
public class FlappyManager : SceneManager<FlappyManager> {
    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public int gameMode = 0; //1: in game, 2: game over
    private Text tScore, tBest, tLife;
    private int score = 0, life = 3;

    public int Life {
        get { return life; }
    }
    public int Score {
        get { return score; }
    }

    protected override void Awake() {
        base.Awake();
        SetScreenTransitionEffect("Fade", "Canvas");

        tScore = GameObject.Find("txtScore").GetComponent<Text>();
        tBest = GameObject.Find("txtBest").GetComponent<Text>();
        tLife = GameObject.Find("txtLife").GetComponent<Text>();

        Invoke("SetCountDown", 1);
    }

    private void Start() { tBest.text = $"{PIAManager.Inst.Name}\n{PIAManager.Inst.BestF}"; }

    //game start
    public override void GameStart() {
        gameMode = 1;
        GameObject.Find("FlappyManager").GetComponent<ObstaclePool>().InitColumnCreate();
        GameObject.Find("Bird").SendMessage("GameStart");
        GameObject.Find("Ground").SendMessage("GameStart");
        GameObject.Find("Ground2").SendMessage("GameStart");
        GameObject[] objs = GameObject.FindGameObjectsWithTag("HorzScroll");
        foreach (var o in objs) o.SendMessage("GameStart");
    }

    //game over
    public override void GameOver() {
        if (gameMode == 2) return;
        gameMode = 2;
        isGameOver = true;

        GameObject obj = InstantiateUI("boardResult", "Canvas");
    }
    
    //spawn count down UI object
    private void SetCountDown() { InstantiateUI("txtCount", "Canvas"); }

    //add score
    public void SetAddScore() {
        score += 10;
        tScore.text = string.Format("Score : {0}", score);
    }

    //decrease life
    public void SetLifeDown() { tLife.text = string.Format("Life : {0}", --life); }

    public override void LoadScene(int sceneIdx) {
        base.LoadScene(sceneIdx);

        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().sceneIdx = sceneIdx;
        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().EndEffectFirst();
    }
}
