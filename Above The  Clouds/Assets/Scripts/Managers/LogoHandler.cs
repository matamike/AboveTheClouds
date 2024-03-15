using System.Collections.Generic;
using UnityEngine;
using ObjTween;

public class LogoHandler : Singleton<LogoHandler>{
    [SerializeField] List<Letter> letterPrefabs = new List<Letter>();
    [SerializeField] Vector3 offset = Vector3.right;
    [SerializeField] Vector3 startingLogoPosition = Vector3.zero;
    [SerializeField] string logo = "";

    private Dictionary<char, Letter> letterDictionary = new Dictionary<char, Letter>();
    private List<GameObject> letters = new List<GameObject>();

    private void Start(){
        InitializeLetterDictionary();
    }

    private void InitializeLetterDictionary(){
        foreach (Letter letter in letterPrefabs){
            letterDictionary.Add(letter.GetLetter(), letter);
        }
    }

    private void CreateLogo(){
        int times = 0;
        foreach (char c in logo){
            if (letterDictionary.ContainsKey(c)){
                Vector3 targetPosition = startingLogoPosition + (offset * times);
                GameObject x = Instantiate(letterDictionary[c].gameObject, Vector3.down * 100f, Quaternion.identity);
                letters.Add(x);
                float tweenTime = Random.Range(1f, 2f);
                float timeout = 12f;
                TweenParameters tweenParameters = new TweenParameters(letters[letters.Count - 1], targetPosition, Vector3.zero, x.transform.localScale, tweenTime, timeout);
                TweenHandler.Instance.CreateTween(tweenParameters);
            }
            times++;
        }
    }

    public void RequestTrigger() => CreateLogo();

    public void RequestCharacterRemoval(GameObject go, float delay){
        if (letters.Find(x => x.gameObject == go)){
            letters.Remove(go);
            Destroy(go, delay);
        }
    }
}