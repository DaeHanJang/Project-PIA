using System;
using UnityEngine;

//camera transform
public class CameraTransform : MonoBehaviour {
    private GameObject player1, player2;
    private GameObject cam;

    private Vector3 position; //player1, player2 middle position
    private Vector3 rotTargetAngle; //회전시킬 각도
    private float distance; //FOV value

    private bool bSlowMotion = false;
    private bool bSlowMotionBack = false;
    private float spdSlowMotion = 2f;
    private float valSlowMotion = 0;

    private void Awake() { cam = transform.GetChild(0).gameObject; }

    private void Start() {
        player1 = PIFightManager.Inst.player1;
        player2 = PIFightManager.Inst.player2;
    }

    private void FixedUpdate() {
        Vector3 posPlayer1 = player1.transform.position; posPlayer1.y = 1; //player1 position
        Vector3 posPlayer2 = player2.transform.position; posPlayer2.y = 1; //player2 position
        position = (posPlayer1 + posPlayer2) / 2; //player1, player2 middle position

        Vector3 vecMiddleToPlayer = posPlayer1 - position; //middle position -> player1 position vector
        Vector3 vecMiddleToCam = cam.transform.position - position; //middle position -> camera position vector

        //vecPlayerToMiddle를 기준으로 vecMiddleToCam의 각도를 계산
        //Vector3.up은 왼손법칙의 엄지손가락에 해당함
        float angle = Vector3.SignedAngle(vecMiddleToCam, vecMiddleToPlayer, Vector3.up);

        if (0 <= angle && angle <= 90) {
            angle -= 90;
        }
        else if (90 < angle && angle <= 180) {
            angle -= 90;
        }
        else if (-90 <= angle && angle < 0) {
            angle -= 90;
        }
        else if (-180 <= angle && angle < -90) {
            angle += 270;
        }

        Vector3 rotCurrentAngle = transform.rotation.eulerAngles; //current angle
        rotTargetAngle = rotCurrentAngle + new Vector3(0, angle, 0); //new angle

        //player1, player2의 거리
        distance = Distance2D(player1.transform.position.x, player1.transform.position.z,
            player2.transform.position.x, player2.transform.position.z);
        distance *= 5;
        if (distance < 30) distance = 30; //min distance
        if (distance > 60) distance = 60; //max distance
    }

    private void LateUpdate() {
        if (!PIFightManager.Inst.isGameOver) {
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotTargetAngle);
            cam.GetComponent<Camera>().fieldOfView = distance;
        }
        else if (!bSlowMotion) SlowMotion();
    }

    //slow motion effect
    private void SlowMotion() {
        if (!bSlowMotionBack) {
            valSlowMotion += Time.deltaTime * spdSlowMotion;
            cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(distance, 15, valSlowMotion);
            if (valSlowMotion >= 1) bSlowMotionBack = true;
        }
        else {
            valSlowMotion -= Time.deltaTime * spdSlowMotion;
            cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(distance, 15, valSlowMotion);
            if (valSlowMotion <= 0) {
                bSlowMotion = true;
                Time.timeScale = 1f;
            }
        }
    }

    //2차원 좌표에서 두 점 사이의 거리
    private float Distance2D(float x1, float y1, float x2, float y2) {
        float width = x2 - x1;
        float height = y2 - y1;
        float distance = (float)Math.Sqrt(width * width + height * height);
        return distance;
    }
}
