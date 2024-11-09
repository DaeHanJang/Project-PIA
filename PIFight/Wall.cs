using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//wall object
public class Wall : MonoBehaviourPun {
    private void Update() { gameObject.layer = 0; }

    [PunRPC]
    public void SetHide(bool b) { gameObject.layer = 7; }
}
