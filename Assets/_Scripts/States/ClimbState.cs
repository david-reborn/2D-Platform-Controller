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
            Vector2 tempSpeed = ctx.Speed;
            tempSpeed.x = 0;
            tempSpeed.y *= Constants.ClimbGrabYMult;
            //TODO 其他参数
            ctx.ClimbNoMoveTimer = Constants.ClimbNoMoveTime;

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
            if (Input.Jump.Pressed() && true)
            {
                if (ctx.MoveX == -(int)ctx.Facing)
                    ctx.WallJump(-(int)ctx.Facing);
                else
                    ctx.ClimbJump();

                return EActionState.Normal;
            }
            //检测前面的墙面是否存在


            //设置速度
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
                    //Up Limit
                    if (ctx.CollideCheck(ctx.Position + Vector2.up * 0.01f))// || (ClimbHopBlockedCheck() && SlipCheck(-1)))
                    {
                        Logging.Log($"========上部有障碍!!");
                        ctx.Speed.y = Mathf.Min(ctx.Speed.y, 0);
                        target = 0;
                        trySlip = true;
                    }
                    //else if (SlipCheck())
                    else if (false)
                    {
                        //Hopping
                        //ClimbHop();
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

            //TODO 滑行
            //if (trySlip && SlipCheck())
            //    target = ClimbSlipSpeed;

            ctx.Speed.y = Mathf.MoveTowards(ctx.Speed.y, target, Constants.ClimbAccel * deltaTime);

            //if (Input.MoveY.Value != 1 && Speed.Y > 0 && !CollideCheck<Solid>(Position + new Vector2((int)Facing, 1)))
            //    Speed.Y = 0;
            Logging.Log($"===ctx.MoveY:[{ctx.MoveY}]当前速度:{ctx.Speed}");
            return state;
        }
    }
}
