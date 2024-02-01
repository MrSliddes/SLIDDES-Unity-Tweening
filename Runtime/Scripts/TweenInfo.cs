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
        /// <summary>
        /// Is this tweenInfo coupled to a gameobject? (If the gameobject reference is lost aka null, the tween will be removed)
        /// </summary>
        public bool CoupledToGameObject { get; internal set; }
        /// <summary>
        /// Can this tweenInfo be freed from memory? If true it will not run further and be removed
        /// </summary>
        public bool FreeFromMemory { get; internal set; }
        /// <summary>
        /// Has the tweenInfo started?
        /// </summary>
        public bool HasStarted { get; internal set; }
        /// <summary>
        /// Has the tweenInfo completed its tween?
        /// </summary>
        public bool Completed { get; internal set; }
        /// <summary>
        /// Keeps track of how many times this tween has looped
        /// </summary>
        public int LoopCount { get; internal set; }
        /// <summary>
        /// The amount of times this tweenInfo will loop
        /// </summary>
        public int LoopTarget { get; internal set; }
        /// <summary>
        /// Keeps track of how many times this tweenInfo has ping ponged its from / to value
        /// </summary>
        public int PingPongCount { get; internal set; }
        /// <summary>
        /// If > 0 then this tweenInfo, once completed, will go back to the value.from or value.to (depending how many times it already has ping ponged)
        /// </summary>
        public int PingPongTarget { get; internal set; }
        /// <summary>
        /// The start delay time
        /// </summary>
        public float Delay { get; internal set; }
        /// <summary>
        /// Time in seconds to wait before updating tweenInfo
        /// </summary>
        public float DelayTimer { get; internal set; }
        /// <summary>
        /// The deltaTime at which the tweenInfo runs
        /// </summary>
        public float DeltaTime { get; internal set; }
        /// <summary>
        /// The float value of tweenInfo
        /// </summary>
        public float Float => Vector3.x;
        /// <summary>
        /// The start time 
        /// </summary>
        public float Time { get; internal set; }
        /// <summary>
        /// The time left for this tweenInfo before completion
        /// </summary>
        public float Timer { get; internal set; }
        /// <summary>
        /// The current time form the tweenInfo between its start (0) to end (1) time normalized (0 to 1)
        /// </summary>
        public float TimeNormalized { get; internal set; }
        /// <summary>
        /// The current playback of the tween
        /// </summary>
        public TweenPlayback Playback { get; internal set; }
        /// <summary>
        /// The type of tween this tweenInfo is
        /// </summary>
        public Easing Easing { get; internal set; }
        /// <summary>
        /// The difference between to - from
        /// </summary>
        public Vector3 Difference { get; internal set; }
        /// <summary>
        /// The vector2 value of tweenInfo
        /// </summary>
        public Vector2 Vector2 => new Vector2(Vector3.x, Vector3.y);
        /// <summary>
        /// The current teenInfo vector3 value
        /// </summary>
        public Vector3 Vector3 { get; internal set; }
        /// <summary>
        /// The value the tweenInfo started in
        /// </summary>
        public Vector3 From { get; internal set; }
        /// <summary>
        /// The value the tweenInfo needs to be when time <= 0
        /// </summary>
        public Vector3 To { get; internal set; }
        /// <summary>
        /// The associated gameObject of the tweenInfo
        /// </summary>
        public GameObject GameObject
        {
            get
            {
                return gameObject;
            }
            set
            {
                gameObject = value;
                CoupledToGameObject = gameObject != null;
                Transform = gameObject.transform;
            }
        }
        /// <summary>
        /// The transform of gameObject
        /// </summary>
        public Transform Transform { get; internal set; }
        /// <summary>
        /// Delegate for callback when updating tweenInfo
        /// </summary>
        public delegate void UpdateDelegate();

        /// <summary>
        /// Callback when the tweenInfo starts
        /// </summary>
        internal Action<TweenInfo> onStart;
        /// <summary>
        /// Callback when the tweenInfo changes
        /// </summary>
        internal Action<TweenInfo> onChange;
        /// <summary>
        /// Callback when the tweenInfo transition is complete
        /// </summary>
        internal Action<TweenInfo> onComplete;
        /// <summary>
        /// When the tween gets destroyed
        /// </summary>
        internal Action<TweenInfo> onDestroy;
        /// <summary>
        /// The coroutines used for the intervals
        /// </summary>
        internal List<Coroutine> coroutineIntervals = new List<Coroutine>();

        /// <summary>
        /// Is the tweenInfo being forced to complete?
        /// </summary>
        private bool forceComplete;
        /// <summary>
        /// Gameobject reference of the tween
        /// </summary>
        private GameObject gameObject;
        /// <summary>
        /// When the tweenInfo is updated by tweenInfo.Update() this delegate will be called when it needs to update the tweenInfo values
        /// </summary>
        private UpdateDelegate onUpdateValues;

        public TweenInfo() 
        {
            onComplete += x =>
            {
                foreach(Coroutine coroutine in coroutineIntervals)
                {
                    Tween.Instance.StopCoroutine(coroutine);
                }
            };
        }

        /// <summary>
        /// Updates the tweenInfo
        /// </summary>
        /// <returns>True if tweenInfo is finished</returns>
        internal bool UpdateValues()
        {
            if(FreeFromMemory) return true;
            if(Completed) return true;
            if(Playback == TweenPlayback.paused) return false;

            // Start callback
            if(!HasStarted)
            {
                HasStarted = true;
                ResetTimer();

                onStart?.Invoke(this);
            }

            DeltaTime = UnityEngine.Time.deltaTime;

            // Reduce delay
            if(DelayTimer > 0)
            {
                DelayTimer -= DeltaTime;
                return false;
            }

            // If tweenInfo is coupled to gameobject, check if gameobject still exists
            if(CoupledToGameObject && GameObject == null)
            {
                // Free this tweenInfo from memory
                Free();
                return false;
            }

            // Update the delegate 
            if(onUpdateValues != null) onUpdateValues();

            // Only reduce timer if time was set positive
            if(Time > 0) Timer -= DeltaTime;
            // Calculate time normalized
            TimeNormalized = Mathf.Clamp01((Time - Timer) / Time);

            // Check if tweenInfo is not done or when time was set as negative
            if(Timer > 0 || Time < 0)
            {
                // Not done
                onChange?.Invoke(this);
                return false;
            }
            // Done

            // Check if tweenInfo needs to ping pong
            if(PingPongTarget < 0 || PingPongCount < PingPongTarget)
            {
                // Switch values (ping pong)
                onChange?.Invoke(this);
                Vector3 temp = From;
                From = To;
                To = temp;
                ResetTimer();
                PingPongCount = Mathf.Clamp(PingPongCount + 1, 0, int.MaxValue);
                return false;
            }

            // Check if needed to loop
            if(LoopTarget < 0 || LoopCount < LoopTarget)
            {
                // Reset with the loop
                LoopCount = Mathf.Clamp(LoopCount + 1, 0, int.MaxValue);
                ResetForLoop();
                return false;
            }

            SetCompleted();
            return true;
        }

        #region TweenInfo Methods

        /// <summary>
        /// Free the tweenInfo from memory / the update loop
        /// </summary>
        /// <returns></returns>
        public TweenInfo Free()
        {
            FreeFromMemory = true;
            return this;
        }

        /// <summary>
        /// Force the tweenInfo to complete
        /// </summary>
        /// <returns></returns>
        public void ForceComplete()
        {
            forceComplete = true;
            SetCompleted();
        }

        /// <summary>
        /// After completing the tween loop again forever
        /// </summary>
        /// <returns></returns>
        public TweenInfo Loop()
        {
            return Loop(-1);
        }

        /// <summary>
        /// After completing the tween loop again
        /// </summary>
        /// <param name="times">Times to loop</param>
        /// <returns></returns>
        public TweenInfo Loop(int times)
        {
            LoopTarget = times;
            return this;
        }

        /// <summary>
        /// Ping pong the tweenInfo forever between its from / to value
        /// </summary>
        /// <returns></returns>
        public TweenInfo PingPong()
        {
            PingPongTarget = -1;
            return this;
        }

        /// <summary>
        /// Set the times this tweenInfo needs to ping pong between its from value to to value
        /// </summary>
        /// <param name="amount">The amount of times the tween info should ping/pong its value (gets added to total ping pong target value)</param>
        /// <returns></returns>
        public TweenInfo PingPong(int amount)
        {
            amount = Mathf.Clamp(amount, -1, int.MaxValue);
            PingPongTarget = amount;
            return this;
        }

        /// <summary>
        /// Set tweenInfo playback to playing
        /// </summary>
        /// <returns></returns>
        public TweenInfo Play()
        {
            Playback = TweenPlayback.playing;
            return this;
        }

        /// <summary>
        /// Callback when the tweenInfo values gets updated
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TweenInfo OnChange(Action<TweenInfo> action)
        {
            onChange += action;
            return this;
        }

        /// <summary>
        /// Callback when the tweenInfo gets destoryed by Tween
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TweenInfo OnDestroy(Action<TweenInfo> action)
        {
            onDestroy += action;
            return this;
        }

        /// <summary>
        /// Callback when the tweenInfo is completed
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TweenInfo OnComplete(Action<TweenInfo> action)
        {
            onComplete += action;
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
            coroutineIntervals.Add(Tween.Instance.StartCoroutine(Interval(seconds, action)));
            return this;
        }

        /// <summary>
        /// Callback when the tweenInfo is tarted
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TweenInfo OnStart(Action<TweenInfo> action)
        {
            onStart += action;
            return this;
        }

        /// <summary>
        /// Set a delay for the tween to wait before executing
        /// </summary>
        /// <param name="delay">The delay in seconds</param>
        /// <returns>TweenInfo</returns>
        public TweenInfo SetDelay(float delay)
        {
            Delay = delay;
            DelayTimer = delay;
            return this;
        }

        /// <summary>
        /// Set the ease method of the tween
        /// </summary>
        /// <param name="tweenType">What type of tween it is</param>
        /// <returns>TweenInfo</returns>
        public TweenInfo SetEase(Easing easing)
        {
            Easing = easing;
            return this;
        }

        public TweenInfo Stop()
        {
            Playback = TweenPlayback.paused;
            return this;
        }

        #endregion TweenInfo Methods

        #region Internal Methods

        internal Vector3 EasingMethodValue()
        {
            // Based on tweenInfo tweenType, get adjustedTimeNormalized back
            float adjustedTimeNormalized = Easing switch
            {
                Easing.linear => TimeNormalized,

                Easing.easeInBack => TweenEasing.EaseInBack(TimeNormalized),
                Easing.easeOutBack => TweenEasing.EaseOutBack(TimeNormalized),
                Easing.easeInOutBack => TweenEasing.EaseInOutBack(TimeNormalized),

                Easing.easeInBounce => TweenEasing.EaseInBounce(TimeNormalized),
                Easing.easeOutBounce => TweenEasing.EaseOutBounce(TimeNormalized),
                Easing.easeInOutBounce => TweenEasing.EaseInOutBounce(TimeNormalized),

                Easing.easeInCirc => TweenEasing.EaseInCirc(TimeNormalized),
                Easing.easeOutCirc => TweenEasing.EaseOutCirc(TimeNormalized),
                Easing.easeInOutCirc => TweenEasing.EaseInOutCirc(TimeNormalized),

                Easing.easeInCubic => TweenEasing.EaseInCubic(TimeNormalized),
                Easing.easeOutCubic => TweenEasing.EaseOutCubic(TimeNormalized),
                Easing.easeInOutCubic => TweenEasing.EaseInOutCubic(TimeNormalized),

                Easing.easeInElastic => TweenEasing.EaseInElastic(TimeNormalized),
                Easing.easeOutElastic => TweenEasing.EaseOutElastic(TimeNormalized),
                Easing.easeInOutElastic => TweenEasing.EaseInOutElastic(TimeNormalized),

                Easing.easeInExpo => TweenEasing.EaseInExpo(TimeNormalized),
                Easing.easeOutExpo => TweenEasing.EaseOutExpo(TimeNormalized),
                Easing.easeInOutExpo => TweenEasing.EaseInOutExpo(TimeNormalized),

                Easing.easeInSine => TweenEasing.EaseInSine(TimeNormalized),
                Easing.easeOutSine => TweenEasing.EaseOutSine(TimeNormalized),
                Easing.easeInOutSine => TweenEasing.EaseInOutSine(TimeNormalized),

                Easing.easeInQuart => TweenEasing.EaseInQuart(TimeNormalized),
                Easing.easeOutQuart => TweenEasing.EaseOutQuart(TimeNormalized),
                Easing.easeInOutQuart => TweenEasing.EaseInOutQuart(TimeNormalized),

                Easing.easeInQuad => TweenEasing.EaseInQuad(TimeNormalized),
                Easing.easeOutQuad => TweenEasing.EaseOutQuad(TimeNormalized),
                Easing.easeInOutQuad => TweenEasing.EaseInOutQuad(TimeNormalized),

                Easing.easeInQuint => TweenEasing.EaseInQuint(TimeNormalized),
                Easing.easeOutQuint => TweenEasing.EaseOutQuint(TimeNormalized),
                Easing.easeInOutQuint => TweenEasing.EaseInOutQuint(TimeNormalized),

                _ => TimeNormalized
            };

            // Return the current value the tween is on
            return From + (Difference * adjustedTimeNormalized);
        }

        /// <summary>
        /// Reset the timer of the tweenInfo to its inital value
        /// </summary>
        internal void ResetTimer()
        {
            Timer = Time;
            TimeNormalized = 0;
            Difference = To - From;
        }

        internal void ResetForLoop()
        {
            ResetTimer();
            Vector3 = From;
        }

        internal TweenInfo SetFloat()
        {
            onUpdateValues = () => 
            {
                Vector3 v = this.Vector3;
                v.x = EasingMethodValue().x;
                this.Vector3 = v;
            };
            return this;
        }

        internal TweenInfo SetVector2()
        {
            onUpdateValues = () =>
            {
                Vector3 v = EasingMethodValue();
                Vector3 = new Vector3(v.x, v.y, Vector3.z);
            };
            return this;
        }

        internal TweenInfo SetVector3()
        {
            onUpdateValues = () => { Vector3 = EasingMethodValue(); };
            return this;
        }

        internal TweenInfo Move()
        {
            onUpdateValues = () => { Transform.position = EasingMethodValue(); };
            onComplete += x => { Transform.position = To; };
            return this;
        }

        internal TweenInfo MoveX()
        {
            onUpdateValues = () => { Transform.position = new Vector3(EasingMethodValue().x, Transform.position.y, Transform.position.z); };
            onComplete += x => { Transform.position = To; };
            return this;
        }

        internal TweenInfo MoveY()
        {
            onUpdateValues = () => { Transform.position = new Vector3(Transform.position.x, EasingMethodValue().x, Transform.position.z); };
            onComplete += x => { Transform.position = To; };
            return this;
        }

        internal TweenInfo MoveZ()
        {
            onUpdateValues = () => { Transform.position = new Vector3(Transform.position.x, Transform.position.y, EasingMethodValue().z); };
            onComplete += x => { Transform.position = To; };
            return this;
        }

        internal TweenInfo MoveLocal()
        {
            onUpdateValues = () => { Transform.localPosition = EasingMethodValue(); };
            onComplete += x => { Transform.localPosition = To; };
            return this;
        }

        internal TweenInfo MoveLocalX()
        {
            onUpdateValues = () => { Transform.localPosition = new Vector3(EasingMethodValue().x, Transform.localPosition.y, Transform.localPosition.z); };
            onComplete += x => { Transform.localPosition = To; };
            return this;
        }

        internal TweenInfo MoveLocalY()
        {
            onUpdateValues = () => { Transform.localPosition = new Vector3(Transform.localPosition.x, EasingMethodValue().y, Transform.localPosition.z); };
            onComplete += x => { Transform.localPosition = To; };
            return this;
        }

        internal TweenInfo MoveLocalZ()
        {
            onUpdateValues = () => { Transform.localPosition = new Vector3(Transform.localPosition.x, Transform.localPosition.y, EasingMethodValue().z); };
            onComplete += x => { Transform.localPosition = To; };
            return this;
        }

        #endregion

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
                action.Invoke(this);
                yield return new WaitForSeconds(seconds);
            }
        }

        /// <summary>
        /// Called when the tweenInfo is completed
        /// </summary>
        private void SetCompleted()
        {
            // TweenInfo complete
            Completed = true;
            // Make sure values are dead on
            Vector3 = To;
            // Invoke
            // Prevent stack overflow by checking if forceComplete was called (from an onChange callback)
            if(!forceComplete) onChange?.Invoke(this);
            onComplete?.Invoke(this);
        }
    }
}
