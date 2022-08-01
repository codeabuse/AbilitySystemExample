using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "New Ability Graph", menuName = "Abilities/Ability Graph", order = 0)]
    public class AbilityGraph : ScriptableObject
    {
        public List<AbilityGraphNode> Nodes => _nodes;
        public List<NodeConnection> Connections => _connections;

        public AbilityGraphNode RootNode
        {
            get => _root;
            set => _root = value;
        }
        
        [SerializeField]
        private AbilityGraphNode _root;
        
        [SerializeField]
        private List<AbilityGraphNode> _nodes = new();

        [SerializeField]
        private List<NodeConnection> _connections = new();

        public void SetROot(AbilityGraphNode node) => RootNode = node;

        public void AddNode(AbilityGraphNode node)
        {
            _nodes.Add(node);
            if (!RootNode) RootNode = node;
        }

        public void RemoveNode(AbilityGraphNode node)
        {
            _nodes.Remove(node);
        }

        public bool AddConnection(NodeConnection connection)
        {
            var identicalConnection = _connections.Find(x => x == connection);
            if (identicalConnection == null)
            {
                _connections.Add(connection);
                return true;
            }

            return false;
        }

        public void RemoveConnection(NodeConnection connection)
        {
            _connections.Remove(connection);
        }
    }
}