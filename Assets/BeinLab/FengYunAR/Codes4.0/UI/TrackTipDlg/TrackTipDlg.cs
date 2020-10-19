using BeinLab.Util;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TrackTipDlg : MonoBehaviour
{
    private RectTransform arror;
    private RectTransform kuang;
    public float movePosY;
    public float moveTime = 0.3f;
    public Ease easeMoveLine;

    public float zoomSize;
    public float zoomTime = 0.3f;
    public Ease easeZoomLine;
    public Ease easeFadeLine;
    // Start is called before the first frame update
    void Start()
    {
        arror = UnityUtil.GetTypeChildByName<RectTransform>(gameObject, "ArrorImg");
        kuang = UnityUtil.GetTypeChildByName<RectTransform>(gameObject, "KuangImg");
        arror.DOAnchorPosY(movePosY, moveTime).SetEase(easeMoveLine).SetLoops(-1, LoopType.Yoyo);
        kuang.DOScale(Vector3.one * zoomSize, zoomTime).SetEase(easeZoomLine).SetLoops(-1, LoopType.Restart).SetDelay(moveTime);
        kuang.GetComponent<Image>().DOFade(0, zoomTime).SetEase(easeZoomLine).SetLoops(-1, LoopType.Restart).SetDelay(moveTime);
    }

}
