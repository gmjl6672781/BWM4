using System;
using UnityEngine;
namespace BeinLab.Util
{
    public class ColliderButton : MonoBehaviour
    {
        public event Action ClickCollider;
        public void OnClickCollider()
        {
            if (ClickCollider != null)
            {
                ClickCollider();
            }
        }
    }
}