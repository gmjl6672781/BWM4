using BeinLab.FengYun.Controller;
using BeinLab.FengYun.Modus;
using System;
using UnityEngine;
namespace BeinLab.FengYun.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class GameCell : MonoBehaviour
    {
        public event Action<GameCell, CellConf,int> OnClickCellEvent;
        public string mainBtnName = "MainButton";
        public CellConf cellConf;
        private int showIndex;
        private void Start()
        {
            if (cellConf)
            {
                SetData(cellConf, showIndex);
            }
        }
        /// <summary>
        /// 初始化时，默认不可点击状态
        /// </summary>
        /// <param name="cellConf"></param>
        public void SetData(CellConf cellConf,int index)
        {
            this.cellConf = cellConf;
            cellConf.SetData(this, index);
            this.showIndex = index;
        }
        /// <summary>
        /// 初始化cell
        /// </summary>
        public void InitCell()
        {

        }

        public void SetActiveEvent(CellConf cellConf,int index=0)
        {
            if (OnClickCellEvent != null)
            {
                if (CreatLogoDlg.Instance)
                    CreatLogoDlg.Instance.CreateDlg(cellConf.name);
                OnClickCellEvent(this, cellConf, index);
            }
        }
        /// <summary>
        /// 取消某事件
        /// </summary>
        /// <param name="cellConf"></param>
        public void UnSelect(CellConf cellConf)
        {
            if (cellConf && cellConf.unActiveAction)
            {
                DynamicActionController.Instance.DoAction(cellConf.unActiveAction);
            }
            if (!string.IsNullOrEmpty(cellConf.unTrigger))
            {
                DynamicActionController.Instance.DoAction(cellConf.unTrigger);
            }
            //print(cellConf.unActiveAction);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Select()
        {
            if (cellConf)
            {
                cellConf.Select();
            }
        }
    }
}