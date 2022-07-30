using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DragManipulator : MouseManipulator
{
    public event Action<Vector2> OnDragged;
    public event Action<Vector2> OnDraggedScaled;

    public float DragScale
    {
        get => _dragScale; 
        set => _dragScale = Mathf.Clamp(value, 0, 10f);
    }


    private float _dragScale = 1;
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
            DraggedElement.HandleEvent(new ElementDragStartedEvent());
        }
    }

    protected virtual void OnMouseMove(MouseMoveEvent evt)
    {
        if (_isDragging && target.HasMouseCapture())
        {
            var scaledDelta = new Vector3(evt.mouseDelta.x, evt.mouseDelta.y) * _dragScale;
            DraggedElement.transform.position += scaledDelta;
            OnDragged?.Invoke(evt.mouseDelta);
            OnDraggedScaled?.Invoke(scaledDelta);
            evt.StopPropagation();
            DraggedElement.HandleEvent(new ElementDraggedEvent());
        }
    }

    protected virtual void OnMouseUp(MouseUpEvent evt)
    {
        if (!_isDragging|| !target.HasMouseCapture() || !CanStopManipulation(evt))
            return;
        _isDragging = false;
        target.ReleaseMouse();
        evt.StopPropagation();
        DraggedElement.HandleEvent(new ElementDragStoppedEvent());
    }
}

public class ElementDraggedEvent : MouseEventBase<ElementDraggedEvent>
{
    
}

public class ElementDragStartedEvent : MouseEventBase<ElementDraggedEvent>
{
    
}

public class ElementDragStoppedEvent : MouseEventBase<ElementDraggedEvent>
{
    
}