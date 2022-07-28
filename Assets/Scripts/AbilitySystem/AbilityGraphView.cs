using System.Collections.Generic;
using PixelHunt;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    public class AbilityGraphView : VisualElement
    {

        #region Shorthands

        private const string ability_button_class = "button-ability";
        private static Texture2D default_icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/icon-fire.png");
        private static StyleEnum<Position> position_relative => new (Position.Relative);
        private static StyleEnum<Position> position_absolute => new (Position.Absolute);

        #endregion
        
        
        private List<AbilityButton> _abilityButtons = new();

        private AbilityManager _manager;
        private AbilityGraph _graph;
        private VisualElement
                _abilityInspector;


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
                            position = position_absolute,
                            backgroundImage = node.Ability?.Icon
                    },
            };
            nodeButton.RegisterOnLayoutDoneCallback(()=> nodeButton.transform.position = calculateButtonPosition(nodeButton) );
            nodeButton.AddToClassList(ability_button_class);
            _abilityButtons.Add(nodeButton);
            Add(nodeButton);
            
            return nodeButton;
        }

        private Vector3 calculateButtonPosition(AbilityButton nodeButton)
        {
            return nodeButton.Node.Position + nodeButton.parent.layout.size * .5f - nodeButton.layout.size* .5f;
        }

        public void RemoveButtonWithNode(AbilityGraphNode node)
        {
            var buttonFound = _abilityButtons.Find(b => b.Node == node);
            if (buttonFound != null)
            {
                Remove(buttonFound);
                _abilityButtons.Remove(buttonFound);
            }
        }

        public void RemoveAbilityButton(AbilityButton button)
        {
            if (_abilityButtons.Contains(button))
            {
                _abilityButtons.Remove(button);
            }
            Remove(button);
        }

        public void DrawConnection(GraphNodeConnection connection)
        {
            
        }

        public void MoveAbilityButton(AbilityButton node, Vector2 delta)
        {
            
        }

        public void CleanButtons()
        {
            var childButtons =this.Query<AbilityButton>();
            childButtons.ForEach(b =>
            {
                if (b.Node == null) Remove(b);
            });
        }
    }
}