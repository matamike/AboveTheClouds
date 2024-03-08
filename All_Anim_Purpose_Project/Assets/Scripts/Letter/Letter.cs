using UnityEngine;

public class Letter : MonoBehaviour{
    [SerializeField] private char letter = ' ';
    public char GetLetter() => letter;
}
