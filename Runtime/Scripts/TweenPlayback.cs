using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Tweening
{
    public enum TweenPlayback
    {
        [Tooltip("The tween gets updated")]
        playing,
        [Tooltip("The tween does not get updated")]
        paused,
        [Tooltip("The tween values are reset to start values")]
        reset,
        [Tooltip("The tween skips to the end of its values")]
        end
    }
}
