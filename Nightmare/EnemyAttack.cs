using UnityEngine;

//enemy attack
public class EnemyAttack : MonoBehaviour {
    private const float timeBetweenAttacks = 0.5f; //공격 대기 시간

    private GameObject player;
    private PlayerHealth playerHP;
    private Animator anim;
    private bool playerInRange = false; //공격 범위에 들어왔는지 여부
    private float timer; //공격 대기 시간 타이머

    public int attackDamage = 2;

    private void Awake() {
        player = GameObject.Find("Player");
        playerHP = player.GetComponent<PlayerHealth>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        timer += Time.deltaTime;
        if (timer >= timeBetweenAttacks && playerInRange) Attack();

        if (playerHP.isDead) anim.SetTrigger("PlayerDead");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == player) playerInRange = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == player) playerInRange = false;
    }

    //attack
    void Attack() {
        timer = 0f;
        if (!playerHP.isDead) playerHP.TakeDamage(attackDamage);
    }
}
