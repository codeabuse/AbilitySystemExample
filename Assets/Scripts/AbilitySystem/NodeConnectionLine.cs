using UnityEngine;

namespace AbilitySystem
{
    public class NodeConnectionLine : LineSegmentDrawer
    {
        public new class UxmlFactory : LineSegmentDrawer.UxmlFactory {}

        public new class UxmlTraits : LineSegmentDrawer.UxmlTraits { }

        public void HandleStartDraggedEvent(ElementDraggedEvent evt)
        {
            Start += evt.mouseDelta;
        }
        
        public void HandleEndDraggedEvent(ElementDraggedEvent evt)
        {
            End += evt.mouseDelta;
        }

        public NodeConnectionLine(Vector3 start, Vector3 end, float width) : base(start, end, width) { }
        public NodeConnectionLine(Vector2 start, Vector2 end, float width) : base(start, end, width) { }

        public NodeConnectionLine() { }
    }
}