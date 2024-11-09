using Management;
using Logic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//omok manager
public class OmokManager : SceneManager<OmokManager> {
    public LogicOmok lo; //omok logic class

    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public int gameMode = 0; //1: in game, 2: game over
    private GameObject[,] pos; //¹ÙµÏÆÇ ÁÂÇ¥ object
    private GameObject txtGameOver;
    private GameObject cvStartBoard;
    private Text tRate;
    private int boardLength = 19; //board length
    
    public bool clickAI = false;
    public int win = 0, lose = 0;
    public int player, ai; //player, ai turn index

    protected override void Awake() {
        base.Awake();
        SetScreenTransitionEffect("Fade", "Canvas");

        lo = new LogicOmok(boardLength, boardLength);

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
        win = PIAManager.Inst.BestOm[0];
        lose = PIAManager.Inst.BestOm[1];
        tRate.text = string.Format($"{PIAManager.Inst.Name}\nWin/Lose : {win}/{lose}");
    }

    //game start
    public override void GameStart() {
        gameMode = 1;
        cvStartBoard = InstantiateUI("cvsStartBoard", "Canvas");
    }

    //game over
    public override void GameOver() {
        if (gameMode == 2) return;
        gameMode = 2;
        isGameOver = true;
        if (lo.Turn != player) {
            txtGameOver.GetComponent<Text>().text = "Player Win!!";
            ++win;
        }
        else {
            txtGameOver.GetComponent<Text>().text = "Player Lose..";
            ++lose;
        }
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
        if (ai == 1) Invoke("OmokAI", 0.5f);
        Destroy(cvStartBoard);
    }

    //set stone
    public void SetStone(int n) {
        if (lo.setData(n % 19, n / 19)) GameOver();
        else {
            if (lo.Turn == ai) Invoke("OmokAI", 0.5f);
        }
    }

    //set result board
    private void SetResultBoard() {
        GameObject br = InstantiateUI("boardResult", "Canvas");
        br.GetComponent<Transform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    //omok ai logic
    private void OmokAI() {
        clickAI = true;
        List<KeyValuePair<int, int>>[] playermok = new List<KeyValuePair<int, int>>[4];
        List<KeyValuePair<int, int>>[] aimok = new List<KeyValuePair<int, int>>[4];
        for (int i = 0; i < 4; i++) {
            playermok[i] = new List<KeyValuePair<int, int>>();
            aimok[i] = new List<KeyValuePair<int, int>>();
        }
        for (int r = 0; r < 19; r++) {
            for (int c = 0; c < 19; c++) {
                if (lo.getValue(r, c) == player) {
                    lo.subAnalyze(ref playermok, player, r, c);
                }
                else if (lo.getValue(r, c) == ai) {
                    lo.subAnalyze(ref aimok, ai, r, c);
                }
                else continue;
            }
        }
        for (int i = 0; i < 4; i++) {
            playermok[i] = playermok[i].Distinct().ToList();
            aimok[i] = aimok[i].Distinct().ToList();
        }
        if (playermok[3].Count > 0) {
            int rnd = Random.Range(0, playermok[3].Count);
            pos[playermok[3][rnd].Key, playermok[3][rnd].Value].GetComponent<OmokPosition>().OnMouseDown();
        }
        else if (aimok[3].Count > 0) {
            int rnd = Random.Range(0, aimok[3].Count);
            pos[aimok[3][rnd].Key, aimok[3][rnd].Value].GetComponent<OmokPosition>().OnMouseDown();
        }
        else if (playermok[2].Count > 0) {
            int rnd = Random.Range(0, playermok[2].Count);
            pos[playermok[2][rnd].Key, playermok[2][rnd].Value].GetComponent<OmokPosition>().OnMouseDown();
        }
        else if (aimok[2].Count > 0) {
            int rnd = Random.Range(0, aimok[2].Count);
            pos[aimok[2][rnd].Key, aimok[2][rnd].Value].GetComponent<OmokPosition>().OnMouseDown();
        }
        else if (aimok[1].Count > 0) {
            int rnd = Random.Range(0, aimok[1].Count);
            pos[aimok[1][rnd].Key, aimok[1][rnd].Value].GetComponent<OmokPosition>().OnMouseDown();
        }
        else if (aimok[0].Count > 0) {
            int rnd = Random.Range(0, aimok[0].Count);
            pos[aimok[0][rnd].Key, aimok[0][rnd].Value].GetComponent<OmokPosition>().OnMouseDown();
        }
        else if (playermok[1].Count > 0) {
            int rnd = Random.Range(0, playermok[1].Count);
            pos[playermok[1][rnd].Key, playermok[1][rnd].Value].GetComponent<OmokPosition>().OnMouseDown();
        }
        else if (playermok[0].Count > 0) {
            int rnd = Random.Range(0, playermok[0].Count);
            pos[playermok[0][rnd].Key, playermok[0][rnd].Value].GetComponent<OmokPosition>().OnMouseDown();
        }
        else {
            int rndr = Random.Range(8, 11), rndc = Random.Range(8, 11);
            while (lo.getValue(rndr, rndc) != 0) {
                rndr = Random.Range(8, 11);
                rndc = Random.Range(8, 11);
            }
            pos[rndr, rndc].GetComponent<OmokPosition>().OnMouseDown();
        }
        clickAI = false;
    }

    public override void LoadScene(int sceneIdx) {
        base.LoadScene(sceneIdx);

        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().sceneIdx = sceneIdx;
        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().EndEffectFirst();
    }
}
