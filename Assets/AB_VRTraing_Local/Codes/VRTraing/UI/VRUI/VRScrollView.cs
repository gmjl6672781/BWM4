using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VRScrollView : ScrollRect
{
    public override void OnDrag(PointerEventData eventData)
    {
        this.UpdateBounds();
        ///当前的组件的位置
        Vector2 pos = this.content.anchoredPosition;
        Vector2 delt = eventData.delta;

        if (!this.horizontal)
        {
            delt.x = 0;
        }
        if (!this.vertical)
        {
            delt.y = 0;
        }
        pos += delt;
        this.SetContentAnchoredPosition(pos);
    }
}
