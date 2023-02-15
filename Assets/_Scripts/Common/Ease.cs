using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myd.Common
{
    public static class Ease
    {
        public static readonly Ease.Easer Linear = (Ease.Easer)(t => t);
        public static readonly Ease.Easer SineIn = (Ease.Easer)(t => (float)(-Math.Cos(1.57079637050629 * (double)t) + 1.0));
        public static readonly Ease.Easer SineOut = (Ease.Easer)(t => (float)Math.Sin(1.57079637050629 * (double)t));
        public static readonly Ease.Easer SineInOut = (Ease.Easer)(t => (float)(-Math.Cos(3.14159274101257 * (double)t) / 2.0 + 0.5));
        public static readonly Ease.Easer QuadIn = (Ease.Easer)(t => t * t);
        public static readonly Ease.Easer QuadOut = Ease.Invert(Ease.QuadIn);
        public static readonly Ease.Easer QuadInOut = Ease.Follow(Ease.QuadIn, Ease.QuadOut);
        public static readonly Ease.Easer CubeIn = (Ease.Easer)(t => t * t * t);
        public static readonly Ease.Easer CubeOut = Ease.Invert(Ease.CubeIn);
        public static readonly Ease.Easer CubeInOut = Ease.Follow(Ease.CubeIn, Ease.CubeOut);
        public static readonly Ease.Easer QuintIn = (Ease.Easer)(t => t * t * t * t * t);
        public static readonly Ease.Easer QuintOut = Ease.Invert(Ease.QuintIn);
        public static readonly Ease.Easer QuintInOut = Ease.Follow(Ease.QuintIn, Ease.QuintOut);
        public static readonly Ease.Easer ExpoIn = (Ease.Easer)(t => (float)Math.Pow(2.0, 10.0 * ((double)t - 1.0)));
        public static readonly Ease.Easer ExpoOut = Ease.Invert(Ease.ExpoIn);
        public static readonly Ease.Easer ExpoInOut = Ease.Follow(Ease.ExpoIn, Ease.ExpoOut);
        public static readonly Ease.Easer BackIn = (Ease.Easer)(t => (float)((double)t * (double)t * (2.70158004760742 * (double)t - 1.70158004760742)));
        public static readonly Ease.Easer BackOut = Ease.Invert(Ease.BackIn);
        public static readonly Ease.Easer BackInOut = Ease.Follow(Ease.BackIn, Ease.BackOut);
        public static readonly Ease.Easer BigBackIn = (Ease.Easer)(t => (float)((double)t * (double)t * (4.0 * (double)t - 3.0)));
        public static readonly Ease.Easer BigBackOut = Ease.Invert(Ease.BigBackIn);
        public static readonly Ease.Easer BigBackInOut = Ease.Follow(Ease.BigBackIn, Ease.BigBackOut);
        public static readonly Ease.Easer ElasticIn = (Ease.Easer)(t =>
        {
            float num1 = t * t;
            float num2 = num1 * t;
            return (float)(33.0 * (double)num2 * (double)num1 + -59.0 * (double)num1 * (double)num1 + 32.0 * (double)num2 + -5.0 * (double)num1);
        });
        public static readonly Ease.Easer ElasticOut = (Ease.Easer)(t =>
        {
            float num1 = t * t;
            float num2 = num1 * t;
            return (float)(33.0 * (double)num2 * (double)num1 + -106.0 * (double)num1 * (double)num1 + 126.0 * (double)num2 + -67.0 * (double)num1 + 15.0 * (double)t);
        });
        public static readonly Ease.Easer ElasticInOut = Ease.Follow(Ease.ElasticIn, Ease.ElasticOut);
        public static readonly Ease.Easer BounceIn = (Ease.Easer)(t =>
        {
            t = 1f - t;
            if ((double)t < 0.363636374473572)
                return (float)(1.0 - 121.0 / 16.0 * (double)t * (double)t);
            if ((double)t < 0.727272748947144)
                return (float)(1.0 - (121.0 / 16.0 * ((double)t - 0.545454561710358) * ((double)t - 0.545454561710358) + 0.75));
            return (double)t < 0.909090936183929 ? (float)(1.0 - (121.0 / 16.0 * ((double)t - 0.818181812763214) * ((double)t - 0.818181812763214) + 15.0 / 16.0)) : (float)(1.0 - (121.0 / 16.0 * ((double)t - 0.954545438289642) * ((double)t - 0.954545438289642) + 63.0 / 64.0));
        });
        public static readonly Ease.Easer BounceOut = (Ease.Easer)(t =>
        {
            if ((double)t < 0.363636374473572)
                return 121f / 16f * t * t;
            if ((double)t < 0.727272748947144)
                return (float)(121.0 / 16.0 * ((double)t - 0.545454561710358) * ((double)t - 0.545454561710358) + 0.75);
            return (double)t < 0.909090936183929 ? (float)(121.0 / 16.0 * ((double)t - 0.818181812763214) * ((double)t - 0.818181812763214) + 15.0 / 16.0) : (float)(121.0 / 16.0 * ((double)t - 0.954545438289642) * ((double)t - 0.954545438289642) + 63.0 / 64.0);
        });
        public static readonly Ease.Easer BounceInOut = (Ease.Easer)(t =>
        {
            if ((double)t < 0.5)
            {
                t = (float)(1.0 - (double)t * 2.0);
                if ((double)t < 0.363636374473572)
                    return (float)((1.0 - 121.0 / 16.0 * (double)t * (double)t) / 2.0);
                if ((double)t < 0.727272748947144)
                    return (float)((1.0 - (121.0 / 16.0 * ((double)t - 0.545454561710358) * ((double)t - 0.545454561710358) + 0.75)) / 2.0);
                return (double)t < 0.909090936183929 ? (float)((1.0 - (121.0 / 16.0 * ((double)t - 0.818181812763214) * ((double)t - 0.818181812763214) + 15.0 / 16.0)) / 2.0) : (float)((1.0 - (121.0 / 16.0 * ((double)t - 0.954545438289642) * ((double)t - 0.954545438289642) + 63.0 / 64.0)) / 2.0);
            }
            t = (float)((double)t * 2.0 - 1.0);
            if ((double)t < 0.363636374473572)
                return (float)(121.0 / 16.0 * (double)t * (double)t / 2.0 + 0.5);
            if ((double)t < 0.727272748947144)
                return (float)((121.0 / 16.0 * ((double)t - 0.545454561710358) * ((double)t - 0.545454561710358) + 0.75) / 2.0 + 0.5);
            return (double)t < 0.909090936183929 ? (float)((121.0 / 16.0 * ((double)t - 0.818181812763214) * ((double)t - 0.818181812763214) + 15.0 / 16.0) / 2.0 + 0.5) : (float)((121.0 / 16.0 * ((double)t - 0.954545438289642) * ((double)t - 0.954545438289642) + 63.0 / 64.0) / 2.0 + 0.5);
        });
        private const float B1 = 0.3636364f;
        private const float B2 = 0.7272727f;
        private const float B3 = 0.5454546f;
        private const float B4 = 0.9090909f;
        private const float B5 = 0.8181818f;
        private const float B6 = 0.9545454f;

        public static Ease.Easer Invert(Ease.Easer easer)
        {
            return (Ease.Easer)(t => 1f - easer(1f - t));
        }

        public static Ease.Easer Follow(Ease.Easer first, Ease.Easer second)
        {
            return (Ease.Easer)(t => (double)t > 0.5 ? (float)((double)second((float)((double)t * 2.0 - 1.0)) / 2.0 + 0.5) : first(t * 2f) / 2f);
        }

        public static float UpDown(float eased)
        {
            return (double)eased <= 0.5 ? eased * 2f : (float)(1.0 - ((double)eased - 0.5) * 2.0);
        }

        public delegate float Easer(float t);
    }
}
