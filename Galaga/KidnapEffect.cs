using UnityEngine;

public class KidnapEffect : MonoBehaviour {
    private GameObject owner;

    public GameObject Owner {
        get { return owner; }
        set { owner = value; }
    }

    private void Update() {
        if (GalagaManager.Inst.pause) EndEffect();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Gyaraga>() != null) {
            owner.GetComponent<BossGalaga>().Kidnap = true; //납치 레이저 발사한 BossGalaga의 납치 파라미터 true
            owner.GetComponent<BossGalaga>().KidnapPlayer = collision.gameObject; //BossGalaga에 납치된 플레이어 오브젝트 적재
            collision.gameObject.GetComponent<Gyaraga>().OwnerEnemy = owner; //납치된 플레이어에 납치한 BossGalaga 오브젝트 참조
            collision.gameObject.GetComponent<Gyaraga>().SetEnemyMode(); //플레이어 Enemy Mode 설정
            GalagaManager.Inst.Life--; //납치된 것도 죽은 것으로 판정
            GalagaManager.Inst.ReproducePlayer(); //플레이어 재생산
            StartCoroutine(SoundManager.Inst.PlayPlayerKidnap()); //sound manager에서 납치음 재생
        }
    }

    public void EndEffect() { Destroy(gameObject); }
}