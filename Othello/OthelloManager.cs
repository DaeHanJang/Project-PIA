using Management;
using Logic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//othello manager
public class OthelloManager : SceneManager<OthelloManager> {
    public LogicOthello lo; //othello logic class

    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public int gameMode = 0; //1: in game, 2: game over
    private GameObject[,] pos; //othello board ÁÂÇ¥ object
    private GameObject txtGameOver;
    private GameObject startCanvas;
    private Text tRate;
    private int boardLength = 8; //baord length

    public List<KeyValuePair<int, int>> aistoke = new List<KeyValuePair<int, int>>();
    public bool clickAI = false;
    public int win = 0, lose = 0;
    public int player, ai; //player, ai turn index
    public int playerscr = 0, aiscr = 0; //player, ai score

    protected override void Awake() {
        base.Awake();
        SetScreenTransitionEffect("Fade", "Canvas");

        lo = new LogicOthello(boardLength, boardLength);

        pos = new GameObject[boardLength, boardLength];
        GameObject ppos = GameObject.Find("Position");
        for (int i = 0; i < boardLength * boardLength; i++) {
            pos[i % boardLength, i / boardLength] = ppos.transform.GetChild(i).gameObject;
        }
        txtGameOver = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        tRate = GameObject.Find("txtRate").GetComponent<Text>();
        txtGameOver.SetActive(false);

        Invoke("SetCountDown", 1);
    }

    private void Start() {
        win = PIAManager.Inst.BestOd[0];
        lose = PIAManager.Inst.BestOd[1];
        tRate.text = string.Format($"{PIAManager.Inst.Name}\nWin/Lose : {win}/{lose}");
    }

    //game start
    public override void GameStart() {
        gameMode = 1;
        startCanvas = InstantiateUI("cvsStartBoard", "Canvas");
    }

    //game over
    public override void GameOver() {
        if (gameMode == 2) return;
        gameMode = 2;
        isGameOver = true;
        int[,] a = lo.getData();
        for (int i = 0; i < a.GetLength(0); i++) {
            for (int j = 0; j < a.GetLength(1); j++) {
                if (lo.getValue(i, j) == player) playerscr++;
                else if (lo.getValue(i, j) == ai) aiscr++;
            }
        }
        if (playerscr > aiscr) {
            txtGameOver.GetComponent<Text>().text = "Player Win!!";
            ++win;
        }
        else if (playerscr < aiscr) {
            txtGameOver.GetComponent<Text>().text = "Player Lose..";
            ++lose;
        }
        else txtGameOver.GetComponent<Text>().text = "Drow";
        txtGameOver.SetActive(true);
        GameObject.Find("Position").BroadcastMessage("ColliderEnabled", false);
        Invoke("SetResultBoard", 1);
    }

    //spawn count down UI object
    private void SetCountDown() { InstantiateUI("txtCount", "Canvas"); }

    //set turn
    public void SetTurn(int n) {
        player = n;
        ai = 3 - n;
        GameObject.Find("Position").BroadcastMessage("ColliderEnabled", true);
        PrintBoard();
        if (ai == 1) Invoke("OthelloAI", 0.5f);
        Destroy(startCanvas);
    }

    //set stone
    public void SetStone(int n) {
        lo.setData(n % boardLength, n / boardLength);
        PrintBoard();
        if (ai == lo.Turn && gameMode != 2) Invoke("OthelloAI", 0.5f);
    }

    //rendering board
    public void PrintBoard() {
        lo.deleteCanStone();

        if (!lo.canStoneAnalyze(ref aistoke)) GameOver();
        int[,] dat = lo.getData();
        for (int i = 0; i < dat.GetLength(0); i++) {
            for (int j = 0; j < dat.GetLength(1); j++) {
                pos[i, j].GetComponent<OthelloPosition>().SetSprite(dat[i, j]);
                if (dat[i, j] == 1 || dat[i, j] == 2) pos[i, j].GetComponent<OthelloPosition>().ColliderEnabled(false);
            }
        }

        lo.deleteCanStone();
    }

    //set result board
    private void SetResultBoard() {
        GameObject br = InstantiateUI("boardResult", "Canvas");
        br.GetComponent<Transform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    //othello ai
    private void OthelloAI() {
        clickAI = true;
        int rnd = Random.Range(0, aistoke.Count);
        pos[aistoke[rnd].Key, aistoke[rnd].Value].GetComponent<OthelloPosition>().OnMouseDown();
        clickAI = false;
    }

    public override void LoadScene(int sceneIdx) {
        base.LoadScene(sceneIdx);

        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().sceneIdx = sceneIdx;
        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().EndEffectFirst();
    }
}
