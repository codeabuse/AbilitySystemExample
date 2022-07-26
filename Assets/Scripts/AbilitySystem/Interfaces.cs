using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    public interface IMonoBehaviour
    {
        string name { get; set; }
        GameObject gameObject { get; }
        Transform transform { get; }
    }
    
    public interface IAbility
    {
        int DefaultCost { get; }
        string Description { get; }
        void Activate(ICharacter character);
        void Deactivate(ICharacter character);
    }

    public interface IAbilityLearnRequirement
    {
        bool IsSatisfied { get; }
    }

    public interface ICharacter : IMonoBehaviour
    {
        int AbilityPoints { get; }
        AbilityGraph Abilities { get; }
        bool HasAbilityOfType(Type abilityType);
        bool HasAbility(IAbility node);
        void AddAbility(IAbility ability);
        void RemoveAbility(IAbility ability);

        void ReceiveAbilityPoints(int amount);
        void SpendAbilityPoints(int amount);
    }
}