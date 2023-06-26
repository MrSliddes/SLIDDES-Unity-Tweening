using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SLIDDES.Tweening
{
    /// <summary>
    /// The information about a tween with all its values, that can be used for chaining
    /// </summary>
    public class TweenInfo
    {
        #region Quick Reference Values

        /// <summary>
        /// The float value of tweenInfo
        /// </summary>
        public float Float => values.vector3.x;
        /// <summary>
        /// The vector2 value of tweenInfo
        /// </summary>
        public Vector2 Vector2 => new Vector2(values.vector3.x, values.vector3.y);
        /// <summary>
        /// The vector3 value of tweenInfo
        /// </summary>
        public Vector3 Vector3 => values.vector3;
        /// <summary>
        /// The gameObject of tweenInfo
        /// </summary>
        public GameObject GameObject => values.gameObject;
        /// <summary>
        /// The transform of tweenInfo
        /// </summary>
        public Transform Transform => values.transform;

        #endregion

        public InternalMethods internalMethods;
        /// <summary>
        /// The values of the tweenInfo.
        /// </summary>
        public Values values;
        /// <summary>
        /// Delegate for callback when updating tweenInfo
        /// </summary>
        public delegate void UpdateDelegate();

        /// <summary>
        /// When the tweenInfo is updated by tweenInfo.Update() this delegate will be called when it needs to update the tweenInfo values
        /// </summary>
        private UpdateDelegate onUpdateValues;
        
        public TweenInfo()
        {
            internalMethods = new InternalMethods(this);
            values = new Values();
            // Stop all coroutines on complete
            values.onComplete += x => 
            { 
                foreach(Coroutine coroutine in values.coroutineIntervals)
                {
                    Tween.Instance.StopCoroutine(coroutine);
                }
            };
        }

        /// <summary>
        /// Updates the tweenInfo
        /// </summary>
        /// <returns>True if tweenInfo is finished</returns>
        public bool UpdateValues()
        {
            if(values.freeFromMemory) return true;
            if(values.hasCompleted) return true;
            if(values.playback == TweenPlayback.paused) return false;

            // Start callback
            if(!values.hasStarted)
            {
                values.hasStarted = true;
                values.timer = values.time;
                values.onStart?.Invoke(this);
            }

            values.deltaTime = Time.deltaTime;

            // Reduce delay
            if(values.delayTimer > 0)
            {
                values.delayTimer -= values.deltaTime;
                return false;
            }

            // Update the delegate 
            if(onUpdateValues != null) onUpdateValues();
            // Only reduce timer if time was set positive
            if(values.time > 0) values.timer -= values.deltaTime;
            values.timeNormalized = (values.time - values.timer) / values.time;
            values.onChange?.Invoke(this);

            // If tweenInfo timer is not done return (or when time was set as negative)
            if(values.timer > 0 || values.time < 0) return false;

            // TweenInfo complete
            values.hasCompleted = true;
            // Make sure values are dead on
            values.vector3 = values.to;
            // Invoke
            values.onChange?.Invoke(this); // Also invoke onChange!
            values.onComplete?.Invoke(this);
            return true;
        }

        #region TweenInfo Methods

        /// <summary>
        /// Free the tweenInfo from memory / the update loop
        /// </summary>
        /// <returns></returns>
        public TweenInfo Free()
        {
            values.freeFromMemory = true;
            return this;
        }

        /// <summary>
        /// Set tweenInfo playback to playing
        /// </summary>
        /// <returns></returns>
        public TweenInfo Play()
        {
            values.playback = TweenPlayback.playing;
            return this;
        }

        /// <summary>
        /// Callback when the tweenInfo values gets updated
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TweenInfo OnChange(Action<TweenInfo> action)
        {
            values.onChange = action;
            return this;
        }

        /// <summary>
        /// Callback when the tweenInfo is completed
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TweenInfo OnComplete(Action<TweenInfo> action)
        {
            values.onComplete = action;
            return this;
        }

        /// <summary>
        /// Set an interval callback
        /// </summary>
        /// <param name="seconds">Seconds to wait before triggering action callback</param>
        /// <param name="repeating">After triggering the action callback should the interval continue?</param>
        /// <param name="gameObject">The gameobject that the interval is active on. (For )</param>
        /// <param name="action">callback</param>
        /// <returns>TweenInfo</returns>
        public TweenInfo OnInterval(float seconds, Action<TweenInfo> action)
        {
            values.coroutineIntervals.Add(Tween.Instance.StartCoroutine(Interval(seconds, action)));
            return this;
        }

        /// <summary>
        /// Callback when the tweenInfo is tarted
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TweenInfo OnStart(Action<TweenInfo> action)
        {
            values.onStart = action;
            return this;
        }

        /// <summary>
        /// Set a delay for the tween to wait before executing
        /// </summary>
        /// <param name="delay">The delay in seconds</param>
        /// <returns>TweenInfo</returns>
        public TweenInfo SetDelay(float delay)
        {
            values.delay = delay;
            values.delayTimer = delay;
            return this;
        }

        /// <summary>
        /// Set the ease method of the tween
        /// </summary>
        /// <param name="tweenType">What type of tween it is</param>
        /// <returns>TweenInfo</returns>
        public TweenInfo SetEase(TweenType tweenType)
        {
            values.type = tweenType;
            return this;
        }

        public TweenInfo Stop()
        {
            values.playback = TweenPlayback.paused;
            return this;
        }

        #endregion TweenInfo Methods


        /// <summary>
        /// Sets an ienumerator interval
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="repeating"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private IEnumerator Interval(float seconds, Action<TweenInfo> action)
        {
            while(true)
            {
                yield return new WaitForSeconds(seconds);
                action.Invoke(this);
            }
        }


        /// <summary>
        /// Methods that shoudn't be accessed, but need to be public for Tween
        /// </summary>
        public class InternalMethods
        {
            private TweenInfo tweenInfo;

            public InternalMethods(TweenInfo tweenInfo)
            {
                this.tweenInfo = tweenInfo;
            }

            public Vector3 EasingMethodValue()
            {
                float timeNormalized = tweenInfo.values.timeNormalized; // shortcut

                // Based on tweenInfo tweenType, get adjustedTimeNormalized back
                float adjustedTimeNormalized = tweenInfo.values.type switch
                {
                    TweenType.linear => timeNormalized,

                    TweenType.easeInBack => TweenEasing.EaseInBack(timeNormalized),
                    TweenType.easeOutBack => TweenEasing.EaseOutBack(timeNormalized),
                    TweenType.easeInOutBack => TweenEasing.EaseInOutBack(timeNormalized),

                    TweenType.easeInBounce => TweenEasing.EaseInBounce(timeNormalized),
                    TweenType.easeOutBounce => TweenEasing.EaseOutBounce(timeNormalized),
                    TweenType.easeInOutBounce => TweenEasing.EaseInOutBounce(timeNormalized),

                    TweenType.easeInCirc => TweenEasing.EaseInCirc(timeNormalized),
                    TweenType.easeOutCirc => TweenEasing.EaseOutCirc(timeNormalized),
                    TweenType.easeInOutCirc => TweenEasing.EaseInOutCirc(timeNormalized),

                    TweenType.easeInCubic => TweenEasing.EaseInCubic(timeNormalized),
                    TweenType.easeOutCubic => TweenEasing.EaseOutCubic(timeNormalized),
                    TweenType.easeInOutCubic => TweenEasing.EaseInOutCubic(timeNormalized),

                    TweenType.easeInElastic => TweenEasing.EaseInElastic(timeNormalized),
                    TweenType.easeOutElastic => TweenEasing.EaseOutElastic(timeNormalized),
                    TweenType.easeInOutElastic => TweenEasing.EaseInOutElastic(timeNormalized),

                    TweenType.easeInExpo => TweenEasing.EaseInExpo(timeNormalized),
                    TweenType.easeOutExpo => TweenEasing.EaseOutExpo(timeNormalized),
                    TweenType.easeInOutExpo => TweenEasing.EaseInOutExpo(timeNormalized),

                    TweenType.easeInSine => TweenEasing.EaseInSine(timeNormalized),
                    TweenType.easeOutSine => TweenEasing.EaseOutSine(timeNormalized),
                    TweenType.easeInOutSine => TweenEasing.EaseInOutSine(timeNormalized),

                    TweenType.easeInQuart => TweenEasing.EaseInQuart(timeNormalized),
                    TweenType.easeOutQuart => TweenEasing.EaseOutQuart(timeNormalized),
                    TweenType.easeInOutQuart => TweenEasing.EaseInOutQuart(timeNormalized),

                    TweenType.easeInQuad => TweenEasing.EaseInQuad(timeNormalized),
                    TweenType.easeOutQuad => TweenEasing.EaseOutQuad(timeNormalized),
                    TweenType.easeInOutQuad => TweenEasing.EaseInOutQuad(timeNormalized),

                    TweenType.easeInQuint => TweenEasing.EaseInQuint(timeNormalized),
                    TweenType.easeOutQuint => TweenEasing.EaseOutQuint(timeNormalized),
                    TweenType.easeInOutQuint => TweenEasing.EaseInOutQuint(timeNormalized),

                    _ => timeNormalized
                };

                // Return the current value the tween is on
                return tweenInfo.values.from + (tweenInfo.values.difference * adjustedTimeNormalized);
            }

            public TweenInfo SetFloat()
            {
                tweenInfo.onUpdateValues = () => { tweenInfo.values.vector3.x = EasingMethodValue().x;  };
                return tweenInfo;
            }

            public TweenInfo SetVector2()
            {
                tweenInfo.onUpdateValues = () =>
                {
                    Vector3 v = EasingMethodValue();
                    tweenInfo.values.vector3.x = v.x;
                    tweenInfo.values.vector3.y = v.y;
                };
                return tweenInfo;
            }

            public TweenInfo SetVector3()
            {
                tweenInfo.onUpdateValues = () => { tweenInfo.values.vector3 = EasingMethodValue(); };
                return tweenInfo;
            }

            public TweenInfo Move()
            {
                tweenInfo.onUpdateValues = () => { tweenInfo.values.transform.position = EasingMethodValue(); };
                tweenInfo.values.onComplete += x => { tweenInfo.values.transform.position = tweenInfo.values.to; };
                return tweenInfo;
            }

            public TweenInfo MoveX()
            {
                tweenInfo.onUpdateValues = () => { tweenInfo.values.transform.position = new Vector3(EasingMethodValue().x, tweenInfo.values.transform.position.y, tweenInfo.values.transform.position.z); };
                tweenInfo.values.onComplete += x => { tweenInfo.values.transform.position = tweenInfo.values.to; };
                return tweenInfo;
            }

            public TweenInfo MoveY()
            {
                tweenInfo.onUpdateValues = () => { tweenInfo.values.transform.position = new Vector3(tweenInfo.values.transform.position.x, EasingMethodValue().x, tweenInfo.values.transform.position.z); };
                tweenInfo.values.onComplete += x => { tweenInfo.values.transform.position = tweenInfo.values.to; };
                return tweenInfo;
            }

            public TweenInfo MoveZ()
            {
                tweenInfo.onUpdateValues = () => { tweenInfo.values.transform.position = new Vector3(tweenInfo.values.transform.position.x, tweenInfo.values.transform.position.y, EasingMethodValue().z); };
                tweenInfo.values.onComplete += x => { tweenInfo.values.transform.position = tweenInfo.values.to; };
                return tweenInfo;
            }

            public TweenInfo MoveLocal()
            {
                tweenInfo.onUpdateValues = () => { tweenInfo.values.transform.localPosition = EasingMethodValue(); };
                tweenInfo.values.onComplete += x => { tweenInfo.values.transform.localPosition = tweenInfo.values.to; };
                return tweenInfo;
            }

            public TweenInfo MoveLocalX()
            {
                tweenInfo.onUpdateValues = () => { tweenInfo.values.transform.localPosition = new Vector3(EasingMethodValue().x, tweenInfo.values.transform.localPosition.y, tweenInfo.values.transform.localPosition.z); };
                tweenInfo.values.onComplete += x => { tweenInfo.values.transform.localPosition = tweenInfo.values.to; };
                return tweenInfo;
            }

            public TweenInfo MoveLocalY()
            {
                tweenInfo.onUpdateValues = () => { tweenInfo.values.transform.localPosition = new Vector3(tweenInfo.values.transform.localPosition.x, EasingMethodValue().y, tweenInfo.values.transform.localPosition.z); };
                tweenInfo.values.onComplete += x => { tweenInfo.values.transform.localPosition = tweenInfo.values.to; };
                return tweenInfo;
            }

            public TweenInfo MoveLocalZ()
            {
                tweenInfo.onUpdateValues = () => { tweenInfo.values.transform.localPosition = new Vector3(tweenInfo.values.transform.localPosition.x, tweenInfo.values.transform.localPosition.y, EasingMethodValue().z); };
                tweenInfo.values.onComplete += x => { tweenInfo.values.transform.localPosition = tweenInfo.values.to; };
                return tweenInfo;
            }
        }

        /// <summary>
        /// Holds all the tweenInfo values that are set / updated
        /// </summary>
        public class Values
        {
            /// <summary>
            /// Can this tweenInfo be freed from memory? If true it will not run further and be removed
            /// </summary>
            public bool freeFromMemory;
            /// <summary>
            /// Has the tweenInfo started?
            /// </summary>
            public bool hasStarted;
            /// <summary>
            /// Has the tweenInfo completed its tween?
            /// </summary>
            public bool hasCompleted;
            /// <summary>
            /// The start delay time
            /// </summary>
            public float delay;
            /// <summary>
            /// Time in seconds to wait before updating tweenInfo
            /// </summary>
            public float delayTimer;
            /// <summary>
            /// The deltaTime at which the tweenInfo runs
            /// </summary>
            public float deltaTime;
            /// <summary>
            /// The start time 
            /// </summary>
            public float time;
            /// <summary>
            /// The time left for this tweenInfo before completion
            /// </summary>
            public float timer;
            /// <summary>
            /// The current time form the tweenInfo between its start (0) to end (1) time normalized (0 to 1)
            /// </summary>
            public float timeNormalized;
            /// <summary>
            /// The current playback of the tween
            /// </summary>
            public TweenPlayback playback;
            /// <summary>
            /// The type of tween this tweenInfo is
            /// </summary>
            public TweenType type;
            /// <summary>
            /// The difference between to - from
            /// </summary>
            public Vector3 difference;
            /// <summary>
            /// The current teenInfo vector3 value
            /// </summary>
            public Vector3 vector3;
            /// <summary>
            /// The value the tweenInfo started in
            /// </summary>
            public Vector3 from;
            /// <summary>
            /// The value the tweenInfo needs to be when time <= 0
            /// </summary>
            public Vector3 to;
            /// <summary>
            /// The associated gameObject of the tweenInfo
            /// </summary>
            public GameObject gameObject;
            /// <summary>
            /// The transform of gameObject
            /// </summary>
            public Transform transform;
            /// <summary>
            /// Callback when the tweenInfo starts
            /// </summary>
            public Action<TweenInfo> onStart;
            /// <summary>
            /// Callback when the tweenInfo changes
            /// </summary>
            public Action<TweenInfo> onChange;
            /// <summary>
            /// Callback when the tweenInfo transition is complete
            /// </summary>
            public Action<TweenInfo> onComplete;
            /// <summary>
            /// The coroutines used for the intervals
            /// </summary>
            public List<Coroutine> coroutineIntervals = new List<Coroutine>();
        }
    }
}
