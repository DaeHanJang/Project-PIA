using UnityEngine;

public class Momiji : Enemy {
    protected override void Start() {
        base.Start();
        HP = 1;
        score = 160;
    }

    protected override void Update() { base.Update(); }

    protected override void DestroyObject() {
        StartCoroutine(SoundManager.Inst.PlayEnemyDestory(7));
        base.DestroyObject();
    }
}
