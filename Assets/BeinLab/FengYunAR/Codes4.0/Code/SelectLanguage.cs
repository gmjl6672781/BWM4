using BeinLab.Util;
using BeinLab.VRTraing.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLanguage : Singleton<SelectLanguage>
{
    public static bool isOver = false;

    public List<string> words;
    public List<AudioClip> audioClips_chinese;
    public List<AudioClip> audioClips_english;

    public List<Text> texts;
    public AudioSource audioSource;

    private int index = 0;
    private List<string[]> wordsArray = new List<string[]>();
    private List<List<AudioClip>> audioClips = new List<List<AudioClip>>();
    private List<AudioClip> currentClip = new List<AudioClip>();
    private void Awake()
    {
        isOver = false;
        foreach (var item in words)
        {
            wordsArray.Add(item.Split('|'));
        }
        audioClips.Add(audioClips_chinese);
        audioClips.Add(audioClips_english);
    }
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        index = LanguageMgr.Instance.languageIndex;
        if (texts.Count == wordsArray[index].Length)
        {
            for (int i = 0; i < texts.Count; i++)
            {
                texts[i].text = wordsArray[index][i];
            }
            currentClip = audioClips[index];
            StartCoroutine(PlayAudio());
        }
    }

    IEnumerator PlayAudio()
    {
        float time = 0;
        foreach (var item in currentClip)
        {
            time = item.length;
            //print(time);
            audioSource.clip = item;
            audioSource.Play();
            yield return new WaitForSeconds(time);
        }
        isOver = true;
    }
    
}
