using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class Exp
    {

        private static Dictionary<string, Expression<Func<Author, object>>> _rules;
        static Exp()
        {
            _rules = new Dictionary<string, Expression<Func<Author, object>>>();
            _rules.Add("Id", a => a.Id);
            _rules.Add("CreatedBy", a => a.CreatedBy);
        }

        public static Expression<Func<T, ExpandoObject>> ToDynamicProjection<T>(string[] pNames)
        {
            var entity = Expression.Parameter(typeof(T));

            var model = Expression.Variable(typeof(ExpandoObject));
            var modelAssign = Expression.Assign(model, Expression.Constant(new ExpandoObject()));
            var returnValue = Expression.Label(Expression.Label(typeof(ExpandoObject)), model);

            var addPropMethod = typeof(ExpandoObjectExtensions)
                .GetMethod(nameof(ExpandoObjectExtensions.AddProperty), new[] { typeof(ExpandoObject), typeof(string), typeof(object) });

            var executePredicate = typeof(ExpressionHelpers).GetMethod(nameof(ExpressionHelpers.Execute)).MakeGenericMethod(typeof(T));

            var expressions = new List<Expression>();
            expressions.Add(modelAssign);
            expressions.AddRange(
                _rules
                    .Where(x => pNames.Contains(x.Key))
                    .Select(x =>
                        Expression.Call(
                            null,
                            addPropMethod,
                            model,
                            Expression.Constant(x.Key, typeof(string)),
                            Expression.Call(null, executePredicate, entity, Expression.Quote(x.Value))
                        )
                    )
            );
            expressions.Add(returnValue);

            var block = Expression.Block(
                new[] { model },
                expressions
            );

            return Expression.Lambda<Func<T, ExpandoObject>>(block, entity);
        }

        public static IQueryable<dynamic> ToDynamic<T>(this IQueryable<T> source, string[] pNames)
        {
            //public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)

            var exp = Exp.ToDynamicProjection<T>(pNames);
            return source.Select(exp);
        }
    }

    public static class ExpandoObjectExtensions
    {
        public static void AddProperty(this ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }

    public static class ExpressionHelpers
    {
        public static object Execute<T>(T source, Expression<Func<T, object>> exp)
        {
            return exp.Compile()(source);
        }
    }

    public static class TypeExtensions
    {
        public const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        public const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public static MethodInfo StaticGenericMethod(this Type type, string methodName, int parametersCount)
        {
            foreach (MethodInfo foundMethod in type.GetMember(methodName, MemberTypes.Method, StaticFlags & ~BindingFlags.NonPublic))
            {
                if (foundMethod.IsGenericMethodDefinition && foundMethod.GetParameters().Length == parametersCount)
                {
                    return foundMethod;
                }
            }
            throw new ArgumentOutOfRangeException(nameof(methodName), $"Cannot find suitable method {type}.{methodName}({parametersCount} parameters).");
        }
    }
}
