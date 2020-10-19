using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace BeinLab.Util
{
    public class IButton : Button
    {
        public event Action<bool> onPress;
        private bool isPress;
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            isPress = true;
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            isPress = false;
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            isPress = false;
        }
        private void Update()
        {
            if (onPress != null)
            {
                onPress(isPress);
            }
        }
    }
}