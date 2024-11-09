using Management;
using UnityEngine;
using UnityEngine.UI;

//Menu manager
public class MenuManager : SceneManager<MenuManager> {
    private Text tName = null;
    private AudioSource _as = null;

    protected override void Awake() {
        base.Awake();
        SetScreenTransitionEffect("Fade", "Canvas");

        _as = GetComponent<AudioSource>();
        tName = GameObject.Find("txtName").GetComponent<Text>();
    }

    private void Start() { tName.text = PIAManager.Inst.Name; }

    //game scene ¿Ãµø
    public void ClickBtn(int idx) {
        LoadScene(idx);
        _as.Play();
    }

    public override void LoadScene(int sceneIdx) {
        base.LoadScene(sceneIdx);

        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().sceneIdx = sceneIdx;
        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().EndEffectFirst();
    }

    public override void GameStart() { }

    public override void GameOver() { }
}
