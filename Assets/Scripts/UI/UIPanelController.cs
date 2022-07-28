using System;
using PixelHunt;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class UIPanelController : MonoBehaviour
{
    public VisualElement Root => Document ? Document.rootVisualElement : null;
    public UIDocument Document => _document? _document : _document = GetComponent<UIDocument>();
    
    public InputActionReference TogglePanelAction => _togglePanelAction;

    [SerializeField]
    private bool _showOnStart;
    [SerializeField]
    private InputActionReference _togglePanelAction;
    
    public UIPanelEvent OnPanelShown;
    public UIPanelEvent OnPanelHidden;

    private UIDocument _document;


    protected virtual void Start()
    {
        if (!Document)
        {
            Debug.LogError($"{name} doesn't have UIDocument component!");
            return;
        }
        UIManager.Instance.RegisterPanel(this);
        Document.rootVisualElement.RegisterOnLayoutDoneCallback(OnDocumentLayoutDone);
    }

    protected virtual void OnEnable()
    {
        TogglePanelAction.action.Enable();
    }

    protected virtual void OnDestroy()
    {
        TogglePanelAction.action.Disable();
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        OnShown();
        OnPanelShown?.Invoke(this);
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
        OnHidden();
        OnPanelHidden?.Invoke(this);
    }

    public void OnToggleAction(InputAction.CallbackContext context)
    {
        TogglePanel();
    }

    public void TogglePanel()
    {
        if (gameObject.activeInHierarchy)
            HidePanel();
        else
            ShowPanel();
    }

    protected abstract void OnDocumentLayoutDone();
    
    protected virtual void OnShown(){}
    protected virtual void OnHidden(){}
}