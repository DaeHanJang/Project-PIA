using HorzTools;
using UnityEngine;

//belt scroll repeat
[RequireComponent(typeof(BoxCollider2D))]
public class RepeatBackground : HRepeat {
    private void Awake() { SetBoxCollider(); }

    private void Update() { UpdateObject(); }
}
