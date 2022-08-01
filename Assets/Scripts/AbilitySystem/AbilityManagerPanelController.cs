using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    public class AbilityManagerPanelController : UIPanelController
    {
        #region Shorthands

        private static StyleEnum<Visibility> element_hidden => new (Visibility.Hidden);
        private static StyleEnum<Visibility> element_visible => new(Visibility.Visible);
        private static StyleEnum<DisplayStyle> display_flex => new(DisplayStyle.Flex);
        private static StyleEnum<DisplayStyle> display_none => new(DisplayStyle.None);

        
        #endregion
        
        protected AbilityGraphView GraphView { get; set; }

        private VisualElement _abilityInspectorRoot;
        private Label
                _abilityLabel,
                _abilityDescription,
                _abilityCost,
                _currentPoints;
        
        private Button
                _learnButton,
                _forgetButton,
                _forgetAllButton;

        private AbilityButton _inspectedNodeButton;
        private Character _selectedCharacter;

        protected void Awake()
        {
            AbilityManager.Instance.OnCharacterSelected += CharacterSelected;
        }

        protected override void OnEnable()
        {
            GraphView = Root.Q<AbilityGraphView>();
            _abilityInspectorRoot = Document.rootVisualElement.Q("ability-inspector");
            _abilityInspectorRoot.style.visibility = element_hidden;
            _abilityLabel = Document.rootVisualElement.Q<Label>("ability-title");
            _abilityDescription = Document.rootVisualElement.Q<Label>("ability-description");
            _abilityCost = Document.rootVisualElement.Q<Label>("ability-cost-value");
            _currentPoints = Document.rootVisualElement.Q<Label>("ability-points-value");
            _learnButton = Document.rootVisualElement.Q<Button>("learn-button");
            _forgetButton = Document.rootVisualElement.Q<Button>("forget-button");
            _forgetAllButton = Document.rootVisualElement.Q<Button>("forget-all-button");

            _learnButton.clicked += LearnSelected;
            _forgetButton.clicked += ForgetSelected;
            _forgetAllButton.clicked += ForgetAll;
            
            if (AbilityManager.Instance.SelectedCharacter)
            {
                BuildGraph(AbilityManager.Instance.SelectedCharacter.AbilityGraph);
            }
        }

        private void OnDisable()
        {
            _learnButton.clicked -= LearnSelected;
            _forgetButton.clicked -= ForgetSelected;
            _forgetAllButton.clicked -= ForgetAll;
        }

        private void CharacterSelected(Character character)
        {
            if (PanelEnabled)
            {
                _selectedCharacter = character;
                GraphView.Clear();
                BuildGraph(_selectedCharacter.AbilityGraph);
                _currentPoints.text = _selectedCharacter.AbilityPoints.ToString();
            }
        }
        
        private void InspectNode(AbilityGraphNode node)
        {
            AbilityManager.Instance.SelectAbilityTreeNode(node);
            if (!node.Ability) return;
            if (_inspectedNodeButton != null)
            {
                GraphView.StopSelecting(_inspectedNodeButton);
            }

            _abilityInspectorRoot.style.visibility = element_visible;
            UpdateAbilityInspectorState(node);
            _inspectedNodeButton = node.NodeButton;
            GraphView.SetSelectedState(_inspectedNodeButton);
        }

        private void UpdateAbilityInspectorState(AbilityGraphNode node)
        {
            _abilityLabel.text = node.Ability.DescriptiveName ?? string.Empty;
            _abilityDescription.text = node.Ability.Description ?? string.Empty;
            _abilityCost.text = node.LearningCost.ToString();
            _learnButton.style.visibility = AbilityManager.Instance.CanLearnAbility(node) ? element_visible : element_hidden;
            _forgetButton.style.visibility = 
                    AbilityManager.Instance.IsAbilityLearned(node) && AbilityManager.Instance.CanForgetAbility(node) 
                    ? element_visible : element_hidden;
        }

        private void LearnSelected()
        {
            AbilityManager.Instance.LearnSelectedAbility();
            GraphView.SetLearnedState(_inspectedNodeButton);
            _currentPoints.text = _selectedCharacter.AbilityPoints.ToString();
            UpdateAbilityInspectorState(_inspectedNodeButton.Node);
        }

        private void ForgetSelected()
        {
            AbilityManager.Instance.ForgetSelectedAbility();
            GraphView.SetNormalState(_inspectedNodeButton);
            _currentPoints.text = _selectedCharacter.AbilityPoints.ToString();
            UpdateAbilityInspectorState(_inspectedNodeButton.Node);
        }

        private void ForgetAll()
        {
            AbilityManager.Instance.ForgetAllAbilities();
            GraphView.Query<AbilityButton>().ForEach(b =>
            {
                if (b.Node != _selectedCharacter.AbilityGraph.RootNode) GraphView.SetNormalState(b);
            });
            _currentPoints.text = _selectedCharacter.AbilityPoints.ToString();
            UpdateAbilityInspectorState(_inspectedNodeButton.Node);
        }

        private void BuildGraph(AbilityGraph characterAbilityGraph)
        {
            foreach (var node in characterAbilityGraph.Nodes)
            {
                 node.NodeButton = GraphView.CreateNodeButton(node);
                 node.NodeButton.clicked += () => InspectNode(node);
                 if (AbilityManager.Instance.IsAbilityLearned(node))
                 {
                     GraphView.SetLearnedState(node.NodeButton);
                 }
            }
            
            CreateConnectionLines(2);
            
            async void CreateConnectionLines(int millisecondsDelay)
            {
                await Task.Delay(millisecondsDelay);
                foreach (var connection in characterAbilityGraph.Connections)
                {
                    var connectionLine = GraphView.CreateConnection(connection.NodeA.NodeButton, connection.NodeB.NodeButton);
                }
            }
        }
    }
}