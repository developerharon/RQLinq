using System.Linq.Expressions;
using System.Reflection;

namespace RQLinq
{
    public class RqlEvaluator
    {
        private readonly RqlExpressionSyntax _root;

        public RqlEvaluator(RqlExpressionSyntax root)
        {
            _root = root;
        }

        public Expression<Func<T, bool>> Evaluate<T>()
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var bodyExpression = EvaluateExpressionSyntax<T>(parameterExpression, _root);
            return Expression.Lambda<Func<T, bool>>(bodyExpression, parameterExpression);
        }

        public static Expression<Func<T, bool>> Evaluate<T>(Dictionary<string, string[]> query)
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var conditions = new List<Expression>();

            foreach (var (key, values) in query)
            {
                var (left, right) = CreateExpression<T>(parameterExpression, key, values.First());
                var expression = Expression.Equal(left, right);
                conditions.Add(expression);
            }

            var bodyExpression = conditions.Aggregate(Expression.AndAlso);
            return Expression.Lambda<Func<T, bool>>(bodyExpression, parameterExpression);
        }

        private Expression EvaluateExpressionSyntax<T>(ParameterExpression parameterExpression, RqlNode node)
        {
            if (node is BinaryExpressionSyntax binaryExpression)
                return EvaluateBinaryExpression<T>(parameterExpression, binaryExpression);

            var conditions = new List<Expression>();

            if (node is CompoundExpressionSyntax compoundExpression)
            {
                var expressions = compoundExpression.Expressions;

                foreach (var expression in expressions)
                {
                    var condition = EvaluateExpressionSyntax<T>(parameterExpression, expression);
                    conditions.Add(condition);
                }

                if (compoundExpression.OperatorToken.Kind == RqlTokenKind.AND)
                    return conditions.Aggregate(Expression.AndAlso);

                if (compoundExpression.OperatorToken.Kind == RqlTokenKind.OR
                    || compoundExpression.OperatorToken.Kind == RqlTokenKind.IN
                    || compoundExpression.OperatorToken.Kind == RqlTokenKind.OUT)
                    return conditions.Aggregate(Expression.OrElse);

                throw new Exception($"Unexpected compound operator {compoundExpression.OperatorToken.Kind}");
            }

            throw new Exception($"Unexpected node {node.Kind}");
        }

        private Expression EvaluateBinaryExpression<T>(ParameterExpression parameterExpression, BinaryExpressionSyntax binaryExpression)
        {
            var (left, right) = CreateExpression<T>(parameterExpression, binaryExpression.Left.PropertyToken.Text, binaryExpression.Right.PropertyToken.Text);

            if (binaryExpression.OperatorToken.Kind == RqlTokenKind.EQ)
                return Expression.Equal(left, right);

            if (binaryExpression.OperatorToken.Kind == RqlTokenKind.NE)
                return Expression.NotEqual(left, right);

            if (binaryExpression.OperatorToken.Kind == RqlTokenKind.LT)
                return Expression.LessThan(left, right);

            if (binaryExpression.OperatorToken.Kind == RqlTokenKind.LE)
                return Expression.LessThanOrEqual(left, right);

            if (binaryExpression.OperatorToken.Kind == RqlTokenKind.GT)
                return Expression.GreaterThan(left, right);

            if (binaryExpression.OperatorToken.Kind == RqlTokenKind.GE)
                return Expression.GreaterThanOrEqual(left, right);

            throw new Exception($"Unexpected binary operator {binaryExpression.OperatorToken.Kind}");
        }

        private static (Expression, Expression) CreateExpression<T>(ParameterExpression parameterExpression, string key, string value)
        {
            var propertyNames = key.Split('.');
            var propertyName = propertyNames.Last();

            var propertyExpression = propertyNames
                .SkipLast(1)
                .Aggregate((Expression)parameterExpression, Expression.Property);

            // If property is null, then we are probably not implementing it
            var propertyType = propertyExpression.Type.GetProperty(
                propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?
                .PropertyType;

            if (propertyExpression.Type == typeof(IDictionary<string, string[]>))
            {
                // If the property is an IDictionary<string, string[]>, use ContainsKey and Contains methods to check if the specified key and value exist
                var valueParam = Expression.Constant(value, typeof(string));
                var dictionaryKey = Expression.Constant(propertyName, typeof(string));
                var dictionaryContainsKey = Expression.Call(propertyExpression, "ContainsKey", null, dictionaryKey);

                // Create an expression to access the indexer of the dictionary using the key parameter
                Expression accessExpr = Expression.MakeIndex(
                    propertyExpression,
                    typeof(IDictionary<string, string[]>).GetProperty("Item"),
                    new[] { dictionaryKey });

                // Convert the indexer expression to a string[] expression
                Expression stringArrayExpr = Expression.Convert(accessExpr, typeof(string[]));

                // The Contains method lives on the Enumerable type in System.Linq
                var containsMethods = typeof(System.Linq.Enumerable)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(mi => mi.Name == "Contains");

                MethodInfo whereMethod = containsMethods.First().MakeGenericMethod(new[] { typeof(string) });

                var containsExpr = Expression.Call(null, whereMethod, stringArrayExpr, valueParam);

                var containsKeyExpression = Expression.Equal(dictionaryContainsKey, Expression.Constant(true));
                var containsValueExpression = Expression.Equal(containsExpr, Expression.Constant(true));

                return (containsKeyExpression, containsValueExpression);
            }
            else if ((propertyExpression.Type != typeof(T) && typeof(ICollection<>) == propertyExpression.Type.GetGenericTypeDefinition())
                || propertyType == typeof(ICollection<string>))
            {
                Type elementType;
                object? valueParam;
                ParameterExpression lambdaParameterExpression;
                BinaryExpression equalExpression;

                if (propertyType == typeof(ICollection<string>))
                {
                    elementType = typeof(string);
                    valueParam = Convert.ChangeType(value, typeof(string));
                    lambdaParameterExpression = Expression.Parameter(elementType, "y");
                    equalExpression = Expression.Equal(lambdaParameterExpression, Expression.Constant(valueParam));
                    var ai = propertyExpression.Type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    propertyExpression = Expression.Property(propertyExpression, ai);
                }
                else
                {
                    elementType = propertyExpression.Type.GetGenericArguments()[0];
                    propertyType = elementType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?.PropertyType;

                    if (propertyType == null)
                        throw new NotImplementedException($"query string {propertyName} is not implemented.");

                    valueParam = ParseType(value, propertyType);
                    lambdaParameterExpression = Expression.Parameter(elementType, "y");
                    var prop = Expression.Property(lambdaParameterExpression, propertyName);

                    // We need to change here to refect our factor equation
                    // this expression is run in our any method
                    // We define if any in our collection fits our lambda
                    equalExpression = Expression.Equal(prop, Expression.Constant(valueParam));
                }

                var predicateType = typeof(Func<,>).MakeGenericType(elementType, typeof(bool));
                var anyMethod = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Length == 2).MakeGenericMethod(elementType);

                var lambda = Expression.Lambda(predicateType, equalExpression, lambdaParameterExpression);

                var anyExpression = Expression.Call(null, anyMethod, propertyExpression, lambda);
                var trueExpression = Expression.Constant(true);

                return (anyExpression, trueExpression);
            }
            else
            {
                if (propertyType == null)
                    throw new NotImplementedException($"query string {propertyName} is not implemented.");

                var propertyValue = ParseType(value, propertyType);

                var propertyAccessExpression = Expression.Property(propertyExpression, propertyName);
                var propertyValueExpression = Expression.Constant(propertyValue);

                return (propertyAccessExpression, propertyValueExpression);
            }
        }

        private static object? ParseType(string value, Type conversionType)
        {
            if (value == null)
                return null;

            try
            {
                return Convert.ChangeType(value, conversionType);
            }
            catch (InvalidCastException ex)
            {
                if (conversionType == typeof(Guid))
                    return new Guid(value);

                if (conversionType == typeof(Uri))
                    return new Uri(value);

                if (conversionType.IsEnum)
                {
                    var conversionSuccess = Enum.TryParse(conversionType, value, true, out object? outputEnum);

                    if (conversionSuccess)
                        return outputEnum;

                    throw new ArgumentException($"{value} is not a valid enumeration.");
                }

                throw ex;
            }
        }
    }
}