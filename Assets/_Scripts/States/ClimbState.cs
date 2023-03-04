using Myd.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class ClimbState : BaseActionState
    {
        public ClimbState(PlayerController context) : base(EActionState.Climb, context)
        {
        }

        public override IEnumerator Coroutine()
        {
            yield return null;
        }

        public override bool IsCoroutine()
        {
            return false;
        }

        public override void OnBegin()
        {
            ctx.Speed.x = 0;
            ctx.Speed.y *= Constants.ClimbGrabYMult;
            //TODO 其他参数
            ctx.WallSlide?.ResetTime();
            ctx.WallBoost?.ResetTime();
            ctx.ClimbNoMoveTimer = Constants.ClimbNoMoveTime;

            //两个像素的吸附功能
            ctx.ClimbSnap();
            //TODO 表现
        }

        public override void OnEnd()
        {
            //TODO 
        }

        public override EActionState Update(float deltaTime)
        {
            ctx.ClimbNoMoveTimer -= deltaTime;
            //处理跳跃
            if (Input.Jump.Pressed() && (!ctx.Ducking || ctx.CanUnDuck))
            {
                if (ctx.MoveX == -(int)ctx.Facing)
                    ctx.WallJump(-(int)ctx.Facing);
                else
                    ctx.ClimbJump();

                return EActionState.Normal;
            }
            if (ctx.CanDash)
            {
                return this.ctx.Dash();
            }
            //放开抓取键,则回到Normal状态
            if (!Input.Grab.Checked())
            {
                //Speed += LiftBoost;
                //Play(Sfxs.char_mad_grab_letgo);
                return EActionState.Normal;
            }

            //检测前面的墙面是否存在
            if (!ctx.CollideCheck(ctx.Position, Vector2.right * (int)ctx.Facing))
            {
                //Climbed over ledge?
                if (ctx.Speed.y < 0)
                {
                    //if (ctx.WallBoosting)
                    //{
                        //    Speed += LiftBoost;
                        //    Play(Sfxs.char_mad_grab_letgo);
                    //}
                    //else
                    {
                        ClimbHop(); //自动翻越墙面
                    }
                }

                return EActionState.Normal;
            }

            {
                //Climbing
                float target = 0;
                bool trySlip = false;
                if (ctx.ClimbNoMoveTimer <= 0)
                {
                    if (false)//(ClimbBlocker.Check(Scene, this, Position + Vector2.UnitX * (int)Facing))  
                    {
                        //trySlip = true;
                    }
                    else if (ctx.MoveY == 1)
                    {
                        //往上爬
                        target = Constants.ClimbUpSpeed;
                        //向上攀爬的移动限制,顶上有碰撞或者SlipCheck
                        if (ctx.CollideCheck(ctx.Position, Vector2.up))
                        {
                            Debug.Log("=======ClimbSlip_Type1");
                            ctx.Speed.y = Mathf.Min(ctx.Speed.y, 0);
                            target = 0;
                            trySlip = true;
                        }
                        //如果在上面0.6米处存在障碍，且前上方0.1米处没有阻碍，依然不允许向上
                        else if (ctx.ClimbHopBlockedCheck() && ctx.SlipCheck(0.1f)){
                            Debug.Log("=======ClimbSlip_Type2");
                            ctx.Speed.y = Mathf.Min(ctx.Speed.y, 0);
                            target = 0;
                            trySlip = true;
                        }
                        //如果前上方没有阻碍, 则进行ClimbHop
                        else if (ctx.SlipCheck())
                        {
                            //Hopping
                            ClimbHop();
                            return EActionState.Normal;
                        }
                    }
                    else if (ctx.MoveY == -1)
                    {
                        //往下爬
                        target = Constants.ClimbDownSpeed;

                        if (ctx.OnGround)
                        {
                            ctx.Speed.y = Mathf.Max(ctx.Speed.y, 0);    //落地时,Y轴速度>=0
                            target = 0;
                        }
                        else
                        {
                            //TODO 创建粒子效果
                            //CreateWallSlideParticles((int)Facing);
                        }
                    }
                    else
                    {
                        trySlip = true;
                    }
                }
                else
                {
                    trySlip = true;
                }

                //滑行
                if (trySlip && ctx.SlipCheck())
                {
                    Debug.Log("=======ClimbSlip_Type4");
                    target = Constants.ClimbSlipSpeed;
                }
                ctx.Speed.y = Mathf.MoveTowards(ctx.Speed.y, target, Constants.ClimbAccel * deltaTime);
            }
            //TrySlip导致的下滑在碰到底部的时候,停止下滑
            if (ctx.MoveY != -1 && ctx.Speed.y < 0 && !ctx.CollideCheck(ctx.Position, new Vector2((int)ctx.Facing, -1)))
            {
                ctx.Speed.y = 0;
            }
            //TODO Stamina
            return state;
        }

        private void ClimbHop()
        {
            Debug.Log("=====ClimbHop");
            //播放Hop的精灵动画
            //playFootstepOnLand = 0.5f;

            //获取目标的落脚点
            bool hit = ctx.CollideCheck(ctx.Position, Vector2.right * (int)ctx.Facing);
            if (hit)
            {
                ctx.HopWaitX = (int)ctx.Facing;
                ctx.HopWaitXSpeed = (int)ctx.Facing * Constants.ClimbHopX;
            }
            //ctx.ClimbHopSolid = ctx.CollideClimbHop((int)ctx.Facing);
            //if (ctx.ClimbHopSolid)
            //{
            //    //climbHopSolidPosition = climbHopSolid.Position;
            //    ctx.HopWaitX = (int)ctx.Facing;
            //    ctx.HopWaitXSpeed = (int)ctx.Facing * Constants.ClimbHopX;
            //}
            else
            {
                ctx.HopWaitX = 0;
                ctx.Speed.x = (int)ctx.Facing * Constants.ClimbHopX;
            }

            ctx.Speed.y = Math.Max(ctx.Speed.y, Constants.ClimbHopY);
            ctx.ForceMoveX = 0;
            ctx.ForceMoveXTimer = Constants.ClimbHopForceTime;
        } 
    }
}
