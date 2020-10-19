using BeinLab.VRTraing.Conf;
using UnityEngine;
namespace BeinLab.VRTraing.Gamer
{
    /// <summary>
    /// 一个抽象的任务对象
    /// 要执行任务，监听任务是否完成
    /// </summary>
    public class GameTasker : MonoBehaviour
    {
        public TaskConf taskConf;
    }
}