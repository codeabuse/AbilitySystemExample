using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    [CustomPropertyDrawer(typeof(AbilityGraphNode))]
    public class AbilityGraphNodeDrawer : PropertyDrawer
    {
        private Vector2Field _positionField;
        private ObjectField _abilityField;
        private IntegerField _learningCostField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement() { style = { flexGrow = 1 } };
            _positionField = new Vector2Field() { label = "Position" };
            _abilityField = new ObjectField("Ability");
            _learningCostField = new IntegerField("Learning Cost");
            _positionField.BindProperty(property.FindPropertyRelative(AbilityGraphNode.POSITION_PROP_NAME));
            _abilityField.BindProperty(property.FindPropertyRelative(AbilityGraphNode.ABILITY_PROP_NAME));
            _learningCostField.BindProperty(property.FindPropertyRelative(AbilityGraphNode.LEARNING_COST_PROP_NAME));
            root.Add(_positionField);
            root.Add(_abilityField);
            root.Add(_learningCostField);
            
            return root;
        }
    }
}