using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    [CustomPropertyDrawer(typeof(AbilityGraphNode))]
    public class AbilityGraphNodeDrawer : PropertyDrawer
    {
        private const string inspector_visual_tree = "Assets/UI/Editor/ability-node-inspector.uxml";
        
        private static VisualTreeAsset _visualTreeAsset;
        private static VisualTreeAsset visualTreeAsset => _visualTreeAsset? 
                _visualTreeAsset : 
                _visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(inspector_visual_tree);
        
        private Vector2Field _positionField;
        private ObjectField _abilityField;
        private IntegerField _learningCostField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            visualTreeAsset.CloneTree(root);
            
            var bindableElements = root.Query<BindableElement>().
                    Where(b => !string.IsNullOrEmpty(b.bindingPath));
            bindableElements.ForEach(x => { x.BindProperty(property.FindPropertyRelative(x.bindingPath)); x.SetEnabled(true); });
            
            // _positionField = new Vector2Field() { label = "Position" };
            // _abilityField = new ObjectField("Ability"){objectType = typeof(Ability)};
            // _learningCostField = new IntegerField("Learning Cost");
            // _positionField.BindProperty(property.FindPropertyRelative(AbilityGraphNode.POSITION_PROP_NAME));
            // _abilityField.BindProperty(property.FindPropertyRelative(AbilityGraphNode.ABILITY_PROP_NAME));
            // root.Add(_positionField);
            // root.Add(_abilityField);
            // root.Add(_learningCostField);
            
            return root;
        }
    }
}