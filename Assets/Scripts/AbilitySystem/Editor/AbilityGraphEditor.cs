using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    [CustomEditor(typeof(AbilityGraph))]
    public class AbilityGraphEditor : Editor
    {
        private const string graph_asset_name = "Assets/Resources/UI/VisualTrees/ability-management-panel.uxml";
        private const string styles_asset_name = "Assets/Resources/UI/Styles/ability-management-styles.uss";


        private AbilityGraph _graph;
        private AbilityGraphView _graphView;
        private VisualTreeAsset _graphVisualTree;
        private StyleSheet _styles;

        private VisualElement 
                _root,
                _abilityNodesRoot;
        private List<Button> _nodeViews = new();

        private void Awake()
        {
            _graphVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(graph_asset_name);
            _styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(styles_asset_name);
            
            _root = new VisualElement();
            _root.styleSheets.Add(_styles);
            _graph = target as AbilityGraph;
        }

        private void OnEnable()
        {
            
        }

        public override VisualElement CreateInspectorGUI()
        {
            _graphVisualTree.CloneTree(_root);
            var buttonsRoot = new VisualElement()
            {
                    style =
                    {
                            flexGrow = 1, flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                    }
            };
            buttonsRoot.Add(new Button(()=>
            {
                _graph.CreateNode();
                
            }){text = "Create Node"});
            
            _root.Add(buttonsRoot);
            _graphView = _root.Q<AbilityGraphView>();
            _graphView.BindToGraph(_graph);
            return _root;
        }
    }
}