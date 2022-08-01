using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "Ability1", menuName = "Abilities/Ability1")]
    public class AbilityDummy1: Ability
    {
        public override void Activate(Character character) { }

        public override void Deactivate(Character character) { }
    }
    
}