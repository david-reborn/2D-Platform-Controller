using System.Collections;
using UnityEngine;

namespace Myd.Platform.Demo
{
    //基础参数
    public class NormalComponent : MonoBehaviour
    {
        [Header("水平方向参数")]
        [Tooltip("最大水平速度")]
        public int MaxRun;
        [Tooltip("水平方向加速度")]
        public int RunAccel;
        [Tooltip("水平方向减速度")]
        public int RunReduce = 40;
        [Space]
        [Header("竖直方向参数")]
        [Tooltip("重力加速度")]
        public float Gravity = 90f; //重力
        [Tooltip("当速度小于该阈值时，重力减半。值越小，滞空时间越长")]
        public float HalfGravThreshold = 4f;
        [Tooltip("下落的最大速度（带方向，向上为正）")]
        public float MaxFall = -16; //普通最大下落速度
        [Tooltip("急速下落的最大速度（带方向，向上为正）")]
        public float FastMaxFall = -24f;  //快速最大下落速度
        [Tooltip("下落->急速下坠加速度")]
        public float FastMaxAccel = 30f; //快速下落加速度

        [Space]
        [Header("跳跃参数")]
        [Tooltip("最大跳跃速度")]
        public float JumpSpeed = 10.5f;
        [Tooltip("跳跃持续时间（跳起时,会持续响应跳跃按键[VarJumpTime]秒,影响跳跃的最高高度）")]
        public float VarJumpTime = 0.2f; 
        [Tooltip("跳跃水平方向，水平方向减速度")]
        public float JumpHBoost = 4f;
        [Tooltip("土狼时间（离开平台时，还能响应跳跃的时间）")]
        public float JumpGraceTime = 0.1f;
    }
}