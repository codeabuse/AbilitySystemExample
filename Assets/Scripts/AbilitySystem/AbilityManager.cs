using System;
using System.Linq;
using PixelHunt;
using UnityEngine;

namespace AbilitySystem
{
    public class AbilityManager : SingletonMonobehaviour<AbilityManager>
    {
        [SerializeField]
        protected Character _selectedCharacter;
        [SerializeField]
        protected AbilityGraphNode _selectedAbilityNode;

        public Action<AbilityGraphNode> OnAbilityNodeSelected;
        public Action<Character> OnCharacterSelected;
        
        // get character
        // obtain his ability tree
        // get learned abilities to build tree status
        // learn/forget abilities

        public void SelectCharacter(Character character) => _selectedCharacter = character;

        public void SelectAbilityTreeNode(AbilityGraphNode abilityNode) => _selectedAbilityNode = abilityNode;

        public void LearnSelected()
        {
            if (_selectedCharacter == null || _selectedAbilityNode == null)
            {
                return;
            }

            if (_selectedAbilityNode.Ability.Requirements.All(r => r.IsSatisfied(_selectedAbilityNode, _selectedCharacter)))
            {
                _selectedCharacter.AddAbility(_selectedAbilityNode.Ability);
                _selectedAbilityNode.Ability.Activate(_selectedCharacter);
            }
        }

        public void ForgetSelected()
        {
            if (_selectedCharacter == null || _selectedAbilityNode == null || _selectedAbilityNode.CanBeForgotten)
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