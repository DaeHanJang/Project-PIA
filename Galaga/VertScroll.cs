using HorzTools;
using UnityEngine;
using UnityEngine.SceneManagement;

//vertical scrool object
[RequireComponent(typeof(Rigidbody2D))]
public class VertScroll : VScroll {
    private int buildIndex;

    protected override void Awake() {
        base.Awake();
        buildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Start() {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        SetStart(2);
    }

    private void Update() { if (GalagaManager.Inst.isGameOver) setStop(); }
}
