using PixelHunt;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    public class AbilityGraphView : VisualElement
    {

        #region Shorthands

        private const string ability_button_class = "button-ability";
        private static StyleEnum<Position> position_relative => new (Position.Relative);
        private static StyleEnum<Position> position_absolute => new (Position.Absolute);
        
        private const string button_normal_class = "button-ability";
        private const string button_selected_class = "button-ability-selected";
        private const string button_learned_class = "button-ability-learned";
        private const string line_normal = "connection-line";
        private const string line_active = "connection-line-active";

        #endregion

        public float LineWidth
        {
            get => _lineWidth;
            set => _lineWidth = Mathf.Clamp(value, .5f, 50f);
        }

        private AbilityManager _manager;
        private AbilityGraph _graph;
        private VisualElement
                _abilityInspector;
        private float _lineWidth = 3;

        public new class UxmlFactory : UxmlFactory<AbilityGraphView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        public AbilityButton CreateNodeButton(AbilityGraphNode node)
        {
            var nodeButton = new AbilityButton(node, null)
            {
                    style =
                    {
                            position = position_absolute
                    },
            };
            nodeButton.ExecuteAfterLayoutDone(()=> nodeButton.transform.position = calculateButtonPosition(nodeButton) );
            nodeButton.AddToClassList(ability_button_class);
            if (node.Ability && node.Ability.Icon)
            {
                nodeButton.style.backgroundImage = node.Ability.Icon;
            }
            Add(nodeButton);
            
            return nodeButton;
        }

        public void SetSelectedState(AbilityButton button)
        {
            button.AddToClassList(button_selected_class);
        }

        public void StopSelecting(AbilityButton button)
        {
            button.RemoveFromClassList(button_selected_class);
        }

        private Vector3 calculateButtonPosition(AbilityButton nodeButton)
        {
            return nodeButton.Node.GraphPosition + layout.size * .5f - nodeButton.layout.size * .5f;
        }

        public void RemoveAbilityButton(AbilityButton button)
        {
            Remove(button);
        }

        public void SetLearnedState(AbilityButton button)
        {
            button.AddToClassList(button_learned_class);
        }

        public void SetNormalState(AbilityButton button)
        {
            button.RemoveFromClassList(button_learned_class);
        }

        public void LineNormal(NodeConnectionLine line) => line.RemoveFromClassList(line_active);
        public void LineActive(NodeConnectionLine line) => line.AddToClassList(line_active);

        public NodeConnectionLine CreateConnection(AbilityButton a, AbilityButton b)
        {
            var start = calculateButtonPosition(a) + (Vector3)a.HalfSize;
            var end = calculateButtonPosition(b) + (Vector3)b.HalfSize;
            var line = new NodeConnectionLine(start, end, _lineWidth);
            Add(line);
            line.AddToClassList(line_normal);
            line.SendToBack();
            return line;
        }
    }
}