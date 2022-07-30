using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    public class AbilityGraphNode : ScriptableObject
    {
        public static string POSITION_PROP_NAME => nameof(_position);
        public static string ABILITY_PROP_NAME => nameof(_ability);
        public List<AbilityLearnRequirement> Requirements => _requirements;
        public List<NodeConnection> Connections => _connections;

        [SerializeField]
        private Vector2 _position;
        
        [SerializeReference]
        private Ability _ability;

        [SerializeField]
        private List<NodeConnection> _connections = new ();

        [SerializeReference]
        private List<AbilityLearnRequirement> _requirements = new();

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


        public bool ConnectedWith(AbilityGraphNode node, bool directly = false)
        {
            if (directly)
            {
                var connection = _connections.Find(x => x.IsConnecting(node, true));
                return connection != null;
            }
            
            foreach (var connection in _connections)
            {
                if (connection.IsConnecting(node)) return true;
            }

            return false;
        }
        
        public void AddConnection(NodeConnection connection)
        {
            _connections.Add(connection);
        }

        public void RemoveConnection(NodeConnection connection)
        {
            _connections.Remove(connection);
        }

        public void AddRequirement(AbilityLearnRequirement requirement) => 
                _requirements.Add(requirement);

        public void RemoveRequirement(AbilityLearnRequirement requirement) => 
                _requirements.Remove(requirement);
    }
}