using Myd.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class NormalState : BaseActionState
    {
        public NormalState(PlayerController controller):base(EActionState.Normal, controller)
        {
        }

        public override IEnumerator Coroutine()
        {
            throw new NotImplementedException();
        }

        public override bool IsCoroutine()
        {
            return false;
        }

        public override void OnBegin()
        {
            this.ctx.MaxFall = Constants.MaxFall;
        }

        public override void OnEnd()
        {
            this.ctx.WallBoost?.ResetTime();
            this.ctx.WallSpeedRetentionTimer = 0;
            this.ctx.HopWaitX = 0;
        }

        public override EActionState Update(float deltaTime)
        {
            //Climb
            if (Input.Grab.Checked() && !ctx.Ducking)
            {
                //Climbing
                if (ctx.Speed.y <= 0 && Math.Sign(ctx.Speed.x) != -(int)ctx.Facing)
                {
                    if (ctx.ClimbCheck((int)ctx.Facing))
                    {
                        ctx.Ducking = false;
                        return EActionState.Climb;
                    }
                }
            }

            //Dashing
            if (this.ctx.CanDash)
            {
                return this.ctx.Dash();
            }

            //Ducking
            if (ctx.Ducking)
            {
                if (ctx.OnGround && ctx.MoveY != -1)
                {
                    if (ctx.CanUnDuck)
                    {
                        ctx.Ducking = false;
                    }
                    else if (ctx.Speed.x == 0)
                    {
                        //根据角落位置，进行挤出操作
                    }
                }
            }
            else if (ctx.OnGround && ctx.MoveY == -1 && ctx.Speed.y <= 0)
            {
                ctx.Ducking = true;
                EventManager.Get().FireOnDuck(true);
            }

            //水平面上移动,计算阻力
            if (ctx.Ducking && ctx.OnGround)
            {
                ctx.Speed.x = Mathf.MoveTowards(ctx.Speed.x, 0, Constants.DuckFriction * deltaTime);
            }
            else
            {
                float mult = ctx.OnGround ? 1 : Constants.AirMult;
                //计算水平速度
                float max = ctx.Holding == null ? Constants.MaxRun : Constants.HoldingMaxRun;
                if (Math.Abs(ctx.Speed.x) > max && Math.Sign(ctx.Speed.x) == this.ctx.MoveX)
                {
                    //同方向加速
                    ctx.Speed.x = Mathf.MoveTowards(ctx.Speed.x, max * this.ctx.MoveX, Constants.RunReduce * mult * Time.deltaTime);
                }
                else
                {
                    //反方向减速
                    ctx.Speed.x = Mathf.MoveTowards(ctx.Speed.x, max * this.ctx.MoveX, Constants.RunAccel * mult * Time.deltaTime);
                }
            }
            //计算竖直速度
            {
                //计算最大下落速度
                {
                    float maxFallSpeed = Constants.MaxFall;
                    float fastMaxFallSpeed = Constants.FastMaxFall;

                    if (this.ctx.MoveY == -1 && this.ctx.Speed.y <= maxFallSpeed)
                    {
                        this.ctx.MaxFall = Mathf.MoveTowards(this.ctx.MaxFall, fastMaxFallSpeed, Constants.FastMaxAccel * deltaTime);

                        //处理表现
                        EventManager.Get().FireOnFall(this.ctx.Speed.y);
                    }
                    else
                    {
                        this.ctx.MaxFall = Mathf.MoveTowards(this.ctx.MaxFall, maxFallSpeed, Constants.FastMaxAccel * deltaTime);
                    }
                }

                if (!ctx.OnGround)
                {
                    float max = this.ctx.MaxFall;//最大下落速度
                    //Wall Slide
                    if (ctx.WallSlide != null)
                    {
                        max = ctx.WallSlide.AdjustMaxFall(max);
                    }
                    float mult = (Math.Abs(ctx.Speed.y) < Constants.HalfGravThreshold && (Input.Jump.Checked())) ? .5f : 1f;
                    //空中的情况,需要计算Y轴速度
                    ctx.Speed.y = Mathf.MoveTowards(ctx.Speed.y, max, Constants.Gravity * mult * deltaTime);
                }

                //处理跳跃
                if (ctx.VarJumpTimer > 0)
                {
                    if (Input.Jump.Checked())
                    {
                        //如果按住跳跃，则跳跃速度不受重力影响。
                        ctx.Speed.y = Math.Max(ctx.Speed.y, ctx.VarJumpSpeed);
                    }
                    else
                        ctx.VarJumpTimer = 0;
                }
            }

            if (Input.Jump.Pressed())
            {
                //土狼时间范围内,允许跳跃
                if (this.ctx.JumpCheck.AllowJump())
                {
                    this.ctx.Jump();
                }else if (ctx.CanUnDuck)
                {
                    //如果右侧有墙
                    if (ctx.WallJumpCheck(1))
                    {
                        if (ctx.Facing == Facings.Right && Input.Grab.Checked())
                            ctx.ClimbJump();
                        else
                            ctx.WallJump(-1);
                    }
                    //如果左侧有墙
                    else if (ctx.WallJumpCheck(-1))
                    {
                        if (ctx.Facing == Facings.Left && Input.Grab.Checked())
                            ctx.ClimbJump();
                        else
                            ctx.WallJump(1);
                    }
                }
            }

            return state;
        }
    }


}
