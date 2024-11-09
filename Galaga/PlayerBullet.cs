using ObjectTools;
using UnityEngine;

//player bullet object
public class PlayerBullet : Bullet {
    private Gyaraga player;

    public Gyaraga Player {
        set { player = value; }
    }

    protected override void Start() {
        base.Start();
        HP = 1;
        UnitIdx = 1;
        speed = 15f;
        mRB.velocity = new Vector2(0f, speed);
    }

    private void Update() {
        if (HP <= 0) DestroyObject();
        if (transform.position.y >= 8f) DestroyObject();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Object2D>() != null) {
            if (collision.gameObject.GetComponent<Object2D>().UnitIdx != UnitIdx && collision.gameObject.tag != "Bullet" && !hit) {
                hit = true;
                HP--;
            }
        }
    }

    protected override void DestroyObject() {
        player.bulletCnt--;
        base.DestroyObject();
    }
}
