using UnityEngine;

namespace Core.Tools
{
    [RequireComponent(typeof(Camera))]
    public class AspectRatioCameraFitter : MonoBehaviour
    {
        [SerializeField] private Camera cam;

        private readonly float _defaultAspectRatio = 1170f / 2532f;
        private float _defaultSize = 100f;
 
        private Vector2 lastResolution;

#if !UNITY_EDITOR
        public void Awake()
        {
            ResizeCamera();
        }
#endif
        

#if UNITY_EDITOR
        public void LateUpdate()
        {
            ResizeCamera();
        }
#endif
       

        private void ResizeCamera()
        {
            var currentScreenResolution = new Vector2(Screen.width, Screen.height);
 
            // Don't run all the calculations if the screen resolution has not changed
            if (lastResolution != currentScreenResolution)
            {
                CalculateCameraRect(currentScreenResolution);
            }
 
            lastResolution = currentScreenResolution;
        }

        private void CalculateCameraRect(Vector2 currentScreenResolution)
        {
            float currentAspectRatio = currentScreenResolution.x / currentScreenResolution.y;
            if (_defaultAspectRatio < currentAspectRatio)
                cam.orthographicSize = _defaultSize;
            else
                cam.orthographicSize = _defaultSize * _defaultAspectRatio / currentAspectRatio;
        }
    }
}