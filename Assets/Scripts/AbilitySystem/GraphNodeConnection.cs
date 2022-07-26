using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    [Serializable]
    public class GraphNodeConnection
    {
        [SerializeField]
        [Tooltip("In One way connection Node A is the main and Node B depends on it")]
        private bool _oneWay;
        [SerializeReference]
        private AbilityGraphNode _nodeA;
        [SerializeReference]
        private AbilityGraphNode _nodeB;

        public bool OneWay
        {
            get => _oneWay;
            set => _oneWay = value;
        }
        
        public AbilityGraphNode NodeA => _nodeA;
        public AbilityGraphNode NodeB => _nodeB;

        public GraphNodeConnection(AbilityGraphNode nodeA, AbilityGraphNode nodeB)
        {
            if (nodeA == null || nodeB == null)
                throw new NullReferenceException("Nodes can't be null!");
            (_nodeA, _nodeB) = (nodeA, nodeB);
        }

        protected GraphNodeConnection() { }

        public bool Connects(AbilityGraphNode node, bool directly = false)
        {
            return directly? node == _nodeA || node == _nodeB : 
                    Connects(node, new List<GraphNodeConnection>{this});
        }

        public bool Connects(AbilityGraphNode node, List<GraphNodeConnection> excludeFromCheck)
        {
            foreach (var connection in node.Connections)
            {
                if (excludeFromCheck.Contains(connection))
                    continue;
                if (connection.Connects(node, true))
                    return true;
                excludeFromCheck.Add(connection);
                if (connection.Connects(node, excludeFromCheck)) return true;
            }

            return false;
        }

        public AbilityGraphNode Other(AbilityGraphNode node)
        {
            return _nodeA == node ? 
                    _nodeB : 
                    _nodeB == node ? 
                            _nodeA : 
                            null;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is GraphNodeConnection otherConnection)
                return Equals(otherConnection);
            return false;
        }

        protected bool Equals(GraphNodeConnection other)
        {
            return _nodeA == other._nodeA && _nodeB == other._nodeB ||
                   _oneWay && other._oneWay && _nodeA == other._nodeB && _nodeB == other._nodeA;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NodeA, NodeB);
        }
    }
}