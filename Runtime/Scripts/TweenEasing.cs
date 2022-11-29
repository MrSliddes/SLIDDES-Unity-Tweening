using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Tweening
{
    /// <summary>
    /// Holds all easing formulas for tweening
    /// </summary>
    [HelpURL("https://easings.net/")] // reference for how each ease looks visually + credit formulas
    public class TweenEasing
    {
        #region Back

        public static float EaseInBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return c3 * x * x * x - c1 * x * x;
        }

        public static float EaseOutBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return 1 + c3 * (float)Math.Pow(x - 1, 3) + c1 * (float)Math.Pow(x - 1, 2);
        }

        public static float EaseInOutBack(float x)
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            return x < 0.5 ? ((float)Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2 : ((float)Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
        }

        #endregion Back

        #region Bounce

        public static float EaseInBounce(float x)
        {
            return 1 - EaseOutBounce(1 - x);
        }

        public static float EaseOutBounce(float x)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if(x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if(x < 2 / d1)
            {
                return n1 * (x -= 1.5f / d1) * x + 0.75f;
            }
            else if(x < 2.5 / d1)
            {
                return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            }
            else
            {
                return n1 * (x -= 2.625f / d1) * x + 0.984375f;
            }
        }

        public static float EaseInOutBounce(float x)
        {
            return x < 0.5 ? (1 - EaseOutBounce(1 - 2 * x)) / 2 : (1 + EaseOutBounce(2 * x - 1)) / 2;
        }

        #endregion

        #region Circ

        public static float EaseInCirc(float x)
        {
            return 1 - (float)Math.Sqrt(1 - Math.Pow(x, 2));
        }

        public static float EaseOutCirc(float x)
        {
            return (float)Math.Sqrt(1 - Math.Pow(x - 1, 2));
        }

        public static float EaseInOutCirc(float x)
        {
            return x < 0.5  ? (1 - (float)Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2  : ((float)Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;
        }

        #endregion Circ

        #region Cubic

        public static float EaseInCubic(float x)
        {
            return x * x * x;
        }

        public static float EaseOutCubic(float x)
        {
            return 1 - (float)Math.Pow(1 - x, 3);
        }

        public static float EaseInOutCubic(float x)
        {
            return x < 0.5 ? 4 * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 3) / 2;
        }

        #endregion Cubic

        #region Elastic

        public static float EaseInElastic(float x)
        {
            float c4 = (2 * (float)Math.PI) / 3;

            return x == 0 ? 0 : 
                x == 1 ? 1 : (float)(-Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * c4));
        }

        public static float EaseOutElastic(float x)
        {
            float c4 = (2 * (float)Math.PI) / 3;

            return x == 0 ? 0 : 
                x == 1 ? 1 :
                (float)(Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * c4) + 1);
        }

        public static float EaseInOutElastic(float x)
        {
            float c5 = (2 * (float)Math.PI) / 4.5f;

            return x == 0 ? 0 : 
                x == 1 ? 1 : 
                x < 0.5 ? (float)(-(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * c5)) / 2) : (float)((Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * c5)) / 2 + 1);
        }

        #endregion Elastic

        #region Expo

        public static float EaseInExpo(float x)
        {
            return x == 0 ? 0 : (float)Math.Pow(2, 10 * x - 10);
        }

        public static float EaseOutExpo(float x)
        {
            return x == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * x);
        }

        public static float EaseInOutExpo(float x)
        {
            return x == 0 ? 0 : 
                x == 1 ? 1 : 
                x < 0.5 ? (float)Math.Pow(2, 20 * x - 10) / 2 : (2 - (float)Math.Pow(2, -20 * x + 10)) / 2;
        }

        #endregion

        #region Sine

        /// <summary>
        /// Ease In Sine
        /// </summary>
        /// <param name="x">The time of a tween on a scale from 0 to 1</param>
        /// <returns>float between 0 to 1</returns>
        public static float EaseInSine(float x)
        {
            return (float)(1 - Math.Cos((x * Math.PI) / 2));
        }

        public static float EaseOutSine(float x)
        {
            return (float)Math.Sin((x * Math.PI) / 2);
        }

        public static float EaseInOutSine(float x)
        {
            return (float)((-Math.Cos(Math.PI * x) - 1) / 2);
        }

        #endregion Sine

        #region Quart

        public static float EaseInQuart(float x)
        {
            return x * x * x * x;
        }

        public static float EaseOutQuart(float x)
        {
            return 1 - (float)Math.Pow(1 - x, 4);
        }

        public static float EaseInOutQuart(float x)
        {
            return x < 0.5 ? 8 * x * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 4) / 2;
        }

        #endregion Quart

        #region Quad

        public static float EaseInQuad(float x)
        {
            return x * x;
        }

        public static float EaseOutQuad(float x)
        {
            return 1 - (1 - x) * (1 - x);
        }

        public static float EaseInOutQuad(float x)
        {
            return x < 0.5f ? 2 * x * x : 1 - (float)Math.Pow(-2 * x + 2, 2) / 2;
        }

        #endregion Quad

        #region Quint

        public static float EaseInQuint(float x)
        {
            return x * x * x * x * x;
        }

        public static float EaseOutQuint(float x)
        {
            return 1 - (float)Math.Pow(1 - x, 5);
        }

        public static float EaseInOutQuint(float x)
        {
            return x < 0.5 ? 16 * x * x * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 5) / 2;
        }

        #endregion
    }
}
