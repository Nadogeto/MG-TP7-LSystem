using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemGeneratorB : MonoBehaviour
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
        //the rule and the constants : + - [ ]//

        //rules.Add('F', "FF+[+F-F-F]-[-F+F+F]"); //first rule - test

        //case 'A'
        //rules.add('F', "F[+F]F[-F]F");
        //angle = 25.7f;

        //case 'B'
        rules.Add('F', "F[+F]F[-F][F]"); 
        angle = 20f;

        //case 'C'
        //rules.Add('F', "FF-[-F+F+F]+[+F-F-F]"); 
        //angle = 22.5f;

        currentString = axiom; //start at the axiom 'F'
        length = 10;

        //calls for the generation
        StartCoroutine(GenerateLSystem());
    }

    //repeats the generation//
    IEnumerator GenerateLSystem()
    {
        int count = 0;

        while (count < 5)
        {
            if (!isGenerating)
            {
                isGenerating = true;
                StartCoroutine(Generate());
                count++;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    //generates the plant//
    IEnumerator Generate()
    {
        //params initialization
        length = length / 2;
        string newString = "";

        //applies the rules to each string (ex: F --> FF)
        char[] stringCharacters = currentString.ToCharArray();

        for (int i = 0; i < stringCharacters.Length; i++)
        {
            char currentCharacter = stringCharacters[i];

            if (rules.ContainsKey(currentCharacter))
            {
                newString += rules[currentCharacter];
            }
            else
            {
                newString += currentCharacter.ToString();
            }
        }

        currentString = newString;
        Debug.Log(currentString);

        stringCharacters = currentString.ToCharArray();


        //apply the rules of going forward (F), turn right (-) or left (+) & save ( [ ) or restore ( ] ) (position,rotation) for every string//
        for (int i = 0; i < stringCharacters.Length; i++)
        {
            char currentCharacter = stringCharacters[i];

            if (currentCharacter == 'F') //forward
            {
                Vector3 initialPos = transform.position;
                transform.Translate(Vector3.forward * length);
                Debug.DrawLine(initialPos, transform.position, Color.green, 10000f, false);
                yield return null;
            }
            else if (currentCharacter == '+') //turn left
            {
                transform.Rotate(Vector3.up * angle);
            }
            else if (currentCharacter == '-') //turn right
            {
                transform.Rotate(Vector3.up * -angle);
            }
            else if (currentCharacter == '[') //saves the position and rotation (angle)
            {
                TransformInfo ti = new TransformInfo();
                ti.position = transform.position;
                ti.rotation = transform.rotation;
                transformStack.Push(ti);
            }
            else if (currentCharacter == ']') //restores the position and rotation (angle)
            {
                TransformInfo ti = transformStack.Pop();
                transform.position = ti.position;
                transform.rotation = ti.rotation;
            }
        }

        isGenerating = false;
    }
}
