using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Management;

//pifight field manager
public class PIFightManager : NetworkSceneManager<PIFightManager> {
    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public int gameMode = 0; //0:Pause, 1:InGmae, 2:GameOver
    private int[] hp = { 300, 300 }; //Plyaer HP
    private float timer = 60; //Timer
    private int idxWin; //win index

    public Image[] imgHP = new Image[2];
    public GameObject player1, player2; //Player Character
    public Text txtTimer, txtState;
    public int idxClient; //1:MasterClient, 2:Client
    public int winJudgment; //0:1P win, 1:2P win, 2:Time out
    public int win, lose; //Client Win/Lose

    protected override void Awake() {
        base.Awake();

        if (PhotonNetwork.IsMasterClient) {
            idxClient = 1;
            player1.GetComponent<PhotonView>().RequestOwnership();
            player1.GetComponent<Player>().enemy = player2;
        }
        else {
            idxClient = 2;
            player2.GetComponent<PhotonView>().RequestOwnership();
            player2.GetComponent<Player>().enemy = player1;
        }
        win = PIAManager.Inst.BestPF[0];
        lose = PIAManager.Inst.BestPF[1];

        Invoke("SetCountDown", 1);
    }

    private void Start() {
        win = PIAManager.Inst.BestPF[0];
        lose = PIAManager.Inst.BestPF[1];
        txtState.enabled = false;
    }

    private void Update() {
        if (gameMode == 1) {
            if (timer <= 0f) {
                timer = 0f;
                winJudgment = 2;
                GameOver();
            }
            else {
                timer -= Time.deltaTime;
                if (timer <= 10f) txtTimer.color = Color.red;
                txtTimer.text = string.Format("{0:F0}", timer);
            }

            if (hp[0] <= 0) {
                hp[0] = 0;
                player1.GetComponent<Player>().photonView.RPC("SetDie", RpcTarget.All);
                winJudgment = 1;
                GameOver();
            }
            else if (hp[1] <= 0) {
                hp[1] = 0;
                player2.GetComponent<Player>().photonView.RPC("SetDie", RpcTarget.All);
                winJudgment = 0;
                GameOver();
            }
        }
    }

    //game start
    public override void GameStart() { gameMode = 1; }

    //game over
    public override void GameOver() {
        if (gameMode == 2) return;
        gameMode = 2;
        isGameOver = true;
        Time.timeScale = 0.25f;

        if (winJudgment == 0) {
            txtState.text = "1P Win!!";
            idxWin = 1;
        }
        else if (winJudgment == 1) {
            txtState.text = "2P Win!!";
            idxWin = 2;
        }
        else if (winJudgment == 2) {
            if (hp[0] > hp[1]) {
                txtState.text = "1P Win!!";
                idxWin = 1;
            }
            else if (hp[0] < hp[1]) {
                txtState.text = "2P Win!!";
                idxWin = 2;
            }
        }
        txtState.enabled = true;

        if (idxWin == 1) {
            if (idxClient == 1) ++win;
            else ++lose;
        }
        else {
            if (idxClient == 1) ++lose;
            else ++win;
        }

        Invoke("EnabledBoard", 2f);
    }

    //spawn count down UI object
    private void SetCountDown() { InstantiateUI("txtCount", "Canvas"); }

    //spawn result board UI object
    private void EnabledBoard() {
        InstantiateUI("boardResult", "Canvas");
    }

    //set health point
    public void SetHP(int idx) {
        int HPBox = 300 - hp[idx];
        if (HPBox > 300) HPBox = 300;
        imgHP[idx].rectTransform.offsetMin = new Vector2(HPBox, 0);
    }

    //result board button
    public void BtnBack() { LoadScene(9); }


    //decrease health point
    [PunRPC]
    public void DecreaseHP(int idx, int damage) {
        hp[idx] -= damage;
        SetHP(idx);
    }

    public override void LoadScene(int sceneIdx) {
        base.LoadScene(sceneIdx);

        screenTransitionEffect.GetComponent<NetworkScreenTransitionEffect>().sceneIdx = 2;
        screenTransitionEffect.GetComponent<NetworkScreenTransitionEffect>().EndEffectFirst();
    }
}
