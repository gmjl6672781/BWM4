using BeinLab.FengYun.Moudus;
using BeinLab.Util;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IScrollView : ScrollRect
{
    private Coroutine moveCor;
    public int showCount;
    private GameObject lectIcon;
    private GameObject rightIcon;
    public float moveSpeed = 10;
    /// <summary>
    /// 当移动的某一页
    /// </summary>
    public event Action<int> OnMoveIndex;
    public event Action OnBeginMove;
    public bool isShowNext;
    public float watieTime = 0;
    public float swipeDet = 0.5f;

    public bool isFreeMove = false;
    private float lastPosition;
    protected override void Awake()
    {
        base.Awake();
        lectIcon = UnityUtil.GetChildByName(gameObject, "LeftIcon");
        rightIcon = UnityUtil.GetChildByName(gameObject, "RightIcon");
        lectIcon.SetActive(false);
        rightIcon.SetActive(false);
    }
    /// <summary>
    /// 先写竖屏代码
    /// </summary>
    public void InitScroll(int showCount, Vector2 cellSize, Vector2 space, int allChild,
        Vector2 extra, MovementType moveType = MovementType.Elastic, bool isFreeMove = false)
    {
        float target = showCount * cellSize.x + space.x * (showCount - 1);
        RectTransform rt = GetComponent<RectTransform>();
        this.showCount = showCount;
        this.isFreeMove = isFreeMove;
        inertia = isFreeMove;
        if (vertical)
        {
            target = showCount * cellSize.y + space.y * (showCount - 1);
            rt.sizeDelta = new Vector2(cellSize.x, target) + extra;
            rightIcon.SetActive(false);
            //if(allChild <= showCount)
            lectIcon.SetActive(false);
            verticalNormalizedPosition = 1;
            this.DOVerticalNormalizedPos(1, 0.2f);
            if (isShowNext)
            {
                rightIcon.SetActive(allChild > showCount);
            }
        }
        else
        {
            rt.sizeDelta = new Vector2(target, cellSize.y) + extra;
            horizontalNormalizedPosition = 0;
            if (isShowNext)
            {
                rightIcon.SetActive(allChild > showCount);
            }
            //if(allChild <= showCount)
            lectIcon.SetActive(false);
        }
        movementType = moveType;
    }

    /// <summary>
    /// 配置
    /// </summary>
    /// <param name="conf"></param>
    public void SetData(ScrollConf conf)
    {
        vertical = conf.vertical;
        horizontal = conf.horizontal;
        moveSpeed = conf.moveSpeed;
        swipeDet = conf.swipeDet;
        isShowNext = conf.isShowNext;
        isFreeMove = conf.isFreeMove;
        watieTime = conf.watieTime;
        InitScroll(conf.showCount, conf.size, conf.space, conf.allCount, conf.extra, conf.swipe);
    }

    /// <summary>
    /// 当滑动结束后
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (!isFreeMove)
        {
            if (moveCor != null)
            {
                StopCoroutine(moveCor);
                moveCor = null;
            }
            //moveCor = StartCoroutine(AutoMove());
            moveCor = StartCoroutine(AutoTween(0));
        }
    }

    /// <summary>
    /// 开始
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        lastPosition = verticalNormalizedPosition;
        if (horizontal)
        {
            lastPosition = horizontalNormalizedPosition;
        }
        if (moveCor != null)
        {
            StopCoroutine(moveCor);
            moveCor = null;
        }
        if (OnBeginMove != null)
        {
            OnBeginMove();
        }
    }

    public void Rest()
    {
        lectIcon.SetActive(false);
        rightIcon.SetActive(false);
    }

    public void RestIcon()
    {
        if (!isShowNext) return;
        if (horizontal)
        {
            lectIcon.SetActive(horizontalNormalizedPosition > 0.001f);
            rightIcon.SetActive(Mathf.Abs(horizontalNormalizedPosition - 1) > 0.001f);
            int all = content.transform.childCount;
            if (all <= showCount || all == 0)
            {
                rightIcon.SetActive(false);
                lectIcon.SetActive(false);
            }
        }
        else if (vertical)
        {
            lectIcon.SetActive(Mathf.Abs(verticalNormalizedPosition - 1) > 0.001f);
            rightIcon.SetActive(verticalNormalizedPosition > 0.001f);
            int all = content.transform.childCount;
            if (all <= showCount || all == 0)
            {
                rightIcon.SetActive(false);
                lectIcon.SetActive(false);
            }
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (moveCor != null)
        {
            StopCoroutine(moveCor);
            moveCor = null;
        }
    }
    private IEnumerator AutoTween(int next)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(watieTime);
        float curValue = verticalNormalizedPosition;
        if (horizontal)
        {
            curValue = horizontalNormalizedPosition;
        }


        int all = content.transform.childCount;
        ///一个标准的各自
        float det = 1.0f / (all - showCount);
        float target = 0;
        int index = (int)(curValue / det + 0.5f);
        if (Mathf.Abs(lastPosition - curValue) >= det * swipeDet)
        {
            index = (int)(curValue / det) + (lastPosition - curValue > det * swipeDet ? 0 : (lastPosition - curValue == det * swipeDet ? 0 : 1));
            index += next;
        }
        else
        {
            ///计算占多少格子 四舍五入
            index += next;
        }
        index = Mathf.Clamp(index, 0, all - 1);
        target = index * det;
        target = Mathf.Clamp(target, 0, 1);
        while (Mathf.Abs(curValue - target) > 0.001f)
        {
            curValue = Mathf.Lerp(curValue, target, Time.deltaTime * moveSpeed);

            if (horizontal)
            {
                horizontalNormalizedPosition = curValue;
                //lectIcon.SetActive(horizontalNormalizedPosition > 0.001f);
                //rightIcon.SetActive(Mathf.Abs(horizontalNormalizedPosition - 1) > 0.001f);
            }
            if (vertical)
            {
                verticalNormalizedPosition = curValue;
            }
            yield return new WaitForFixedUpdate();
        }
        if (horizontal)
        {
            horizontalNormalizedPosition = target;
            //lectIcon.SetActive(horizontalNormalizedPosition > 0.001f);
            //rightIcon.SetActive(Mathf.Abs(horizontalNormalizedPosition - 1) > 0.001f);
        }
        if (vertical)
        {
            verticalNormalizedPosition = target;
        }
        if (horizontal && isShowNext)
        {
            lectIcon.SetActive(horizontalNormalizedPosition > 0.001f);
            rightIcon.SetActive(Mathf.Abs(horizontalNormalizedPosition - 1) > 0.001f);
            if (all <= showCount)
            {
                rightIcon.SetActive(false);
                lectIcon.SetActive(false);
            }
        }
        else if (vertical && isShowNext)
        {
            lectIcon.SetActive(Mathf.Abs(verticalNormalizedPosition - 1) > 0.001f);
            rightIcon.SetActive(verticalNormalizedPosition > 0.001f);
            if (all <= showCount)
            {
                rightIcon.SetActive(false);
                lectIcon.SetActive(false);
            }
        }
        if (OnMoveIndex != null)
        {
            OnMoveIndex(index);
        }

    }

    /// <summary>
    /// 移动到下一格
    /// </summary>
    public void MoveNext()
    {
        if (moveCor != null)
        {
            StopCoroutine(moveCor);
            moveCor = null;
        }
        //moveCor = StartCoroutine(AutoMove());
        moveCor = StartCoroutine(AutoTween(1));
    }
    public void MoveLast()
    {
        if (moveCor != null)
        {
            StopCoroutine(moveCor);
            moveCor = null;
        }
        //moveCor = StartCoroutine(AutoMove());
        moveCor = StartCoroutine(AutoTween(-1));
    }

    /// <summary>
    /// 计算距离最近的点
    /// </summary>
    /// <returns></returns>
    //private IEnumerator AutoMove()
    //{
    //    yield return new WaitForFixedUpdate();
    //    float curHeight = content.anchoredPosition.y;
    //    if (horizontal)
    //    {
    //        curHeight = content.anchoredPosition.x;
    //    }

    //    float cellSize = content.GetComponent<GridLayoutGroup>().cellSize.y;
    //    if (horizontal)
    //    {
    //        cellSize = content.GetComponent<GridLayoutGroup>().cellSize.x;
    //    }

    //    float space = content.GetComponent<GridLayoutGroup>().spacing.y;
    //    if (horizontal)
    //    {
    //        space = content.GetComponent<GridLayoutGroup>().spacing.x;
    //    }

    //    float det = cellSize + space;
    //    int index = (int)(curHeight / det + 0.5f);
    //    float target = index * det;
    //    float max = 0;
    //    RectTransform rt = GetComponent<RectTransform>();
    //    int all = content.transform.childCount;
    //    max = (all - showCount) * (cellSize + space);
    //    target = Mathf.Clamp(target, 0, max);

    //    if (vertical)
    //    {
    //        while (Mathf.Abs(content.anchoredPosition.y - target) > 0.01 && verticalNormalizedPosition >= 0 && verticalNormalizedPosition <= 1)
    //        {
    //            Vector2 pos = content.anchoredPosition;
    //            pos.y = Mathf.Lerp(pos.y, target, Time.deltaTime * 30);
    //            content.anchoredPosition = pos;
    //            yield return new WaitForFixedUpdate();
    //        }
    //        print(verticalNormalizedPosition);
    //    }
    //    else if (horizontal)
    //    {
    //        while (Mathf.Abs(content.anchoredPosition.x - target) > 0.01 && horizontalNormalizedPosition >= 0 && horizontalNormalizedPosition <= 1)
    //        {
    //            Vector2 pos = content.anchoredPosition;
    //            pos.x = Mathf.Lerp(pos.y, target, Time.deltaTime * 30);
    //            content.anchoredPosition = pos;
    //            yield return new WaitForFixedUpdate();
    //        }
    //    }
    //}
}
