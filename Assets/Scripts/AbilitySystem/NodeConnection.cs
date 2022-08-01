using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    [Serializable]
    public class NodeConnection : ScriptableObject
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

        public AbilityGraphNode NodeA
        {
            get => _nodeA;
            set
            {
                if (_nodeA)
                {
                    _nodeA.RemoveConnection(this);
                }

                _nodeA = value;
            }
        }
        public AbilityGraphNode NodeB {
            get => _nodeB;
            set
            {
                if (_nodeB)
                {
                    _nodeB.RemoveConnection(this);
                }

                _nodeB = value;
            }
        }

        public void Connect(AbilityGraphNode nodeA, AbilityGraphNode nodeB)
        {
            if (nodeA == null || nodeB == null)
                throw new NullReferenceException("Nodes can't be null!");
            if (nodeA == nodeB)
            {
                Debug.LogError("Connecting Node to itself is not allowed!");
                return;
            }
            (_nodeA, _nodeB) = (nodeA, nodeB);
            _nodeA.AddConnection(this);
            _nodeB.AddConnection(this);
        }

        public void Swap() => (_nodeA, _nodeB) = (_nodeB, _nodeA);

        // public bool IsConnecting(AbilityGraphNode node, bool directly = false)
        // {
        //     return directly? node == _nodeA || node == _nodeB : 
        //             IsConnecting(node, new List<NodeConnection>{this});
        // }

        // public bool IsConnecting(AbilityGraphNode node, List<NodeConnection> excludeFromCheck, Predicate<AbilityGraphNode> nodePassingRule = null)
        // {
        //     if (nodePassingRule == null) nodePassingRule = graphNode => true;
        //     if (excludeFromCheck == null) excludeFromCheck = new();
        //     foreach (var connection in node.Connections)
        //     {
        //         if (excludeFromCheck.Contains(connection))
        //             continue;
        //         if (connection.IsConnecting(node, true) && nodePassingRule(connection.Other(node)))
        //             return true;
        //         excludeFromCheck.Add(connection);
        //         if (connection.IsConnecting(node, excludeFromCheck) && nodePassingRule(connection.Other(node))) 
        //             return true;
        //     }
        //
        //     return false;
        // }

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
            if (obj is NodeConnection otherConnection)
                return Equals(otherConnection);
            return false;
        }

        protected bool Equals(NodeConnection other)
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