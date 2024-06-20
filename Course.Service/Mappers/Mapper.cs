using Course.Service.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Course.Service.Mappers
{
    public static class Mapper<TSource, TDestination> where TDestination : class where TSource : class
    {
        private static readonly Func<TSource, TDestination> MapFunction = CreateMapFunction();
        private static readonly Action<TSource, TDestination> MapExistingFunction = CreateMapExistingFunction();

        public static TDestination Map(TSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return MapFunction(source);
        }

        public static void Map(TSource source, TDestination destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            MapExistingFunction(source, destination);
        }

        private static Func<TSource, TDestination> CreateMapFunction()
        {
            var sourceParameter = Expression.Parameter(typeof(TSource), "source");
            var bindings = typeof(TDestination).GetProperties()
                .Where(destProp => destProp.CanWrite)
                .Select(destProp =>
                {
                    var sourceProperty = typeof(TSource).GetProperty(destProp.Name);
                    if (sourceProperty != null && sourceProperty.CanRead)
                    {
                        return Expression.Bind(destProp, Expression.Property(sourceParameter, sourceProperty));
                    }
                    return null;
                })
                .Where(binding => binding != null)
                .ToList();

            var initializer = Expression.MemberInit(Expression.New(typeof(TDestination)), bindings);
            var lambda = Expression.Lambda<Func<TSource, TDestination>>(initializer, sourceParameter);
            return lambda.Compile();
        }

        private static Action<TSource, TDestination> CreateMapExistingFunction()
        {
            var sourceParameter = Expression.Parameter(typeof(TSource), "source");
            var destinationParameter = Expression.Parameter(typeof(TDestination), "destination");

            var bindings = typeof(TDestination).GetProperties()
                .Where(destProp => destProp.CanWrite)
                .Select(destProp =>
                {
                    var sourceProperty = typeof(TSource).GetProperty(destProp.Name);
                    if (sourceProperty != null && sourceProperty.CanRead)
                    {
                        var assign = Expression.Assign(
                            Expression.Property(destinationParameter, destProp),
                            Expression.Property(sourceParameter, sourceProperty));
                        return assign;
                    }
                    return null;
                })
                .Where(binding => binding != null)
                .ToList();

            var block = Expression.Block(bindings);
            var lambda = Expression.Lambda<Action<TSource, TDestination>>(block, sourceParameter, destinationParameter);
            return lambda.Compile();
        }
    }

}