using Management;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//nightmare manager
public class NightmareManager : SceneManager<NightmareManager> {
    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public int gameMode = 0; //1: in game, 2: game over
    private GameObject[] enemy = new GameObject[3];
    private GameObject[] subWall = new GameObject[2];
    private GameObject player;
    private Text tScore, tBest, tTimer;
    private bool timeBetweenCam = true; //카메라 시점 변환 가능 여부
    private float timer; //game limit timer
    private int score = 0;

    public GameObject[] cam = new GameObject[2]; //3인칭, 1인칭 카메라
    public Transform[] enemyCreatePosition = new Transform[4]; //적 위치 생성 장소

    public int Score {
        get { return score; }
    }

    protected override void Awake() {
        base.Awake();

        SetScreenTransitionEffect("Fade", "HUDCanvas");

        tScore = GameObject.Find("txtScore").GetComponent<Text>();
        tBest = GameObject.Find("txtBest").GetComponent<Text>();
        tTimer = GameObject.Find("txtTimer").GetComponent<Text>();
        enemy[0] = Resources.Load("Nightmare/Zombunny") as GameObject;
        enemy[1] = Resources.Load("Nightmare/Zombear") as GameObject;
        enemy[2] = Resources.Load("Nightmare/Hellephant") as GameObject;
        player = GameObject.Find("Player");
        subWall[0] = GameObject.Find("Wall2");
        subWall[1] = GameObject.Find("Sides4");
        cam[0] = GameObject.Find("Main Camera");
        cam[1] = GameObject.Find("PlayerCamera");
        player.GetComponent<PlayerMovement>().enabled = false;
        player.transform.GetChild(1).gameObject.GetComponent<PlayerShooting>().enabled = false;
        subWall[0].SetActive(false);
        subWall[1].SetActive(false);
        cam[1].SetActive(false);

        Invoke("setCountDown", 1);
    }

    private void Start() { tBest.text = $"{PIAManager.Inst.Name}\n{PIAManager.Inst.BestN}"; }

    private void Update() {
        if (gameMode == 1) {
            timer += Time.deltaTime;
            tTimer.text = string.Format("{0:0.0}", timer);
        }

        if (Input.GetButton("Jump") && timeBetweenCam) {
            timeBetweenCam = false;
            if (!cam[0].activeSelf) {
                Cursor.lockState = CursorLockMode.None;
                cam[0].SetActive(true);
                cam[0].GetComponent<CameraFollow>().CameraInit();
                cam[1].SetActive(false);
                subWall[0].SetActive(false);
                subWall[1].SetActive(false);
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
                cam[1].SetActive(true);
                cam[0].SetActive(false);
                subWall[0].SetActive(true);
                subWall[1].SetActive(true);
            }
            Invoke("DelayCamMode", 1f);
        }
    }

    //game start
    public override void GameStart() {
        gameMode = 1;
        player.GetComponent<PlayerMovement>().enabled = true;
        player.transform.GetChild(1).gameObject.GetComponent<PlayerShooting>().enabled = true;
        StartCoroutine(CreateEnemy());
    }

    //game over
    public override void GameOver() {
        if (gameMode == 2) return;
        gameMode = 2;
        isGameOver = true;
        Cursor.lockState = CursorLockMode.None;
        StopCoroutine(CreateEnemy());
        InstantiateUI("boardResult", "HUDCanvas");
    }

    //spawn count down UI object
    private void setCountDown() { InstantiateUI("txtCount", "HUDCanvas"); }

    //카메라 시점 변환 가능
    private void DelayCamMode() { timeBetweenCam = true; }

    //add score
    public void SetAddScore(int n) {
        score += n;
        tScore.text = string.Format("Score : {0}", score);
    }

    //enemy create
    IEnumerator CreateEnemy() {
        int rand;
        while (true) {
            if (timer >= 50) rand = Random.Range(0, 3);
            else if (timer >= 30) rand = Random.Range(0, 2);
            else rand = 0;
            Instantiate(enemy[rand], enemyCreatePosition[Random.Range(0, 4)].position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
    }

    public override void LoadScene(int sceneIdx) {
        base.LoadScene(sceneIdx);

        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().sceneIdx = sceneIdx;
        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().EndEffectFirst();
    }
}
