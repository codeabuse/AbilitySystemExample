using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    [Serializable]
    public class AbilityGraphNode
    {
        public static string POSITION_PROP_NAME => nameof(_position);
        public static string ABILITY_PROP_NAME => nameof(_ability);
        public IEnumerable<AbilityLearnRequirement> Requirements => _requirements;

        [SerializeField]
        private Vector2 _position;
        
        [SerializeReference]
        private Ability _ability;

        [SerializeReference]
        private List<GraphNodeConnection> _connections = new ();

        [SerializeReference]
        private List<AbilityLearnRequirement> _requirements = new();

        public AbilityGraph Graph { get; }

        public Vector2 Position
        {
            get => _position; 
            set => _position = value;
        }
        
        
        public int LearningCost => (_requirements.Find(x => x is AbilityPointsRequirement) as AbilityPointsRequirement)?.LearningCost?? 0;
        
        public Ability Ability 
        { 
            get => _ability;
            set => _ability = value;
        }

        public IEnumerable<GraphNodeConnection> Connections => _connections;

        public AbilityGraphNode(Ability ability, AbilityGraph graph)
        {
            _ability = ability;
            Graph = graph;
        }

        public AbilityGraphNode() { }

        public bool ConnectedWith(AbilityGraphNode node, bool directly = false)
        {
            if (directly)
            {
                var connection = _connections.Find(x => x.Connects(node, true));
                return connection != null;
            }
            
            foreach (var connection in _connections)
            {
                if (connection.Connects(node)) return true;
            }

            return false;
        }
        
        public void AddConnection(GraphNodeConnection connection)
        {
            _connections.Add(connection);
        }

        public void RemoveConnection(GraphNodeConnection connection)
        {
            _connections.Remove(connection);
        }

        public void AddRequirement(AbilityLearnRequirement requirement) => 
                _requirements.Add(requirement);

        public void RemoveRequirement(AbilityLearnRequirement requirement) => 
                _requirements.Remove(requirement);
    }
}