using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TomBenBlockParser : MonoBehaviour
{
    [SerializeField]
    private string InputFile;

    //Current Block Parsing & List of Already Parsed Blocks
    private ParsedBlock currentBlock = new ParsedBlock();
    public List<ParsedBlock> blocks = new List<ParsedBlock>();
    
    //Parser States
    private enum ParserState
    {
        InsideBlockBody, InsideBlockHeader, OutsideBlock
    }

    //Default To Outside Block
    private ParserState state = ParserState.OutsideBlock;

    //Temp Buffer for Parsing
    private string charBuffer = "";
    private int charIndex = 0;

    //File to Parse
    private string fileContent = "";
    private void Awake()
    {
        if (!File.Exists(InputFile))
            throw new UnityException("Cant Open File");

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

    private bool BufferHas(string token ) => charBuffer.EndsWith(token);
    private bool BufferHasAny(params string[] tokens)
    {
        foreach (var token in tokens)
        {
            if (BufferHas(token)) 
                return true;
        }
        return false;
    }

    public List<ParsedBlock> ParseFromFile(string filepath)
    {
        //Reset State
        charIndex = 0;
        charBuffer = "";
        state = ParserState.OutsideBlock;
        //Read All the File
        fileContent =  File.ReadAllText(filepath);

        Debug.Log(fileContent);
        while (!ReachedEnd())
        {
            switch (state)
            {
                case ParserState.OutsideBlock:
                    ParseOutsideBlock();
                    break;
                case ParserState.InsideBlockHeader:
                    ParseInsideBlockHeader();
                    break;
                case ParserState.InsideBlockBody:
                    ParseInsideBlock();
                    blocks.Add(currentBlock);
                    break;
            }
        }
        return blocks;
    }

    private void ParseOutsideBlock()
    {
        //Look for Keywords
        while(!BufferHasAny("cluster","type","wave") && !ReachedEnd())
            NextChar();
        
        //End Of File? Stop Parsing
        if (ReachedEnd())
            return;

        currentBlock.type = GetLastMatchedBlockType();

        ChangeState(ParserState.InsideBlockHeader);
    }
    private void ParseInsideBlockHeader()
    {
        //Find Integer & Name
        while (!BufferHasAny(" ") && !ReachedEnd())
            NextChar();

        if (ReachedEnd())
            return;
        currentBlock.id = 9;
        currentBlock.name = "This is a Name";

        ChangeState(ParserState.InsideBlockBody);
    }
    private void ParseInsideBlock()
    {
        //Read All Data between _Tom & _Ben
        while (!BufferHasAny(" ") && !ReachedEnd())
            NextChar();

        if (ReachedEnd())
            return;

        currentBlock.content = "This is Content";

        ChangeState(ParserState.OutsideBlock);
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