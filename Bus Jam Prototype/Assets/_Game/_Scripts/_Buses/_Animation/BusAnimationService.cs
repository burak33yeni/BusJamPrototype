using System;
using DG.Tweening;
using UnityEngine;

namespace _Game._Scripts._Buses._Animation
{
    public static class BusAnimationService
    {
        private static readonly float _animationDuration = 3f;
        
        public static Tween GoOutOfScreenAnimation(Transform t, Vector3 position,Action onComplete)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Pause();
            sequence.Append(t.DOLocalMove(position, _animationDuration));
            sequence.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
            sequence.Play();
            return sequence;
        }
        
        public static Tween MoveToBusStopAnimation(Transform t, Vector3 position, Action onComplete)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Pause();
            sequence.Append(t.DOLocalMove(position, _animationDuration));
            sequence.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
            sequence.Play();
            return sequence;
        }
    }
}