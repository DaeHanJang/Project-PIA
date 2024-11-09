using ObjectTools;
using UnityEngine;

public class Bullet : Object2D {
    protected Rigidbody2D mRB;
    protected bool hit; //�浹 ����

    protected virtual void Awake() { mRB = GetComponent<Rigidbody2D>(); }

    protected virtual void Start() { hit = false; }
}