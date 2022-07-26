using UnityEngine;

namespace PixelHunt
{
    public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T: MonoBehaviour
    {
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            if (!Instance)
            {
                Instance ??= this as T;
                return;
            }

            if (Instance == this) return;
            
            Debug.LogError($"Another instance of {nameof(T)} was created! Destroying");
            Destroy(this);
        }
    }
}