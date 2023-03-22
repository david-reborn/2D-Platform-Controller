using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Myd.Platform
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
            ctx.WallSlide?.ResetTime();
            ctx.DashCooldownTimer = Constants.DashCooldown;
            beforeDashSpeed = ctx.Speed;
            ctx.Speed = Vector2.zero;
            DashDir = Vector2.zero;
            ctx.DashTrailTimer = 0;
            ctx.DashStartedOnGround = ctx.OnGround;
            //顿帧
            ctx.EffectControl.Freeze(0.05f);
        }

        public override void OnEnd()
        {
            //CallDashEvents();
            ctx.PlayDashFluxEffect(DashDir, false);
        }

        public override EActionState Update(float deltaTime)
        {
            //Trail
            if (ctx.DashTrailTimer > 0)
            {
                ctx.DashTrailTimer -= deltaTime;
                if (ctx.DashTrailTimer <= 0)
                    ctx.PlayTrailEffect((int)ctx.Facing);
            }
            //Grab Holdables
            //Super Jump
            if (DashDir.y == 0)
            {
                //Super Jump
                if (ctx.CanUnDuck && GameInput.Jump.Pressed() && ctx.JumpCheck.AllowJump())
                {
                    ctx.SuperJump();
                    return EActionState.Normal;
                }
            }
            //Super Wall Jump
            if (DashDir.x == 0 && DashDir.y == 1)
            {
                //向上Dash情况下，检测SuperWallJump
                if (GameInput.Jump.Pressed() && ctx.CanUnDuck)
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
                if (GameInput.Jump.Pressed() && ctx.CanUnDuck)
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
            //惯性
            if (Math.Sign(beforeDashSpeed.x) == Math.Sign(newSpeed.x) && Math.Abs(beforeDashSpeed.x) > Math.Abs(newSpeed.x))
            {
                newSpeed.x = beforeDashSpeed.x;
            }
            ctx.Speed = newSpeed;

            DashDir = dir;
            if (DashDir.x != 0)
                ctx.Facing = (Facings)Math.Sign(DashDir.x);

            ctx.PlayDashFluxEffect(DashDir, true);
            //TODO Dash Slide

            ctx.PlayDashEffect(ctx.Position, dir);
            ctx.SpriteControl.Slash(true);
            ctx.PlayTrailEffect((int)ctx.Facing);
            ctx.DashTrailTimer = .08f;
            yield return Constants.DashTime;

            ctx.SpriteControl.Slash(false);
            ctx.PlayTrailEffect((int)ctx.Facing);
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
    }
}
