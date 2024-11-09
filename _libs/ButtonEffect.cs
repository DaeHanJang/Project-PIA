using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    protected RectTransform rt;

    public float defualtScale = 1f;
    public float expendScale = 1.1f;

    protected virtual void Awake() { rt = GetComponent<RectTransform>(); }

    public virtual void OnPointerEnter(PointerEventData eventData) { rt.localScale = new Vector3(expendScale, expendScale, expendScale); }

    public virtual void OnPointerExit(PointerEventData eventData) { rt.localScale = new Vector3(defualtScale, defualtScale, defualtScale); }

    public virtual void OnPointerClick(PointerEventData eventData) { rt.localScale = new Vector3(defualtScale, defualtScale, defualtScale); }
}
