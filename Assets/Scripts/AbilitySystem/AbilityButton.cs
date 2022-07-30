using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem
{
    public class AbilityButton : Button
    {
        public new class UxmlFactory : UxmlFactory<AbilityButton, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        public AbilityGraphNode Node { get; set; }

        public Vector2 HalfSize => layout.size * .5f + new Vector2(
                resolvedStyle.marginTop * .5f + resolvedStyle.marginBottom * .5f, 
                resolvedStyle.marginLeft * .5f + resolvedStyle.marginRight * .5f);

        public AbilityButton(AbilityGraphNode node, Action onButtonClicked) : base(onButtonClicked)
        {
            Node = node;
        }

        public AbilityButton() { }
    }
}