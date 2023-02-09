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
            ctx.Speed.y = Mathf.MoveTowards(ctx.Speed.y, target, Constants.ClimbAccel * deltaTime);

            return state;
        }
    }
}
