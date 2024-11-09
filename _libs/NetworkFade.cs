using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NetworkFade : NetworkScreenTransitionEffect {
    private int sceneIdx = -1;

    public override void StartEffectFirst() { }

    public override void StartEffectLast() {
        gameObject.SetActive(false);
    }

    public override void EndEffectFirst() {
        GetComponent<Animator>().SetTrigger("FadeOut");
    }

    public override void EndEffectLast() {
        if (sceneIdx == -1) return;

        PhotonNetwork.LoadLevel(sceneIdx);
    }
}
