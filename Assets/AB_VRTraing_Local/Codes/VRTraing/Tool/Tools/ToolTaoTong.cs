using BeinLab.VRTraing.Conf;
using Valve.VR.InteractionSystem;
namespace BeinLab.VRTraing
{
    public class ToolTaoTong : ToolBasic
    {
        private ToolBanShou curToolBanShou;
        protected override void Start()
        {
            base.Start();
            OnPutAoCao += OnPutBanShou;
        }
        /// <summary>
        /// 当套上套筒时
        /// </summary>
        /// <param name="obj"></param>
        private void OnPutBanShou(PutTooConf obj)
        {
            if (obj.triggerTool.toolBasic is ToolBanShou)
            {
                curToolBanShou = (obj.triggerTool.toolBasic as ToolBanShou);
                curToolBanShou.OnCatchTaoTong(toolConf);
                SetTrigger(true);
            }
            else
            {
                if (curToolBanShou)
                {
                    curToolBanShou.OnCatchTaoTong(null);
                }
                curToolBanShou = null;
            }
        }

        /// <summary>
        /// 当套筒被抓在手中时，去掉对应的跟随效果
        /// </summary>
        /// <param name="hand"></param>
        protected override void OnCatch_(Hand hand)
        {
            base.OnCatch_(hand);
            if (curToolBanShou)
            {
                curToolBanShou.OnCatchTaoTong(null);
            }
            transform.SetParent(null);
            curToolBanShou = null;
        }
    }
}