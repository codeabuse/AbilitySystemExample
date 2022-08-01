using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    public abstract class VisualTreeEditor<T> : Editor where T: Object
    {
        #region Resources
        
        /// <summary>
        /// Assign inspector's Visual Tree on the child script itself
        /// </summary>
        protected VisualTreeAsset inspectorVisualTree
        {
            get =>
                    _inspectorVisualTree
                            ? _inspectorVisualTree
                            : _inspectorVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(getVisualTreeFromPath);
            set => _inspectorVisualTree = value;
        }

        /// <summary>
        /// Override this property in children implementation to provide a fallback for <see cref="inspectorVisualTree"/>
        /// </summary>
        protected virtual string getVisualTreeFromPath => string.Empty;
        
        
        /// <summary>
        /// Assign inspector's Style Sheet on the child script itself
        /// </summary>
        protected StyleSheet inspectorStyles
        {
            get =>
                    _inspectorStyles
                            ? _inspectorStyles
                            : _inspectorStyles = AssetDatabase.LoadAssetAtPath<StyleSheet>(getVisualTreeFromPath);
            set => _inspectorStyles = value;
        }

        /// <summary>
        /// Override this property in children implementation to provide a fallback for <see cref="inspectorStyles"/>
        /// </summary>
        protected virtual string getStyleSheetFromPath => string.Empty;
        
        [SerializeField]
        private VisualTreeAsset _inspectorVisualTree;
        [SerializeField]
        private StyleSheet _inspectorStyles;
        

        #endregion

        protected T inspectedObject => target as T;

        protected VisualElement rootVisualElement { get; private set; }
        

        public override VisualElement CreateInspectorGUI()
        {
            rootVisualElement ??= new VisualElement();
            
            if (inspectorVisualTree)
            {
                inspectorVisualTree.CloneTree(rootVisualElement);
            }
            else
            {
                Debug.LogError($"There is no VisualTree Asset specified for {GetType().Name}!");
                return rootVisualElement;
            }
            
            if (inspectorStyles)
            {
                rootVisualElement.styleSheets.Add(inspectorStyles);
            }
            else
            {
                //Debug.LogWarning($"You didn't specified a StyleSheet for {GetType().Name}");
            }

            WaitForLayout();
            return rootVisualElement;
        }

        private async void WaitForLayout()
        {
            while (float.IsNaN(rootVisualElement.layout.size.x))
            {
                await Task.Delay(2);
            }
            OnInspectorTreeCreated();
        }

        /// <summary>
        /// This method will be called on the next Update after the Inspector GUI is created.
        /// Normally the first layout is done at this point.
        /// </summary>
        protected virtual void OnInspectorTreeCreated() { }

        /// <summary>
        /// This method binds all controls of the root with non-empty Binding Path
        /// to the corresponding Serialized Properties of the Inspector itself.
        /// Use <see cref="SerializeField"/> attribute on Inspector's properties to automatically update them
        /// when the control is chaged. 
        /// </summary>
        protected virtual void BindInspectorProperties()
        {
            BindElementsTo(rootVisualElement, new SerializedObject(this));
        }

        /// <summary>
        /// This method binds all controls of the root with non-empty Binding Path
        /// to the corresponding Serialized Properties of the target.
        /// </summary>
        protected virtual void BindInspectedProperties()
        {
            BindElementsTo(rootVisualElement, serializedObject);
        }

        protected void BindElementsTo(VisualElement targetRoot, SerializedObject targetSerializedObject)
        {
            var bindableElements = targetRoot.Query<BindableElement>().
                    Where(b => !string.IsNullOrEmpty(b.bindingPath));
            bindableElements.ForEach(x => { x.Bind(targetSerializedObject); x.SetEnabled(true); });
        }
    }
}