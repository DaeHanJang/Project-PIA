using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public abstract class NetworkScreenTransitionEffect : MonoBehaviourPunCallbacks {
    public int sceneIdx = -1;

    public abstract void StartEffectFirst();

    public abstract void StartEffectLast();

    public abstract void EndEffectFirst();

    public abstract void EndEffectLast();
}
