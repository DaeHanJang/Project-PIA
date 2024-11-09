using UnityEngine;
using UnityEngine.AI;

//enemy movement
public class EnemyMovement : MonoBehaviour {
    private NavMeshAgent _nav;
    private Transform _player;
    private PlayerHealth _playerHealth;
    private EnemyHealth _enemyHealth;

    private void Awake() {
        _nav = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        _enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update() {
        if (!_playerHealth.isDead && !_enemyHealth.isDead) _nav.SetDestination(_player.position); //player에게 자동 전진
        else _nav.enabled = false;
    }
}
