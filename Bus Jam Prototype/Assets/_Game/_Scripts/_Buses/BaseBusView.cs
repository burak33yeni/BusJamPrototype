using System;
using System.Collections.Generic;
using _Game._Scripts._Buses._Animation;
using DG.Tweening;
using UnityEngine;

namespace _Game._Scripts._Buses
{
    public abstract class BaseBusView : MonoBehaviour, IBaseBusView
    {
        [SerializeField] private Transform _passengerParent;
        [SerializeField] private Transform[] _passengerPositions;
        [SerializeField] private MeshRenderer _busRenderer;
        
        public Transform PassengerParent => _passengerParent;
        
        private List<Tween> _tweens = new List<Tween>();

        public bool TryGetPassengerPosition(int index, out Vector3 position)
        {
            position = Vector3.zero;
            if (index >= _passengerPositions.Length) return false;
            position = _passengerPositions[index].position;
            return true;
        }
        
        public void SetLocalPosition(Vector3 position)
        {
            _passengerParent.localPosition = position;
            _passengerParent.localScale = Vector3.one;
        }
        
        public void SetColor(Color color)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_BaseColor", color);
            _busRenderer.SetPropertyBlock(propertyBlock);
        }
        
        public void SetPassengersParent(Transform passenger)
        {
            passenger.SetParent(_passengerParent);
        }

        public void GoPositionAndRemoveBus(Vector3 position, Action onComplete)
        {
            _tweens.Add(BusAnimationService.GoOutOfScreenAnimation(_passengerParent, position, () =>
            {
                onComplete?.Invoke();
            }));
        }
        
        public void MoveToPosition(Vector3 position, Action onComplete)
        {
            _tweens.Add(BusAnimationService.MoveToBusStopAnimation(_passengerParent, position, () =>
            {
                onComplete?.Invoke();
            }));
        }

        public void KillAllAnimations()
        {
            foreach (var tween in _tweens)
            {
                if (tween != null)
                {
                    tween.Kill();
                }
            }
        }
        
    }

    public interface IBaseBusView
    {
        Transform PassengerParent { get; }
        bool TryGetPassengerPosition(int index, out Vector3 position);
        void SetLocalPosition(Vector3 position);
        void SetColor(Color color);
        void SetPassengersParent(Transform passenger);
        void GoPositionAndRemoveBus(Vector3 position, Action onComplete);
        void MoveToPosition(Vector3 position, Action onComplete);
        void KillAllAnimations();
    }
}