using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace TimelapseCreator
{
    public class TimelapseCreator : IUserMod
    {
        public string Name => "TimelapseCreator";

        public string Description => "Lets hope it works. - " + "V1.0";

        //Mod configs
        public ConfigDTO config;

        //Currently only reason is check with key combination pushed
        private GameObject _manager;

        //Options panel buttons
        private UIButton _cameraButton;
        private UIButton _startButton;

        public void OnSettingsUI(UIHelperBase helper)
        {
            //Create manager and config when initialize
            if (_manager == null)
            {
                _manager = new GameObject("TimelapseCreatorManager");
                _manager.AddComponent<KeyMapper>();
            }
            if(config == null)
                config = new ConfigDTO();

            UIHelperBase group = helper.AddGroup("TimelapseCreator");

            group.AddTextfield("Where to save captured images", this.config.ImageSavePath, (string text) => this.config.ImageSavePath = text);
            
            group.AddSpace(20);
            group.AddGroup("Interval between captures(In game time)");

            UIHelperBase IntervalGroup = group.AddGroup("Interval for capture");
            IntervalGroup.AddTextfield("DD/HH/MM", IntMinuteToString(this.config.Interval), (string text) => this.config.Interval = StringToIntMinute(text));

            UIHelperBase CameraPositionGroup = group.AddGroup("Camera position set button");
            this._cameraButton = ((UIButton)CameraPositionGroup.AddButton(this.config.CameraPositionButton.ToString(true), GetKeyMapForCameraPosition));
            
            UIHelperBase StartButtonGroup = group.AddGroup("Start lapse key");

            this._startButton = (UIButton)StartButtonGroup.AddButton(this.config.StartButton.ToString(true), GetKeyMapForStartButton);


            group.AddButton("Save", OnSaveButtonEvent);
        }

        //Camera postion button events
        private void GetKeyMapForCameraPosition()
        {
            this._startButton.text = this.config.StartButton.ToString(true);

            this._cameraButton.text = "Press Any Key";

            this._manager.GetComponent<KeyMapper>().ButtonPressedEvent = (bool isCtrl, bool isShift, bool isAlt, KeyCode code) =>
            {
                this.config.CameraPositionButton = new ConfigDTO.KeyCombination( isCtrl, isShift, isAlt, code);
                this._cameraButton.text = this.config.CameraPositionButton.ToString(true);
            };
        }

        //Start button events
        private void GetKeyMapForStartButton()
        {
            this._cameraButton.text = this.config.CameraPositionButton.ToString(true);

            this._startButton.text = "Press Any Key";

            this._manager.GetComponent<KeyMapper>().ButtonPressedEvent = (bool isCtrl, bool isShift, bool isAlt, KeyCode code) =>
            {
                this.config.StartButton = new ConfigDTO.KeyCombination(isCtrl, isShift, isAlt, code);
                this._startButton.text = this.config.StartButton.ToString(true);
            };
        }

        private int StringToIntMinute(string str)
        {
            int result = 0;
            int tmp;

            string[] arr = str.Split('/');

            if (arr.Length == 3)
            {
                int.TryParse(arr[0], out tmp);
                result = tmp * 24;

                int.TryParse(arr[1], out tmp);
                result = (result + tmp) * 60;

                int.TryParse(arr[2], out tmp);
                result += tmp;
            }

            return result;
        }

        private string IntMinuteToString(int minute)
        {
            string result = "";
            int tmp;

            tmp = (minute / (24 * 60));
            minute -= tmp * 24 * 60;
            result += tmp.ToString() + "/";

            tmp = minute / 60;

            minute -= tmp * 60;

            result += tmp.ToString() + "/";

            result += minute.ToString();

            return result;
        }

        public void OnSaveButtonEvent()
        {
            this.config.Save();
        }
    }
}
