using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Random = System.Random;

public class tests : MonoBehaviour
{
    public int n, k;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log(solution(n,k));
        }
    }

    /// <summary>
    /// Return a palindrome of length N which consists of K distinct 
    /// </summary>
    /// <param name="N">length of the word</param>
    /// <param name="K">number of letters</param>
    /// <returns>the palindrome</returns>
    public string solution(int N, int K) {
        // write your code in C# 6.0 with .NET 4.5 (Mono)

        //There are only 26 letters
        if (K > 26)
            K = 26;
        
        //Since every letter has to appear once, the max value of K is N/2
        int maxK = (int)Math.Ceiling(N / 2.0f);
        if (K > maxK)
            K = maxK;
        
        Char[] letters = GetDistinctLetters(K); //get our letters

        usedLetters = new List<char>(); //Reset the used letters list
        
        String p = GeneratePalindrome(letters, N); //Make the palindrome

        return p;
    }

    /// <summary>
    /// Return an array of K distinct letters
    /// </summary>
    /// <param name="numLetters">How many distinct letters</param>
    /// <returns>an array of distinct letters</returns>
    private char[] GetDistinctLetters(int numLetters)
    {
        List<char> letters = new List<char>(); //Store our distinct letters
        
        for (int i = 0; i < numLetters; i++)
        {
            char rnd = RandomChar();
            while (letters.Contains(rnd)) { //If this random letter wasn't distinct, we have to go again
                rnd = RandomChar();
            }
            
            letters.Add(rnd);
        }

        return letters.ToArray(); //return our chars as an array
    }

    /// <summary>
    /// Return a random Char
    /// </summary>
    /// <returns>rndChar, a random lowercase character</returns>
    private char RandomChar()
    {
        Random rnd = new Random();
        char rndChar = (char) rnd.Next(97, 123); //lowercase Chars start at 97 and go to 122;
        return rndChar;
    }

    /// <summary>
    /// Return a new distinct letter from the list
    /// </summary>
    /// <param name="letters"></param>
    /// <returns>A distinct letter</returns>
    private List<char> usedLetters; //The letters we've already used
    private char GetNextDistinctLetter(char[] letters)
    {
        Random rng = new Random();
        char next = letters[rng.Next(0, letters.Length)]; //Get random

        
        if (letters.Length > 1) //will be infinite if k is 1
        {
            while (usedLetters.Contains(next) && usedLetters.Count < letters.Length) //keep going until different letter
            {
                next = letters[rng.Next(0, letters.Length)];
            }
        }

        usedLetters.Add(next);//Set the last letter
        return next;
    }

    private string GeneratePalindrome(char[] letters, int length)
    {
        string p = "";
        
        //Start with middle letter
        p += GetNextDistinctLetter(letters);

        if (length % 2 == 0) { //If N is even, the middle letters have to be the same
            p += p;
        }
        
        if (length > 1) { //N is not 1, so we have to procedurally make the palindrome
            char next;           
            while (p.Length < length) { //Keep going til the length is right
                next = GetNextDistinctLetter(letters);
                p = next + p + next; //Add next letter to back and fromt
            }
        }

        return p;
    }
}
