using System;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    public class AbilityManagerPanelController : UIPanelController
    {
        public InputActionReference TogglePanelAction;
        public AbilityGraphView GraphView { get; protected set; }

        private void Awake()
        {
            TogglePanelAction.action.Enable();
        }

        private void OnDestroy()
        {
            TogglePanelAction.action.Disable();
        }

        protected override void Initialize()
        {
            GraphView = Root.Q<AbilityGraphView>();
            GraphView.BindToManager(AbilityManager.Instance);
            AbilityManager.Instance.OnCharacterSelected += CharacterSelected;
            TogglePanelAction.action.performed += Toggle;
        }

        private void CharacterSelected(Character character)
        {
            GraphView.ClearAbilityButtons();
            foreach (var graphNode in character.AbilityGraph.Nodes)
            {
                GraphView.CreateNodeView(graphNode);
            }
        }

        private void Toggle(InputAction.CallbackContext context)
        {
            TogglePanel();
        }
    }
}