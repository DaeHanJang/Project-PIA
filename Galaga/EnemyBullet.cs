using ObjectTools;
using UnityEngine;

//enemy bullet object
public class EnemyBullet : Bullet {
    private GameObject player;
    private Vector2 attackPos;

    protected override void Awake() {
        base.Awake();
        player = GalagaManager.Inst.player;
    }

    protected override void Start() {
        base.Start();
        HP = 1;
        UnitIdx = 2;
        speed = 7f;
        if (player != null) {
            attackPos = player.transform.position - transform.position;
            mRB.velocity = attackPos / attackPos.magnitude * speed;
        }
        else DestroyObject();
    }

    private void Update() {
        if (HP <= 0) DestroyObject();
        if (transform.position.y <= -8f) DestroyObject();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Object2D>() != null) {
            if (collision.gameObject.GetComponent<Object2D>().UnitIdx != UnitIdx && collision.gameObject.tag != "Bullet" && !hit) {
                hit = true;
                HP--;
            }
        }
    }
}
