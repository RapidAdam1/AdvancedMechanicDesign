[System.Serializable]
public struct ParsedBlock
{
    public string type;
    public string name;
    public int id;
    public string content;

    public override string ToString()
    {
        return $"ParsedBlock(type={type}, id={id}, name =={name}, content={content})";
    }
}
