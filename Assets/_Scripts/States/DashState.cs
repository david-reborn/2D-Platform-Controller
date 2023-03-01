using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class DashState : BaseActionState
    {
        private Vector2 DashDir;
        private Vector2 beforeDashSpeed; 
        public DashState(PlayerController context) : base(EActionState.Dash, context)
        {
        }

        public override void OnBegin()
        {
            ctx.WallSlideTimer = Constants.WallSlideTime;

            ctx.DashCooldownTimer = Constants.DashCooldown;
            beforeDashSpeed = ctx.Speed;
            ctx.Speed = Vector2.zero;
            DashDir = Vector2.zero;
            ctx.DashTrailTimer = 0;
        }

        public override void OnEnd()
        {
            //CallDashEvents();
        }

        public override EActionState Update(float deltaTime)
        {
            //Trail
            if (ctx.DashTrailTimer > 0)
            {
                ctx.DashTrailTimer -= deltaTime;
                if (ctx.DashTrailTimer <= 0)
                    CreateTrail();
            }
            //Grab Holdables

            //Super Jump
            if (DashDir.y == 0)
            {
                //Super Jump
                if (ctx.CanUnDuck && Input.Jump.Pressed() && ctx.JumpGraceTimer > 0)
                {
                    ctx.SuperJump();
                    return EActionState.Normal;
                }
            }
            //Super Wall Jump
            if (DashDir.x == 0 && DashDir.y == 1)
            {
                //向上Dash情况下，检测SuperWallJump
                if (Input.Jump.Pressed() && ctx.CanUnDuck)
                {
                    if (ctx.WallJumpCheck(1))
                    {
                        ctx.SuperWallJump(-1);
                        return EActionState.Normal;
                    }
                    else if (ctx.WallJumpCheck(-1))
                    {
                        ctx.SuperWallJump(1);
                        return EActionState.Normal;
                    }
                }
            }
            else
            {
                //Dash状态下执行WallJump，并切换到Normal状态
                if (Input.Jump.Pressed() && ctx.CanUnDuck)
                {
                    if (ctx.WallJumpCheck(1))
                    {
                        ctx.WallJump(-1);
                        return EActionState.Normal;
                    }
                    else if (ctx.WallJumpCheck(-1))
                    {
                        ctx.WallJump(1);
                        return EActionState.Normal;
                    }
                }
            }

            return state;
        }

        public override IEnumerator Coroutine()
        {
            yield return null;
            //
            var dir = ctx.LastAim;
            var newSpeed = dir * Constants.DashSpeed;
            //if (Math.Sign(beforeDashSpeed.x) == Math.Sign(newSpeed.x) && Math.Abs(beforeDashSpeed.x) > Math.Abs(newSpeed.x))
            //    newSpeed.x = beforeDashSpeed.x;
            ctx.Speed = newSpeed;

            DashDir = dir;
            if (DashDir.x != 0)
                ctx.Facing = (Facings)Math.Sign(DashDir.x);

            //TODO Dash Slide

            CreateTrail();
            ctx.DashTrailTimer = .08f;
            yield return Constants.DashTime;
            CreateTrail();
            if (this.DashDir.y >= 0)
            {
                ctx.Speed = DashDir * Constants.EndDashSpeed;
                //ctx.Speed.x *= swapCancel.X;
                //ctx.Speed.y *= swapCancel.Y;
            }
            if (ctx.Speed.y > 0)
                ctx.Speed.y *= Constants.EndDashUpMult;

            this.ctx.SetState((int)EActionState.Normal);
        }

        public override bool IsCoroutine()
        {
            return true;
        }

        private void CreateTrail()
        {
            //SceneEffectManager.instance.Add(ctx, Color.white);
        }
    }
}
