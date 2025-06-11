using Codice.CM.WorkspaceServer.DataStore;
using System;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;
    [SerializeField] private Sound[] sounds_play;

    public void Update()
    {
        
    }

    public void Play(string sound_name)
    {
        foreach(Sound sound in sounds)
        {
            if(sound.name == sound_name)
            {
                if(sound.tag != null)
                {
                    for (int i = 0; i < sounds_play.Length; i++)
                    {
                        Sound compare_sound = sounds_play[i];
                        for (int t1 = 0; t1 < sound.tag.Length; t1++)
                        {
                            for (int t2 = 0; t2 < compare_sound.tag.Length; t2++)
                            {
                                if (compare_sound.tag[t2] == sound.tag[t1])
                                {
                                    switch(sound.tag[t2])
                                    {
                                        case Sound.Sound_Tag.None: break;
                                        case Sound.Sound_Tag.Music | Sound.Sound_Tag.Substitute:
                                            Destroy(sounds_play[i].prefab);
                                            sounds_play[i] = sound;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                Sound[] old = sounds_play;

                if (sounds_play.Length == 0)
                    sounds_play = new Sound[1];
                else
                    sounds_play = new Sound[sounds_play.Length];

                for(int i = 0; i < old.Length; i++)
                {
                    sounds_play[i] = sound;
                }

                Sound new_sound = new Sound();
                new_sound.name = sound_name;
                new_sound.prefab = Instantiate(sound.prefab);

                sounds_play[sounds_play.Length-1] = new_sound;
            }
        }
    }

    public void Stop(string sound_name)
    {

    }
}

[Serializable]
public class Sound
{
    public string name;
    public Sound_Tag[] tag;
    public GameObject prefab;

    public enum Sound_Tag
    {
        None,
        Music,
        Substitute,
    }
}