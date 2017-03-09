using System;
using System.Linq.Expressions;

namespace TwentyTwenty.BaseLine
{
    public static class ReflectionExtensions
    {
        public static MemberExpression GetMemberExpression(this LambdaExpression expression, bool enforceMemberExpression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (enforceMemberExpression && memberExpression == null) throw new ArgumentException("Not a member access", "member");
            return memberExpression;
        }
    }
}