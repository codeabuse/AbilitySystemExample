using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "New Ability Graph", menuName = "Abilities/Ability Graph", order = 0)]
    public class AbilityGraph : ScriptableObject
    {
        public Action<AbilityGraphNode> OnAbilityNodeCreated;
        public Action<AbilityGraphNode> OnAbilityNodeRemoved;
        public Action<AbilityGraphNode> OnAbilityNodeSelected;
        // stores abilities as nodes
        // connects nodes into graph
        [SerializeReference]
        private AbilityGraphNode _root;
        
        [SerializeField]
        private List<AbilityGraphNode> _nodes = new();

        [SerializeField]
        private List<GraphNodeConnection> _connections = new();


        public AbilityGraphNode RootNode => _root;

        public IEnumerable<AbilityGraphNode> Nodes => _nodes;
        public IEnumerable<GraphNodeConnection> Connections => _connections;

        public AbilityGraphNode CreateNode()
        {
            var node = new AbilityGraphNode();
            _nodes.Add(node);
            OnAbilityNodeCreated?.Invoke(node);
            Debug.Log($"{_nodes.Count} nodes");
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