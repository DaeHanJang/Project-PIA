using UnityEngine;

//camera object
public class MainCamera : MonoBehaviour {
    private Animator anim;
    private Transform posPlayer;
    private float pos;

    private void Awake() { anim = GetComponent<Animator>(); }

    private void LateUpdate() {
        if (AngryManager.Inst.gameMode != 1) return;

        pos = posPlayer.position.x;
        if (pos < 0f) pos = 0f;
        if (pos > 20.48f) pos = 20.48f;
        if (transform.position.x <= pos) transform.position = new Vector3(pos, 0f, -10f);
    }

    //show plank
    private void GameReady() {
        posPlayer = AngryManager.Inst.stone.transform;
        transform.position = new Vector3(0f, 0f, -10f);
        anim.SetTrigger("ShowPlank");
    }

    public void GameStart() { AngryManager.Inst.gameMode = 1; }
}
