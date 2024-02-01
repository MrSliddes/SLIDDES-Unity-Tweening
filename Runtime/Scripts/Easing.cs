using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Tweening
{
    /// <summary>
    /// What easing is applied to the tween?
    /// </summary>
    public enum Easing
    {
        linear, // the default

        easeInBack,
        easeOutBack,
        easeInOutBack,

        easeInBounce,
        easeOutBounce,
        easeInOutBounce,

        easeInCirc,
        easeOutCirc,
        easeInOutCirc,

        easeInCubic,
        easeOutCubic,
        easeInOutCubic,

        easeInElastic,
        easeOutElastic,
        easeInOutElastic,

        easeInExpo,
        easeOutExpo,
        easeInOutExpo,

        easeInSine,
        easeOutSine,
        easeInOutSine,

        easeInQuart,
        easeOutQuart,
        easeInOutQuart,

        easeInQuad,
        easeOutQuad,
        easeInOutQuad,

        easeInQuint,
        easeOutQuint,
        easeInOutQuint
    }
}
