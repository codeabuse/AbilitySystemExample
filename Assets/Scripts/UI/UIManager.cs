using System.Collections.Generic;
using PixelHunt;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(EventSystem), typeof(InputSystemUIInputModule))]
public class UIManager : SingletonMonobehaviour<UIManager>
{
    [SerializeField]
    private List<UIPanelController> _panels = new();

    public void RegisterPanel(UIPanelController panelController)
    {
        if (!panelController)
        {
            Debug.LogWarning($"You're probably trying to register a destroyed panel");
            return;
        }
        _panels.Add(panelController);
    }

    public void ShowOnlyPanel(UIPanelController panelController)
    {
        if (!panelController)
        {
            Debug.LogError("Panel is null or destroyed");
            return;
        }
        
        foreach (var uiPanel in _panels)
        {
            if (uiPanel == panelController) continue;
            uiPanel.HidePanel();
        }
        
        panelController.ShowPanel();
    }
}