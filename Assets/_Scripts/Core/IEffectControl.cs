using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Myd.Platform.Core
{
    /// <summary>
    /// 特效控制器，用于和外部实现解耦
    /// </summary>
    public interface IEffectControl
    {
        void DashLine(Vector3 position, Vector2 dir);

        void Ripple(Vector3 position);

        void CameraShake(Vector2 dir);

        void JumpDust(Vector3 position);

        void LandDust(Vector3 position);

        void SpeedRing(Vector3 position, Vector2 dir);
        //顿帧
        void Freeze(float time);
    }
}
