using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DragManipulator : MouseManipulator
{
    public event Action<Vector2> OnDragged;
    protected bool _isDragging;

    public VisualElement DraggedElement
    {
        get => _draggedElement ??= target;
        set => _draggedElement = value;
    }

    private VisualElement _draggedElement;

    public DragManipulator(MouseButton dragButton)
    {
        activators.Add(new ManipulatorActivationFilter{ button = dragButton });
    }
    
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected virtual void OnMouseDown(MouseDownEvent evt)
    {
        if (_isDragging)
        {
            evt.StopImmediatePropagation();
            return;
        }

        if (CanStartManipulation(evt))
        {
            _isDragging = true;
            target.CaptureMouse();
            evt.StopPropagation();
        }
    }

    protected virtual void OnMouseMove(MouseMoveEvent evt)
    {
        if (_isDragging && target.HasMouseCapture())
        {
            DraggedElement.transform.position += new Vector3(evt.mouseDelta.x, evt.mouseDelta.y);
            OnDragged?.Invoke(evt.mouseDelta);
            evt.StopPropagation();
        }
    }

    protected virtual void OnMouseUp(MouseUpEvent evt)
    {
        if (!_isDragging|| !target.HasMouseCapture() || !CanStopManipulation(evt))
            return;
        _isDragging = false;
        target.ReleaseMouse();
        evt.StopPropagation();
    }
}