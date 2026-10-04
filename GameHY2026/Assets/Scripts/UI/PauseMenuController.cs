using UnityEngine;
using UnityEngine.InputSystem;

namespace PinkTaxGame
{
    public class PauseMenuController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject pauseMenuPanel;

        [Header("Input")]
        [SerializeField] private InputActionReference pauseAction;

        private bool isPaused = false;

        private void Start()
        {
            pauseMenuPanel.SetActive(false);
        }

        private void OnEnable()
        {
            if (pauseAction != null)
            {
                pauseAction.action.Enable();
                pauseAction.action.performed += TogglePause;
            }
        }

        private void OnDisable()
        {
            if (pauseAction != null)
            {
                pauseAction.action.performed -= TogglePause;
                pauseAction.action.Disable(); 
            }
        }

        private void TogglePause(InputAction.CallbackContext context)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }

        public void Resume()
        {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            
            // Optional: Add a UI click sound here using your AudioManager
            // AudioManager.Instance.PlayButtonClick();
        }

        private void Pause()
        {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            
            // Optional: Add a UI click sound here using your AudioManager
            // AudioManager.Instance.PlayButtonClick();
        }

        public void ExitGame()
        {
            Time.timeScale = 1f; 
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}