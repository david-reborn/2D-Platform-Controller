using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public static class PlayerHelper
    {
        /// <summary>
        /// 检查当前方向上是否可攀爬
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="yAdd"></param>
        /// <returns></returns>
        public static bool ClimbCheck(int dir, int yAdd = 0)
        {
            //检查在关卡范围内
            //if (!this.ClimbBoundsCheck(dir))
            //    return false;

            //且前面两个单元没有ClimbBlock
            //if (ClimbBlocker.Check(base.Scene, this, this.Position + Vector2.UnitY * (float)yAdd + Vector2.UnitX * 2f * (float)this.Facing))
            //    return false;

            //前面两个像素有障碍
            Physics2D.BoxCast(origion, normalHitbox.size, 0, direct, Mathf.Abs(distY), GroundMask);

            if (!base.CollideCheck<Solid>(this.Position + new Vector2((float)(dir * 2), (float)yAdd)))
                return false;

            return true;

        }

    }
}
