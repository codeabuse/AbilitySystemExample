using System.Collections.Generic;
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

        #endregion

        public float LineWidth
        {
            get => _lineWidth;
            set => _lineWidth = Mathf.Clamp(value, .5f, 50f);
        }
        
        private List<AbilityButton> _abilityButtons = new();

        private AbilityManager _manager;
        private AbilityGraph _graph;
        private VisualElement
                _abilityInspector;
        private float _lineWidth = 3;

        public new class UxmlFactory : UxmlFactory<AbilityGraphView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        public AbilityButton CreateNodeButton(AbilityGraphNode node)
        {
            var buttonForNode = _abilityButtons.Find(b => b.Node == node);
            if (buttonForNode != null) return buttonForNode;
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
            _abilityButtons.Add(nodeButton);
            Add(nodeButton);
            
            return nodeButton;
        }

        private Vector3 calculateButtonPosition(AbilityButton nodeButton)
        {
            return nodeButton.Node.Position + layout.size * .5f - nodeButton.layout.size * .5f;
        }

        public void RemoveAbilityButton(AbilityButton button)
        {
            if (_abilityButtons.Contains(button))
            {
                _abilityButtons.Remove(button);
            }
            Remove(button);
        }

        public NodeConnectionLine CreateConnection(NodeConnection connection)
        {
            var halfLayout = layout.size * .5f;
            var (start, end) = (connection.NodeA.Position + halfLayout, connection.NodeB.Position + halfLayout);
            var line = createLine(start, end);
            Add(line);
            line.SendToBack();
            return line;
        }

        public NodeConnectionLine CreateConnection(AbilityButton a, AbilityButton b)
        {
            var start = calculateButtonPosition(a) + (Vector3)a.HalfSize;
            var end = calculateButtonPosition(b) + (Vector3)b.HalfSize;
            var line = createLine(start, end);
            Add(line);
            line.SendToBack();
            return line;
        }

        private NodeConnectionLine createLine(Vector3 start, Vector3 end)
        {
            return new NodeConnectionLine(start, end, LineWidth)
            {
                    style = { color = new StyleColor(Color.green) }
            };
        }
    }
}