using BeinLab.FengYun.Controller;
using BeinLab.FengYun.Modus;
using BeinLab.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeinLab.FengYun.UI
{
    /// <summary>
    /// Cell集合列表组件
    /// 此为工具类
    /// </summary>
    public class CellListDlg : MonoBehaviour
    {
        /// <summary>
        /// 组件的展示节点
        /// </summary>
        public Transform cellRoot;
        /// <summary>
        /// 当前的配置列表
        /// </summary>
        private CellListConf cellList;

        public CellListConf CellList
        {
            get
            {
                return cellList;
            }

            set
            {
                cellList = value;
            }
        }
        /// <summary>
        /// 当老的配置被替换的时候
        /// </summary>
        public event Action<CellListConf> ChangeCellList;
        /// <summary>
        /// 当新的配置被创建的时候
        /// </summary>
        public event Action<CellListConf> CreateCellList;
        public CellListConf cellListConf;
        public string cellListConfPath;
        public Image selectBG;
        public bool isUseSelectBG;
        public event Action<GameCell, CellConf, int> OnClickGameCell;
        private void Awake()
        {
            ChangeCellList += OnChangCellList;
            CreateCellList += OnCreateCellList;
        }
        private void Start()
        {
            if (!string.IsNullOrEmpty(cellListConfPath))
            {
                var obj = GameAssetLoader.Instance.LoadObject(cellListConfPath);
                if (obj && obj is CellListConf)
                {
                    cellListConf = obj as CellListConf;
                }
            }

            if (cellListConf)
            {
                SetData(cellListConf);
            }
            if (selectBG)
            {
                selectBG.gameObject.SetActive(false);
            }
        }

        private List<CellConf> cellConfList = null;
        private List<GameCell> buttons = new List<GameCell>();
        private Vector2 bgExtra = Vector2.zero;
        /// <summary>
        /// 当创建Cell List的时候
        /// </summary>
        /// <param name="curCellList"></param>
        private void OnCreateCellList(CellListConf curCellList)
        {
            cellConfList = curCellList.cellConfList;
            if (curCellList != null)
            {
                if (selectBG)
                {
                    selectBG.gameObject.SetActive(false);
                }
                bgExtra = curCellList.bgExtra;
                isUseSelectBG = curCellList.isUseSelectBG;
                if (curCellList.selectBG && isUseSelectBG) 
                {
                    selectBG.sprite = curCellList.selectBG;
                    selectBG.SetNativeSize();
                }
                DynamicActionController.Instance.DoAction(curCellList.createAction);
                if (curCellList.scrollConf)
                {
                    IScrollView scroll = UnityUtil.GetChildByName(gameObject, curCellList.scrollConf.scrollName).GetComponent<IScrollView>();
                    curCellList.scrollConf.allCount = curCellList.cellConfList.Count;
                    cellRoot.GetComponent<GridLayoutGroup>().cellSize = curCellList.scrollConf.size;
                    cellRoot.GetComponent<GridLayoutGroup>().spacing = curCellList.scrollConf.space;
                    if (scroll)
                    {
                        scroll.SetData(curCellList.scrollConf);
                    }
                }
                GameCell defCell = null;
                if (buttons != null)
                    buttons.Clear();
                ///组件的配置列表
                for (int i = 0; i < curCellList.cellConfList.Count; i++)
                {
                    CellConf cellConf = curCellList.cellConfList[i];
                    GameCell cell = Instantiate(curCellList.cellPrefab);
                    cell.name = curCellList.cellPrefab.name;
                    UnityUtil.SetParent(cellRoot, cell.transform);
                    int index = i;
                    cell.SetData(cellConf, index);
                    cell.OnClickCellEvent += OnClickCellEvent;
                    cell.transform.localScale = curCellList.scale;
                    if (i == curCellList.defSelect)
                    {
                        defCell = cell;
                    }
                    if (curCellList.cellPrefab.GetComponent<Button>()&&!isUseSelectBG)
                        buttons.Add(cell);
                }
                if (defCell)
                {
                    TimerMgr.Instance.CreateTimer(defCell.Select, 0.05f);
                }
            }
        }
        private GameCell lastCell;
        private void OnClickCellEvent(GameCell cell, CellConf cellConf, int index)
        {
            if (lastCell && lastCell != cell)
            {
                lastCell.UnSelect(lastCell.cellConf);
            }
            if (!isUseSelectBG)
            {
                for (int i = 0; i < cellConfList.Count; i++)
                {
                    if (cellConf.name != cellConfList[i].name)
                    {
                        buttons[i].GetComponent<Image>().sprite = cellConfList[i].icon;
                        buttons[i].GetComponent<Image>().SetNativeSize();
                    }
                }
            }
            if (selectBG && isUseSelectBG)
            {
                selectBG.gameObject.SetActive(true);
                selectBG.rectTransform.sizeDelta = cell.GetComponent<RectTransform>().sizeDelta + bgExtra;
                selectBG.transform.SetParent(cell.transform);
                selectBG.rectTransform.anchoredPosition3D = Vector3.zero;
                selectBG.rectTransform.localRotation = Quaternion.identity;
                selectBG.rectTransform.localScale = Vector3.one;
            }
            else
            {
                if (cellConf.pressIcon)
                {
                    cell.GetComponent<Image>().sprite = cellConf.pressIcon;
                    cell.GetComponent<Image>().SetNativeSize();
                }
            }
            lastCell = cell;
            if (OnClickGameCell != null)
            {
                OnClickGameCell(cell, cellConf, index);
            }
        }


        /// <summary>
        /// 当前的CellList
        /// 即将变成其他的
        /// </summary>
        /// <param name="lastCellList"></param>
        private void OnChangCellList(CellListConf lastCellList)
        {
            ///清除
            if (lastCellList)
            {
                DynamicActionController.Instance.DoAction(lastCellList.delectAction);
                for (int i = 0; i < cellRoot.childCount; i++)
                {
                    Destroy(cellRoot);
                }
                if (selectBG)
                {
                    selectBG.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 设置CellListConf
        /// </summary>
        /// <param name="cellList"></param>
        public void SetData(CellListConf cellList)
        {
            if (this.CellList != cellList)
            {
                if (ChangeCellList != null)
                {
                    ChangeCellList(this.CellList);
                }
                this.CellList = cellList;
                if (CreateCellList != null)
                {
                    CreateCellList(cellList);
                }
            }
        }

    }
}