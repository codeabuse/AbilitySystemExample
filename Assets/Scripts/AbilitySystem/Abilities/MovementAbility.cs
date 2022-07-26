using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "Movement Ability", menuName = "Abilities/Movement Ability")]
    public class MovementAbility : Ability
    {
        public override void Activate(Character character)
        {
            character.gameObject.AddComponent<CharacterController>();
        }

        public override void Deactivate(Character character)
        {
            Destroy(character.gameObject.GetComponent<CharacterController>());
        }
    }
}