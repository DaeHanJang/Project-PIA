using UnityEngine;

//Àå¾Ö¹° object
[RequireComponent(typeof(BoxCollider2D))]
public class Columns : MonoBehaviour {
    private BoxCollider2D box;

    private void Awake() {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Bird>() != null) FlappyManager.Inst.SetAddScore();
    }
}
