using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Audio
{
    MainTheme,
    Pickup

}


public class AudioManager : MonoBehaviour
{

    [SerializeField] AudioSource[] sfxSources;

    public static AudioManager Instance;

    private void Awake()
    {
        // Check for duplicated audio managers
        AudioManager[] audioMgrs = GameObject.FindObjectsOfType<AudioManager>();
        if (audioMgrs.Length > 1)
        {
            DontDestroyOnLoad(audioMgrs[0].gameObject);
            Destroy(audioMgrs[1].gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        // At this point we have only one instance in the scene
        Instance = this;
    }

    public void PlayAudio(Audio audioSrc, bool looping)
    {
        if (sfxSources[(int)audioSrc].isPlaying)
            sfxSources[(int)audioSrc].Stop();

        sfxSources[(int)audioSrc].loop = looping;
        sfxSources[(int)audioSrc].Play();

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
