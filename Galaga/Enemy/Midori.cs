using UnityEngine;

public class Midori : ExEnemy {
    protected override void Start() {
        base.Start();

        if (mode == Mode.Attack) {
            if (transform.position.x < 0) transform.rotation = Quaternion.Euler(0, 0, 135f);
            else transform.rotation = Quaternion.Euler(0, 0, -135f);
        }
    }

    protected override void Update() {
        base.Update();

        if (mode == Mode.Attack && timer > idx * 0.1f) {
            if (attackState == 0) {
                RB.velocity = transform.up * speed;
                if (timer > 0.3f) attackState = 1;
            }
            else if (attackState == 1) {
                attackState = 2;
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
        StartCoroutine(SoundManager.Inst.PlayEnemyDestory(4));
        base.DestroyObject();
    }
}
