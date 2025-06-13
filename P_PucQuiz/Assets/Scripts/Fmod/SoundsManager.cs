using System;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] private Sound_Config sounds;
    [SerializeField] private Sound_Play[] sounds_play;

    public void Awake()
    {
        if(sounds == null) { sounds = Resources.Load<Sound_Config>("Config/Sounds"); }
    }
    public void Update()
    {
        
    }

    public void Play(string sound_tag_name, string sound_name)
    {
        //Debug.Log("Sound Play Chamado");

        Sound_Play new_sound_play = new Sound_Play();

        foreach (Sound sound in sounds.sound_list)
        {
            if(sound.name == sound_name)
            {
                //Debug.Log("Sound Name Encontrado");
                if (sound.tag != null)
                {
                    for (int i = 0; i < sounds_play.Length; i++)
                    {
                        //Debug.Log("Sound Play na posicao "+i);
                        Sound_Play compare_sound = sounds_play[i];
                        if (compare_sound != null)
                        {
                            for (int t1 = 0; t1 < sound.tag.Length; t1++)
                            {
                                //Debug.Log("Sound Tag is " + sound.tag[t1]);
                                for (int t2 = 0; t2 < compare_sound.sound.tag.Length; t2++)
                                {
                                    //Debug.Log("Sound Play Tag is " + sound.tag[t2]);
                                    if (sound.tag[t1] == compare_sound.sound.tag[t2])
                                    {
                                        //Debug.Log("Sound Play Tag and Sound Tag is equal. ");
                                        switch (sound.tag[t1])
                                        {
                                            case Sound.Sound_Tag.None: break;
                                            case Sound.Sound_Tag.Music:
                                                //Debug.Log("Sound Tag = Music");
                                                Destroy(sounds_play[i].sound.prefab);
                                                new_sound_play.Set(sound_tag_name, sound);
                                                sounds_play[i] = new_sound_play;
                                                return;
                                            case Sound.Sound_Tag.Substitute:
                                                if (sound_tag_name == sounds_play[i].tag_name)
                                                {
                                                    //Debug.Log("Sound Tag = Substitute");
                                                    Destroy(sounds_play[i].sound.prefab);
                                                    new_sound_play.Set(sound_tag_name, sound);
                                                    sounds_play[i] = new_sound_play;
                                                    return;
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Sound_Play[] old = sounds_play;

                if (sounds_play.Length == 0)
                    sounds_play = new Sound_Play[1];
                else
                    sounds_play = new Sound_Play[sounds_play.Length+1];

                for(int i = 0; i < old.Length; i++)
                {
                    sounds_play[i] = old[i];
                }

                new_sound_play.Set(sound_tag_name,sound);

                sounds_play[sounds_play.Length-1] = new_sound_play;
            }
        }
    }

    public void Stop()
    {
        foreach (var sound in sounds_play)
        {
            Destroy(sound.sound.prefab);
        }
    }
}

[CreateAssetMenu(fileName = "NewSoundsConfig",menuName = "SoundsManager/SoundsConfig")]
public class Sound_Config : ScriptableObject
{
    public Sound[] sound_list;
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
[Serializable]
public class Sound_Play
{
    public string tag_name;
    public Sound sound;

    public void Set(string name, Sound sound_prefab)
    {
        tag_name = name;

        Sound new_sound = new Sound();
        new_sound.name = sound_prefab.name;
        new_sound.tag = sound_prefab.tag;
        new_sound.prefab = GameObject.Instantiate(sound_prefab.prefab);

        sound = new_sound;
    }
}