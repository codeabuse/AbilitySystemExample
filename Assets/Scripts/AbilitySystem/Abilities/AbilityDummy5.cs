using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "Ability5", menuName = "Abilities/Ability5")]
    public class AbilityDummy5: Ability
    {
        public override void Activate(Character character) { }

        public override void Deactivate(Character character) { }
    }
}