using ICities;
using System;
using UnityEngine;

namespace TimelapseCreator
{
    public class TimelapseCreatorThreading: IThreadingExtension
    {
        private CameraController _cameraController;
        private CameraHook _cameraHook;
        private Camera _mainCamera;
        private Camera _uiViewCamera;
        private Camera _undergroundCamera;
        private ToolController _toolController;
        private ToolBase _lastToolBase;
        private DefaultTool _defaultTool;
        
        private OverlayEffect _overlayEffect;

        private ConfigDTO _config;

        private bool _isCaptureStarted = false;

        private int _lastTime;

        private IThreading _thread;

        private DateTime LastTime;


        private Vector3 pos;
        private Quaternion rot;

        public void OnCreated(IThreading threading)
        {
            this._config = new ConfigDTO();

            this._cameraController = GameObject.FindObjectOfType<CameraController>();
            this._mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

            this._overlayEffect = GameObject.Find("Main Camera").GetComponent<OverlayEffect>();
            this._toolController = GameObject.FindObjectOfType<ToolController>();
            this._defaultTool = GameObject.FindObjectOfType<DefaultTool>();

            this._cameraHook = this._cameraController.gameObject.AddComponent<CameraHook>();
            this._cameraHook.Thread = this;

            this._thread = threading;
            this._thread.simulationPaused = true;

            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (var cam in cameras)
            {
                Debug.Log(cam.name);
                if (cam.name == "UIView")
                    this._uiViewCamera = cam;
                else if (cam.name == "Underground View")
                    this._undergroundCamera = cam;
            }

        }


        public void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (this._isCaptureStarted)
            {
                this._lastTime -= (this._thread.simulationTime - this.LastTime).Minutes;

                if (this._lastTime < 0)
                {
                    if (this.pos != Vector3.zero)
                        this._cameraHook.StartCapture(this.pos, this.rot);
                    else
                        this._cameraHook.StartCapture(
                            this._cameraController.transform.position,
                            this._cameraController.transform.rotation
                            );

                    this._lastTime = this._config.Interval;
                }
                    this.LastTime = this._thread.simulationTime;
            }

            if (!this._isCaptureStarted && this._config.StartButton.IsCombinationPressed())
            {
                if (this._uiViewCamera == null || this._undergroundCamera == null)
                {
                    Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
                    foreach (var cam in cameras)
                    {
                        if (cam.name == "UIView")
                            this._uiViewCamera = cam;
                        else if (cam.name == "Underground View")
                            this._undergroundCamera = cam;
                    }
                }

                this._config = new ConfigDTO();
                this._isCaptureStarted = true;

                this.LastTime = SimulationManager.instance.m_currentGameTime;

                this._lastTime = this._config.Interval;

                if (this.pos != Vector3.zero)
                    this._cameraHook.StartCapture(this.pos, this.rot);
                else
                    this._cameraHook.StartCapture(
                        this._cameraController.transform.position,
                        this._cameraController.transform.rotation
                        );
            }
            else if(this._isCaptureStarted && this._config.StartButton.IsCombinationPressed())
            {
                this._isCaptureStarted = false;
            }

            else if (this._config.CameraPositionButton.IsCombinationPressed())
            { 
                this.pos = this._cameraController.transform.position;
                this.rot = this._cameraController.transform.rotation;
                this._thread.simulationPaused = true;
            }
        }


        //I copied this hidq and show function from in a github repository but i cannot find it now if you know which repository is the original. I need to referance it to
        public void Hide()
        {
            this._mainCamera.rect = CameraController.kFullScreenRect;

            this._overlayEffect.enabled = false;
            bool cachedEnabled = this._cameraController.enabled;


            this._uiViewCamera.enabled = false;
            this._undergroundCamera.enabled = false;
            this._mainCamera.enabled = true;

            this._lastToolBase = this._toolController.CurrentTool;
            this._toolController.CurrentTool = this._defaultTool;

            //TODO: For some reason, the cameracontroller's not picking this up before it's disabled...
            this._cameraController.enabled = true;
            this._cameraController.m_freeCamera = true;
            this._cameraController.enabled = cachedEnabled;
        }

        public void Show()
        {
            this._overlayEffect.enabled = true;

            bool cachedEnabled = this._cameraController.enabled;

            this._uiViewCamera.enabled = true;
            this._undergroundCamera.enabled = false;
            this._mainCamera.enabled = true;

            this._toolController.CurrentTool = this._lastToolBase;

            //TODO: For some reason, the cameracontroller's not picking this up before it's disabled...
            this._cameraController.enabled = true;
            this._cameraController.m_freeCamera = false;
            this._cameraController.enabled = cachedEnabled;

            this._mainCamera.rect = CameraController.kFullScreenWithoutMenuBarRect;
        }


        public void OnReleased()
        {
            GameObject.Destroy(this._cameraHook);
        }

        public void OnBeforeSimulationTick()
        {
        }

        public void OnBeforeSimulationFrame()
        {
        }

        public void OnAfterSimulationFrame()
        {
        }

        public void OnAfterSimulationTick()
        {
        }
    }
}
