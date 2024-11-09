using UnityEngine;

//plank object
public class Plank : MonoBehaviour {
    private AudioSource _as;
    private bool init = true;

    private void Awake() { _as = GetComponent<AudioSource>(); }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<AngryBird>() != null) {
            if (init) init = false;
            else collision.gameObject.GetComponent<Animator>().enabled = true;
        }
        else if (collision.gameObject.GetComponent<Plank>() != null) {
            if (!init) _as.Play();
        }
    }
}
