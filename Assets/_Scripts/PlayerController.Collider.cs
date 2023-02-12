using Myd.Common;
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
        const float DEVIATION = 0.02f;  //碰撞检测误差

        private readonly Rect normalHitbox = new Rect(0, -0.25f, 0.8f, 1.1f);
        private readonly Rect duckHitbox = new Rect(0, -0.5f, 0.8f, 0.6f);
        private readonly Rect normalHurtbox = new Rect(0f, -0.15f, 0.8f, 0.9f);
        private readonly Rect duckHurtbox = new Rect(8f, 4f, 0.8f, 0.4f);

        private Rect hitbox;

        //碰撞检测
        public bool CollideCheck(Vector2 position, Vector2 dir, float dist = 0)
        {
            return Physics2D.OverlapBox(position + dir * (dist + DEVIATION), normalHitbox.size, 0, GroundMask);
        }

        public bool CollideCheck(Vector2 position)
        {
            return Physics2D.OverlapPoint(position, GroundMask);
        }

        //攀爬检查
        public bool ClimbCheck(int dir, int yAdd = 0)
        {
            //检查在关卡范围内
            //if (!this.ClimbBoundsCheck(dir))
            //    return false;

            //且前面两个单元没有ClimbBlock
            //if (ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitY * (float)yAdd + Vector2.UnitX * 2f * (float)this.Facing))
            //    return false;

            //获取当前的碰撞体
            if (Physics2D.OverlapBox(this.Position + Vector2.up * (float)yAdd + Vector2.right * dir * DEVIATION , normalHitbox.size, 0, GroundMask))
            {
                return true;
            }
            return false;
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

            RaycastHit2D hit = Physics2D.BoxCast(origion, normalHitbox.size, 0, direct, Mathf.Abs(distX) + DEVIATION, GroundMask);
            if (hit)
            {
                //如果发生碰撞,则移动距离
                targetPosition += direct * (hit.distance - DEVIATION);
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
            RaycastHit2D hit = Physics2D.BoxCast(origion, normalHitbox.size, 0, direct, Mathf.Abs(distY) + DEVIATION, GroundMask);
            if (hit && hit.normal == -direct)
            {
                //如果发生碰撞,则移动距离
                targetPosition += direct * (hit.distance - DEVIATION);
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
            Vector2 origion = this.Position + normalHitbox.position;
            Collider2D hit = Physics2D.OverlapBox(origion + Vector2.down * DEVIATION, normalHitbox.size, 0, GroundMask);
            if (hit)
            {
                return true;
            }
            return false;
        }

        //根据整个关卡的边缘框进行检测,确保人物在关卡的框内.
        public bool ClimbBoundsCheck(int dir)
        {
            return true;
            //return base.Left + (float)(dir * 2) >= (float)this.level.Bounds.Left && base.Right + (float)(dir * 2) < (float)this.level.Bounds.Right;
        }

        //墙壁上跳检测
        public bool WallJumpCheck(int dir)
        {
            return ClimbBoundsCheck(dir) && this.CollideCheck(Position, Vector2.right * dir, Constants.WallJumpCheckDist);
        }

        public RaycastHit2D ClimbHopSolid { get; set; }
        public RaycastHit2D CollideClimbHop(int dir)
        {
            Vector2 origion = this.Position + normalHitbox.position;
            RaycastHit2D hit = Physics2D.BoxCast(Position, normalHitbox.size, 0, Vector2.right * dir, DEVIATION, GroundMask);
            return hit;
            //if (hit && hit.normal.x == -dir)
            //{

            //}
        }

        public bool SlipCheck(float addY = 0)
        {
            Vector2 origion = this.Position + normalHitbox.position + Vector2.up * (0.25f + addY);
            return !Physics2D.OverlapBox(origion + Vector2.right * (int)this.Facing * DEVIATION, new Vector2(0.8f, 0.5f), 0, GroundMask );
        }

        public bool ClimbHopBlockedCheck()
        {
            return true;
        }
    }
}
