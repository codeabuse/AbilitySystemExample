using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace PixelHunt
{
    public static class VisualElementExtensions
    {
        public static void RegisterOnLayoutDoneCallback(this VisualElement element, Action callback)
        {
            async void CheckLayoutStatus()
            {
                var timeout = 16;
                while (timeout > 0 && float.IsNaN(element?.layout.height ?? 
                                                  throw new Exception("Element destroyed before layout happened")))
                {
                    timeout--;
                    await Task.Delay(16);
                }
                
                callback?.Invoke();
            }
            
            CheckLayoutStatus();
        }
    }
}