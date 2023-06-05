using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace z.Autowire
{
    internal class DelegateCompiler
    {
        public delegate void Setter(object? target, object? value);

        public delegate object? Factory(object?[] arguments);

        public delegate object Constructor(object?[] arguments);

        public static Setter CreateSetter(Type type, MethodInfo setter)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");
            ParameterExpression valueParameter = Expression.Parameter(typeof(object), "value");

            MethodCallExpression call = Expression.Call(
                Expression.Convert(instanceParameter, type),
                setter,
                Expression.Convert(valueParameter, setter.GetParameters()[0].ParameterType));

            Expression<Setter> lambda = Expression.Lambda<Setter>(
                call,
                instanceParameter,
                valueParameter);

            return lambda.Compile();
        }

        public static Factory CreateFactory(MethodInfo method)
        {
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            MethodCallExpression call = Expression.Call(
                null,
                method,
                CreateParameterExpressions(method, argumentsParameter));

            Expression<Factory> lambda = Expression.Lambda<Factory>(
                Expression.Convert(call, typeof(object)),
                argumentsParameter);

            return lambda.Compile();
        }

        public static Constructor CreateConstructor(ConstructorInfo constructor)
        {
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            NewExpression call = Expression.New(
                constructor,
                CreateParameterExpressions(constructor, argumentsParameter));

            Expression<Constructor> lambda = Expression.Lambda<Constructor>(
                Expression.Convert(call, typeof(object)),
                argumentsParameter);

            return lambda.Compile();
        }

        private static Expression[] CreateParameterExpressions(MethodBase method, Expression argumentsParameter)
        {
            return method.GetParameters()
                .Select((parameter, index) =>
                    Expression.Convert(
                        Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)),
                        parameter.ParameterType))
                .ToArray();
        }
    }
}
