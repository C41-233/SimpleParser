using System.Collections.Generic;

namespace SimpleParser
{

    public interface IASTVisitor
    {

        void BeginNode(IASTNode node);

        void EndNode(IASTNode node);

    }

    public interface IASTNode
    {
        string Name { get; }
        bool IsTerminal { get; }
        IEnumerable<string> Symbols { get; }

    }

}
