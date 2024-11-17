using System;
using UnityEngine;

public class FollowCam : MonoBehaviour {
    private GameObject player1, player2;
    private Camera cam;
    private Vector3 posPreMiddle, posCurMiddle;
    private Vector3 vecPre, vecCur;

    private void Awake() {
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");
        cam = GetComponent<Camera>();
    }

    private void Start() {
        posPreMiddle = transform.position;
        vecPre = player1.transform.position - player2.transform.position;
    }

    private void LateUpdate() {
        posCurMiddle = MiddleCoordinates(player1.transform.position.x, 1, player1.transform.position.z,
            player2.transform.position.x, 1, player2.transform.position.z);
        vecCur = player1.transform.position - player2.transform.position;
        float angle = Vector3.SignedAngle(Vector3.up, vecCur, vecPre);
        transform.position = posCurMiddle;
        transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));

        posPreMiddle = posCurMiddle;
        vecPre = vecCur;
    }

    private void MiddlePoint2D(double x1, double y1, double x2, double y2, out double x, out double y) {
        x = (x1 + x2) / 2;
        y = (y1 + y2) / 2;
    }

    private Vector3 MiddleCoordinates(Vector3 a, Vector3 b) {
        Vector3 mid = (a + b) / 2;
        return mid;
    }

    private Vector3 MiddleCoordinates(float x1, float y1, float z1, float x2, float y2, float z2) {
        Vector3 mid = new Vector3((x1 + x2) / 2, (y1 + y2) / 2, (z1 + z2) / 2);
        return mid;
    }

    private double Distance2D(double x1, double y1, double x2, double y2) {
        double width = x2 - x1;
        double height = y2 - y1;
        double distance = Math.Sqrt(width * width + height * height);
        return distance;
    }

    private double Slope2D(double x1, double y1, double x2, double y2) {
        double rise = y2 - y1;
        double run = x2 - x1;
        double slope = rise / run;
        return slope;
    }

    private double LinearFucntion(double a, double b, double x) {
        double y = a * x + b;
        return y;
    }
}
