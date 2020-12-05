using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemGenerator : MonoBehaviour
{
    //params of L-System
    private string axiom = "F";
    private float angle;

    private float length;
    private string currentString;

    //contains the set of rules
    private Dictionary<char, string> rules = new Dictionary<char, string>();

    //contains a stack of transforms in order to save and restore (positions,rotations)
    private Stack<TransformInfo> transformStack = new Stack<TransformInfo>();

    private bool isGenerating = false;

    // Start is called before the first frame update
    void Start()
    {
        //the rule and the constants : + - []
        rules.Add('F', "FF+[+F-F-F]-[-F+F+F]");
        currentString = axiom; //start at the axiom 'F'
        angle = 25f;
        length = 10;

        //calls for the generation
        StartCoroutine(GenerateLSystem());
    }

    //repeats the generation
    IEnumerator GenerateLSystem()
    {
        int count = 0;

        while (count < 5)
        {
            if(!isGenerating)
            {
                isGenerating = true;
                StartCoroutine(Generate());
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    IEnumerator Generate()
    {
        //params initialization
        length = length / 2;
        string newString = "";

        char[] stringCharacters = currentString.ToCharArray();

        for(int i = 0; i < stringCharacters.Length; i++)
        {
            char currentCharacter = stringCharacters[i];

            if (rules.ContainsKey(currentCharacter))
            {
                newString += rules[currentCharacter];
            } else
            {
                newString += currentCharacter.ToString();
            }
        }

        currentString = newString;
        Debug.Log(currentString);

        stringCharacters = currentString.ToCharArray();


        //apply the rules of going forward (F), turn right (+) or left (-) & save ( [ ) or restore ( ] ) (position,rotation) for every string
        for (int i = 0; i < stringCharacters.Length; i++)
        {
            char currentCharacter = stringCharacters[i];

            if (currentCharacter == 'F')
            {
                Vector3 initialPos = transform.position;
                transform.Translate(Vector3.forward * length);
                Debug.DrawLine(initialPos, transform.position, Color.green, 10000f, false);
                yield return null;
            } else if(currentCharacter == '+')
            {
                transform.Rotate(Vector3.up * angle);
            }
            else if (currentCharacter == '-')
            {
                transform.Rotate(Vector3.up * -angle);
            }
            else if(currentCharacter == '[')
            {
                TransformInfo ti = new TransformInfo();
                ti.position = transform.position;
                ti.rotation = transform.rotation;
                transformStack.Push(ti);
            }
            else if(currentCharacter == ']')
            {
                TransformInfo ti = transformStack.Pop();
                transform.position = ti.position;
                transform.rotation = ti.rotation;
            }
        }

        isGenerating = false;
    }
}
