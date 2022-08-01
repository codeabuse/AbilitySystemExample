using AbilitySystem;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class MainScreenPanel : UIPanelController
{
    public UnityEvent OnAbilitiesButtonClicked;
    
    private Button _abilityManagementButton;
    
    protected override void OnEnable()
    {
        _abilityManagementButton = Document.rootVisualElement.Q<Button>("ability-management");
        _abilityManagementButton.clicked += OnAbilitiesButtonClicked.Invoke;
    }
}