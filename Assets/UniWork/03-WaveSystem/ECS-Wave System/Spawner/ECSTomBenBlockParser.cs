using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Collections;

public static class ECSTomBenBlockParser
{
    public static List<ParsedBlock> InitialDataRead(FixedString512Bytes filepath)
    {
        
        List<ParsedBlock> Blocks = new List<ParsedBlock>();
        string fileContent = "";
        fileContent = File.ReadAllText(filepath.ToString());


        Regex FilePattern = new Regex(@"(type|cluster|wave) - (\d)(.*?) (?:_Tom (.*?) _Ben)"); //Finds All Content
        MatchCollection Matches = FilePattern.Matches(fileContent);
        
        for(int i = 0; i < Matches.Count; i++)
        {
            ParsedBlock Temp = new ParsedBlock();
            Temp.type = Matches[i].Groups[1].ToString();
            Temp.id = int.Parse(Matches[i].Groups[2].ToString());
            Temp.content = Matches[i].Groups[4].ToString();

            if (Matches[i].Groups[3].Success)
            {
                Regex NamePatterm = new Regex(@"\((\w*)\)");
                Match MatchName = NamePatterm.Match(Matches[i].Groups[3].ToString());
                Temp.name = MatchName.Groups[1].ToString();
            }
            CheckAndUpdateBlock(Temp, ref Blocks);

        }
        return Blocks;
    }

    static void CheckAndUpdateBlock(ParsedBlock UpdatedInfo, ref List<ParsedBlock> blockList)
    {
        for (int i = 0; i < blockList.Count; i++)
        {
            if (UpdatedInfo.id == blockList[i].id && UpdatedInfo.type == blockList[i].type)
            {
                blockList.RemoveAt(i);
                blockList.Add(UpdatedInfo);
                return;
            }
        }
        blockList.Add(UpdatedInfo) ;
    }

    public static ParsedBlock GetBlockFromID(int Id, string Type, List<ParsedBlock> BlockList)
    {
        ParsedBlock TempBlock = new ParsedBlock();
        TempBlock.id = -1;
        foreach (ParsedBlock Block in BlockList)
        {
            if(Block.id == Id && Block.type == Type)
            {
                return Block;
            }
        }
        return TempBlock;
    }

    public static int GetNextWaveID(int CurrentWaveID ,List<ParsedBlock> BlockList)
    {
        bool Next = false;
        ParsedBlock TempBlock = new ParsedBlock();
        TempBlock.id = -1;
        foreach (ParsedBlock Block in BlockList)
        {
            if (Next)
            {
                TempBlock.id = Block.id;
                break;
            }
            if(Block.id == CurrentWaveID && Block.type=="wave")
            {
                Next = true;
            }
        }

        return TempBlock.id;
    }

    public static int GetFirstWaveID(List<ParsedBlock> BlockList)
    {
        foreach (ParsedBlock Block in BlockList)
        {
            if(Block.type == "wave")
                return Block.id;
        }
        return -1;
    }
}