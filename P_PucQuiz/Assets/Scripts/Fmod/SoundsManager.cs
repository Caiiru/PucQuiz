using Newtonsoft.Json.Bson;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static Sound;

[Serializable]
public class SoundsManager
{
    static public SoundsManager Instance;
    [SerializeField] public LayoutManager manager;
    [SerializeField] private Sound_Config sounds;
    [SerializeField] private Sound_Play[] sounds_play;

    public SoundsManager() { Instance = this; }

    public void Awake()
    {
        //if(sounds == null) { sounds = Resources.Load<Sound_Config>("Config/Sounds"); }
    }
    public void Update()
    {
        foreach (Sound_Play sound_play in sounds_play)
        {
            sound_play.Run();
            if (sound_play.sound.tag != null || sound_play.sound.tag.Length != 0)
            {
                foreach (Sound.Sound_Tag tag in sound_play.sound.tag)
                {
                    if (tag == Sound.Sound_Tag.InHost)
                    {
                        if ((!GameManager.Instance.IsHost || !GameManager.Instance.IsServer) && GameManager.Instance.GetTop5Players().Length != 0)
                        {
                            Stop(sound_play.tag_name, Sound_Play.Sound_Play_Tag.BreakInFirst);
                        }
                    }
                }
            }
        }
    }
    public void Click()
    {
        //Debug.Log("Click");
        Play("Click","Click");
    }

    public void Play(string sound_tag_name, string sound_name)
    {
        LocalPlay(sound_tag_name,sound_name);
    }
    public void Stop(string sound_tag, params Sound_Play.Sound_Play_Tag[] use_tag)
    {
        //sound_action_list.Add(new SoundAction(sound_tag));
        LocalStop(sound_tag,use_tag);
    }
    public bool InSound(string name)
    {
        if(sounds_play != null)
        {
            foreach (Sound_Play sound_play in sounds_play)
            {
                if (sound_play.sound.name == name) { return true; }
            }
        }
        return false;
    }
    private void LocalPlay(string sound_tag_name, string sound_name)
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
                                    //Debug.Log("Sound Play Tag is " + compare_sound.sound.tag[t2]);
                                    if (sound.tag[t1] == compare_sound.sound.tag[t2])
                                    {
                                        //Debug.Log("Sound Play Tag and Sound Tag is equal. ");
                                        switch (sound.tag[t1])
                                        {
                                            case Sound.Sound_Tag.None: break;
                                            case Sound.Sound_Tag.Music:
                                                //Debug.Log("Sound Tag = Music");
                                                GameObject.Destroy(sounds_play[i].sound.prefab);
                                                new_sound_play.Set(sound_tag_name, sound);
                                                sounds_play[i] = new_sound_play;
                                                return;
                                            case Sound.Sound_Tag.Substitute:
                                                if (sound_tag_name == sounds_play[i].tag_name)
                                                {
                                                    //Debug.Log("Sound Tag = Substitute");
                                                    GameObject.Destroy(sounds_play[i].sound.prefab);
                                                    new_sound_play.Set(sound_tag_name, sound);
                                                    sounds_play[i] = new_sound_play;
                                                    return;
                                                }
                                                break;
                                            case Sound.Sound_Tag.OneForOne:
                                                if (sound_tag_name == sounds_play[i].tag_name) { return; }
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
    private void LocalStop(string sound_tag, params Sound_Play.Sound_Play_Tag[] use_tag)
    {
        //Debug.Log("Sound Stop Chamado");
        int nulls = 0;

        for(int i = 0; i < sounds_play.Length; i++)
        {
            Sound_Play sound_play = sounds_play[i];
            if (sound_play.tag_name == sound_tag)
            {
                bool break_in_first = false;

                if(sound_play.tag != null)
                {
                    foreach (Sound_Play.Sound_Play_Tag tag in sound_play.tag)
                    {
                        switch (tag)
                        {
                            case Sound_Play.Sound_Play_Tag.BreakInFirst:
                                break_in_first = true;
                                break;
                        }
                    }
                }
                if(use_tag != null)
                {
                    foreach (Sound_Play.Sound_Play_Tag tag in use_tag)
                    {
                        switch (tag)
                        {
                            case Sound_Play.Sound_Play_Tag.BreakInFirst:
                                break_in_first = true;
                                break;
                        }
                    }
                }

                GameObject.Destroy(sounds_play[i].sound.prefab);
                nulls++;
                if (break_in_first) {  break; }
            }
        }

        Sound_Play[] new_sounds_play = new Sound_Play[sounds_play.Length - nulls];

        if (nulls == 0) { return; }
        else { nulls = 0; }

        if(new_sounds_play.Length != 0)
        {
            for (int i = 0; i < new_sounds_play.Length; i++)
            {
                if (sounds_play[i].sound.prefab != null) { new_sounds_play[i - nulls] = sounds_play[i]; }
                else { nulls++; }
            }
        }

        sounds_play = new_sounds_play;
    }

    internal void StopAllSounds()
    {
        Sound_Play[] soundsOld = sounds_play;
        foreach (var sound in soundsOld)
        {
            Stop(sound.tag_name);
        }
        foreach (var sound in soundsOld)
        {
            GameObject.Destroy(sound.sound.prefab);
        }
    }
}
[Serializable]
public class Sound
{
    public string name;
    public Sound_Tag[] tag;
    public GameObject prefab;
    public Timer timer;

    public enum Sound_Tag
    {
        None,
        Music,
        Substitute,
        OneForOne,
        InHost
    }
}
[Serializable]
public class Sound_Play
{
    public string tag_name;
    public Sound_Play_Tag[] tag;
    public Sound sound;
    public enum Sound_Play_Tag
    {
        None,
        BreakInFirst,
    }

    public void Set(string name, Sound sound_prefab, Timer timer = null ,params Sound_Play_Tag[] tags)
    {
        tag_name = name;
        tag = tags;

        Sound new_sound = new Sound();
        new_sound.name = sound_prefab.name;
        new_sound.tag = sound_prefab.tag;
        new_sound.prefab = GameObject.Instantiate(sound_prefab.prefab);

        Timer new_timer = new Timer(5);

        if(timer != null) { new_timer.start = timer.start; new_timer.infinity = timer.infinity; }
        else { new_timer.start = sound_prefab.timer.start; new_timer.infinity = sound_prefab.timer.infinity; }

        new_sound.timer = new_timer;
        new_sound.timer.start += 0.2f;
        new_sound.timer.Reset();

        sound = new_sound;
    }
    public void Run()
    {
        if(!sound.timer.End())
        {
            sound.timer.Run();
        }
        else
        {
            if (sound.timer != null && !sound.timer.infinity) { SoundsManager.Instance.Stop(tag_name, Sound_Play_Tag.BreakInFirst); }
        }
    }
}