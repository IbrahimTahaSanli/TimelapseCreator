using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TimelapseCreator
{
    //For key mapping: check key inputs in update
    class KeyMapper: MonoBehaviour
    {
        //Callbacks for key pressed
        public delegate void ButtonPressed(bool isCtrl, bool isShift, bool isAlt, KeyCode code);
        public ButtonPressed ButtonPressedEvent;

        public void Update()
        {
            //Check only when key mapping waiting
            if(ButtonPressedEvent != null)
            {
                //if any key pressed
                if (Input.anyKey)
                {
                    //Check witch button pressed
                    KeyCode code = GetKey();
                    if ( code != KeyCode.None )
                        //Call event if key pushed
                        ButtonPressedEvent(
                            Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl),
                            Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift),
                            Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr),
                            code
                        );
                }
            }
        }

        public KeyCode GetKey()
        {
            foreach(KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                //Codes between 300 and 329 is ctrl, shift , etc 
                //Skip this codes for combination
                if( ((int)code < 300 || (int)code > 329) && Input.GetKey(code))
                    return code;
            }

            return KeyCode.None;
        }

        public void OnDisable()
        {
            Destroy(this.gameObject);
        }
    }
}
