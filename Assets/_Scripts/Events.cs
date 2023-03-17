using System;
using System.Collections;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public class EventManager
    {
        //响应蹲下
        public delegate void Duck(bool enable);
        public event Duck OnDuck;

        //下落，速度带方向
        public delegate void Fall(float ySpeed);
        public event Fall OnFall;
        //落地
        public delegate void FallLand(float ySpeed);
        public event FallLand OnFallLand;

        public delegate void Jump();
        public event Jump OnJump;

        public static EventManager instance;

        public static EventManager Get()
        {
            if (instance == null)
            {
                instance = new EventManager();
            }
            return instance;
        }

        public void FireOnDuck(bool enable)
        {
            OnDuck?.Invoke(enable);
        }

        public void FireOnFall(float ySpeed)
        {
            OnFall?.Invoke(ySpeed);
        }

        public void FireOnJump()
        {
            OnJump?.Invoke();
        }
        
        public void FireOnFallLand(float ySpeed)
        {
            OnFallLand?.Invoke(ySpeed);
        }
    }
}