using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Controller;
using BeinLab.FengYun.Gamer;
using System.Collections.Generic;
using UnityEngine;
namespace BeinLab.Util
{
    public class CarDoor : MonoBehaviour
    {
        public List<DynamicConf> openDoorList;
        public List<DynamicConf> closeDoorList;
        public ColliderButton[] colliders;
        public bool isAutoSerch = false;
        public bool defState = false;
        private GameDynamicer gamer;
        private bool isCanDoOpeen = true;
        /// <summary>
        /// 当在某些事件范围内时执行
        /// </summary>
        public List<ActionConf> listenAction;
        /// <summary>
        /// 当在某些事件范围内时不执行
        /// </summary>
        public List<ActionConf> unListenAction;
        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            if (!DynamicActionController.Instance)
            {
                enabled = false;
                return;
            }
            DynamicActionController.Instance.OnDoAction += OnDoAction;
            gamer = GetComponentInParent<GameDynamicer>();
            if (isAutoSerch)
            {
                colliders = GetComponentsInChildren<ColliderButton>();
            }
            if (colliders != null)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].ClickCollider += OnClickCollider;
                }
            }
        }

        private void OnDoAction(ActionConf obj)
        {
            if (!obj) return;
            if (isCanDoOpeen)
            {
                if (unListenAction != null && unListenAction.Contains(obj))
                {
                    if (defState)
                    {
                        OnClickCollider();
                    }
                    isCanDoOpeen = false;
                }
            }
            else
            {
                if (listenAction != null && listenAction.Contains(obj))
                {
                    isCanDoOpeen = true;
                }
            }
        }

        private void OnClickCollider()
        {
            if (isCanDoOpeen)
            {
                defState = !defState;
                if (defState)
                {
                    DoOpenDyn();
                }
                else
                {
                    DoCloseDyn();
                }
            }
        }

        public void DoOpenDyn()
        {
            DoDynamicList(openDoorList);
        }

        public void DoCloseDyn()
        {
            DoDynamicList(closeDoorList);
        }

        private void DoDynamicList(List<DynamicConf> dynList)
        {
            if (dynList != null)
            {
                for (int i = 0; i < dynList.Count; i++)
                {
                    DynamicActionController.Instance.DoDynamic(dynList[i]);
                }
            }
        }
    }
}