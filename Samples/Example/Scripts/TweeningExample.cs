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
            Tween.Value(0, 1, 1).OnChange(x =>
            {
                Debug.Log(x.values.vector3.x);
            }).SetEase(TweenType.easeInElastic);

            Tween.Move(moveable, Vector3.zero, new Vector3(1, 0, 0), 2).SetEase(TweenType.easeInOutBounce).SetDelay(2);

            TweenInfo tweenInfo = Tween.Time(2).OnComplete(x => Debug.Log("Done A"));
            tweenInfo.Free(); // interrupt Done A
            tweenInfo = Tween.Time(2).OnComplete(x => Debug.Log("Done B"));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}