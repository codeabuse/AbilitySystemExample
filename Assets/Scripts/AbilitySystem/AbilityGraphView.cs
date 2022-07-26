using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    public class AbilityGraphView : VisualElement
    {
        private const string ability_button_class = "button-ability";
        private static Texture2D default_icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/icon-fire.png");

        private Dictionary<AbilityGraphNode, Button> _abilityButtons = new();

        private AbilityManager _manager;
        private AbilityGraph _graph;
        private VisualElement _descriptionArea;
        private Label
                _abilityLabel,
                _abilityDescription,
                _abilityCost;

        private StyleEnum<Visibility> hidden => new (Visibility.Hidden);
        private StyleEnum<Visibility> visible => new (Visibility.Visible);

        public new class UxmlFactory : UxmlFactory<AbilityGraphView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        public void BindToManager(AbilityManager manager)
        {
            Debug.Log("Binding AbilityGraphView to manager");
            _manager = manager;
            manager.OnAbilityNodeSelected += ShowAbilityInfo;
            _descriptionArea = this.Q("AbilityDescriptionArea");
            _abilityLabel = this.Q<Label>("AbilityTitle");
            _abilityDescription = this.Q<Label>("AbilityDescription");
            _abilityCost = this.Q<Label>("AbilityCostValue");
        }

        public void BindToGraph(AbilityGraph graph)
        {
            _graph = graph;
            _graph.OnAbilityNodeCreated += CreateNodeView;
            _graph.OnAbilityNodeRemoved += RemoveNodeView;
        }

        ~AbilityGraphView()
        {
            _manager.OnAbilityNodeSelected -= ShowAbilityInfo;
            _graph.OnAbilityNodeCreated -= CreateNodeView;
            _graph.OnAbilityNodeRemoved -= RemoveNodeView;
        }

        public void ClearAbilityButtons()
        {
            foreach (var abilityButton in _abilityButtons)
            {
                Remove(abilityButton.Value);
            }
            _abilityButtons.Clear();
        }

        public void CreateNodeView(AbilityGraphNode node)
        {
            var nodeButton = new Button(
                    () => AbilityManager.Instance.SelectAbilityTreeNode(node));
            nodeButton.AddToClassList(ability_button_class);
            _abilityButtons.Add(node, nodeButton);
        }

        public void RemoveNodeView(AbilityGraphNode node)
        {
            var nodeButton = _abilityButtons[node];
            nodeButton.parent.Remove(nodeButton);
            _abilityButtons.Remove(node);
        }

        private void ShowAbilityInfo(AbilityGraphNode node)
        {
            if (node == null)
            {
                _descriptionArea.style.visibility = hidden;
                return;
            }

            _descriptionArea.style.visibility = visible;
            _abilityLabel.text = node.Ability.DescriptiveName;
            _abilityDescription.text = node.Ability.Description;
            _abilityCost.text = node.LearningCost.ToString();
        }
    }
}