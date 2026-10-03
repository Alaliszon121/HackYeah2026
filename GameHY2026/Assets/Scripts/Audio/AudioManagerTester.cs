using UnityEngine;

public class AudioManagerTester : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButtonSound() {
        AudioManager.Instance.PlayButtonClick();
    }
}
