using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//player object
public class Player : MonoBehaviourPun {
    private Rigidbody rb; //rigidbody
    private Animator anim; //animator
    private AudioSource ads; //audio
    private bool bKnockBack = false; //knockback state
    private float speed = 4f; //speed
    private float timerKnockBack = 0.5f; //knockback timer

    public GameObject enemy; //enemy player
    public AudioClip[] ac = new AudioClip[3]; //0:step, 1:guard, 2:punch
    public bool bGuard = false; //guard state
    public bool bAttack = false; //attack state
    public bool bDie = false; //die

    public Animator Anim {
        get { return anim; }
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        ads = GetComponent<AudioSource>();
    }

    private void Start() {
        BroadcastMessage("SetPlayer", this);
    }

    private void FixedUpdate() {
        if (!photonView.IsMine) return;

        if (PIFightManager.Inst.gameMode == 1) {
            if (bKnockBack) {
                timerKnockBack -= Time.deltaTime;
                if (timerKnockBack <= 0) {
                    bKnockBack = false;
                    timerKnockBack = 0.5f;
                    rb.velocity = Vector3.zero;
                }
            }
            Move();
            InputSystem();
        }
    }

    private void Move() {
        bool bWalk = false;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (x != 0 || z != 0) bWalk = true;

        if (PIFightManager.Inst.idxClient == 1) {
            if (x < 0) bGuard = true;
            z *= -1;
        }
        else if (PIFightManager.Inst.idxClient == 2) {
            if (x > 0) bGuard = true;
            x *= -1;
        }

        Vector3 velocity = transform.TransformDirection(new Vector3(z, 0, x));
        velocity *= speed;
        rb.velocity = velocity;

        Vector3 directionEnemy = enemy.transform.position - transform.position;
        Quaternion rotateEnemy = Quaternion.LookRotation(directionEnemy.normalized);
        transform.rotation = rotateEnemy;

        anim.SetBool("Walk", bWalk);
        if (bWalk) photonView.RPC("PlaySound", RpcTarget.All, 0);
        bGuard = false;
    }

    private void InputSystem() {
        if (Input.GetKey(KeyCode.Z)) {
            bAttack = true;
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Jab")) anim.SetBool("Jab", true);
            else anim.SetBool("Combo", true);
        }
        if (Input.GetKey(KeyCode.X)) {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hook")) {
                bAttack = true;
                anim.SetBool("Hook", true);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jab")) anim.SetBool("Combo", true);
        }
    }

    public void EndAttack() {
        if (anim.GetBool("Combo")) {
            anim.SetBool("Combo", false);
            return;
        }
        anim.SetBool("Jab", false);
        anim.SetBool("Hook", false);
        bAttack = false;
    }

    public void Guard(Transform posHit) {
        PhotonNetwork.Instantiate("PIFight/Particles/Guard", posHit.position, Quaternion.Euler(0, 180, 0));
        enemy.GetComponent<Player>().photonView.RPC("KnockBack", RpcTarget.All, false);
        photonView.RPC("PlaySound", RpcTarget.All, 1);
    }

    public void Hit(Transform posHit) {
        PIFightManager.Inst.photonView.RPC("DecreaseHP", RpcTarget.All, 2 - PIFightManager.Inst.idxClient, 50);
        PhotonNetwork.Instantiate("PIFight/Particles/Hit", posHit.position, Quaternion.Euler(0, 180, 0));
        enemy.GetComponent<Player>().photonView.RPC("KnockBack", RpcTarget.All, true);
        photonView.RPC("PlaySound", RpcTarget.All, 2);
    }

    [PunRPC]
    public void KnockBack(bool bHit) {
        CancelAttack();
        bKnockBack = true;
        if (bHit) {
            anim.SetTrigger("Hit");
            rb.velocity = transform.TransformDirection(new Vector3(0, 0, -0.5f));
        }
        else rb.velocity = transform.TransformDirection(Vector3.back);
    }

    [PunRPC]
    public void EndHit() {
        anim.SetBool("Hit", false);
    }

    [PunRPC]
    public void SetDie() {
        bDie = true;
        anim.SetTrigger("Die");
    }

    [PunRPC]
    public void PlaySound(int idx) {
        if (ads.isPlaying) return;
        ads.PlayOneShot(ac[idx]);
    }

    private void CancelAttack() {
        bAttack = false;
        anim.SetBool("Jab", false);
        anim.SetBool("Hook", false);
        anim.SetBool("Combo", false);
    }
}
