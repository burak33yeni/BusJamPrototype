using Core.ServiceLocator;
using UnityEngine;

public class CanvasService : MonoBehaviour, IInitializable
{
    [SerializeField] private Canvas canvas;


    public void Initialize()
    {
        // canvas.worldCamera = Camera.main;
    }
    
    public float GetScaleFactor()
    {
        return canvas.scaleFactor;
    }
}
