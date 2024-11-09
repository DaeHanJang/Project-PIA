using UnityEngine;

public class Sasori : ExEnemy {
    protected override void Update() {
        base.Update();

        if (mode == Mode.Attack && timer > idx * 0.1f) {
            if (attackState == 0) {
                attackState = 1;
                if (GalagaManager.Inst.player) {
                    posAttack = GalagaManager.Inst.player.transform.position - transform.position;
                    RB.velocity = posAttack.normalized * speed;
                    transform.rotation = SetAngle2D(posAttack);
                }
                else RB.velocity = transform.position.normalized * speed;
            }
        }
    }

    protected override void DestroyObject() {
        StartCoroutine(SoundManager.Inst.PlayEnemyDestory(3));
        base.DestroyObject();
    }
}
