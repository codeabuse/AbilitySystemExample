using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class UIPanelController : MonoBehaviour
{
    public VisualElement Root => _document ? _document.rootVisualElement : null;
    
    public UIPanelEvent OnPanelShown;
    public UIPanelEvent OnPanelHidden;

    protected UIDocument _document;

    protected virtual IEnumerator Start()
    {
        if (!_document) _document = GetComponent<UIDocument>();
        if (!_document)
        {
            Debug.LogError($"{name} doesn't have UIDocument component!");
            yield break;
        }
        UIManager.Instance.RegisterPanel(this);
        yield return null; //dirty hack because ui elements layout is a bit late
        yield return null;

        Initialize();
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

    public void TogglePanel()
    {
        if (gameObject.activeInHierarchy)
            HidePanel();
        else
            ShowPanel();
    }

    protected abstract void Initialize();
    
    protected virtual void OnShown(){}
    protected virtual void OnHidden(){}
}