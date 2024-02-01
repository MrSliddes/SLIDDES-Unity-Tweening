using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SLIDDES.Tweening
{
    /// <summary>
    /// For tweening between values.
    /// </summary>
    /// <remarks>
    /// The methods are executed in order from left to right!
    /// </remarks>
    /// <example>
    /// Tween.SetDelay(1).SetFloat(); will wait 1 second first before setting the float method
    /// </example>
    public class Tween : MonoBehaviour
    {
        /// <summary>
        /// The singular instance of the Tween class
        /// </summary>
        public static Tween Instance
        {
            get 
            { 
                if(instance == null)
                {
                    instance = new GameObject("[Tween]").AddComponent<Tween>();                    
                }
                return instance;
            }
        }

        private static Tween instance;

        // Tween instance values
        /// <summary>
        /// All tweens indexes that need to be removed at end of UpdateTweens()
        /// </summary>
        private List<int> indexesToRemove = new List<int>();
        /// <summary>
        /// List containing all active tweens
        /// </summary>
        private List<TweenInfo> tweens = new List<TweenInfo>();

        private void Update()
        {
            UpdateTweens();
        }

        /// <summary>
        /// Update all the tweeninfo's in tweens
        /// </summary>
        public static void UpdateTweens()
        {
            // Update
            for(int i = 0; i < Instance.tweens.Count; i++)
            {
                TweenInfo tweenInfo = Instance.tweens[i];
                if(tweenInfo == null || tweenInfo.UpdateValues())
                {
                    Instance.indexesToRemove.Add(i);
                }
            }

            // Remove completed tweens (reverse)
            for(int i = Instance.indexesToRemove.Count - 1; i >= 0; i--)
            {
                Instance.tweens[i].onDestroy?.Invoke(Instance.tweens[i]);
                Instance.tweens.RemoveAt(Instance.indexesToRemove[i]);
            }
            Instance.indexesToRemove.Clear();
        }

        #region TweenInfo Methods

        /// <summary>
        /// Trigger an action every x seconds.
        /// </summary>
        /// <param name="seconds">Seconds to wait before triggering the action</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TweenInfo Interval(float seconds, Action<TweenInfo> action)
        {
            TweenInfo t = new TweenInfo();
            t.Time = -1;
            t.OnInterval(seconds, action);
            AddTweenInfo(t);
            return t;
        }

        /// <summary>
        /// Move an gameobject from 1 position to the next
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static TweenInfo Move(GameObject gameObject, Vector3 from, Vector3 to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.GameObject = gameObject;
            t.From = from;
            t.To = to;
            t.Time = time;
            AddTweenInfo(t);
            return t.Move();
        }

        public static TweenInfo MoveX(GameObject gameObject, float from, float to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.GameObject = gameObject;
            t.From = new Vector3(from, 0, 0);
            t.To = new Vector3(to, 0, 0);
            t.Time = time;
            AddTweenInfo(t);
            return t.MoveX();
        }

        public static TweenInfo MoveY(GameObject gameObject, float from, float to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.GameObject = gameObject;
            t.From = new Vector3(0, from, 0);
            t.To = new Vector3(0, to, 0);
            t.Time = time;
            AddTweenInfo(t);
            return t.MoveY();
        }

        public static TweenInfo MoveZ(GameObject gameObject, float from, float to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.GameObject = gameObject;
            t.From = new Vector3(0, 0, from);
            t.To = new Vector3(0, 0, to);
            t.Time = time;
            AddTweenInfo(t);
            return t.MoveZ();
        }

        public static TweenInfo MoveLocal(GameObject gameObject, Vector3 from, Vector3 to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.GameObject = gameObject;
            t.From = from;
            t.To = to;
            t.Time = time;
            AddTweenInfo(t);
            return t.MoveLocal();
        }

        public static TweenInfo MoveLocalX(GameObject gameObject, float from, float to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.GameObject = gameObject;
            t.From = new Vector3(from, 0, 0);
            t.To = new Vector3(to, 0, 0);
            t.Time = time;
            AddTweenInfo(t);
            return t.MoveLocalX();
        }

        public static TweenInfo MoveLocalY(GameObject gameObject, float from, float to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.GameObject = gameObject;
            t.From = new Vector3(0, from, 0);
            t.To = new Vector3(0, to, 0);
            t.Time = time;
            AddTweenInfo(t);
            return t.MoveLocalY();
        }

        public static TweenInfo MoveLocalZ(GameObject gameObject, float from, float to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.GameObject = gameObject;
            t.From = new Vector3(0, 0, from);
            t.To = new Vector3(0, 0, to);
            t.Time = time;
            AddTweenInfo(t);
            return t.MoveLocalZ();
        }

        public static TweenInfo Move(Transform transform, Vector3 from, Vector3 to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.Transform = transform;
            t.From = from;
            t.To = to;
            t.Time = time;
            AddTweenInfo(t);
            return t.Move();
        }

        public static TweenInfo MoveLocal(Transform transform, Vector3 from, Vector3 to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.Transform = transform;
            t.From = from;
            t.To = to;
            t.Time = time;
            AddTweenInfo(t);
            return t.MoveLocal();
        }

        /// <summary>
        /// For when the tween only needs to run a certain time and you want to use your own methods with the tweenInfo callbacks
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static TweenInfo Time(float time)
        {
            TweenInfo t = new TweenInfo();
            t.Time = time;
            AddTweenInfo(t);
            return t;
        }

        public static TweenInfo Value(int from, float to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.From = new Vector3(from, 0, 0);
            t.To = new Vector3(to, 0, 0);
            t.Time = time;
            AddTweenInfo(t);
            return t.SetFloat();
        }

        public static TweenInfo Value(float from, float to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.From = new Vector3(from, 0, 0);
            t.To = new Vector3(to, 0, 0);
            t.Time = time;
            AddTweenInfo(t);
            return t.SetFloat();
        }

        public static TweenInfo Value(Vector2 from, Vector3 to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.From = from;
            t.To = to;
            t.Time = time;
            AddTweenInfo(t);
            return t.SetVector2();         
        }

        public static TweenInfo Value(Vector3 from, Vector3 to, float time)
        {
            TweenInfo t = new TweenInfo();
            t.From= from; 
            t.To = to;
            t.Time = time;
            AddTweenInfo(t);
            return t.SetVector3();
        }
                
        #endregion

        private static void AddTweenInfo(TweenInfo tweenInfo)
        {
            Instance.tweens.Add(tweenInfo);
        }
    }
}
