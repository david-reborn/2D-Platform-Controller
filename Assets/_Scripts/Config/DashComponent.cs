using System.Collections;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class DashComponent : MonoBehaviour
    {
        [Header("Dash冲刺参数")]
        [Tooltip("开始冲刺初速度")]
        public float DashSpeed = 24f;          //冲刺速度
        [Tooltip("结束冲刺后速度")]
        public float EndDashSpeed = 16f;        //结束冲刺速度
        [Tooltip("Y轴向上冲刺的衰减系数")]
        public float EndDashUpMult = .75f;       //如果向上冲刺，阻力。
        [Tooltip("冲刺时间")]
        public float DashTime = .15f;            //冲刺时间
        [Tooltip("冲刺冷却时间")]
        public float DashCooldown = .2f;         //冲刺冷却时间，
        [Tooltip("冲刺重新装填时间")]
        public float DashRefillCooldown = .1f;   //冲刺重新装填时间
        [Tooltip("Dashs水平方向位置校正的像素值")]
        public int DashCornerCorrection = 4;     //水平Dash时，遇到阻挡物的可纠正像素值

        [Tooltip("最大Dash次数")]
        public int MaxDashes = 1;    // 最大Dash次数
    }
}