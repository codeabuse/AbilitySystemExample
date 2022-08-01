using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ImguiToUIEWrapper: Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
       
        var property = serializedObject.GetIterator();
        if (property.NextVisible(true))
        {
            do
            {
                var field = new PropertyField(property);
 
                if (property.name == "m_Script")
                {
                    field.SetEnabled(false);
                }
               
                root.Add(field);
            }
            while (property.NextVisible(false));
        }
 
        return root;
    }
}