using System.Collections;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class ClimbComponent : MonoBehaviour
    {
        [Header("攀爬参数")]
        [Tooltip("攀爬水平方向射线检测像素")]
        public const int ClimbCheckDist = 2;           //攀爬检查像素值
        [Tooltip("攀爬竖直方向射线检测像素")]
        public const int ClimbUpCheckDist = 2;         //向上攀爬检查像素值
        [Tooltip("攀爬无法移动时间")]
        public const float ClimbNoMoveTime = .1f;
        [Tooltip("向上攀爬速度")]
        public const float ClimbUpSpeed = 4.5f;        //上爬速度
        [Tooltip("向下攀爬速度")]
        public const float ClimbDownSpeed = -8f;       //下爬速度
        [Tooltip("攀爬下滑速度")]
        public const float ClimbSlipSpeed = -3f;       //下滑速度
        [Tooltip("攀爬下滑加速度")]
        public const float ClimbAccel = 90f;          //下滑加速度
        [Tooltip("攀爬开始时，对原Y轴速度的衰减")]
        public const float ClimbGrabYMult = .2f;       //攀爬时抓取导致的Y轴速度衰减

        [Header("Hop参数（边缘登陆）")]
        public const float ClimbHopY = 12f;          //Hop的Y轴速度
        public const float ClimbHopX = 10f;           //Hop的X轴速度
        public const float ClimbHopForceTime = .2f;    //Hop时间
        public const float ClimbJumpBoostTime = .2f;   //WallBoost时间
        public const float ClimbHopNoWindTime = .3f;   //Wind情况下,Hop会无风0.3秒
    }
}