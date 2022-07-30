using System;
using System.Linq;
using PixelHunt;
using UnityEngine;

namespace AbilitySystem
{
    public class AbilityManager : SingletonMonobehaviour<AbilityManager>
    {
        public Character SelectedCharacter => _selectedCharacter;
        [SerializeField]
        protected Character _selectedCharacter;
        [SerializeField]
        private AbilityManagerPanelController _panelController;
        protected AbilityGraphNode _selectedAbilityNode;

        public Action<AbilityGraphNode> OnAbilityNodeSelected;
        public Action<Character> OnCharacterSelected;

        private void Start()
        {
            if (_selectedCharacter)
            {
                //OnCharacterSelected?.Invoke(_selectedCharacter);
            }
        }

        public void SelectCharacter(Character character)
        {
            _selectedCharacter = character;
            OnCharacterSelected?.Invoke(character);
        }

        public void SelectAbilityTreeNode(AbilityGraphNode abilityNode) => _selectedAbilityNode = abilityNode;

        public void LearnSelected()
        {
            if (_selectedCharacter == null || _selectedAbilityNode == null)
            {
                return;
            }

            if (_selectedAbilityNode.Requirements.All(r => r.IsSatisfied(_selectedAbilityNode, _selectedCharacter)))
            {
                _selectedCharacter.AddAbility(_selectedAbilityNode.Ability);
                _selectedAbilityNode.Ability.Activate(_selectedCharacter);
            }
        }

        public void ForgetSelected()
        {
            // node should have only one connection
            // or if more than one, should check the remaining connections of the connected nodes
            var abilityConnections = _selectedAbilityNode.Connections;
            var selectedGraphRoot = _selectedCharacter.AbilityGraph.RootNode;
            var canBeForgotten = _selectedAbilityNode != selectedGraphRoot && _selectedAbilityNode.Connections.Count() == 1 || 
                                     (from connection in _selectedAbilityNode.Connections select connection.Other(_selectedAbilityNode)).
                                     All(n => n.Connections.All(
                                             c => c.IsConnecting(selectedGraphRoot, abilityConnections.ToList())));

            if (_selectedCharacter == null || _selectedAbilityNode == null || !canBeForgotten)
            {
                return;
            }
        }

        public bool IsAbilityLeared(AbilityGraphNode node)
        {
            if (_selectedCharacter == null)
            {
                throw new NullReferenceException("Character must be selected");
            }

            return _selectedCharacter.HasAbility(node.Ability);
        }
    }
}