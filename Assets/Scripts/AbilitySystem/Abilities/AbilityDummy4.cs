using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "Ability4", menuName = "Abilities/Ability4")]
    public class AbilityDummy4: Ability
    {
        public override void Activate(Character character) { }

        public override void Deactivate(Character character) { }
    }
}