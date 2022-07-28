using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "New Ability Graph", menuName = "Abilities/Ability Graph", order = 0)]
    public class AbilityGraph : ScriptableObject
    {
        public List<AbilityGraphNode> Nodes => _nodes;
        public IEnumerable<GraphNodeConnection> Connections => _connections;
        public AbilityGraphNode RootNode => _root;
        
        public Action<AbilityGraphNode> OnAbilityNodeCreated;
        public Action<AbilityGraphNode> OnAbilityNodeRemoved;
        
        [SerializeReference]
        private AbilityGraphNode _root;
        
        [SerializeField]
        private List<AbilityGraphNode> _nodes = new();

        [SerializeField]
        private List<GraphNodeConnection> _connections = new();


        public AbilityGraphNode CreateNode()
        {
            var node = new AbilityGraphNode(null, this);
            _nodes.Add(node);
            OnAbilityNodeCreated?.Invoke(node);
            return node;
        }

        public void RemoveNode(AbilityGraphNode node)
        {
            _nodes.Remove(node);
            OnAbilityNodeRemoved?.Invoke(node);
        }

        public void Connect(AbilityGraphNode nodeA, AbilityGraphNode nodeB)
        {
            var newConnection = new GraphNodeConnection(nodeA, nodeB);
            if (!_connections.Contains(newConnection))
            {
                _connections.Add(newConnection);
                nodeA.AddConnection(newConnection);
                nodeB.AddConnection(newConnection);
            }
        }

        public void Disconnect(AbilityGraphNode nodeA, AbilityGraphNode nodeB)
        {
            var connection = _connections.Find(x => x.Connects(nodeA) && x.Connects(nodeB));
            if (connection != null)
            {
                _connections.Remove(connection);
                nodeA.RemoveConnection(connection);
                nodeB.RemoveConnection(connection);
            }
        }
    }
}