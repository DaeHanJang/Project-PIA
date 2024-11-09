using UnityEngine;

//bird object
[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour {
    public AudioClip[] ac = new AudioClip[2]; //0:jump, 1:hit
    public float upForce = 200f; //¶Ù¾î ¿À¸£´Â Èû

    private Rigidbody2D rb;
    private PolygonCollider2D pc;
    private SpriteRenderer sr;
    private Animator anim;
    private AudioSource _as;

    private bool isBlink = false, isShow = true;
    private int blinkCount = 5; //±ôºýÀÓ È½¼ö
    private float fBlink = 3f; //±ôºýÀÏ ½Ã°£

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        pc = GetComponent<PolygonCollider2D>();
        anim = GetComponent<Animator>();
        _as = GetComponent<AudioSource>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        pc.isTrigger = true;
    }

    private void Update() {
        if (FlappyManager.Inst.gameMode != 1) return;

        if (Input.GetMouseButtonDown(0)) {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(0f, upForce));
            anim.SetTrigger("SetFlap");
        }

        BirdBlink();
    }

    private void OnTriggerEnter2D(Collider2D pcision) {
        if (pcision.gameObject.name.Contains("Ground")) {
            PlaySound(1);
            anim.SetTrigger("SetDie");
            pc.isTrigger = false;
            FlappyManager.Inst.GameOver();
            return;
        }
        if (isBlink) return;
        if (pcision.gameObject.tag != "Column") return;

        PlaySound(1);
        isBlink = true;
        fBlink = 3f;
        FlappyManager.Inst.SetLifeDown();
    }

    private void OnCollisionEnter2D(Collision2D pcision) {
        anim.SetTrigger("SetDie");
        FlappyManager.Inst.GameOver();
    }

    private void GameStart() { rb.bodyType = RigidbodyType2D.Dynamic; }

    private void BirdBlink() {
        if (!isBlink) return;

        if (--blinkCount <= 0) {
            blinkCount = 5;
            if (isShow = !isShow) sr.color = Color.white;
            else sr.color = Color.clear;
        }

        fBlink -= Time.deltaTime;
        if (fBlink < 0f) {
            isBlink = false;
            sr.color = Color.white;
            if (FlappyManager.Inst.Life == 0) pc.isTrigger = false;
        }
    }

    public void PlaySound(int n) {
        if (n == 0 && _as.isPlaying) return;
        _as.clip = ac[n];
        _as.Play();
    }
}
