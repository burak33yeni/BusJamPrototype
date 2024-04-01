using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _Game._Scripts._Human._Animation
{
    public static class HumanAnimationService
    {
        private static readonly float MOVE_ANIMATION_DURATION = 0.2f;
        public static void GoToPositionAnimation(Transform t, 
            Stack<Vector3> positions, Action onArrived)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Pause();
            while (positions.Count > 0)
            {
                sequence.Append(t.DOMove(positions.Pop(), MOVE_ANIMATION_DURATION)
                    .SetEase(Ease.Linear));
            }

            sequence.OnComplete(() =>
            {
                onArrived?.Invoke();
            });
            sequence.Play();
        }
    }
}