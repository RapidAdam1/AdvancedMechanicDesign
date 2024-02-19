using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class RegExr 
{
    public static string RegexReader(string InputText, Regex RegexPattern)
    {
        Debug.Log("In Text: " + InputText);
        Match regexMatch = RegexPattern.Match(InputText);

        if(regexMatch.Success)
        {
            for(int i=0;i<regexMatch.Groups.Count;i++) 
            {
                Debug.Log(regexMatch.Groups[i]);
            }
            return regexMatch.Groups[1].ToString();
        }
        return "";
    }
}
