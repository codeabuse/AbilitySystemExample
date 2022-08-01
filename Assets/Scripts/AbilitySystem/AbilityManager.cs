using System;
using System.Collections;
using System.Linq;
using PixelHunt;
using UnityEngine;

namespace AbilitySystem
{
    public class AbilityManager : SingletonMonobehaviour<AbilityManager>
    {
        public Action<Character> OnCharacterSelected;
        
        public Character SelectedCharacter => _selectedCharacter;
        
        [SerializeField]
        protected Character _selectedCharacter;
        [SerializeField]
        private AbilityManagerPanelController _panelController;
        
        
        protected AbilityGraphNode _selectedAbilityNode;


        private IEnumerator Start()
        {
            yield return null;
            if (_selectedCharacter)
            {
                OnCharacterSelected?.Invoke(_selectedCharacter);
            }
        }

        public void SelectCharacter(Character character)
        {
            _selectedCharacter = character;
            OnCharacterSelected?.Invoke(character);
        }

        public void SelectAbilityTreeNode(AbilityGraphNode abilityNode) => _selectedAbilityNode = abilityNode;

        public void LearnSelectedAbility()
        {
            if (_selectedCharacter == null || _selectedAbilityNode == null)
            {
                return;
            }

            var connectedToLearnedAbilities =
                    _selectedAbilityNode.Connections.Any(c => IsAbilityLearned(c.Other(_selectedAbilityNode)));

            if (connectedToLearnedAbilities && _selectedCharacter.AbilityPoints >= _selectedAbilityNode.LearningCost)
            {
                _selectedCharacter.AbilityPoints -= _selectedAbilityNode.LearningCost;
                _selectedCharacter.AddAbility(_selectedAbilityNode.Ability);
                _selectedAbilityNode.Ability.Activate(_selectedCharacter);
                Debug.Log($"{_selectedAbilityNode.Ability.name} learned");
            }
        }

        public void ForgetSelectedAbility()
        {
            if (!_selectedCharacter) return;
            
            if (CanForgetAbility(_selectedAbilityNode))
            {
                var ability = _selectedAbilityNode.Ability;
                _selectedCharacter.RemoveAbility(ability);
                ability.Deactivate(_selectedCharacter);
                _selectedCharacter.AbilityPoints += _selectedAbilityNode.LearningCost;
                Debug.Log($"{_selectedAbilityNode.Ability.name} forgotten");
            }
        }

        public void ForgetAllAbilities()
        {
            if (!_selectedCharacter) return;

            foreach (var abilityNode in _selectedCharacter.AbilityGraph.Nodes)
            {
                _selectedCharacter.AbilityPoints += abilityNode.LearningCost;
            }

            _selectedCharacter.RemoveAllAbilities();
            Debug.Log("Abilities cleared");
        }

        public bool IsAbilityLearned(AbilityGraphNode node)
        {
            if (!_selectedCharacter)
            {
                Debug.LogError("Character must be selected");
                return false;
            }

            return _selectedCharacter.HasAbility(node.Ability);
        }

        public bool CanLearnAbility(AbilityGraphNode node)
        {
            return !IsAbilityLearned(node) && node.Connections.Any(c => IsAbilityLearned(c.Other(node)));
        }

        public bool CanForgetAbility(AbilityGraphNode node)
        {
            var selectedGraphRoot = _selectedCharacter.AbilityGraph.RootNode;
            if (node == selectedGraphRoot) return false;
            
            var connectedLearnedNodes = 
                    node.Connections.Where(c => IsAbilityLearned(c.Other(node)))
                           .Select(c => c.Other(node)).ToList();
            
            if (connectedLearnedNodes.Count == 1) return true;

            var learnedNodesStaysConnected = connectedLearnedNodes.All(
                    n => n.CanTraverseTo(selectedGraphRoot, new() { node }, IsAbilityLearned));
            
            return IsAbilityLearned(node) && learnedNodesStaysConnected;
        }
    }
}