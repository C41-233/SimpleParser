
namespace SimpleParser.Grammars.Parser
{
    public interface IASTNode
    {

        int Id { get; }
        string Name { get; }
        string Value { get; }
        bool IsTerminal { get; }

    }

}
