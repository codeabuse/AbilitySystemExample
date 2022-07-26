using UnityEngine;
using UnityEngine.UIElements;

public class TestPanelController : UIPanelController
{
    private VisualElement a, b;
    
    protected override void Initialize()
    {
        var root = _document.rootVisualElement.Q("Root");
        Debug.Log($"Root size: {root.layout.size}");
        a = _document.rootVisualElement.Q("item-a");
        b = _document.rootVisualElement.Q("item-b");
        _document.rootVisualElement.Add(
                new LineSegmentDrawer(a.layout.center / root.layout.size, b.layout.center / root.layout.size, 4)
                {
                        style =
                        {
                                color = Color.magenta,
                                width = root.layout.size.x,
                                height = root.layout.size.y,
                                
                        }
                        
                });
        Debug.Log($"A:{a.layout.position.ToString()}");
        Debug.Log($"B:{b.layout.position.ToString()}");
    }
}