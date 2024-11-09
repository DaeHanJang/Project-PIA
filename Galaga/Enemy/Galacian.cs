using UnityEngine;

public class Galacian : ExEnemy {
    protected override void Start() { base.Start(); }

    protected override void Update() {
        base.Update();

        if (mode == Mode.Attack) {
            if (attackState == 0) {
                attackState = 1;
                if (GalagaManager.Inst.player) {
                    posAttack = GalagaManager.Inst.player.transform.position - transform.position;
                    if (idx == 0) RB.velocity = posAttack.normalized * speed * 2f;
                    else if (idx == 2) RB.velocity = posAttack.normalized * speed * 1.5f;
                    else RB.velocity = posAttack.normalized * speed;
                    transform.rotation = SetAngle2D(posAttack);
                }
                else RB.velocity = transform.position.normalized * speed;
            }
        }
    }

    protected override void DestroyObject() {
        StartCoroutine(SoundManager.Inst.PlayEnemyDestory(5));
        base.DestroyObject();
    }
}
