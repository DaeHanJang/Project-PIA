using UnityEngine;

public class GoA : Enemy {

    protected override void Start() {
        base.Start();
        HP = 1;
    }

    protected override void Update() {
        if (mode == Mode.Attack) score = 160;
        else score = 80;

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
                    posTemp = transform.position;
                    bezierValue = 0;
                }
            }
            else if (attackState == 1) {
                bezierValue += curveSpeed * Time.deltaTime / 3;
                posBezier = BezierCurve(posTemp, posTemp + unitVectorX * 4, posTemp + unitVectorX * 4 + unitVectorY * -3,  posTemp + unitVectorY * -3, bezierValue);
                SetAngle(posBezier);
                transform.position = posBezier;
                if (transform.position == posTemp + unitVectorY * -3) {
                    attackState = 2;
                    posTemp = transform.position;
                    bezierValue = 0;
                    ShootBullet();
                }
            }
            else if (attackState == 2) {
                bezierValue += curveSpeed * Time.deltaTime / 3;
                posBezier = BezierCurve(posTemp, posTemp + unitVectorX * -4, posTemp + unitVectorX * -4 + unitVectorY * -3, posTemp + unitVectorY * -3, bezierValue);
                SetAngle(posBezier);
                transform.position = posBezier;
                if (transform.position == posTemp + unitVectorY * -3) {
                    attackState = 3;
                    posTemp = transform.position;
                    bezierValue = 0;
                }
            }
            else if (attackState == 3) {
                bezierValue += curveSpeed * Time.deltaTime / 2;
                posBezier = BezierCurve(posTemp, posTemp + unitVectorX * 2, posTemp + unitVectorX * 2 + unitVectorY * -3, posTemp + unitVectorY * -3, bezierValue);
                SetAngle(posBezier);
                transform.position = posBezier;
                if (transform.position == posTemp + unitVectorY * -3) {
                    attackState = 4;
                    posTemp = transform.position;
                    bezierValue = 0;
                }
            }
            else if (attackState == 4) {
                bezierValue += curveSpeed * Time.deltaTime / 2;
                posBezier = BezierCurve(posTemp, posTemp + unitVectorX * -2, posTemp + unitVectorX * -2 + unitVectorY * -3, posTemp + unitVectorY * -3, bezierValue);
                SetAngle(posBezier);
                transform.position = posBezier;
                if (transform.position == posTemp + unitVectorY * -3) attackState = 5;
            }
            else if (attackState == 5) {
                attackState = 6;
                RB.velocity = unitVectorY * -1 * speed;
                SetAngle(unitVectorY * -1);
            }
        }
    }

    protected override void DestroyObject() {
        StartCoroutine(SoundManager.Inst.PlayEnemyDestory(1));
        base.DestroyObject();
    }
}
