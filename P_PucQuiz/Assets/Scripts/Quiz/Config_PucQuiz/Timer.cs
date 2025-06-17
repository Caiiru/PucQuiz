using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class Timer : Unity.Netcode.INetworkSerializable
{
    public float start = 0;
    public float time = 0;
    public bool infinity;
    public Timer(float start)
    {
        //Debug.Log("Start Timer");
        this.start = start;
    }

    public void Run()
    {
        //if(DEV.Instance.isTimerInfinity) return;
        
        if (infinity) { return; }

        if(time <= 0) { time = 0; return; }

        time = time - Time.deltaTime;
    }
    public float SimulateRun()
    {
        
        float test = time - Time.deltaTime;

        if (infinity || test <= 0) { return 0; }

        return test;
    }

    public bool End()
    {
        if(infinity) { return true; }

        if (time <= 0) { return true; }
        return false;
    }

    public void Reset()
    {
        time = start;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref start);
        serializer.SerializeValue(ref time);
        serializer.SerializeValue(ref infinity); 
    }
}
