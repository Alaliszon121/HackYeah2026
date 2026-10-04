using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class VideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string mainSceneName = "MainLevel";
    [SerializeField] private InputActionReference cancelActionReference;

    private void OnEnable()
    {
        if (cancelActionReference != null)
        {
            cancelActionReference.action.Enable();
            cancelActionReference.action.performed += OnCancelPerformed;
        }

        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    private void OnDisable()
    {
        if (cancelActionReference != null)
        {
            cancelActionReference.action.performed -= OnCancelPerformed;
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    private void Update()
    {
        // Fallback for legacy input/Keyboard Escape if no input action reference is assigned
        if (cancelActionReference == null && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ReturnToMainScene();
        }
    }

    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        ReturnToMainScene();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        ReturnToMainScene();
    }

    private void ReturnToMainScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}