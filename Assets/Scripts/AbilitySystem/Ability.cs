using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    public abstract class Ability : ScriptableObject
    {
        public int DefaultCost => _cost;
        public string Description => _description;
        public Texture2D Icon => _icon;
        public IEnumerable<AbilityLearnRequirement> Requirements => _requirements;
        public string DescriptiveName => _descriptiveName;

        [SerializeField]
        private int _cost;

        [SerializeField]
        private string _descriptiveName;
        
        [SerializeField]
        private string _description;

        [SerializeField] 
        private Texture2D _icon;

        [SerializeReference] 
        protected List<AbilityLearnRequirement> _requirements = new();

        public abstract void Activate(Character character);

        public abstract void Deactivate(Character character);
    }
}