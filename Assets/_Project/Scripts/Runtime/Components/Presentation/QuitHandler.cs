using Sailboat.Components.Boat;
using UnityEngine;

namespace Sailboat.Components.Presentation
{
    /// <summary>
    /// Closes the application when the quit input is pressed.
    /// </summary>
    public sealed class QuitHandler : MonoBehaviour
    {
        [SerializeField] private SailboatInputHandler _input;

        private void Awake()
        {
            if (_input == null)
            {
                Debug.LogError($"{nameof(QuitHandler)} needs a reference to {nameof(SailboatInputHandler)}.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (!_input.QuitPressed)
            {
                return;
            }

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}