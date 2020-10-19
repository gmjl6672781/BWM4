using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 弧形UI
/// </summary>
public class ArcUIScroll : MonoBehaviour
{
    public List<GameObject> lstObj = new List<GameObject>();
    public Dictionary<int, GameObject> dicChosenObj = new Dictionary<int, GameObject>();
    /// <summary>
    /// 0是第一个
    /// </summary>
    [HideInInspector]
    public int curChosenID;

    private int targetIndex;
    private int maxNum;
    //平分后每一个item对应的-1到1的长度
    private float everyItem;
    // Start is called before the first frame update
    void Start()
    {
        curChosenID = 0;
        for (int i = 0; i < lstObj.Count; i++)
        {
            lstObj[i].name = i.ToString();
            GameObject t = lstObj[i].transform.GetChild(0).gameObject;
            t.SetActive(false);
            dicChosenObj.Add(i,t.gameObject);            
        }
        maxNum = lstObj.Count;
        everyItem = 1f / (maxNum / 2f);
    }
    /// <summary>
    /// 参数是下标
    /// </summary>
    /// <param name="index"></param>
    public void setTargetObj(int index)
    {
        targetIndex = index;
        InvokeRepeating("FlashHighLight",0.5f,0.5f);
    }
    
    public void UpdateNum(float t)
    {
        //Debug.Log("................................."+t+ "    "+everyItem+ "    " + maxNum);
        if (t==0)
        {
            return;
        }
        for (int i=0;i< maxNum;i++)
        {           
            
            //偶数情况
            if (maxNum % 2 == 0)
            {
                if (i < maxNum / 2)
                {
                   
                    if (t <= -(everyItem) * (maxNum / 2 - 1 - i))
                    {
                        setCurItem(i);
                        return;
                    }
                }
                else
                {
                    if (t <= (everyItem) * (i - maxNum / 2 + 1))
                    {
                        setCurItem(i);
                        return;
                    }
                }

            }
            else
            {
                //奇数判断  正中间的时候应该是一个负数到正数的区间
                if (i < maxNum / 2)
                {

                    if (t <= -(everyItem) * (maxNum / 2 - 1 - i+0.5f))
                    {
                        setCurItem(i);
                        return;
                    }else
                    {
                        if (i==maxNum/2)
                        {
                            if (t>=-0.5f* everyItem && t<0.5f* everyItem)
                            {
                                setCurItem(i);
                                return;
                            }
                        }
                        else
                        {
                            if (t <= (everyItem) * (i - maxNum / 2 + 0.5f))
                            {
                                setCurItem(i);
                                return;
                            }
                        }
                    }
                }
            }

        }
    }

    private void FlashHighLight()
    {
        if (lstObj.Count>=targetIndex)
        {
            lstObj[targetIndex].SetActive(!lstObj[targetIndex].activeSelf);
        }
    }
    /// <summary>
    /// 0是第一个
    /// </summary>
    /// <param name="index"></param>
    public void setCurItem(int index)
    {
        //Debug.Log("_________________________________________+"+index);
        if (dicChosenObj.ContainsKey(index))
        {
            foreach (int item in dicChosenObj.Keys)
            {
                dicChosenObj[item].SetActive(false);
            }
            dicChosenObj[index].SetActive(true);
        }
        curChosenID = index;
    }

    //private void OnDisable()
    //{
    //    CancelInvoke("FlashHighLight");
    //}

    private void OnDestroy()
    {
        CancelInvoke("FlashHighLight");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
