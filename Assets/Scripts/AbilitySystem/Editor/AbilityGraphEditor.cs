using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        private List<AbilityButton> _abilityButtons = new();

        private AbilityGraphNode _inspectedAbilityNode;
        private AbilityButton _inspectedNodeButton;
        private VisualElement _buttonDragHandle;
        private DragManipulator _buttonDragManipulator;
        private Vector2Field _nodePositionField;
        private ObjectField _nodeAbilityField;

        private int selectedNodeIndex => _abilityButtons.IndexOf(_inspectedNodeButton); //_graph.Nodes.IndexOf(_inspectedAbilityNode);

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
            
        }

        public override void OnInspectorGUI()
        {
            
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
            buttonsRoot.Add(new Button(CleanEmptyNodesClicked){text = "Clean empty"});
            buttonsRoot.Add(new Button(DeleteNodeClicked){text = "Delete Node"});
            
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
            _nodeInspectorRoot.Add(_nodePositionField);
            _nodeInspectorRoot.Add(_nodeAbilityField);

            _buttonDragHandle = new VisualElement();// {style = { visibility = new StyleEnum<Visibility>(Visibility.Hidden)}};
            _buttonDragHandle.AddToClassList("handle-drag");
            _buttonDragManipulator = new DragManipulator(MouseButton.LeftMouse);
            _buttonDragManipulator.OnDragged += ButtonDragged;
            _buttonDragHandle.AddManipulator(_buttonDragManipulator);
            _nodeInspectorRoot.Add(_buttonDragHandle);
            foreach (var graphNode in _graph.Nodes)
            {
                CreateButton(graphNode);
            }
            return _root;
        }

        private void CreateNodeClicked()
        {
            var newNode = _graph.CreateNode();
            serializedObject.Update();
            var newButton = CreateButton(newNode);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            InspectNode(newButton);
        }

        private AbilityButton CreateButton(AbilityGraphNode node)
        {
            var newButton = _graphView.CreateNodeButton(node);
            newButton.clicked += () => InspectNode(newButton);
            _abilityButtons.Add(newButton);
            return newButton;
        }

        private void DeleteNodeClicked()
        {
            if (_inspectedAbilityNode == null) return;
            _nodeAbilityField.Unbind();
            _nodePositionField.Unbind();
            _graph.Nodes.Remove(_inspectedAbilityNode);
            _graphView.RemoveAbilityButton(_inspectedNodeButton);
            _nodeInspectorRoot.Add(_buttonDragHandle);
            _abilityButtons.Remove(_inspectedNodeButton);
            _inspectedAbilityNode = null;
            _inspectedNodeButton = null;
            _nodeInspectorRoot.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        private void InspectNode(AbilityButton button)
        {
            _inspectedAbilityNode = button.Node;
            _inspectedNodeButton = button;
            button.Add(_buttonDragHandle);
            _buttonDragManipulator.DraggedElement = button;
            _nodeInspectorRoot.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            var selectedNodeAsProperty = _nodesProperty.GetArrayElementAtIndex(selectedNodeIndex);
            _nodeAbilityField.value = default;
            _nodePositionField.value = default;

            _nodeAbilityField.BindProperty(selectedNodeAsProperty.FindPropertyRelative("_ability"));
            _nodePositionField.BindProperty(selectedNodeAsProperty.FindPropertyRelative("_position"));
        }


        private void ButtonDragged(Vector2 delta)
        {
            _nodePositionField.value += delta;
        }

        private void CleanEmptyNodesClicked()
        {
            var emptyNodes = _graph.Nodes.Where(
                    n => n.Ability == null && !n.Connections.Any()).ToArray();
            for (var i = 0; i < emptyNodes.Length; i++)
            {
                var emptyNode = emptyNodes[i];
                _graph.RemoveNode(emptyNode);
            }

            _graphView.CleanButtons();
        }
    }
}