using System.Linq.Expressions;

namespace SynthShop.Tests.Extensions;

public class ExpressionEqualityComparer : IEqualityComparer<Expression>
{
    public static readonly ExpressionEqualityComparer Instance = new();

    public bool Equals(Expression x, Expression y)
    {
        return ExpressionComparer.AreEqual(x, y);
    }

    public int GetHashCode(Expression obj)
    {
        return obj.ToString().GetHashCode();
    }
}

public static class ExpressionComparer
{
    public static bool AreEqual(Expression x, Expression y)
    {
        return new ExpressionComparisonVisitor().Visit(x, y);
    }

    private class ExpressionComparisonVisitor : ExpressionVisitor
    {
        private bool _areEqual = true;

        public bool Visit(Expression x, Expression y)
        {
            if (x == null || y == null) return x == y;

            if (x.NodeType != y.NodeType || x.Type != y.Type)
            {
                _areEqual = false;
                return false;
            }

            base.Visit(x);
            base.Visit(y);

            return _areEqual;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var y = node as BinaryExpression;
            if (node.Method != y.Method || node.IsLifted != y.IsLifted || node.IsLiftedToNull != y.IsLiftedToNull)
                _areEqual = false;
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var y = node as MemberExpression;
            if (node.Member != y.Member) _areEqual = false;
            return node;
        }
    }
}