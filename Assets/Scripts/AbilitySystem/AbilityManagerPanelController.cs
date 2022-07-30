using System.Threading.Tasks;
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
        
        public VisualTreeAsset InspectorVisualTree;
        protected AbilityGraphView GraphView { get; set; }

        private VisualElement _abilityInspectorRoot;
        private Label
                _abilityLabel,
                _abilityDescription,
                _abilityCost;

        protected void Awake()
        {
            AbilityManager.Instance.OnCharacterSelected += CharacterSelected;
            AbilityManager.Instance.OnAbilityNodeSelected += InspectNode;
        }

        protected override void OnDocumentLayoutDone()
        {
            GraphView = Root.Q<AbilityGraphView>();
            _abilityInspectorRoot = Document.rootVisualElement.Q("ability-inspector");
            _abilityInspectorRoot.style.display = display_none;
            _abilityLabel = Document.rootVisualElement.Q<Label>("AbilityTitle");
            _abilityDescription = Document.rootVisualElement.Q<Label>("AbilityDescription");
            _abilityCost = Document.rootVisualElement.Q<Label>("AbilityCostValue");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            OnDocumentLayoutDone();
            if (AbilityManager.Instance.SelectedCharacter)
            {
                BuildGraph(AbilityManager.Instance.SelectedCharacter.AbilityGraph);
            }
        }

        private void CharacterSelected(Character character)
        {
            BuildGraph(character.AbilityGraph);
        }
        
        private void InspectNode(AbilityGraphNode node)
        {
            AbilityManager.Instance.SelectAbilityTreeNode(node);
            //if (node.Ability == null) return;
            _abilityInspectorRoot.style.display = display_flex;
            _abilityLabel.text = node.Ability?.DescriptiveName ?? string.Empty;
            _abilityDescription.text = node.Ability?.Description ?? string.Empty;
            _abilityCost.text = node.LearningCost.ToString();
        }

        private void BuildGraph(AbilityGraph characterAbilityGraph)
        {
            foreach (var node in characterAbilityGraph.Nodes)
            {
                 var button = GraphView.CreateNodeButton(node);
                 button.clicked += () => InspectNode(node);
            }
            
            CreateConnectionLines(2);
            
            async void CreateConnectionLines(int milliseconds)
            {
                await Task.Delay(milliseconds);
                foreach (var connection in characterAbilityGraph.Connections)
                {
                    var connectionLine = GraphView.CreateConnection(connection);
                }
            }
        }
    }
}