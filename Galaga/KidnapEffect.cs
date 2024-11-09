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
            owner.GetComponent<BossGalaga>().Kidnap = true; //��ġ ������ �߻��� BossGalaga�� ��ġ �Ķ���� true
            owner.GetComponent<BossGalaga>().KidnapPlayer = collision.gameObject; //BossGalaga�� ��ġ�� �÷��̾� ������Ʈ ����
            collision.gameObject.GetComponent<Gyaraga>().OwnerEnemy = owner; //��ġ�� �÷��̾ ��ġ�� BossGalaga ������Ʈ ����
            collision.gameObject.GetComponent<Gyaraga>().SetEnemyMode(); //�÷��̾� Enemy Mode ����
            GalagaManager.Inst.Life--; //��ġ�� �͵� ���� ������ ����
            GalagaManager.Inst.ReproducePlayer(); //�÷��̾� �����
            StartCoroutine(SoundManager.Inst.PlayPlayerKidnap()); //sound manager���� ��ġ�� ���
        }
    }

    public void EndEffect() { Destroy(gameObject); }
}