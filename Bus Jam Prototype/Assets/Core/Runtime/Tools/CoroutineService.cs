using System.Collections;
using UnityEngine;

public class CoroutineService : MonoBehaviour
{
    // either below methods or directly monoBehaviour methods can be used
    public Coroutine RunCoroutine(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }
    
    public void TryStopCoroutine(Coroutine coroutine)
    {
        if (coroutine == null) return;
        StopCoroutine(coroutine);
    }
}
