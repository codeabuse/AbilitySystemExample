using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AbilitySystem
{
    public class AbilityGraphNode : ScriptableObject
    {
        public Ability Ability 
        { 
            get => _ability;
            set => _ability = value;
        }
        
        public Vector2 GraphPosition
        {
            get => _graphPosition;
            set => _graphPosition = value;
        }

        public int LearningCost => _learningCost;
        
        
        public AbilityButton NodeButton { get; set; }
        
        public List<NodeConnection> Connections => _connections;

        [SerializeField, Min(0)]
        private int _learningCost;
        
        [SerializeReference]
        private Ability _ability;
        
        [SerializeField]
        private Vector2 _graphPosition;
        

        [SerializeField]
        private List<NodeConnection> _connections = new ();

        public bool CanTraverseTo(
                AbilityGraphNode targetNode, 
                List<AbilityGraphNode> excludeFromSearch, 
                Predicate<AbilityGraphNode> nodePassingRule = null)
        {
            excludeFromSearch ??= new();
            nodePassingRule ??= n => true;
            foreach (var connection in _connections)
            {
                var otherNode = connection.Other(this);
                if (excludeFromSearch.Contains(otherNode)) continue;
                if (otherNode == targetNode) return true;
                
                excludeFromSearch.Add(otherNode);
                if (nodePassingRule(otherNode))
                {
                    var canTraverse = otherNode.CanTraverseTo(targetNode, excludeFromSearch, nodePassingRule);
                    if (canTraverse) return true;
                }
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
    }
}