using System;
using System.Collections.Generic;
using UnityEngine;

namespace Myd.Platform.Demo
{
    /// <summary>
    /// 记录PlayerController关于碰撞相关功能
    /// 
    /// //每个接触面之间留0.01精度的缝隙.
    /// </summary>
    public partial class PlayerController
    {

        private readonly Rect normalHitbox = new Rect(0, -0.25f, 0.8f, 1.1f);
        private readonly Rect duckHitbox = new Rect(0, -0.5f, 0.8f, 0.6f);
        private readonly Rect normalHurtbox = new Rect(0f, -0.15f, 0.8f, 0.9f);
        private readonly Rect duckHurtbox = new Rect(8f, 4f, 0.8f, 0.4f);

        private Rect hitbox;

        public bool CollideCheck(Vector2 position)
        {
            return Physics2D.OverlapBox(position, normalHitbox.size, 0, GroundMask);
        }

        public bool ClimbCheck(int dir, int yAdd = 0)
        {
            //检查在关卡范围内
            //if (!this.ClimbBoundsCheck(dir))
            //    return false;

            //且前面两个单元没有ClimbBlock
            //if (ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitY * (float)yAdd + Vector2.UnitX * 2f * (float)this.Facing))
            //    return false;

            //获取当前的碰撞体
            if (Physics2D.OverlapBox(this.Position + Vector2.up * (float)yAdd + Vector2.right * 2f * dir * 0.01f, normalHitbox.size, 0, GroundMask))
            {
                return true;
            }

            return true;
        }

        //根据碰撞调整X轴上的最终移动距离
        protected void UpdateCollideX(float distX)
        {
            if (distX == 0)
                return;
            //目标位置
            Vector2 direct = Math.Sign(distX) > 0 ? Vector2.right : Vector2.left;
            Vector2 targetPosition = this.Position;

            Vector2 origion = this.Position + normalHitbox.position;

            RaycastHit2D hit = Physics2D.BoxCast(origion, normalHitbox.size, 0, direct, Mathf.Abs(distX) + 0.01f, GroundMask);
            if (hit)
            {
                //如果发生碰撞,则移动距离
                targetPosition += direct * (hit.distance - 0.01f);
                //Speed retention
                //if (wallSpeedRetentionTimer <= 0)
                //{
                //    wallSpeedRetained = this.speed.x;
                //    wallSpeedRetentionTimer = Constants.WallSpeedRetentionTime;
                //}
                this.Speed.x = 0;
            }
            else
            {
                targetPosition += Vector2.right * distX;
            }
            this.Position = targetPosition;
        }

        protected void UpdateCollideY(float distY)
        {
            Vector2 targetPosition = this.Position;
            Vector2 direct = Math.Sign(distY) > 0 ? Vector2.up : Vector2.down;
            Vector2 origion = this.Position + normalHitbox.position;
            RaycastHit2D hit = Physics2D.BoxCast(origion, normalHitbox.size, 0, direct, Mathf.Abs(distY) + 0.01f, GroundMask);
            if (hit && hit.normal == -direct)
            {
                //如果发生碰撞,则移动距离
                targetPosition += direct * (hit.distance - 0.01f);
            }
            else
            {
                targetPosition += Vector2.up * distY;
            }
            this.Position = targetPosition;
        }

        //针对横向,进行碰撞检测.如果发生碰撞,
        private bool CheckGround()
        {
            //Vector2 origion = this.Position + normalHitbox.position;
            //RaycastHit2D hit = Physics2D.Over(origion, normalHitbox.size, 0, Vector2.down, 0.01f, GroundMask);
            //if (hit && hit.normal == Vector2.up)
            //{
            //    return true;
            //}
            //return false;
            return true;
        }
    }
}
