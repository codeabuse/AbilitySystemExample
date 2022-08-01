using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "Ability2", menuName = "Abilities/Ability2")]
    public class AbilityDummy2: Ability
    {
        public override void Activate(Character character) { }

        public override void Deactivate(Character character) { }
    }
}