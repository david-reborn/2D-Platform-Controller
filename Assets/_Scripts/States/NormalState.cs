﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class NormalState : BaseActionState
    {
        public NormalState(IPlayerContext context):base(EActionState.Normal, context)
        {
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
            //水平面上移动
            float mult = ctx.OnGround ? 1 : Constants.AirMult;
            //计算水平速度
            float max = ctx.Holding == null ? Constants.MaxRun : Constants.HoldingMaxRun;
            Vector2 speed = ctx.Speed;
            if (Math.Abs(speed.x) > max && Math.Sign(speed.x) == this.ctx.MoveX)
            {
                //同方向加速
                speed.x = Mathf.MoveTowards(speed.x, max * this.ctx.MoveX, Constants.RunReduce * mult * Time.deltaTime);
            }
            else
            {
                //反方向减速
                speed.x = Mathf.MoveTowards(speed.x, max * this.ctx.MoveX, Constants.RunAccel * mult * Time.deltaTime);
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
                    float maxY = this.ctx.MaxFall;//最大下落速度
                    //TODO Wall Slide
                    float multY = (Math.Abs(speed.y) < Constants.HalfGravThreshold && (ctx.JumpChecked)) ? .5f : 1f;
                    //空中的情况,需要计算Y轴速度
                    speed.y = Mathf.MoveTowards(speed.y, maxY, Constants.Gravity * multY * deltaTime);
                }

                //处理跳跃
                if (ctx.VarJumpTimer > 0)
                {
                    if (this.ctx.JumpChecked)
                    {
                        //如果按住跳跃，则跳跃速度不受重力影响。
                        speed.y = Math.Max(speed.y, ctx.VarJumpSpeed);
                    }
                    else
                        ctx.VarJumpTimer = 0;
                }
            }
            ctx.Speed = speed;

            if (this.ctx.JumpPressed)
            {
                //土狼时间范围内,允许跳跃
                if (this.ctx.JumpGraceTimer > 0)
                {
                    this.ctx.Jump();
                }
            }

            return state;
        }
    }
}
