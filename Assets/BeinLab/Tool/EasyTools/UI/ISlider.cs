using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace BeinLab.Util
{
    public class ISlider : Slider
    {
        public event Action OnSwipeOver;
        public event Action OnSwipeStart;
        public event Action OnSwipe;
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (OnSwipeStart != null)
            {
                OnSwipeStart();
            }
        }
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            if (OnSwipe != null)
            {
                OnSwipe();
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (OnSwipeOver != null)
            {
                OnSwipeOver();
            }
        }
    }
}