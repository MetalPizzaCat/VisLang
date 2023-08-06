namespace VisLang;


public class Variable
{
    public Variable(string name, uint address)
    {
        Name = name;
        Address = address;
    }

    public string Name { get; set; } = String.Empty;

    public uint Address { get; set; } = 0;
}