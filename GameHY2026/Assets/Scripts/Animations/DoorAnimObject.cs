using UnityEditor.Animations;
using UnityEngine;

public class DoorAnimObject : MonoBehaviour
{
    [SerializeField] private AudioClip doorOpenSoundClip;
    [SerializeField] private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void PlayDoorOpenSound() {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(doorOpenSoundClip);
    }

    public void StartDoorOpenAnimation() {
        animator.SetTrigger("DoorOpen");
    }
}
