using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TimelapseCreator
{
    public class ConfigDTO
    {
        private readonly string INTERVALKEY = "TimelapseCreatorIntervalKey";
        private readonly string IMAGESAVEPATH = "TimelapseCreatorFileSavePath";
        private readonly string CAMERAPOSITIONKEY = "TimelapseCreatorCameraPositionKey";
        private readonly string STARTKEY = "TimelapseCreatorStartKey";


        public class KeyCombination
        {
            public bool isCtrl;
            public bool isShift;
            public bool isAlt;
            public KeyCode key;

            public KeyCombination(bool isCtrl, bool isShift, bool isAlt, KeyCode key)
            {
                this.isCtrl = isCtrl;
                this.isShift = isShift;
                this.isAlt = isAlt;
                this.key = key;
            }

            public KeyCombination(string save)
            {
                string[] splitted = save.Split('/');
                this.isCtrl = bool.Parse(splitted[0]);
                this.isShift = bool.Parse(splitted[1]);
                this.isAlt = bool.Parse(splitted[2]);
                this.key = (KeyCode)int.Parse(splitted[3]);
            }

            public override string ToString()
            {
                return isCtrl + "/" + isShift + "/" + isAlt + "/" + (int)key;
            }

            public string ToString(bool beautify = false)
            {
                if (beautify)
                    return (isCtrl ? "Ctrl+" : "" ) + (isShift ? "Shift+": "") + (isAlt ? "Alt+" : "") + key;
                else
                    return isCtrl + "/" + isShift + "/" + isAlt + "/" + (int)key;

            }

            public bool IsCombinationPressed()
            {
                return Input.GetKeyDown(key) &&
                    (
                        isCtrl ? Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) : true &&
                        isShift ? Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) : true &&
                        isAlt ? Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr) : true
                    );
            }
        }

        public int Interval { get; set; }
        public string ImageSavePath { get; set; }

        public KeyCombination CameraPositionButton;
        public KeyCombination StartButton;

        public ConfigDTO()
        {
            this.ImageSavePath = PlayerPrefs.GetString(IMAGESAVEPATH, "C:\\temp");
            this.Interval = PlayerPrefs.GetInt(INTERVALKEY, ((1 * 24) + 0 ) * 60 + 0 );
            this.CameraPositionButton = new KeyCombination(PlayerPrefs.GetString(CAMERAPOSITIONKEY, "False/False/False/108"));
            this.StartButton = new KeyCombination(PlayerPrefs.GetString(STARTKEY, "False/False/False/107"));
        }

        public void Save()
        {
            PlayerPrefs.SetString(IMAGESAVEPATH, this.ImageSavePath);
            PlayerPrefs.SetInt(INTERVALKEY, this.Interval);
            PlayerPrefs.SetString(CAMERAPOSITIONKEY, this.CameraPositionButton.ToString());
            PlayerPrefs.SetString(STARTKEY, this.StartButton.ToString());
            PlayerPrefs.Save();
        }
    }
}
