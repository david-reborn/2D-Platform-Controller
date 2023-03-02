using System;
using System.Collections;
using UnityEngine;

namespace Myd.Platform.Demo
{
    /// <summary>
    /// Wall Boost组件
    /// 当攀墙时跳跃，如果没有指定X轴方向，则系统自动产生一个推离墙面的力。且给予一定的持续时间
    /// If you climb jump and then do a sideways input within this timer, switch to wall jump
    /// </summary>
    public class WallBoost
    {
        private float timer;
        private int dir;
        private PlayerController controller;
        public float Timer => timer;
        public WallBoost(PlayerController playerController)
        {
            this.controller = playerController;
            this.dir = 0;
            this.ResetTime();
        }

        public void ResetTime()
        {
            this.timer = 0;
        }

        public void Update(float deltaTime)
        {
            if (timer > 0)
            {
                timer -= deltaTime;
                if (controller.MoveX == dir)
                {
                    controller.Speed.x = Constants.WallJumpHSpeed * controller.MoveX;
                    timer = 0;
                }
            }
        }

        //跳跃时，激活
        public void Active()
        {
            if (controller.MoveX == 0)
            {
                Debug.Log("====WallBoost");
                this.dir = -(int)controller.Facing;
                this.timer = Constants.ClimbJumpBoostTime;
            }
        }
    }
}