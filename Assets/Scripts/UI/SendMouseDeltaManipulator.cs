using UnityEngine.UIElements;

public class SendMouseDeltaManipulator : DragManipulator
{
    public SendMouseDeltaManipulator(MouseButton dragButton) : base(dragButton) { }

    protected override void OnMouseMove(MouseMoveEvent evt)
    {
        if (!_isDragging || !target.HasMouseCapture()) return;
        if (target is IMouseDeltaReceiver receiver)
        {
            receiver.ReceiveDelta(evt.mouseDelta);
            evt.StopPropagation();
        }
    }
}