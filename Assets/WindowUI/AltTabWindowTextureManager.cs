using UnityEngine;

namespace uWindowCapture
{

    public class AltTabWindowTextureManager : UwcWindowTextureManager
    {
        void Start()
        {
            UwcManager.onWindowAdded.AddListener(OnWindowAdded);
            UwcManager.onWindowRemoved.AddListener(OnWindowRemoved);

            foreach (var pair in UwcManager.windows)
            {
                OnWindowAdded(pair.Value);
            }
        }

        void OnWindowAdded(UwcWindow window)
        {
            window.parentWindow = null;
            if (window.parentWindow != null) return; // handled by UwcWindowTextureChildrenManager
            if (!window.isVisible || !window.isAltTabWindow || window.isBackground) return;
            window.RequestCapture();
            AddWindowTexture(window);
        }

        void OnWindowRemoved(UwcWindow window)
        {
            RemoveWindowTexture(window);
        }
    }

}