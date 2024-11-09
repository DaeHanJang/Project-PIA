using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HitSystem : MonoBehaviourPun {
    private Rigidbody rb;

    public Player player;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Player>() == null) return;
        if (other.GetComponent<Player>() != player) {
            if (player.bDie) return;
            if (player.bAttack) {
                if (other.GetComponent<Player>().bGuard) player.Guard(transform);
                else player.Hit(transform);
            }
        }
    }

    public void SetPlayer(Player _player) {
        player = _player;
    }
}
