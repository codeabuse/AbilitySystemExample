using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    public abstract class Ability : ScriptableObject
    {
        public string DescriptiveName => _descriptiveName;
        public string Description => _description;
        public Texture2D Icon => _icon;

        [SerializeField]
        private string _descriptiveName;
        
        [SerializeField]
        private string _description;

        [SerializeField] 
        private Texture2D _icon;

        public abstract void Activate(Character character);

        public abstract void Deactivate(Character character);
    }
}