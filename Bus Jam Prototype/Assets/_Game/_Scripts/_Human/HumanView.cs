using System;
using System.Collections.Generic;
using _Game._Scripts._Human._Animation;
using Core.Pool;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game._Scripts._Human
{
    public class HumanView : MonoBehaviour, IHumanView, IPointerDownHandler
    {
        private static readonly float _yPosition = 1.5f;
        [SerializeField] private Transform _transform;
        [SerializeField] private MeshRenderer _renderer;

        private Action _onClick;
        public void Initialize(HumanViewModel viewModel)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetColor("_BaseColor", viewModel.color);
            _renderer.SetPropertyBlock(mpb,0);
            _transform.position = viewModel.position;
            _transform.localScale = Vector3.one;
            _onClick = viewModel.onClick;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _onClick?.Invoke();
        }
        
        public void SetMovementIndicator(bool open)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetFloat("_Scale", open ? 1.2f : 0);
            _renderer.SetPropertyBlock(mpb,1);
        }

        public void GoToPositions(Stack<Vector3> positions, Action onArrived)
        {
            HumanAnimationService.GoToPositionAnimation(_transform, positions, onArrived);
        }
        
        public Transform GetTransform()
        {
            return _transform;
        }
    }
    
    public class HumanViewModel
    {
        public Color color;
        public Vector3 position;
        public Action onClick;
        
        public HumanViewModel(Color color, Vector3 position, Action onClick)
        {
            this.color = color;
            this.position = position;
            this.onClick = onClick;
        }
    }

    public interface IHumanView
    {
        void Initialize(HumanViewModel viewModel);
        void SetMovementIndicator(bool open);
        void GoToPositions(Stack<Vector3> positions, Action onArrived);
        Transform GetTransform();
    }
    
    public class HumanViewPool : ObjectPool<HumanView>
    {
    }
}