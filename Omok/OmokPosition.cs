using UnityEngine;

//¹ÙµÏÆÇ ÁÂÇ¥ object
public class OmokPosition : MonoBehaviour {
    private BoxCollider2D bc;
    private SpriteRenderer sr;
    private AudioSource ads;
    public Sprite[] sprites = new Sprite[3];

    private void Start() {
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        ads = GetComponent<AudioSource>();
        ColliderEnabled(false);
    }

    public void OnMouseDown() {
        if (OmokManager.Inst.ai == OmokManager.Inst.lo.Turn) {
            if (!OmokManager.Inst.clickAI) return;
        }
        sr.sprite = sprites[OmokManager.Inst.lo.Turn];
        OmokManager.Inst.SetStone(transform.GetSiblingIndex());
        ads.Play();
        ColliderEnabled(false);
    }

    public void ColliderEnabled(bool b) { bc.enabled = b; }

    public void SetSprite(int n) { sr.sprite = sprites[n]; }
}
