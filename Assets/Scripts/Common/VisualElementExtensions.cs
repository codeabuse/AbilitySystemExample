using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace PixelHunt
{
    public static class VisualElementExtensions
    {
        public static void ExecuteAfterLayoutDone(this VisualElement element, Action callback)
        {
            async void CheckLayoutStatus()
            {
                var attempts = 16;
                while (attempts > 0 && float.IsNaN(element?.layout.height ?? 
                                                  throw new Exception("Element destroyed before layout happened")))
                {
                    attempts--;
                    await Task.Delay(2);
                }
                
                callback?.Invoke();
            }
            
            CheckLayoutStatus();
        }
    }
}