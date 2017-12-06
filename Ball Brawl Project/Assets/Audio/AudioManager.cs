using UnityEngine;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine.Networking;

public class AudioManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject _redCrowd;
    [SerializeField]
    private GameObject _blueCrowd;
    [SerializeField]
    private GameObject _redCrowdOrigin;
    [SerializeField]
    private GameObject _blueCrowdOrigin;

    [SerializeField]
    [EventRef]
    private string _crowdBoo;
    [SerializeField]
    [EventRef]
    private string _crowdCheer;
    [SerializeField]
    [EventRef]
    private string _crowdMiss;
    [SerializeField]
    [EventRef]
    private string _crowdLoop;

    public List<FMODInstance> FMODInstances;
    [System.Serializable]
    public class FMODInstance
    {
        [EventRef]
        public string eventString;
        public GameObject origin;
        public FMOD.Studio.EventInstance instance;
    }

    private FMOD.Studio.EventInstance nill;
    private static AudioManager audioManager = null;

    private void Awake()
    {
        PlayOneShot(_crowdLoop, _redCrowd);
        PlayOneShot(_crowdLoop, _blueCrowd);
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

    public static void PlayEvent(string eventString, GameObject gameObject, bool attach = false, bool instantDelete = true)
    {
        // Storing all values in a list, so we can later adjust values/variables of those specific events.
        // As FMOD doesn't actually store this data anywhere.
        FMODInstance newInstance = new FMODInstance();
        newInstance.origin = gameObject;
        newInstance.eventString = eventString;
        audioManager.FMODInstances.Add(newInstance);

        // Playing the new event
        newInstance.instance = RuntimeManager.CreateInstance(newInstance.eventString);
        newInstance.instance.set3DAttributes(RuntimeUtils.To3DAttributes(newInstance.origin.transform.position));
        newInstance.instance.start();

        // If desired to attach to a rigidbody
        if (attach)
        {
            Rigidbody rigidbody = newInstance.origin.GetComponent<Rigidbody>();
            RuntimeManager.AttachInstanceToGameObject(newInstance.instance, newInstance.origin.transform, rigidbody);
        }

        if (instantDelete)
        {
            newInstance.instance.release();
            audioManager.FMODInstances.Remove(newInstance);
        }

    }

    public static void stopInstance(string eventString, GameObject gameObject, bool fadeOut = true)
    {
        FMOD.Studio.EventInstance instanceToDelete = getInstance(gameObject, eventString);
        for (int i = 0; i < audioManager.FMODInstances.Count; i++)
        {
            if (audioManager.FMODInstances[i].eventString == eventString && audioManager.FMODInstances[i].origin == gameObject)
                audioManager.FMODInstances.RemoveAt(i);
        }

        if (fadeOut)
        {
            instanceToDelete.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        else
        {
            instanceToDelete.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    private static FMOD.Studio.EventInstance getInstance(GameObject gameObject, string eventString)
    {
        foreach (FMODInstance instance in audioManager.FMODInstances)
        {
            if (instance.eventString == eventString && instance.origin == gameObject)
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


    public static void GoalScored(string team)
    {
        if (team == Teams.TEAM_RED)
        {
            PlayOneShot(audioManager._crowdCheer, audioManager._redCrowd);
            PlayOneShot(audioManager._crowdBoo, audioManager._blueCrowd);
            SetCrowd(Teams.TEAM_RED, 5f);
            SetCrowd(Teams.TEAM_BLUE, 0.5f);
        }
        else
        {
            PlayOneShot(audioManager._crowdCheer, audioManager._blueCrowd);
            PlayOneShot(audioManager._crowdBoo, audioManager._redCrowd);
            SetCrowd(Teams.TEAM_BLUE, 5f);
            SetCrowd(Teams.TEAM_RED, 0.5f);
        }
    }

    public static void CrowdMiss(string team)
    {
        if (team == Teams.TEAM_RED)
        {
            PlayOneShot(audioManager._crowdMiss, audioManager._redCrowd);
        }
        else
        {
            PlayOneShot(audioManager._crowdMiss, audioManager._blueCrowd);
        }
    }

    public static void SetCrowd(string team, float value)
    {
        if (team == Teams.TEAM_BLUE)
            audioManager._blueCrowd.GetComponent<Crowd>().cheering = value;
        else
            audioManager._redCrowd.GetComponent<Crowd>().cheering = value;
    }
}
