using BeinLab.FengYun.Gamer;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
#endif
namespace BeinLab.CarShow.Modus
{
    public class UIMsgDynamicConf : UIDynamicConf
    {
        public int fontSize = 36;
        [HideInInspector]
        public Text label;
        public static string sysMsg;
        private Coroutine updateCoroutine;

        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            if (mainRect&&!label)
            {
                label = mainRect.GetComponent<Text>();
                label.fontSize = fontSize;
                if (updateCoroutine != null)
                {
                    gameDynamicer.StopCoroutine(updateCoroutine);
                }
                //updateCoroutine = gameDynamicer.StartCoroutine(ShowMsg(gameDynamicer));
            }
        }

        private IEnumerator ShowMsg(GameDynamicer gameDynamicer)
        {

            yield return new WaitForSeconds(1);
        }
    }
}