using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Codice.Client.Common.GameUI;
using System.Runtime.InteropServices.WindowsRuntime;

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
    bool IsDuplicate = false;
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
        IsDuplicate = false;
        currentBlock.type = GetLastMatchedBlockType();

        ChangeState(ParserState.InsideBlockHeader);
    }
    private void ParseInsideBlockHeader()
    {
        //Read All Up to the _Tom
        while (!BufferHasAny("_Tom") && !ReachedEnd())
            NextChar();

        Regex RegexSearchID = new Regex(@"(\d)");
        Regex NameSearch = new Regex(@"(?:\((.*)\))");

        int FoundID = int.Parse(RegExr.RegexReader(charBuffer, RegexSearchID));
        string FoundName = RegExr.RegexReader(charBuffer, NameSearch);
        
        currentBlock.id = FoundID;
        currentBlock.name = FoundName;

        if (ReachedEnd())
            return;
        ChangeState(ParserState.InsideBlockBody);
    }
    private void ParseInsideBlock()
    {
        //Read All Data between _Tom & _Ben
        while (!BufferHasAny("_Ben") && !ReachedEnd())
            NextChar();

        Regex ContentSearch = new Regex(@"(?:\s(.*) _Ben)");
        
        string Content = RegExr.RegexReader(charBuffer,ContentSearch);
        currentBlock.content = Content;

        if (IsDuplicateBlock(currentBlock.type, currentBlock.id))
        {
            UpdateBlock(currentBlock.type, currentBlock.id, currentBlock.name,currentBlock.content);
        }
        else
        {
            blocks.Add(currentBlock);
        }
     
        if (ReachedEnd())
            return;
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

    void UpdateBlock(string type, int ID, string Name, string content) 
    {
        //Get The Current Block
        for (int i = 0; i<blocks.Count;i++)
        {
            if (blocks[i].id == ID && blocks[i].type == type)
            {
                ParsedBlock UpdateBlock = blocks[i];
                blocks.Remove(blocks[i]);

                if(Name.Length != 0)
                    UpdateBlock.name = Name;
                
                UpdateBlock.content = content;
                blocks.Add(UpdateBlock);
                break;
            }
        }

        return;
    }

    bool IsDuplicateBlock(string type, int ID)
    {
        foreach (ParsedBlock block in blocks) 
        {
            if (block.id == ID && block.type == type)
                return true;
        }

        return false;
    }

}