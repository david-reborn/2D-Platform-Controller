using System;
using System.Collections;
using UnityEngine;

namespace Myd.Platform.Demo
{
    /// <summary>
    /// 沿着墙壁下滑组件
    /// </summary>
    public class WallSlide
    {
        private float timer;
        private int dir;

        private PlayerController controller;
        public float Timer => timer;
        public int Direct => dir;

        public WallSlide(PlayerController playerController)
        {
            this.controller = playerController;
            this.dir = 0;
            this.ResetTime();
        }

        public void ResetTime()
        {
            this.timer = Constants.WallSlideTime;
        }

        public void Update(float deltaTime)
        {
            if (this.dir == 0)
                return;
            this.timer = Math.Max(this.timer - deltaTime, 0);
            this.dir = 0;
        }

        public void Check(bool onGround, bool isClimbState)
        {
            if (!onGround)
                return;
            if (isClimbState)
                return;
            this.timer = Constants.WallSlideTime;
        }

        public float AdjustMaxFall(float maxFall)
        {
            float max = maxFall;
            if ((controller.MoveX == (int)controller.Facing || (controller.MoveX == 0 && Input.Grab.Checked())) && controller.MoveY != -1)
            {
                //判断是否向下做Wall滑行
                if (controller.Speed.y <= 0 && timer > 0 && controller.ClimbBoundsCheck((int)controller.Facing) && controller.CollideCheck(controller.Position, Vector2.right * (int)controller.Facing) && controller.CanUnDuck)
                {
                    controller.Ducking = false;
                    this.dir = (int)controller.Facing;
                }

                if (this.dir != 0)
                {
                    //if (ctx.WallSlideTimer > Constants.WallSlideTime * 0.5f && ClimbBlocker.Check(level, this, Position + Vector2.UnitX * wallSlideDir))
                    //    ctx.WallSlideTimer = Constants.WallSlideTime * .5f;

                    max = Mathf.Lerp(Constants.MaxFall, Constants.WallSlideStartMax, timer / Constants.WallSlideTime);
                    if ((timer / Constants.WallSlideTime) > .65f)
                    {
                        //TODO 播放特效
                        //CreateWallSlideParticles(wallSlideDir);
                    }
                }
            }
            return max;
        }
    }
}