using Myd.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class NormalState : BaseActionState
    {
        public NormalState(PlayerController context):base(EActionState.Normal, context)
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
            this.ctx.WallSpeedRetentionTimer = 0;
        }

        public override EActionState Update(float deltaTime)
        {
            //Climb
            if (Input.Grab.Checked() && !ctx.IsTired && !ctx.Ducking)
            {
                //Grabbing Holdables
                //foreach (Holdable hold in Scene.Tracker.GetComponents<Holdable>())
                //    if (hold.Check(this) && Pickup(hold))
                //        return StPickup;

                //Climbing
                if (ctx.Speed.y <= 0 && Math.Sign(ctx.Speed.x) != -(int)ctx.Facing)
                {
                    if (ctx.ClimbCheck((int)ctx.Facing))
                    {
                        ctx.Ducking = false;
                        return EActionState.Climb;
                    }
                    //TODO 考虑风场
                    //if (Input.MoveY < 1 && level.Wind.Y <= 0)
                    //{
                    //    for (int i = 1; i <= ClimbUpCheckDist; i++)
                    //    {
                    //        if (!CollideCheck<Solid>(Position + Vector2.UnitY * -i) && ClimbCheck((int)Facing, -i))
                    //        {
                    //            MoveVExact(-i);
                    //            Ducking = false;
                    //            return StClimb;
                    //        }
                    //    }
                    //}
                }
            }

            //Dashing
            if (this.ctx.CanDash)
            {
                //Speed += LiftBoost;
                return this.ctx.Dash();
            }

            //水平面上移动
            if (ctx.Ducking && ctx.OnGround)
            {

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
                    float maxFallSpeed = this.ctx.MaxFall;
                    float fastMaxFallSpeed = Constants.FastMaxFall;

                    //if (this.ctx.MoveY == -1 && speed.y <= maxFallSpeed)
                    //{
                    //    this.ctx.MaxFall = Mathf.MoveTowards(this.ctx.MaxFall, fastMaxFallSpeed, Constants.FastMaxAccel * deltaTime);

                    //    float half = maxFallSpeed + (fastMaxFallSpeed - maxFallSpeed) * .5f;
                    //    if (speed.y >= half)
                    //    {
                    //        float spriteLerp = Math.Min(1f, (speed.y - half) / (fastMaxFallSpeed - half));
                    //        //Sprite.Scale.X = Mathf.Lerp(1f, .5f, spriteLerp);
                    //        //Sprite.Scale.Y = Mathf.Lerp(1f, 1.5f, spriteLerp);
                    //    }
                    //}
                    //else
                    {
                        this.ctx.MaxFall = Mathf.MoveTowards(this.ctx.MaxFall, maxFallSpeed, Constants.FastMaxAccel * deltaTime);
                    }
                }

                if (!ctx.OnGround)
                {
                    float max = this.ctx.MaxFall;//最大下落速度
                    //Wall Slide
                    if ((ctx.MoveX == (int)ctx.Facing || (ctx.MoveX == 0 && Input.Grab.Checked())) && ctx.MoveY != 1)
                    {
                        //判断是否向下做Wall滑行
                        if (ctx.Speed.y <= 0 && ctx.WallSlideTimer > 0 && ctx.ClimbBoundsCheck((int)ctx.Facing) && ctx.CollideCheck(ctx.Position, Vector2.right * (int)ctx.Facing) && ctx.CanUnDuck())
                        {
                            ctx.Ducking = false;
                            ctx.WallSlideDir = (int)ctx.Facing;
                        }

                        if (ctx.WallSlideDir != 0)
                        {
                            //if (ctx.WallSlideTimer > Constants.WallSlideTime * 0.5f && ClimbBlocker.Check(level, this, Position + Vector2.UnitX * wallSlideDir))
                            //    ctx.WallSlideTimer = Constants.WallSlideTime * .5f;

                            max = Mathf.Lerp(ctx.MaxFall, Constants.WallSlideStartMax, ctx.WallSlideTimer / Constants.WallSlideTime);
                            if ((ctx.WallSlideTimer / Constants.WallSlideTime) > .65f)
                            {
                                //TODO 播放特效
                                //CreateWallSlideParticles(wallSlideDir);
                            }
                        }
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
                if (this.ctx.JumpGraceTimer > 0)
                {
                    this.ctx.Jump();
                }else if (ctx.CanUnDuck())
                {
                    if (ctx.WallJumpCheck(1))
                    {
                        if (ctx.Facing == Facings.Right && Input.Grab.Checked())// && Stamina > 0 && Holding == null && !ClimbBlocker.Check(Scene, this, Position + Vector2.UnitX * WallJumpCheckDist))
                            ctx.ClimbJump();
                        //else if (DashAttacking && DashDir.X == 0 && DashDir.Y == -1)
                        //    SuperWallJump(-1);
                        else
                            ctx.WallJump(-1);
                    }
                    //else if (ctx.WallJumpCheck(-1))
                    //{
                    //    if (Facing == Facings.Left && Input.Grab.Check && Stamina > 0 && Holding == null && !ClimbBlocker.Check(Scene, this, Position + Vector2.UnitX * -WallJumpCheckDist))
                    //        ClimbJump();
                    //    else if (DashAttacking && DashDir.X == 0 && DashDir.Y == -1)
                    //        SuperWallJump(1);
                    //    else
                    //        WallJump(1);
                    //}
                }
            }

            return state;
        }
    }


}
