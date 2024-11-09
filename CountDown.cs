using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//count down UI object
public class CountDown : MonoBehaviour {
    private Text textCount;
    private int buildIndex; //current build index
    private int count;

    private void Awake() { buildIndex = SceneManager.GetActiveScene().buildIndex; }

    private void Start() {
        count = 3;
        textCount = GetComponent<Text>();
        if (buildIndex == 5 || buildIndex == 8) textCount.color = Color.white;
    }

    //count down
    public void ChangeCount() {
        count--;
        textCount.text = (count >= 0) ? "" + count : "GO";

        if (count < -1) {
            if (buildIndex == 3) GameObject.Find("FlappyManager").SendMessage("GameStart");
            else if (buildIndex == 4) GameObject.Find("AngryManager").SendMessage("GameStart");
            else if (buildIndex == 5) GameObject.Find("GalagaManager").SendMessage("GameStart");
            else if (buildIndex == 6) GameObject.Find("OmokManager").SendMessage("GameStart");
            else if (buildIndex == 7) GameObject.Find("OthelloManager").SendMessage("GameStart");
            else if (buildIndex == 8) GameObject.Find("NightmareManager").SendMessage("GameStart");
            else if (buildIndex == 10) GameObject.Find("PIFightManager").SendMessage("GameStart");
            Destroy(gameObject);
        }
    }
}
