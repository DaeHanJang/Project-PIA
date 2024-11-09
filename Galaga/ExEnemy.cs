using UnityEngine;

public class ExEnemy : Enemy {
    protected GameObject preJaco;
    protected bool center = false;
    protected float timer = 0f;
    protected int idx;

    public bool Center {
        set { center = value; }
    }

    public int Idx {
        set { idx = value; }
    }

    protected override void Awake() {
        base.Awake();
        preJaco = Resources.Load("Galaga/Jaco") as GameObject;
    }

    protected override void Start() {
        base.Start();
        HP = 1;
        score = 160;
    }

    protected virtual new void Update() {
        timer += Time.deltaTime;

        if (HP <= 0) {
            GalagaManager.Inst.SetAddScore(score);
            if (center) GalagaManager.Inst.enemyLiveCnt--;
            DestroyObject();
        }

        if (mode == Mode.Challenge) SetPath();
        else {
            if (transform.position.y <= -11) {
                if (center) {
                    GameObject obj = Instantiate(preJaco, transform.position, Quaternion.identity);
                    obj.transform.position = new Vector3(transform.position.x, 9f);
                    obj.GetComponent<Enemy>().PosFormation = PosFormation;
                    obj.GetComponent<Enemy>().SetMode(1);
                }
                Destroy(gameObject);
            }
        }
    }
}