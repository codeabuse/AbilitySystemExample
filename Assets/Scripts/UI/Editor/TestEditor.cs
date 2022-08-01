using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    [CustomEditor(typeof(TestBehaviour))]
    public class TestEditor : VisualTreeEditor<TestBehaviour>
    {
        [SerializeField]
        private float _testFloat = 25f;
        [SerializeField]
        private Vector3 _testVector3 = new (3, 2, 1);

        private Slider _floatSlider;
        private Vector3Field _vectorField;
        
        protected override void OnInspectorTreeCreated()
        {
            BindInspectorProperties();
            _floatSlider = rootVisualElement.Q<Slider>("test-slider");
            _vectorField = rootVisualElement.Q<Vector3Field>("test-vector");
            _floatSlider.RegisterCallback<ChangeEvent<float>>((evt)=> Debug.Log($"{_testFloat.ToString()}"));
            _vectorField.RegisterCallback<ChangeEvent<Vector3>>((evt)=> Debug.Log($"{_testVector3.ToString()}"));
        }
    }
}