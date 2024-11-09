using Management;

//Logo scene manager
public class LogoManager : SceneManager<LogoManager> {
    protected override void Awake() {
        base.Awake();
        SetScreenTransitionEffect("Fade", "Canvas");
        Invoke("SetActiveFade", 1.5f);
    }

    private void SetActiveFade() {
        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().sceneIdx = 1;
        screenTransitionEffect.GetComponent<ScreenTransitionEffect>().EndEffectFirst();
    }

    public override void GameStart() { }

    public override void GameOver() { }
}
