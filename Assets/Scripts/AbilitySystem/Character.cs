using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    public class Character : MonoBehaviour
    {
        public int AbilityPoints
        {
            get => _abilityPoints; 
            set => _abilityPoints = Mathf.Clamp(value, 0, int.MaxValue);
        }
        public IEnumerable<Ability> Abilities => _abilities;
        public AbilityGraph AbilityGraph => _abilityGraph;
        
        [SerializeField]
        protected int _abilityPoints;

        [SerializeField]
        protected List<Ability> _abilities = new();

        [SerializeField]
        protected AbilityGraph _abilityGraph;

        private void Start()
        {
            foreach (var ability in _abilities)
            {
                ability.Activate(this);
            }
        }

        public bool HasAbility(Ability ability)
        {
            return _abilities.Contains(ability);
        }
        
        public void AddAbility(Ability ability)
        {
            _abilities.Add(ability);
        }

        public void RemoveAbility(Ability ability)
        {
            _abilities.Remove(ability);
        }

        public void RemoveAllAbilities()
        {
            var rootAbility = _abilityGraph.RootNode.Ability;
            foreach (var ability in _abilities)
            {
                if (ability == rootAbility)
                {
                    continue;
                }
                ability.Deactivate(this);
            }
            _abilities.Clear();
            _abilities.Add(rootAbility);
        }
    }
}