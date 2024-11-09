using UnityEngine;

//othello board ÁÂÇ¥ object
public class OthelloPosition : MonoBehaviour {
    private BoxCollider2D bc;
    private SpriteRenderer sr;
    private AudioSource ads;
    public Sprite[] sprites = new Sprite[4];

    private void Start() {
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        ads = GetComponent<AudioSource>();
        ColliderEnabled(false);
    }

    public void OnMouseDown() {
        if (OthelloManager.Inst.ai == OthelloManager.Inst.lo.Turn) {
            if (!OthelloManager.Inst.clickAI) return;
        }
        OthelloManager.Inst.SetStone(transform.GetSiblingIndex());
        ads.Play();
    }

    public void ColliderEnabled(bool b) { bc.enabled = b; }

    public void SetSprite(int n) { sr.sprite = sprites[n]; }
}
