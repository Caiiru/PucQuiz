using System;
using System.IO;
using UnityEngine;
using UnityEngine.Windows;

[Serializable]
public class Translator
{
    private static Translator instance;
    public static Translator Get()
    {
        if (instance == null) { instance = new Translator(); }
        return instance;
    }

    public void Save_Attributes(Attributes[] attributes, string path)
    {
        var save = JsonUtility.ToJson(attributes);
        System.IO.File.WriteAllText(path,save);
    }
    public Attributes Load_Attributes(Attributes[] attributes, string path)
    { 
        var load = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<Attributes>(load);
    }
}
