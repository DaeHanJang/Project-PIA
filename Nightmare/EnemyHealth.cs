using UnityEngine;
using UnityEngine.AI;

//enemy health point
public class EnemyHealth : MonoBehaviour {
    private CapsuleCollider capsuleCollider;
    private Animator anim;
    private AudioSource enemyAudio;
    private ParticleSystem hitParticles;
    private bool isSinking; //��ü ó�� ����
    private int currentHealth;
    private float sinkSpeed = 2.5f; //��ü ó�� �ӵ�

    public AudioClip deathClip;
    public bool isDead;
    public int startingHealth;
    public int score;

    private void Awake() {
        capsuleCollider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        hitParticles = GetComponentInChildren<ParticleSystem>();
        currentHealth = startingHealth;
    }

    private void Update() {
        if (isSinking) transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
    }

    //�ǰ�
    public void TakeDamage(int amount, Vector3 hitPoint) {
        if (isDead) return;

        enemyAudio.Play();
        currentHealth -= amount;
        hitParticles.transform.position = hitPoint;
        hitParticles.Play();

        if (currentHealth <= 0) Death();
    }

    //die
    void Death() {
        isDead = true;
        capsuleCollider.isTrigger = true;
        anim.SetTrigger("Dead");
        enemyAudio.clip = deathClip;
        enemyAudio.Play();
        NightmareManager.Inst.SetAddScore(score);
    }

    //��ü ó��
    public void StartSinking() {
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        isSinking = true;
        Destroy(gameObject, 2f);
    }
}
