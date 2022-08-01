using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "Ability3", menuName = "Abilities/Ability3")]
    public class AbilityDummy3: Ability
    {
        public override void Activate(Character character) { }

        public override void Deactivate(Character character) { }
    }
}