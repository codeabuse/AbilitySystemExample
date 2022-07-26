using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AbilitySystem
{
    [Serializable]
    public class AbilityGraphNode
    {
        public static string POSITION_PROP_NAME => nameof(_position);
        public static string ABILITY_PROP_NAME => nameof(_ability);
        public static string LEARNING_COST_PROP_NAME => nameof(_learningCost);
        
        [SerializeField]
        private Vector2 _position;
        
        [SerializeReference]
        private Ability _ability;

        [SerializeField]
        private int _learningCost;

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
        
        // node should have only one connection
        // or if more than one, should check the remaining connections of the connected nodes
        public bool CanBeForgotten => Graph.RootNode != this && _connections.Count == 1 || 
                                 (from connection in _connections select connection.Other(this)).
                                 All(n => n._connections.All(c => c.Connects(Graph.RootNode, _connections)));

        public int LearningCost => _learningCost;
        public Ability Ability 
        { 
            get => _ability;
            set
            {
                _ability = value;
                _learningCost = _ability.DefaultCost;
            }
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
    }
}