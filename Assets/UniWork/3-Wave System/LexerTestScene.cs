using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LexerTestScene : MonoBehaviour
{
    [SerializeField] private string InputFile;

    private void Start()
    {
        List<ParsedBlock> BlockArr = new List<ParsedBlock>();
        BlockArr.AddRange(ECSTomBenBlockParser.InitialDataRead(InputFile));
 
    }
}
