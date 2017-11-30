using UnityEngine;
using FMODUnity;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    

    public List<FMODInstance> FMODInstances;
    [System.Serializable]
    public class FMODInstance
    {
        [EventRef]
        public string eventName;
        public GameObject origin;
        public FMOD.Studio.EventInstance instance;
    }

    private FMOD.Studio.EventInstance nill;
    private static AudioManager audioManager = null;

    private void Awake()
    {
        if (audioManager != null && audioManager != this)
        {
            Destroy(this.gameObject);
        }

        audioManager = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public static void PlayOneShot(string eventString, GameObject gameObject)
    {
        RuntimeManager.PlayOneShot(eventString, gameObject.transform.position);
    }

    public static void PlayEvent(string eventString, GameObject gameObject, bool attach = false)
    {
        // Storing all values in a list, so we can later adjust values/variables of those specific events.
        // As FMOD doesn't actually store this data anywhere.
        FMODInstance newInstance = new FMODInstance();
        newInstance.origin = gameObject;
        newInstance.eventName = eventString;
        audioManager.FMODInstances.Add(newInstance);

        // Playing the new event
        newInstance.instance = RuntimeManager.CreateInstance(newInstance.eventName);
        newInstance.instance.set3DAttributes(RuntimeUtils.To3DAttributes(newInstance.origin.transform.position));
        newInstance.instance.start();

        // If desired to attach to a rigidbody
        if (attach)
        {
            Rigidbody rigidbody = newInstance.origin.GetComponent<Rigidbody>();
            RuntimeManager.AttachInstanceToGameObject(newInstance.instance, newInstance.origin.transform, rigidbody);
        }
    }

    public static void stopInstance(string eventString, GameObject gameObject)
    {
        getInstance(gameObject, eventString).stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private static FMOD.Studio.EventInstance getInstance(GameObject gameObject, string eventName)
    {
        foreach (FMODInstance instance in audioManager.FMODInstances)
        {
            if (instance.eventName == eventName && instance.origin == gameObject)
            {
                return instance.instance;
            }
        }
        return audioManager.nill;
    }

    public static void setParameter(string eventString, GameObject gameObject, string parameter, float value)
    {
        getInstance(gameObject, eventString).setParameterValue(parameter, value);
    }

    public static void setPitch(string eventString, GameObject gameObject, float value)
    {
        getInstance(gameObject, eventString).setPitch(value);
    }

    public static void setVolume(string eventString, GameObject gameObject, float value)
    {
        getInstance(gameObject, eventString).setVolume(value);
    }
}
