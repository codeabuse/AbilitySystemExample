using System.Collections.Generic;
using System.Linq;

namespace AbilitySystem
{
    public abstract class AbilityLearnRequirement
    {
        public static IEnumerable<AbilityLearnRequirement> DefaultRequirements =>
                new AbilityLearnRequirement[]
                {
                        new AnyConnectionRequirement(),
                        new AbilityPointsRequirement()
                };
        public abstract bool IsSatisfied(AbilityGraphNode node, Character character);
    }

    public class AnyConnectionRequirement : AbilityLearnRequirement
    {
        public override bool IsSatisfied(AbilityGraphNode node, Character character)
        {
            return node.Connections.Any(connection =>
            {
                var otherNode = connection.Other(node);
                return connection.OneWay
                        ? AbilityManager.Instance.IsAbilityLeared(otherNode) && otherNode == connection.NodeA  //its dependency node is learned
                        : AbilityManager.Instance.IsAbilityLeared(otherNode); //or the connected ability is just leaned
            });
        }
        
        public override string ToString()
        {
            return $"Any connected ability should be learned";
        }
    }

    public class AbilityPointsRequirement : AbilityLearnRequirement
    {
        public int LearningCost;
        
        public override bool IsSatisfied(AbilityGraphNode node, Character character)
        {
            return character.AbilityPoints >= node.LearningCost;
        }
        public override string ToString()
        {
            return $"Ability points required: {LearningCost}";
        }
    }
}