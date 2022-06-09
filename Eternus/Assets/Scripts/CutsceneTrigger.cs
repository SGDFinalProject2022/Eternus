using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class CutsceneTrigger : MonoBehaviour
{
    public static CutsceneTrigger instance;
    PlayableDirector dir;
    void Awake()
    {
        instance = this;
        dir = GetComponent<PlayableDirector>();
    }
    
    public void Play()
    {
        dir.Play();
    }
}
