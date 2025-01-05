using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace N2GStudio.SportsGirl
{
    public class DemoSceneMain : MonoBehaviour
    {
        public Text description;

        public PlayerControl playerControl;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnClickButton(int _actionType)
        {
            if ((ACTION_TYPE)_actionType == ACTION_TYPE.QuickStep)
            {
                playerControl.transform.position = new Vector3(-1.0f, 0.0f, 0.0f);
            }
            else
            {
                playerControl.transform.position = Vector3.zero;
            }
            description.text = ((ACTION_TYPE)_actionType).ToString();
            playerControl.SetAction((ACTION_TYPE)_actionType);

            
        }
    }

}