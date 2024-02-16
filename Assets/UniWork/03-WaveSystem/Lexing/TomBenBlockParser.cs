using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

public class TomBenBlockParser : MonoBehaviour
{
    [SerializeField] private string regexPattern;
    /*    [SerializeField] private string InputText;

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
        }*/

    [SerializeField]
    private string InputFile;

    private ParsedBlock currentBlock = new ParsedBlock();
    public List<ParsedBlock> blocks = new List<ParsedBlock>();

    private enum ParserState
    {
        InsideBlockBody, InsideBlockHeader, OutsideBlock
    }

    private ParserState state = ParserState.OutsideBlock;

    private string charBuffer = "";
    private int charIndex = 0;

    private string fileContent = "";
    private void Awake()
    {
        if (!File.Exists(InputFile))
            throw new UnityException("Cant Open File");
        return;
        TomBenBlockParser blockParser = new TomBenBlockParser();
        blocks = blockParser.ParseFromFile(InputFile);
    }

    private void ClearBuffer() => charBuffer = "";
    private bool ReachedEnd() => charIndex >= fileContent.Length;
    private char NextChar()
    {
        charBuffer += fileContent[charIndex];
        return fileContent[charIndex++];
    }
    private void ChangeState(ParserState state)
    {
        this.state = state;
        ClearBuffer();
    }

    private bool BufferHas(string token ) => charBuffer.EndsWith( token );
    private bool BufferHasAny(params string[] tokens)
    {
        foreach (var token in tokens)
        {

            if (BufferHas(token))
            { 

                return true;
            }
        }
        return false;
    }

    public List<ParsedBlock> ParseFromFile(string filepath)
    {
        charIndex = 0;
        charBuffer = "";
        state = ParserState.OutsideBlock;

        fileContent =  File.ReadAllText(filepath);

        while (!ReachedEnd())
        {
            switch (state)
            {
                case ParserState.OutsideBlock:
                    ParseOutsideBlock();
                    break;
                case ParserState.InsideBlockBody:
                    //ParseInsideBlock();
                    break;
                case ParserState.InsideBlockHeader:
                    //ParseInsideBlockHeader();
                    break;
            }
        }
        return blocks;
    }

    private void ParseOutsideBlock()
    {
        while(!BufferHasAny("cluster","type","wave") && !ReachedEnd())
            NextChar();
        
        if (ReachedEnd())
            return;

        currentBlock.type = GetLastMatchedBlockType();

        ChangeState(ParserState.InsideBlockBody);
    }

    private string GetLastMatchedBlockType()
    {
        string lastMatched = null;

        lastMatched ??= (BufferHas("cluster") ? "cluster" : null);
        lastMatched ??= (BufferHas("wave") ? "wave" : null);
        lastMatched ??= (BufferHas("type") ? "type" : null);

        return lastMatched;
    }

 
}

[System.Serializable]
public struct ParsedBlock
{
    public string type;
    public string name;
    public string id;
    public string content;

    public override string ToString()
    {
        return $"ParsedBlock(type={type}, id={id}, name =={name}, content={content})";
    }
}