using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonEffect : ButtonEffect {
    public float defualtSize = 250;
    public float expendSize = 260;

    protected override void Awake() { base.Awake(); }

    public override void OnPointerEnter(PointerEventData eventData) {
        rt.sizeDelta = new Vector2(expendSize, expendSize);
        rt.anchoredPosition -= new Vector2((expendSize - defualtSize) / 2, 0);
    }

    public override void OnPointerExit(PointerEventData eventData) {
        rt.sizeDelta = new Vector2(defualtSize, defualtSize);
        rt.anchoredPosition += new Vector2((expendSize - defualtSize) / 2, 0);
    }

    public override void OnPointerClick(PointerEventData eventData) {
        rt.sizeDelta = new Vector2(defualtSize, defualtSize);
        rt.anchoredPosition += new Vector2((expendSize - defualtSize) / 2, 0);
    }
}
