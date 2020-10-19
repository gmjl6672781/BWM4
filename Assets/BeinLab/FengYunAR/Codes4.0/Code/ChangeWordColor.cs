using BeinLab.FengYun.Modus;
using BeinLab.FengYun.UI;
using BeinLab.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWordColor : MonoBehaviour
{
    private CellListDlg cellListDlg;
    private GameCell lastCell;
    public Color selectColor = Color.white;
    public Color defColor = Color.white;
    // Start is called before the first frame update
    void Start()
    {
        cellListDlg = GetComponent<CellListDlg>();
        if (cellListDlg)
        {
            cellListDlg.OnClickGameCell += OnClickGameCell;
        }
    }

    private void OnClickGameCell(GameCell cell, CellConf arg2,int index)
    {
        if (lastCell != cell)
        {
            ChangeCellColor(lastCell, defColor);
            lastCell = cell;
            ChangeCellColor(lastCell, selectColor);
        }
    }
    public void ChangeCellColor(GameCell cell, Color textColor)
    {
        if (cell)
        {
            Text label = UnityUtil.GetTypeChildByName<Text>(cell.gameObject, "Text");
            label.color = textColor;
        }
    }

}
