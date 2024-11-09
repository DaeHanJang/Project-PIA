using HorzTools;
using UnityEngine;

//vertical scroll repeat
[RequireComponent(typeof(BoxCollider2D))]
public class VRepeatBackground : VRepeat {
    private void Start() { SetBoxCollider(); }

    private void Update() { UpdateObject(); }
}
