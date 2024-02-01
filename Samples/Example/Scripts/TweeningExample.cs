using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLIDDES.Tweening;

namespace SLIDDES.Tweening.Samples
{
    public class TweeningExample : MonoBehaviour
    {
        public GameObject moveable;

        // Start is called before the first frame update
        void Start()
        {
            // Examples commented

            //Tween.Value(0, 1, 1).OnChange(x =>
            //{
            //    //Debug.Log(x.Vector3);
            //}).OnDestroy(x => Debug.LogWarning("This tween got removed :)"));

            //Tween.Move(moveable, Vector3.zero, new Vector3(1, 0, 0), 1).SetDelay(1);

            //Tween.Move(moveable, Vector3.zero, new Vector3(1, 0, 0), 1).SetEase(Easing.easeInOutCubic).SetDelay(2);

            //Tween.MoveLocalY(moveable, 0, 1, 1).SetDelay(3);

            //Tween.Time(1).OnChange(x =>
            //{
            //    if(x.TimeNormalized > 0.5f)
            //    {
            //        //Destroy(moveable);
            //    }
            //}).SetDelay(4);

            //Tween.Value(new Vector3(0, 0, 0), new Vector3(1, 1, 1), 1).OnChange(x =>
            //{
            //    Debug.Log(x.Vector3);
            //});

            //Tween.MoveX(moveable, 0, 1, 1).Loop(2).SetDelay(5);

            Tween.MoveX(moveable, 0, 5, 5).OnChange(x =>
            {
                if(x.TimeNormalized > 0.5f) x.ForceComplete();
            });

            Tween.Time(10).OnInterval(1, x =>
            {
                Debug.Log("1sec Interval");
            }).OnChange(x => 
            {
                if(x.TimeNormalized > 0.5f) x.ForceComplete();
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}