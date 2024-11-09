using HorzTools;
using UnityEngine;

//belt scroll object
[RequireComponent(typeof(Rigidbody2D))]
public class HorzScroll : HScroll {
    protected override void Awake() { base.Awake(); }

    private void Update() {
        if (FlappyManager.Inst.isGameOver) SetStop();
    }

    private void GameStart() { SetStart(2); }
}
