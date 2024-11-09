using Management;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : ScreenTransitionEffect {
    public override void StartEffectFirst() { }

    public override void StartEffectLast() {
        gameObject.SetActive(false);
    }

    public override void EndEffectFirst() {
        gameObject.SetActive(true);
        GetComponent<Animator>().SetTrigger("FadeOut");
    }

    public override void EndEffectLast() {
        if (sceneIdx == -1) return;

        SceneManager.LoadScene(sceneIdx);
    }
}
