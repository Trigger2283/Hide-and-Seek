using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N2GStudio.SportsGirl
{

    public class PlayerControl : MonoBehaviour
    {
        public Animator characterAninator;

        // Start is called before the first frame update
        void Start()
        {

        }



        public void SetAction(ACTION_TYPE _actionType)
        {
            characterAninator.Play(_actionType.ToString(), 0, 0f);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnEndAnimation(int _actionType)
        {

        }
    }


    public enum ACTION_TYPE
    {
        None,
        Idle,
        Walking,
        Jogging,
        Running,
        FastRunning,
        Squat,
        Jump,
        JumpingJack,
        QuickStep,
        Pushup,
        Waving
    }
}

