using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    public class Character : MonoBehaviour
    {
        public int AbilityPoints => _abilityPoints;
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

        public void AddAbility(Ability ability)
        {
            _abilities.Add(ability);
        }

        public void RemoveAbility(Ability ability)
        {
            _abilities.Remove(ability);
        }

        public bool HasAbility(Ability ability)
        {
            return _abilities.Contains(ability);
        }
    }
}