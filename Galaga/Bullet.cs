using ObjectTools;
using UnityEngine;

public class Bullet : Object2D {
    protected Rigidbody2D mRB;
    protected bool hit; //충돌 여부

    protected virtual void Awake() { mRB = GetComponent<Rigidbody2D>(); }

    protected virtual void Start() { hit = false; }
}