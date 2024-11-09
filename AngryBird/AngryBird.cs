using UnityEngine;

//angry bird object
public class AngryBird : MonoBehaviour {
    private Animator _anim;
    private AudioSource _as;
    public bool init = true;

    private void Awake() {
        _anim = GetComponent<Animator>();
        _as = GetComponent<AudioSource>();
        _anim.enabled = false;
    }

    public void PlayClip() { _as.Play(); }

    public void SetDestroy() {
        AngryManager.Inst.SetAddScore();
        Destroy(gameObject);
    }
}
