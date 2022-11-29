using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Tweening
{
    /// <summary>
    /// The type a tween is
    /// </summary>
    public enum TweenType
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
