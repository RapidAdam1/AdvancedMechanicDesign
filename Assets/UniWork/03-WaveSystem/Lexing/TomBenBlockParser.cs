using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

public class TomBenBlockParser : MonoBehaviour
{
    [SerializeField] private string InputText;
    [SerializeField] private string regexPattern;

    private void OnValidate()
    {
        Regex regex =  new Regex(regexPattern);
        Match regexMatch = regex.Match(InputText);
        if (regexMatch.Success)
        {
            Debug.Log($"Matched Text {regexMatch.Groups[0]}");
            for(int i = 1; i < regexMatch.Groups.Count; i++)
            {
                Debug.Log($"Capture Group {i} = {regexMatch.Groups[i]}");
            }
        }
    }
}
