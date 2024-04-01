using System;
using UnityEngine;

public class MonoActionService : MonoBehaviour
{
    private Action _onUpdate;
    private Action _onLateUpdate;
    private Action<bool> _onApplicationPause;
    private Action _onApplicationQuit;

    public void AddUpdateListener(Action onUpdate)
    {
        _onUpdate += onUpdate;
    }
    
    public void RemoveUpdateListener(Action onUpdate)
    {
        _onUpdate -= onUpdate;
    }
    
    public void AddLateUpdateListener(Action onLateUpdate)
    {
        _onLateUpdate += onLateUpdate;
    }
    
    public void RemoveLateUpdateListener(Action onLateUpdate)
    {
        _onLateUpdate -= onLateUpdate;
    }
    
    public void AddApplicationPauseListener(Action<bool> onApplicationPause)
    {
        _onApplicationPause += onApplicationPause;
    }
    
    public void RemoveApplicationPauseListener(Action<bool> onApplicationPause)
    {
        _onApplicationPause -= onApplicationPause;
    }
    
    private void Update()
    {
        _onUpdate?.Invoke();
    }
    
    private void LateUpdate()
    {
        _onLateUpdate?.Invoke();
    }
    
#if UNITY_EDITOR
    private void OnApplicationFocus(bool pauseStatus)
    {
        pauseStatus = !pauseStatus;
#else
    private void OnApplicationPause(bool pauseStatus)
    {
#endif
        _onApplicationPause?.Invoke(pauseStatus);
    }

    private void OnApplicationQuit()
    {
        _onApplicationQuit?.Invoke();
    }

    public void AddOnApplicationQuitListener(Action onApplicationQuit)
    {
        _onApplicationQuit += onApplicationQuit;
    }
    
    public void RemoveOnApplicationQuitListener(Action onApplicationQuit)
    {
        _onApplicationQuit -= onApplicationQuit;
    }
}