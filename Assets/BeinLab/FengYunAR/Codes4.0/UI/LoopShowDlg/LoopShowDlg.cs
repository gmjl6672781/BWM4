using BeinLab.Util;
using DG.Tweening;
using UnityEngine;

public class LoopShowDlg : MonoBehaviour
{
    public float showTime = 1f;
    private RectTransform carShow;
    // Start is called before the first frame update
    void Start()
    {
        carShow = UnityUtil.GetTypeChildByName<RectTransform>(gameObject, "CarShow");
        DoLogoShow();
    }
    private void DoLogoShow()
    {
        if (carShow)
        {
            Vector2 initPos = Vector2.zero;
            initPos.x = Screen.width / 2 + carShow.sizeDelta.x / 2;
            carShow.anchoredPosition3D = initPos;
            carShow.DOAnchorPosX(0, showTime);
        }
    }
}
