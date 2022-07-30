using System.Collections.Generic;
using System.Linq;
using PixelHunt;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    [CustomEditor(typeof(AbilityGraph))]
    public class AbilityGraphEditor : Editor
    {
        private const string graph_asset_name = "Assets/Resources/UI/VisualTrees/ability-graph.uxml";
        private const string styles_asset_name = "Assets/Resources/UI/Styles/ability-management-styles.uss";


        private AbilityGraph _graph;
        private AbilityGraphView _graphView;
        private VisualTreeAsset _graphVisualTree;
        private StyleSheet _graphStyles;

        private VisualElement 
                _root,
                _abilityNodesRoot,
                _nodeInspectorRoot;
        

        private SerializedProperty _nodesProperty;

        private ListView _nodesListView;
        private Dictionary<int, AbilityButton> _nodeViews = new();

        private AbilityGraphNode _inspectedAbilityNode;
        private AbilityButton _inspectedNodeButton;
        private VisualElement _buttonDragHandle;
        private DragManipulator _buttonDragManipulator;
        private Vector2Field _nodePositionField;
        private ObjectField _nodeAbilityField;
        private Slider _graphScaleSlider;
        private Button _connectButton;

        private float _graphScale = 1f;
        private Dictionary<NodeConnection, NodeConnectionLine> _connectionLines = new ();
        private bool _buttonPickMode;

        private void OnEnable()
        {
            _graphVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(graph_asset_name);
            _graphStyles = AssetDatabase.LoadAssetAtPath<StyleSheet>(styles_asset_name);
            _root = new VisualElement();
            _root.styleSheets.Add(_graphStyles);
            _graph = target as AbilityGraph;
            _nodesProperty = serializedObject.FindProperty("_nodes");
        }

        private void OnDisable()
        {
            AssetDatabase.SaveAssetIfDirty(_graph);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _graphVisualTree.CloneTree(_root);
            _graphView = _root.Q<AbilityGraphView>();
            var buttonsRoot = new VisualElement()
            {
                    style =
                    {
                            flexGrow = .15f, 
                            flexShrink = 0,
                            flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                    }
            };
            buttonsRoot.Add(new Button(CreateNodeClicked){text = "Create Node"});
            buttonsRoot.Add(_connectButton = new Button(CreateConnectionClicked){text = "Connect..."});
            buttonsRoot.Add(new Button(DeleteNodeClicked){text = "Delete Node"});
            buttonsRoot.Add(_graphScaleSlider = new Slider( 0.5f, 3f)
            {
                    style = { flexGrow = 1},
                    //labelElement = { style = { flexShrink = 1, maxWidth = 50}}
            });
            _graphScaleSlider.value = _graphScale;
            _graphScaleSlider.RegisterCallback<ChangeEvent<float>>(x=>
            {
                _graphView.transform.scale = Vector3.one * (1f / (_graphScale = x.newValue));
                if (_buttonDragManipulator == null) return;
                _buttonDragManipulator.DragScale = _graphScale;
            });
            _root.Add(buttonsRoot);
            _root.Add(_nodeInspectorRoot = new ScrollView
            {
                    style = {flexGrow = 1, 
                            minHeight = 150,
                            display = new StyleEnum<DisplayStyle>(DisplayStyle.None)
                    }, 
                    horizontalScrollerVisibility = ScrollerVisibility.Hidden,
            });
            _nodeInspectorRoot.Add(new Label("Node Inspector"){style = { unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold)}});
            _nodePositionField = new Vector2Field("Position"){ bindingPath = "_position"};
            _nodeAbilityField = new ObjectField("Ability") { objectType = typeof(Ability), bindingPath = "_ability"};
            _nodeAbilityField.RegisterCallback<ChangeEvent<Object>>(HandleAbilityChangedEvent);
            _nodeInspectorRoot.Add(_nodePositionField);
            _nodeInspectorRoot.Add(_nodeAbilityField);

            _buttonDragHandle = new VisualElement();
            _buttonDragHandle.AddToClassList("handle-drag");
            _buttonDragManipulator = new DragManipulator(MouseButton.LeftMouse);
            _buttonDragManipulator.OnDragged += ButtonDragged;
            _buttonDragHandle.AddManipulator(_buttonDragManipulator);
            _nodeInspectorRoot.Add(_buttonDragHandle);
            foreach (var graphNode in _graph.Nodes)
            {
                CreateButton(graphNode);
            }
            
            EditorApplication.update += OnEditorUpdate;

            return _root;
        }

        private void HandleAbilityChangedEvent(ChangeEvent<Object> evt)
        {
            if (evt.newValue && evt.newValue is Ability ability && _inspectedAbilityNode)
            {
                _nodeViews[_inspectedAbilityNode.GetInstanceID()].style.backgroundImage = ability.Icon;
            }
        }

        private int skipUpdates = 1;

        private void OnEditorUpdate()
        {
            if (skipUpdates > 0)
            {
                skipUpdates--;
                return;
            }
            foreach (var connection in _graph.Connections)
            {
                CreateConnectionLine(connection);
            }
            EditorApplication.update -= OnEditorUpdate;
        }

        private void CreateNodeClicked()
        {
            var node = CreateInstance<AbilityGraphNode>();
            AssetDatabase.AddObjectToAsset(node, AssetDatabase.GetAssetPath(_graph));
            var button = CreateButton(node);
            _graph.AddNode(node);
            serializedObject.Update();
            InspectNode(button);
        }

        private AbilityButton CreateButton(AbilityGraphNode node)
        {
            var newButton = _graphView.CreateNodeButton(node);
            newButton.RegisterCallback<ClickEvent>(HandleButtonClickEvent);
            _nodeViews.Add(node.GetInstanceID(), newButton);
            return newButton;
        }

        private void HandleButtonClickEvent(ClickEvent evt)
        {
            if (evt.target is not AbilityButton button || _buttonPickMode) return;
            InspectNode(button);
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

        private void ProcessButtonPickedEvent(MouseUpEvent evt)
        {
            if (evt.target is AbilityButton button)
            {
                var connection = CreateInstance<NodeConnection>();
                connection.Connect(_inspectedNodeButton.Node, button.Node);
                if (!_graph.AddConnection(connection))
                {
                    ExitButtonPickMode();
                    return;
                }
                AssetDatabase.AddObjectToAsset(connection, AssetDatabase.GetAssetPath(_graph));
                serializedObject.Update();
                CreateConnectionLine(connection);
                ExitButtonPickMode();
            }
        }

        private void EnterButtonPickMode()
        {
            _buttonPickMode = true;
            _connectButton.text = "Cancel connection";
            foreach (var button in _nodeViews.Values)
            {
                if (button == _inspectedNodeButton) continue;
                button.RegisterCallback<MouseUpEvent>(ProcessButtonPickedEvent);
            }
        }

        private void ExitButtonPickMode()
        {
            _buttonPickMode = false;
            foreach (var button in _nodeViews.Values)
            {
                button.UnregisterCallback<MouseUpEvent>(ProcessButtonPickedEvent);
            }
            _connectButton.text = "Connect...";
        }

        private void CreateConnectionLine(NodeConnection connection)
        {
            var buttonA = _nodeViews[connection.NodeA.GetInstanceID()];
            var buttonB = _nodeViews[connection.NodeB.GetInstanceID()];
            var line = _graphView.CreateConnection(buttonA, buttonB);
            buttonA.RegisterCallback<ElementDraggedEvent>(line.HandleStartDraggedEvent);
            buttonB.RegisterCallback<ElementDraggedEvent>(line.HandleEndDraggedEvent);
            _connectionLines.Add(connection, line);
        }

        private void DestroyConnectionLine(NodeConnection connection)
        {
            if (_connectionLines.TryGetValue(connection, out var line))
            {
                var buttonA = _nodeViews[connection.NodeA.GetInstanceID()];
                var buttonB = _nodeViews[connection.NodeB.GetInstanceID()];
                buttonA.UnregisterCallback<ElementDraggedEvent>(line.HandleStartDraggedEvent);
                buttonB.UnregisterCallback<ElementDraggedEvent>(line.HandleEndDraggedEvent);
                _connectionLines.Remove(connection);
            }
        }

        private void DeleteNodeClicked()
        {
            if (_inspectedAbilityNode == null) return;
            _nodeAbilityField.Unbind();
            _nodePositionField.Unbind();
            _graph.Nodes.Remove(_inspectedAbilityNode);
            
            foreach (var connection in _inspectedAbilityNode.Connections)
            {
                DestroyConnectionLine(connection);
                _graph.RemoveConnection(connection);
            }
            _inspectedAbilityNode.Connections.Clear();
            _graphView.RemoveAbilityButton(_inspectedNodeButton);
            _nodeInspectorRoot.Add(_buttonDragHandle);
            _nodeViews.Remove(_inspectedAbilityNode.GetInstanceID());
            _inspectedAbilityNode = null;
            _inspectedNodeButton = null;
            _nodeInspectorRoot.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        private void InspectNode(AbilityButton button)
        {
            if (_inspectedNodeButton != null)
            {
                foreach (var connection in _inspectedNodeButton.Node.Connections)
                {
                    if (_connectionLines.TryGetValue(connection, out var line))
                    {
                        _inspectedNodeButton.UnregisterCallback<ElementDraggedEvent>(line.HandleStartDraggedEvent);
                        _inspectedNodeButton.UnregisterCallback<ElementDraggedEvent>(line.HandleEndDraggedEvent);
                    }
                }
            }
            _inspectedAbilityNode = button.Node;
            _inspectedNodeButton = button;
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
            button.Add(_buttonDragHandle);
            _buttonDragManipulator.DraggedElement = button;
            _nodeInspectorRoot.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            _nodeAbilityField.value = default;
            _nodePositionField.value = default;
            var so = new SerializedObject(_inspectedAbilityNode);
            _nodeAbilityField.BindProperty(so.FindProperty("_ability"));
            _nodePositionField.BindProperty(so.FindProperty("_position"));
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
                        if (connection.NodeA == _inspectedAbilityNode) line.Start += delta;
                        if (connection.NodeB == _inspectedAbilityNode) line.End += delta;
                    }
                }
            }
        }
    }
}