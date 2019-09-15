namespace SimpleParser.Grammars.Parser
{
    public interface IASTVisitor
    {

        void BeginNode(IASTNode node);

        void EndNode(IASTNode node);

    }
}