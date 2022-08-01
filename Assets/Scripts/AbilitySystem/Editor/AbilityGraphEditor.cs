using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AbilitySystem
{
    [CustomEditor(typeof(AbilityGraph))]
    public class AbilityGraphEditor : VisualTreeEditor<AbilityGraph>
    {
        #region Shorthands
        
        private static StyleEnum<DisplayStyle> display_flex = new (DisplayStyle.Flex);
        private static StyleEnum<DisplayStyle> display_none = new (DisplayStyle.None);

        #endregion
        
        private AbilityGraphView _graphView;

        private VisualElement 
                _root,
                _abilityNodesRoot,
                _nodeInspectorRoot;


        private AbilityButton _inspectedNodeButton;
        
        private VisualElement _dragHandle;
        private Vector2Field _nodePositionField;
        private IntegerField _learningCostField;
        private ObjectField _abilityField;
        private Slider _graphScaleSlider;
        private Button 
                _connectNodesButton,
                _deleteNodeButton,
                _disconnectButton;
        
        private Toggle _isRootToggle;

        private DragManipulator _buttonDragManipulator;

        #region Serialized Inspector Data

        [SerializeField]
        private float _graphScale = 1f;
        [SerializeField]
        private string _connectButtonText = "Connect...";
        [SerializeField]
        private AbilityGraphNode _inspectedAbilityNode;
        
        #endregion
        
        private Dictionary<NodeConnection, NodeConnectionLine> _connectionLines = new ();
        private bool _buttonPickMode;

        private void OnDisable()
        {
            AssetDatabase.SaveAssetIfDirty(inspectedObject);
            _connectionLines.Clear();
        }

        protected override void OnInspectorTreeCreated()
        {
            rootVisualElement.Q<Button>("new-node-button").clicked += CreateNodeClicked;
            (_connectNodesButton = rootVisualElement.Q<Button>("connect-node-button")).clicked += CreateConnectionClicked;
            (_deleteNodeButton = rootVisualElement.Q<Button>("delete-node-button")).clicked += DeleteNodeClicked;
            (_disconnectButton = rootVisualElement.Q<Button>("disconnect-button")).clicked += DisconnectClicked;
            (_isRootToggle = rootVisualElement.Q<Toggle>("root-toggle")).RegisterCallback<ChangeEvent<bool>>(HandleRootToggleClicked);
            
            _nodePositionField = rootVisualElement.Q<Vector2Field>("graph-position-field");
            _abilityField = rootVisualElement.Q<ObjectField>("ability-field");
            _abilityField.RegisterCallback<ChangeEvent<Object>>(HandleAbilityChangedEvent);
            _learningCostField = rootVisualElement.Q<IntegerField>("learning-cost-field");
            
            _graphView = rootVisualElement.Q<AbilityGraphView>();
            _graphView.parent.AddManipulator(new DragManipulator(MouseButton.MiddleMouse, _graphView));
            
            _nodeInspectorRoot = rootVisualElement.Q("node-inspector");
            NoNodeInspected();
            
            _graphScaleSlider = rootVisualElement.Q<Slider>("graph-scale-slider");
            _graphScaleSlider.RegisterCallback<ChangeEvent<float>>(x=>
            {
                _graphView.transform.scale = Vector3.one * (1f / (_graphScale = x.newValue));
                if (_buttonDragManipulator == null) return;
                _buttonDragManipulator.DragScale = _graphScale;
            });
            
            CreateDragHandle();
            
            foreach (var graphNode in inspectedObject.Nodes)
            {
                CreateButton(graphNode);
            }
            
            DelayedDrawConnections();
        }


        private void DisconnectClicked()
        {
            if (_inspectedAbilityNode)
            {
                for (var index = 0; index < _inspectedAbilityNode.Connections.Count; index++)
                {
                    var nodeConnection = _inspectedAbilityNode.Connections[index];
                    
                    nodeConnection.Other(_inspectedAbilityNode).Connections.Remove(nodeConnection);
                    _graphView.Remove(_connectionLines[nodeConnection]);
                    _connectionLines.Remove(nodeConnection);
                    inspectedObject.RemoveConnection(nodeConnection);
                    AssetDatabase.RemoveObjectFromAsset(nodeConnection);
                }
                _graphView.MarkDirtyRepaint();
                _inspectedAbilityNode.Connections.Clear();
                serializedObject.Update();
            }
        }

        private async void DelayedDrawConnections()
        {
            await Task.Delay(2);
            foreach (var connection in inspectedObject.Connections)
            {
                CreateConnectionLine(connection);
            }
        }

        private void CreateDragHandle()
        {
            _dragHandle = new VisualElement();
            _dragHandle.AddToClassList("handle-drag");
            _buttonDragManipulator = new DragManipulator(MouseButton.LeftMouse);
            _buttonDragManipulator.OnDragged += ButtonDragged;
            _dragHandle.AddManipulator(_buttonDragManipulator);
            _nodeInspectorRoot.Add(_dragHandle);
        }

        private void ActivateDragHandle(VisualElement targetElement)
        {
            targetElement.Add(_dragHandle);
            _buttonDragManipulator.DraggedElement = targetElement;
        }

        private void DeactivateDragHandle()
        {
            _nodeInspectorRoot.Add(_dragHandle);
        }

        private void HandleAbilityChangedEvent(ChangeEvent<Object> evt)
        {
            if (evt.newValue && evt.newValue is Ability ability)
            {
                _inspectedAbilityNode.NodeButton.style.backgroundImage = ability.Icon;
            }
            _inspectedAbilityNode.Ability = evt.newValue as Ability;
            serializedObject.Update();
        }

        private void CreateNodeClicked()
        {
            var node = CreateInstance<AbilityGraphNode>();
            node.name = "Ability Node";
            AssetDatabase.AddObjectToAsset(node, AssetDatabase.GetAssetPath(inspectedObject));
            var button = CreateButton(node);
            inspectedObject.AddNode(node);
            serializedObject.Update();
            
            if (_inspectedNodeButton != null)
            {
                CreateConnection(_inspectedNodeButton.Node, node);
            }
            InspectNode(button);
        }

        private void HandleButtonClickEvent(ClickEvent evt)
        {
            if (evt.target is not AbilityButton button) return;
            if (_buttonPickMode)
            {
                CreateConnection(_inspectedNodeButton.Node, button.Node);
            }
            InspectNode(button);
        }

        private void CreateConnection(AbilityGraphNode a, AbilityGraphNode b)
        {
            var connection = CreateInstance<NodeConnection>();
            connection.Connect(a, b);
            connection.name = "Connection";
            if (!inspectedObject.AddConnection(connection))
            {
                ExitButtonPickMode();
                return;
            }
            AssetDatabase.AddObjectToAsset(connection, AssetDatabase.GetAssetPath(inspectedObject));
            serializedObject.Update();
            CreateConnectionLineDelayed(connection);
            ExitButtonPickMode();
            _graphView.MarkDirtyRepaint();
        }

        private void HandleRootToggleClicked(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                inspectedObject.RootNode = _inspectedAbilityNode;
                _isRootToggle.SetEnabled(false);
            }
        }

        private AbilityButton CreateButton(AbilityGraphNode node)
        {
            var newButton = _graphView.CreateNodeButton(node);
            newButton.RegisterCallback<ClickEvent>(HandleButtonClickEvent);
            node.NodeButton = newButton;
            return newButton;
        }

        private void CreateConnectionClicked()
        {
            if (!_buttonPickMode)
            {
                EnterButtonPickMode();
                return;
            }
            ExitButtonPickMode();
        }

        private void EnterButtonPickMode()
        {
            _buttonPickMode = true;
            _connectNodesButton.text = "Cancel connection";
        }

        private void ExitButtonPickMode()
        {
            _buttonPickMode = false;
            _connectNodesButton.text = "Connect...";
        }

        private void CreateConnectionLine(NodeConnection connection)
        {
            var buttonA = connection.NodeA.NodeButton;
            var buttonB = connection.NodeB.NodeButton;
            var line = _graphView.CreateConnection(buttonA, buttonB);
            buttonA.RegisterCallback<ElementDraggedEvent>(line.HandleStartDraggedEvent);
            buttonB.RegisterCallback<ElementDraggedEvent>(line.HandleEndDraggedEvent);
            _connectionLines.Add(connection, line);
        }
        
        
        private async void CreateConnectionLineDelayed(NodeConnection connection)
        {
            await Task.Delay(4);
            CreateConnectionLine(connection);
        }

        private void DestroyConnectionLine(NodeConnection connection)
        {
            if (_connectionLines.TryGetValue(connection, out var line))
            {
                connection.NodeA.NodeButton.UnregisterCallback<ElementDraggedEvent>(line.HandleStartDraggedEvent);
                connection.NodeB.NodeButton.UnregisterCallback<ElementDraggedEvent>(line.HandleEndDraggedEvent);
                _connectionLines.Remove(connection);
            }
        }

        private void DeleteNodeClicked()
        {
            if (_inspectedAbilityNode == null) return;
            AssetDatabase.RemoveObjectFromAsset(_inspectedAbilityNode);
            _nodePositionField.Unbind();
            inspectedObject.Nodes.Remove(_inspectedAbilityNode);
            
            foreach (var connection in _inspectedAbilityNode.Connections)
            {
                AssetDatabase.RemoveObjectFromAsset(connection);
                DestroyConnectionLine(connection);
                inspectedObject.RemoveConnection(connection);
            }
            
            _inspectedAbilityNode.Connections.Clear();
            _graphView.RemoveAbilityButton(_inspectedNodeButton);
            DeactivateDragHandle();
            _inspectedAbilityNode = null;
            _inspectedNodeButton = null;
            NoNodeInspected();
            serializedObject.Update();
        }

        private void InspectNode(AbilityButton button)
        {
            if (_inspectedNodeButton == button) return;
            
            if (_inspectedNodeButton != null)
            {
                _graphView.StopSelecting(_inspectedNodeButton);
                foreach (var connection in _inspectedNodeButton.Node.Connections)
                {
                    if (_connectionLines.TryGetValue(connection, out var line))
                    {
                        _inspectedNodeButton.UnregisterCallback<ElementDraggedEvent>(line.HandleStartDraggedEvent);
                        _inspectedNodeButton.UnregisterCallback<ElementDraggedEvent>(line.HandleEndDraggedEvent);
                    }
                }
            }
            _nodePositionField.Unbind();
            _abilityField.Unbind();
            _learningCostField.Unbind();
            
            _inspectedAbilityNode = button.Node;
            _inspectedNodeButton = button;
            _connectNodesButton.style.display =
                    _deleteNodeButton.style.display =
                            _disconnectButton.style.display =
                                _nodeInspectorRoot.style.display = display_flex;
            
            _graphView.SetSelectedState(_inspectedNodeButton);
            var isRoot = _inspectedAbilityNode == inspectedObject.RootNode;
            _isRootToggle.value = isRoot;
            _isRootToggle.SetEnabled(!isRoot);
            
            foreach (var connection in _inspectedNodeButton.Node.Connections)
            {
                if (_connectionLines.TryGetValue(connection, out var line))
                {
                    _inspectedNodeButton.RegisterCallback<ElementDraggedEvent>(
                            connection.NodeA == _inspectedAbilityNode? 
                                    line.HandleStartDraggedEvent : 
                                    line.HandleEndDraggedEvent);
                    
                }
            }

            ActivateDragHandle(_inspectedNodeButton);
            var so = new SerializedObject(_inspectedAbilityNode);
            _nodePositionField.Bind(so);
            _abilityField.Bind(so);
            _learningCostField.Bind(so);
        }

        private void NoNodeInspected()
        {
            _connectNodesButton.style.display =
                    _deleteNodeButton.style.display = 
                            _disconnectButton.style.display =
                                    _nodeInspectorRoot.style.display = display_none;
        }

        private void ButtonDragged(Vector2 delta)
        {
            _nodePositionField.value += delta;
            if (_inspectedAbilityNode != null)
            {
                foreach (var connection in _inspectedAbilityNode.Connections)
                {
                    if (_connectionLines.TryGetValue(connection, out var line))
                    {
                        if (_inspectedAbilityNode == connection.NodeA) 
                            line.Start += delta * _graphScale;
                        else 
                            line.End += delta * _graphScale;
                    }
                }
            }
        }
    }
}