using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class RegExr 
{
    public static string RegexReader(string InputText, Regex RegexPattern)
    {
       
        Match regexMatch = RegexPattern.Match(InputText);

        if(regexMatch.Success)
        {
            return regexMatch.Groups[1].ToString();
        }
        return "";
    }
}
