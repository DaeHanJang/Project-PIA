using UnityEngine;

public class Jaco : Enemy {
    private int mStage = -1;

    protected override void Start() {
        base.Start();
        HP = 1;
        if (4 <= GalagaManager.Inst.stage % 16 && GalagaManager.Inst.stage % 16 <= 6) mStage = 0;
        else if (8 <= GalagaManager.Inst.stage % 16 && GalagaManager.Inst.stage % 16 <= 10) mStage = 1;
        else if (12 <= GalagaManager.Inst.stage % 16 && GalagaManager.Inst.stage % 16 <= 14) mStage = 2;
        else mStage = -1;
    }

    protected override void Update() {
        if (mode == Mode.Standby) score = 50;
        else score = 100;

        if (GalagaManager.Inst.pause && mode != Mode.Flight) mode = Mode.Standby;

        base.Update();

        if (mode == Mode.Attack) {
            if (posTemp == Vector3.zero) {
                posTemp = transform.position;
                if (transform.position.x <= 0) unitVectorX = new Vector3(-1f, 0f);
            }
            if (attackState == 0) {
                bezierValue += curveSpeed * Time.deltaTime;
                posBezier = BezierCurve(posTemp, posTemp + unitVectorY, posTemp + unitVectorX + unitVectorY, posTemp + unitVectorX, bezierValue);
                SetAngle(posBezier);
                transform.position = posBezier;
                if (transform.position == posTemp + unitVectorX) {
                    attackState = 1;
                    if (mStage != -1 && (Random.Range(0, 10) > 8)) Seperlate(mStage);
                    else ShootBullet();
                }
            }
            else if (attackState == 1) {
                attackState = 2;
                if (GalagaManager.Inst.player) {
                    posAttack = GalagaManager.Inst.player.transform.position - transform.position;
                    RB.velocity = posAttack / posAttack.magnitude * speed;
                    SetAngle(posAttack);
                }
                else RB.velocity = transform.position / transform.position.magnitude * speed;
            }
            else if (attackState == 2) {
                if (transform.position.y <= -6.5f) {
                    attackState = 3;
                    RB.velocity = Vector3.zero;
                    posTemp = transform.position;
                    bezierValue = 0;
                    if (GalagaManager.Inst.player) {
                        if (transform.position.x <= GalagaManager.Inst.player.transform.position.x) unitVectorX = new Vector3(1f, 0f);
                        else unitVectorX = new Vector3(-1f, 0f);
                    }
                }
            }
            else if (attackState == 3) {
                bezierValue += curveSpeed * Time.deltaTime;
                posBezier = BezierCurve(posTemp, posTemp + unitVectorY * -2, posTemp + unitVectorX * 2 + unitVectorY * -2, posTemp + unitVectorX * 2, bezierValue);
                SetAngle(posBezier);
                transform.position = posBezier;
                if (transform.position == posTemp + unitVectorX * 2) {
                    attackState = 4;
                    bezierValue = 0;
                }
            }
            else if (attackState == 4) {
                bezierValue += curveSpeed * Time.deltaTime;
                posBezier = BezierCurve(posTemp + unitVectorX * 2, posTemp + unitVectorX * 2 + unitVectorY * 2, posTemp + unitVectorY * 2, posTemp, bezierValue);
                SetAngle(posBezier);
                transform.position = posBezier;
                if (transform.position == posTemp) attackState = 5;
            }
            else if (attackState == 5) {
                attackState = 6;
                RB.velocity = unitVectorY * -1 * speed;
                SetAngle(unitVectorY * -1);
            }
        }
    }

    //jaco 분열
    private void Seperlate(int n) {
        GameObject[] seperlatedObject = new GameObject[3]; //분열체 3기
        GameObject preSeperlateObject = GalagaManager.Inst.GetPreEnemy(n + 3); //3: sasori, 4: midori, 5: galacian
        for (int i = 0; i < 3; i++) {
            seperlatedObject[i] = Instantiate(preSeperlateObject, transform.position, Quaternion.identity); //spawn extra enemy
            seperlatedObject[i].GetComponent<Enemy>().SetMode(2); //set attack mode
            seperlatedObject[i].GetComponent<ExEnemy>().Idx = i; //set extra enemy index
        }
        seperlatedObject[1].GetComponent<Enemy>().PosFormation = PosFormation;
        seperlatedObject[1].GetComponent<ExEnemy>().Center = true;
        Destroy(gameObject);
    }

    protected override void DestroyObject() {
        StartCoroutine(SoundManager.Inst.PlayEnemyDestory(0));
        base.DestroyObject();
    }
}
