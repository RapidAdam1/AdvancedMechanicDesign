using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Codice.Client.Common.GameUI;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections;

public class TomBenBlockParser : MonoBehaviour
{
    [SerializeField] EnemyToSpawn EnemyPrefab;
    [SerializeField] private string InputFile;

    //Current Block Parsing & List of Already Parsed Blocks
    private ParsedBlock currentBlock = new ParsedBlock();
    public List<ParsedBlock> blocks = new List<ParsedBlock>();


    List<ParsedBlock> Types = new List<ParsedBlock>();
    List<ParsedBlock> Waves = new List<ParsedBlock>();
    List<ParsedBlock> Clusters = new List<ParsedBlock>();

    int EnemiesSpawned = 0;
    ParsedBlock m_CurrentWave;
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

        SortToLists(blocks);

        m_CurrentWave = Waves[0];
        StartCoroutine(ReadWave());
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

 
    ///Manage Spawning Logic of Waves
    public void SortToLists(List<ParsedBlock> list)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            switch (list[i].type)
            {
                case "wave":
                    Waves.Add(list[i]);
                    break;
                case "cluster":
                    Clusters.Add(list[i]);
                    break;
                case "type":
                    Types.Add(list[i]);
                    break;
            }
        }
    }

    ParsedBlock GetBlockFromID(int ID, List<ParsedBlock> List)
    {
        for (int i = 0; i < List.Count; i++)
        {
            if (List[i].id == ID)
                return List[i];
        }
        Debug.LogError("NO FOUND ID");
        return new ParsedBlock();
    }

    void SpawnType(ParsedBlock Type)
    {
        int Damage = 0;
        int Speed = 0;
        int Health = 0;

        Regex RegexPattern = new Regex(@"(\w+)=>(\d*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Type.content);
        for (int i = 0; i < RegexMatch.Count; i++)
        {
            switch (RegexMatch[i].Groups[1].ToString())
            {
                case "health":
                    Health = int.Parse(RegexMatch[i].Groups[2].ToString());
                    break;
                case "speed":
                    Speed = int.Parse(RegexMatch[i].Groups[2].ToString());
                    break;
                case "damage":
                    Health = int.Parse(RegexMatch[i].Groups[2].ToString());
                    break;

            }
        }

        Vector3 RandomPos = new Vector3(0, 0, Random.Range(-5, 6));
        EnemyToSpawn Temp = Instantiate(EnemyPrefab, transform.position+RandomPos, transform.rotation);
        Temp.Init(Speed, Damage, Health,Type.id);
        EnemiesSpawned++;
        Temp.UpdateEnemyCount += UpdateCount;
        Debug.Log($"Type {Type.id}");
    }

    void UpdateCount()
    {
        EnemiesSpawned--;
    }

    void ReadCluster(ParsedBlock Cluster)
    {
        Regex RegexPattern = new Regex(@"(\d*):(\d*)");
        MatchCollection RegexMatch = RegexPattern.Matches(Cluster.content);
        for (int i = 0; i < RegexMatch.Count; i++)
        {
            int ReadID = int.Parse(RegexMatch[i].Groups[1].ToString());
            int IDCount = int.Parse(RegexMatch[i].Groups[2].ToString());

            for (int j = 0; j < IDCount; j++)
            {
                SpawnType(GetBlockFromID(ReadID, Types));
            }
        }
    }

    IEnumerator ReadWave()
    {
        Regex RegexPattern = new Regex(@"(?:(\w)(\d)*(?:\<(\d*)\>)*(?:\[(\d)\])*[^\!\?]*)");
        MatchCollection RegexMatch = RegexPattern.Matches(m_CurrentWave.content);
        for (int i = 0; i < RegexMatch.Count; i++)
        {
            Debug.Log(RegexMatch[i]);

            yield return new WaitForSeconds(0.5f);
            //TypeToRead
            char Type = RegexMatch[i].Groups[1].Value.ToString()[0];
            int ID = int.Parse(RegexMatch[i].Groups[2].Value.ToString());


            float WaitTime = RegexMatch[i].Groups[3].Success ? int.Parse(RegexMatch[i].Groups[3].ToString()) : 999;
            int Threshold = RegexMatch[i].Groups[4].Success ? int.Parse(RegexMatch[i].Groups[4].ToString()) : 0;


            float CurrentWaitTime = 0;
            while (CurrentWaitTime < WaitTime && Threshold < EnemiesSpawned)
            {
                CurrentWaitTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            //SpawnEnemy
            if (Type == 'T')
                SpawnType(GetBlockFromID(ID, Types));
            else if (Type == 'C')
                ReadCluster(GetBlockFromID(ID, Clusters));

        }
        Debug.Log(EnemiesSpawned);
        yield return null;
    }
}