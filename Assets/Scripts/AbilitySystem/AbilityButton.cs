using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    public class AbilityButton : Button
    {
        public new class UxmlFactory : UxmlFactory<AbilityButton, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        public Vector2 CenterOffset => layout.center - layout.position;
        public AbilityGraphNode Node { get; set; }


        public AbilityButton(AbilityGraphNode node, Action onButtonClicked) : base(onButtonClicked)
        {
            Node = node;
        }

        public AbilityButton() { }
    }
}