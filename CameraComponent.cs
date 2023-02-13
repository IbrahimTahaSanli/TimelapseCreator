using ColossalFramework.UI;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TimelapseCreator
{
    class CameraHook: MonoBehaviour
    {
        private Texture2D _texture;

        private Vector3 _lastPos;
        private Quaternion _lastRot;

        private CameraController contr;

        private ConfigDTO _config;

        public TimelapseCreatorThreading Thread;

        void OnEnable()
        {
            this._texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        
            this.contr = GameObject.FindObjectOfType<CameraController>();

            this._config = new ConfigDTO();
        }

        public void StartCapture(Vector3 pos, Quaternion rot)
        {
            this._lastPos = this.transform.position;
            this.transform.position = pos;

            this._lastRot = this.transform.rotation;
            this.transform.rotation = rot;

            StartCoroutine(SaveRoutine());
        }

        public void Update()
        {

        }

        private IEnumerator SaveRoutine()
        {
            this.Thread.Hide();
            this.contr.enabled = false;

            yield return new WaitForEndOfFrame();

            this._texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
            byte[] bytes = this._texture.EncodeToPNG();
            try
            {
                File.WriteAllBytes(this._config.ImageSavePath + "\\" + CityInfoPanel.instance.GetCityName() + SimulationManager.instance.m_currentGameTime.ToString("-dd-M-yyyy--HH-mm-ss") + ".png", bytes);
            }
            catch(Exception e)
            {
                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                panel.SetMessage("TimelapseCreator", "Capture couldn't be written to folder. Check your save location.", false);

                Debug.LogError(e.ToString());
            }
            this.transform.position = this._lastPos;
            this.transform.rotation = this._lastRot;
            
            this.contr.enabled = true;

            this.Thread.Show();
        }

    }
}
