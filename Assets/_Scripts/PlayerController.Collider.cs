using Myd.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Myd.Platform.Demo
{
    /// <summary>
    /// 记录PlayerController关于碰撞相关功能
    /// 
    /// //每个接触面之间留0.01精度的缝隙.
    /// </summary>
    public partial class PlayerController
    {
        const float STEP = 0.1f;  //碰撞检测步长，对POINT检测用
        const float DEVIATION = 0.02f;  //碰撞检测误差

        private readonly Rect normalHitbox = new Rect(0, -0.25f, 0.8f, 1.1f);
        private readonly Rect duckHitbox = new Rect(0, -0.5f, 0.8f, 0.6f);
        private readonly Rect normalHurtbox = new Rect(0f, -0.15f, 0.8f, 0.9f);
        private readonly Rect duckHurtbox = new Rect(8f, 4f, 0.8f, 0.4f);

        private Rect collider;


        public void AdjustPosition(Vector2 adjust)
        {
            UpdateCollideX(adjust.x);
            UpdateCollideY(adjust.y);
        }

        //碰撞检测
        public bool CollideCheck(Vector2 position, Vector2 dir, float dist = 0)
        {
            Vector2 origion = position + collider.position;
            return Physics2D.OverlapBox(origion + dir * (dist + DEVIATION), collider.size, 0, GroundMask);
        }

        public bool OverlapPoint(Vector2 position)
        {
            return Physics2D.OverlapPoint(position, GroundMask);
        }

        //攀爬检查
        public bool ClimbCheck(int dir, float yAdd = 0)
        {
            //获取当前的碰撞体
            Vector2 origion = this.Position + collider.position;
            if (Physics2D.OverlapBox(origion + Vector2.up * (float)yAdd + Vector2.right * dir * (Constants.ClimbCheckDist * 0.1f + DEVIATION), collider.size, 0, GroundMask))
            {
                return true;
            } 
            return false;
        }

        //根据碰撞调整X轴上的最终移动距离
        protected void UpdateCollideX(float distX)
        {
            Vector2 targetPosition = this.Position;
            //使用校正
            float distance = distX;
            int correctTimes = 1;
            while (true)
            {
                float moved = MoveXStepWithCollide(distance);
                //无碰撞退出循环
                this.Position += Vector2.right * moved;
                if (moved == distance || correctTimes == 0) //无碰撞，且校正次数为0
                {
                    break;
                }
                float tempDist = distance - moved;
                correctTimes--;
                if (!CorrectX(tempDist))
                {
                    this.Speed.x = 0;//未完成校正，则速度清零

                    //Speed retention
                    if (wallSpeedRetentionTimer <= 0)
                    {
                        wallSpeedRetained = this.Speed.x;
                        wallSpeedRetentionTimer = Constants.WallSpeedRetentionTime;
                    }
                    break;
                }
                distance = tempDist;
            }
        }

        protected void UpdateCollideY(float distY)
        {
            Vector2 targetPosition = this.Position;
            //使用校正
            float distance = distY;
            int correctTimes = 1; // int correctTimes = controllerParams.UseCornerCorrection ? 10 : 0;  //默认可以迭代位置10次
            bool collided = true;
            float speedY = Mathf.Abs(this.Speed.y);
            while (true)
            {
                float moved = MoveYStepWithCollide(distance);
                //无碰撞退出循环
                this.Position += Vector2.up * moved;
                if (moved == distance || correctTimes == 0) //无碰撞，且校正次数为0
                {
                    collided = false;
                    break;
                }
                float tempDist = distance - moved;
                correctTimes--;
                if (!CorrectY(tempDist))
                {
                    this.Speed.y = 0;//未完成校正，则速度清零
                    break;
                }
                distance = tempDist;
            }

            //落地时候，进行缩放
            if (collided && distY < 0)
            {
                if (this.stateMachine.State != (int)EActionState.Climb)
                {
                    EventManager.Get().FireOnFallLand(speedY);
                }
            }
        }
        private bool CheckGround()
        {
            return CheckGround(Vector2.zero);
        }

        //针对横向,进行碰撞检测.如果发生碰撞,
        private bool CheckGround(Vector2 offset)
        {
            Vector2 origion = this.Position + collider.position + offset;
            RaycastHit2D hit = Physics2D.BoxCast(origion, collider.size, 0, Vector2.down, DEVIATION, GroundMask);
            if (hit && hit.normal == Vector2.up)
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
            Vector2 origion = this.Position + collider.position;
            RaycastHit2D hit = Physics2D.BoxCast(Position, collider.size, 0, Vector2.right * dir, DEVIATION, GroundMask);
            return hit;
            //if (hit && hit.normal.x == -dir)
            //{

            //}
        }

        public bool SlipCheck(float addY = 0)
        {
            int direct = Facing == Facings.Right ? 1 : -1;
            Vector2 origin = this.Position + collider.position + Vector2.up * collider.size.y / 2f + Vector2.right * direct * (collider.size.x / 2f + STEP);
            Vector2 point1 = origin + Vector2.up * (-0.4f + addY);

            if(Physics2D.OverlapPoint(point1, GroundMask))
            {
                return false;
            }
            Vector2 point2 = origin + Vector2.up * (0.4f + addY);
            if (Physics2D.OverlapPoint(point2, GroundMask))
            {
                return false;
            }
            return true;
        }

        public bool ClimbHopBlockedCheck()
        {
            return false;
        }

        //单步移动，参数和返回值都带方向，表示Y轴
        private float MoveYStepWithCollide(float distY)
        {
            Vector2 moved = Vector2.zero;
            Vector2 direct = Math.Sign(distY) > 0 ? Vector2.up : Vector2.down;
            Vector2 origion = this.Position + collider.position;
            RaycastHit2D hit = Physics2D.BoxCast(origion, collider.size, 0, direct, Mathf.Abs(distY) + DEVIATION, GroundMask);
            if (hit && hit.normal == -direct)
            {
                //如果发生碰撞,则移动距离
                moved += direct * Mathf.Max((hit.distance - DEVIATION), 0);
            }
            else
            {
                moved += Vector2.up * distY;
            }
            return moved.y;
        }

        private float MoveXStepWithCollide(float distX)
        {
            Vector2 moved = Vector2.zero;
            Vector2 direct = Math.Sign(distX) > 0 ? Vector2.right : Vector2.left;
            Vector2 origion = this.Position + collider.position;
            RaycastHit2D hit = Physics2D.BoxCast(origion, collider.size, 0, direct, Mathf.Abs(distX) + DEVIATION, GroundMask);
            if (hit && hit.normal == -direct)
            {
                //如果发生碰撞,则移动距离
                moved += direct * Mathf.Max((hit.distance - DEVIATION), 0);
            }
            else
            {
                moved += Vector2.right * distX;
            }
            return moved.x;
        }

        private bool CorrectX(float distX)
        {
            Vector2 origion = this.Position + collider.position;
            Vector2 direct = Math.Sign(distX) > 0 ? Vector2.right : Vector2.left;

            if ((this.stateMachine.State == (int)EActionState.Dash))
            {
                if (onGround && DuckFreeAt(Position + Vector2.right * distX))
                {
                    Ducking = true;
                    return true;
                }
                else if (this.Speed.y == 0 && this.Speed.x!=0)
                {
                    for (int i = 1; i <= Constants.DashCornerCorrection; i++)
                    {
                        for (int j = 1; j >= -1; j -= 2)
                        {
                            if (!CollideCheck(this.Position + new Vector2(0, j * i * 0.1f), direct, Mathf.Abs(distX)))
                            {
                                this.Position += new Vector2(distX, j * i * 0.1f);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool CorrectY(float distY)
        {
            Vector2 origion = this.Position + collider.position;
            Vector2 direct = Math.Sign(distY) > 0 ? Vector2.up : Vector2.down;
            
            if (this.Speed.y < 0)
            {
                if ((this.stateMachine.State == (int)EActionState.Dash) && !DashStartedOnGround)
                {
                    if (this.Speed.x <= 0)
                    {
                        for (int i = -1; i >= -Constants.DashCornerCorrection; i--)
                        {
                            float step = (Mathf.Abs(i * 0.1f) + DEVIATION);
                            
                            if (!CheckGround(new Vector2(-step, 0)))
                            {
                                this.Position += new Vector2(-step, distY);
                                return true;
                            }
                        }
                    }

                    if (this.Speed.x >= 0)
                    {
                        for (int i = 1; i <= Constants.DashCornerCorrection; i++)
                        {
                            float step = (Mathf.Abs(i * 0.1f) + DEVIATION);
                            if (!CheckGround(new Vector2(step, 0)))
                            {
                                this.Position += new Vector2(step, distY);
                                return true;
                            }
                        }
                    }
                }
            }
            //向上移动
            else if (this.Speed.y > 0)
            {
                //Y轴向上方向的Corner Correction
                {
                    if (this.Speed.x <= 0)
                    {
                        for (int i = 1; i <= Constants.UpwardCornerCorrection; i++)
                        {
                            RaycastHit2D hit = Physics2D.BoxCast(origion + new Vector2(-i * 0.1f, 0), collider.size, 0, direct, Mathf.Abs(distY) + DEVIATION, GroundMask);
                            if (!hit)
                            {
                                this.Position += new Vector2(-i * 0.1f, 0);
                                return true;
                            }
                        }
                    }

                    if (this.Speed.x >= 0)
                    {
                        for (int i = 1; i <= Constants.UpwardCornerCorrection; i++)
                        {
                            RaycastHit2D hit = Physics2D.BoxCast(origion + new Vector2(i * 0.1f, 0), collider.size, 0, direct, Mathf.Abs(distY) + DEVIATION, GroundMask);
                            if (!hit)
                            {
                                this.Position += new Vector2(i * 0.1f, 0);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        //攀爬时，向上吸附
        public bool ClimbUpSnap()
        {
            for (int i = 1; i <= Constants.ClimbUpCheckDist; i++)
            {
                //检测上方是否存在可以攀爬的墙壁，如果存在则瞬移i个像素
                float yOffset = i * 0.1f;
                if (!CollideCheck(this.Position, Vector2.up, yOffset) && ClimbCheck((int)Facing, yOffset + DEVIATION))
                {
                    this.Position += Vector2.up * yOffset;
                    Debug.Log($"======Climb Correct");
                    return true;
                }
            }
            return false;
        }

        //攀爬水平方向上的吸附
        public void ClimbSnap()
        {
            Vector2 origion = this.Position + collider.position;
            Vector2 dir = Vector2.right * (int)this.Facing;
            RaycastHit2D hit = Physics2D.BoxCast(origion, collider.size, 0, dir, Constants.ClimbCheckDist*0.1f + DEVIATION, GroundMask);
            if (hit)
            {
                //如果发生碰撞,则移动距离
                this.Position += dir * Mathf.Max((hit.distance - DEVIATION), 0);
            }
            //for (int i = 0; i < Constants.ClimbCheckDist; i++)
            //{
            //    Vector2 dir = Vector2.right * (int)ctx.Facing;
            //    if (!ctx.CollideCheck(ctx.Position, dir))
            //    {
            //        ctx.AdjustPosition(dir * 0.1f);
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
        }

        //public void MoveExactY(float distY)
        //{
        //    CorrectY(distY);
        //}
    }
}
