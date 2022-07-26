using UnityEngine.UIElements;

namespace AbilitySystem
{
    public class AbilityManagerPanelController : UIPanelController
    {
        public AbilityGraphView GraphView { get; protected set; }
        protected override void Initialize()
        {
            GraphView = Root.Q<AbilityGraphView>();
            GraphView.BindToManager(AbilityManager.Instance);
            AbilityManager.Instance.OnCharacterSelected += CharacterSelected;
        }

        private void CharacterSelected(Character character)
        {
            GraphView.ClearAbilityButtons();
            foreach (var graphNode in character.AbilityGraph.Nodes)
            {
                GraphView.CreateNodeView(graphNode);
            }
        }
    }
}