using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myd.Platform
{
    //这里涉及坐标的数值需要/10, 除时间类型
    public static class Constants
    {

        public static bool EnableWallSlide = true;
        public static bool EnableJumpGrace = true;
        public static bool EnableWallBoost = true;

        public static float Gravity = 90f; //重力

        public static float HalfGravThreshold = 4f; //滞空时间阈值
        public static float MaxFall = -16; //普通最大下落速度
        public static float FastMaxFall = -24f;  //快速最大下落速度
        public static float FastMaxAccel = 30f; //快速下落加速度
        //最大移动速度
        public static float MaxRun = 9f;
        //Hold情况下的最大移动速度
        public static float HoldingMaxRun = 7f;
        //空气阻力
        public static float AirMult = 0.65f;
        //移动加速度
        public static float RunAccel = 100f;
        //移动减速度
        public static float RunReduce = 40f;
        //
        public static float JumpSpeed = 10.5f;  //最大跳跃速度
        public static float VarJumpTime = 0.2f; //跳跃持续时间(跳起时,会持续响应跳跃按键[VarJumpTime]秒,影响跳跃的最高高度);
        public static float JumpHBoost = 4f; //退离墙壁的力
        public static float JumpGraceTime = 0.1f;//土狼时间

        #region WallJump
        public static float WallJumpCheckDist = 0.3f;
        public static float WallJumpForceTime = .16f; //墙上跳跃强制时间
        public static float WallJumpHSpeed = MaxRun + JumpHBoost;

        #endregion

        #region SuperWallJump
        public static float SuperJumpSpeed = JumpSpeed;
        public static float SuperJumpH = 26f;
        public static float SuperWallJumpSpeed = 16f;
        public static float SuperWallJumpVarTime = .25f;
        public static float SuperWallJumpForceTime = .2f;
        public static float SuperWallJumpH = MaxRun + JumpHBoost* 2;
        #endregion
        #region WallSlide
        public static float WallSpeedRetentionTime = .06f; //撞墙以后可以允许的保持速度的时间
        public static float WallSlideTime = 1.2f; //墙壁滑行时间
        public static float WallSlideStartMax = -2f;


        #endregion

        #region Dash相关参数
        public static float DashSpeed = 24f;           //冲刺速度
        public static float EndDashSpeed = 16f;        //结束冲刺速度
        public static float EndDashUpMult = .75f;       //如果向上冲刺，阻力。
        public static float DashTime = .15f;            //冲刺时间
        public static float DashCooldown = .2f;         //冲刺冷却时间，
        public static float DashRefillCooldown = .1f;   //冲刺重新装填时间
        public static int DashHJumpThruNudge = 6;       //
        public static int DashCornerCorrection = 4;     //水平Dash时，遇到阻挡物的可纠正像素值
        public static int DashVFloorSnapDist = 3;       //DashAttacking下的地面吸附像素值
        public static float DashAttackTime = .3f;       //
        public static int MaxDashes = 1;
        #endregion

        #region Climb参数
        public static float ClimbMaxStamina = 110;       //最大耐力
        public static float ClimbUpCost = 100 / 2.2f;   //向上爬得耐力消耗
        public static float ClimbStillCost = 100 / 10f; //爬着不动耐力消耗
        public static float ClimbJumpCost = 110 / 4f;   //爬着跳跃耐力消耗
        public static int ClimbCheckDist = 2;           //攀爬检查像素值
        public static int ClimbUpCheckDist = 2;         //向上攀爬检查像素值
        public static float ClimbNoMoveTime = .1f;
        public static float ClimbTiredThreshold = 20f;   //表现疲惫的阈值
        public static float ClimbUpSpeed = 4.5f;        //上爬速度
        public static float ClimbDownSpeed = -8f;       //下爬速度
        public static float ClimbSlipSpeed = -3f;       //下滑速度
        public static float ClimbAccel = 90f;          //下滑加速度
        public static float ClimbGrabYMult = .2f;       //攀爬时抓取导致的Y轴速度衰减
        public static float ClimbHopY = 12f;          //Hop的Y轴速度
        public static float ClimbHopX = 10f;           //Hop的X轴速度
        public static float ClimbHopForceTime = .2f;    //Hop时间
        public static float ClimbJumpBoostTime = .2f;   //WallBoost时间
        public static float ClimbHopNoWindTime = .3f;   //Wind情况下,Hop会无风0.3秒
        #endregion

        #region Duck参数
        public static float DuckFriction = 50f;
        public static float DuckSuperJumpXMult = 1.25f;
        public static float DuckSuperJumpYMult = .5f;
        #endregion

        #region Corner Correct
        public static int UpwardCornerCorrection = 4; //向上移动，X轴上边缘校正的最大距离
        #endregion

        public static float LaunchedMinSpeedSq = 196;
    }
}
