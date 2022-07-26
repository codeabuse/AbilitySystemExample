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
    
    /*
     -- 
     The Concept:
     
     Hero should get Traits by learning Abilities
     Ability is a general design-level description, it contains:
     * Trait - an actual game mechanic that is added to the Character
     * Cost - in-game resources to exchange for
     * Prerequesites - other conditions that must be satisfied
     
     Traits are game mechanics: fireball, walking on water, increased armor or chance of miss
     So that the Trait could do many things that could be implemented in many different ways 
     to the Character's GameObject and its components, it should be pretty abstract, yet simple and as blind as possible
     
     Case: 'Movement' Ability
     * Need to be updated on Move Update phase
     * Need to be aware of the Environment and manipulate Character's Transform
     * Can be disabled or buffed by other Traits
     * Tells Character where and how far it could go
     * Trait is the object that 
     
     --
     Classes Layout:
     
    [Ability] : ScriptableObject
     * Storest the description, cost and traits
     * Gives/removes Traits to/from Character when activated/deactivated
     * 
    [Character] : MonoBehavior
     * stores learned Abilites
     * has a link to its abilities map 

    [Trait]
     * holds the info about the mechanic it applies to the Character and its status (permanently disabled or??)
    
     *
    [AbilityGraph] : ScriptableObject
     * 
    
    [AbilityManager] : Monobehaviour
     * allows user to learn/forget Character's abilities 
    */



    // What Character object should do:
}