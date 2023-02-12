using ICities;
using UnityEngine;

namespace TimelapseCreator
{
    public class TimelapseCreatorLoading : ILoadingExtension
    {
        public void OnCreated(ILoading loading)
        {
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            GameObject.Destroy(GameObject.Find("TimelapseCreatorManager"));
        }

        public void OnReleased()
        {
        }

        void ILoadingExtension.OnLevelUnloading()
        {
        }
    }
}
