using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsData 
{
    public float bgmVolume;
    public float sfxVolume;
    public bool fullscreen;

    public SettingsData(float bgm, float sfx, bool isFull)
    {
        bgmVolume = bgm;
        sfxVolume = sfx;
        fullscreen = isFull;
    }
}
