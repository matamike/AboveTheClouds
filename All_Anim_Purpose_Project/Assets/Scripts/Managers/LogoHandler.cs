using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogoHandler : Singleton<LogoHandler>{
    [SerializeField] List<Letter> letterPrefabs = new List<Letter>();
    [SerializeField] Vector3 offset = Vector3.right;
    [SerializeField] Vector3 startingLogoPosition = Vector3.zero;
    [SerializeField] string logo = "";

    private Dictionary<char, Letter> letterDictionary = new Dictionary<char, Letter>();
    [SerializeField]private List<GameObject> letters= new List<GameObject>();

    private void Start(){
        InitializeLetterDictionary();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)){
            CreateLogo();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            FadeOutLogo();
        }
    }

    private void InitializeLetterDictionary(){
        foreach(Letter letter in letterPrefabs){
            letterDictionary.Add(letter.GetLetter(), letter);
        }
    }

    private void CreateLogo(){
        int times = 0;
        foreach(char c in logo){
            if (letterDictionary.ContainsKey(c)){
                Vector3 targetPosition = startingLogoPosition + (offset * times);
                GameObject x = Instantiate(letterDictionary[c].gameObject, Vector3.down * 100f, Quaternion.identity);
                letters.Add(x);
                float tweenTime = Random.Range(2f, 3f);
                float timeout = tweenTime * 2;
                TweenParameters tweenParameters = new TweenParameters(letters[letters.Count-1], targetPosition, Vector3.zero, x.transform.localScale, tweenTime, timeout);
                TweenHandler.Instance.CreateTween(tweenParameters);
            }
            times++;
        }
    }

    private void FadeOutLogo(){
        for (int i = letters.Count -1; i >= 0; i--){
            float tweenTime = Random.Range(2f, 3f);
            float timeout = tweenTime * 2;
            GameObject go = letters[i].gameObject;
            letters.RemoveAt(i);
            TweenParameters tweenParameters = new TweenParameters(go, Vector3.up * 10f, Vector3.zero, go.transform.localScale, tweenTime, timeout);
            TweenHandler.Instance.CreateTween(tweenParameters);
            Destroy(go, timeout);
        }

        letters.Clear();
    }
}

