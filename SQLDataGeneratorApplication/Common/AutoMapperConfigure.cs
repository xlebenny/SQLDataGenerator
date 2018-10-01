using AutoMapper;
using Benny.CSharpHelper;
using SQLDataGeneratorLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLDataGeneratorApplication
{
    internal class AutoMapperConfigure
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg
                    .CreateMap<GenerateConfig, GenerateConfig>()
                    .ForMember(x => x.GenerateFormat, x => x.MapFrom(y => y.GenerateFormat))
                    .ForMember(x => x.GenerateRecordCount, x => x.MapFrom(y => y.GenerateRecordCount))
                    .ForMember(x => x.DatabaseName, x => x.MapFrom(y => y.DatabaseName))
                    .ForMember(x => x.IdentityInsert, x => x.MapFrom(y => y.IdentityInsert))
                    .ForAllOtherMembers(x => x.Ignore());
                cfg
                    .CreateMap<ColumnInformation, GenerateConfig>()
                    .ForMember(x => x.IsNullable, x => x.ResolveUsing(y => ParseBoolean(y.IsNullable)))
                    .ForMember(x => x.DataType, x => x.MapFrom(y => y.DataType.ToLower()));
                cfg
                   .CreateMap<TableConfig, GenerateConfig>()
                   .ForMember(x => x.TableName, x => x.Ignore());
                cfg
                   .CreateMap<DatabaseConfig, GenerateConfig>();
                cfg
                    .CreateMap<GenerateConfig, TableConfig>();
                cfg
                    .CreateMap<IEnumerable<GenerateConfig>, IEnumerable<GenerateConfig>>()
                    .ConvertUsing(UpdateITableColumnKey);
                cfg
                    .CreateMap<IEnumerable<GenerateConfig>, IEnumerable<TableConfig>>()
                    .ConvertUsing(CreateOrUpdateTableConfig);
                cfg
                    .CreateMap<IEnumerable<TableConfig>, IEnumerable<GenerateConfig>>()
                    .ConvertUsing(UpdateITableKey);
                cfg
                   .CreateMap<DatabaseConfig, IEnumerable<GenerateConfig>>()
                   .ConvertUsing(UpdateOneToMany);
            });

            Mapper.Configuration.CompileMappings();
        }

        private static bool ParseBoolean(string str)
        {
            return str != null
                    && (str.ToLower() == "true" || str.ToLower() == "yes");
        }

        private static IEnumerable<TDestination> UpdateITableColumnKey<TSource, TDestination>(IEnumerable<TSource> source, IEnumerable<TDestination> destination)
            where TSource : ITableColumnKey
            where TDestination : GenerateConfig
        {
            var result = MappingHelper.UpdateOrAdd(source, destination, x => x.GetKey(), x => ((ITableColumnKey)x).GetKey());
            return result;
        }

        private static IEnumerable<TDestination> CreateOrUpdateTableConfig<TSource, TDestination>(IEnumerable<TSource> source, IEnumerable<TDestination> destination)
            where TSource : GenerateConfig
            where TDestination : TableConfig
        {
            var tables = source.GroupBy(x => x.TableName).Select(x => x.First()).ToArray();
            var result = MappingHelper.UpdateOrAdd(
                tables, destination ?? tables.Select(x => Mapper.Map<TDestination>(x)).ToArray(),
                x => x.TableName, x => x.TableName);
            return result;
        }

        private static IEnumerable<TDestination> UpdateITableKey<TSource, TDestination>(IEnumerable<TSource> source, IEnumerable<TDestination> destination)
            where TSource : ITableKey
            where TDestination : GenerateConfig
        {
            if (source == null || destination == null) return destination;

            var dict = destination.GroupBy(x => ((ITableKey)x).GetKey()).ToDictionary(x => x.Key);

            source.ToList().ForEach(x =>
                dict[x.GetKey()].ToList().ForEach(y =>
                   Mapper.Map(x, y)
               )
           );

            return destination;
        }

        private static IEnumerable<TDestination> UpdateOneToMany<TSource, TDestination>(TSource source, IEnumerable<TDestination> destination)
            where TSource : class
            where TDestination : class
        {
            if (source == null || destination == null) return destination;

            destination.ToList().ForEach(x =>
                Mapper.Map(source, x)
           );

            return destination;
        }

        private static class MappingHelper
        {
            public static Match<TSource, TDestination>[] FindMatches<TSource, TDestination>(
                IEnumerable<TSource> source, IEnumerable<TDestination> destination,
                Func<TSource, string> getSourceKey, Func<TDestination, string> getDestinationKey)
            {
                if (source == null || destination == null) return new Match<TSource, TDestination>[0];

                var matches = destination
                                .ToDictionary(x => getDestinationKey(x))
                                .LeftJoin(
                                    source.ToDictionary(x => getSourceKey(x)),
                                    x => x.Key, x => x.Key,
                                    (x, y) => new Match<TSource, TDestination> { Destination = x.Value, Source = y.Value }
                                )
                                .ToArray();
                return matches;
            }

            public static IEnumerable<TDestination> UpdateOrAdd<TSource, TDestination>(
                IEnumerable<TSource> source, IEnumerable<TDestination> destination,
                Func<TSource, string> getSourceKey, Func<TDestination, string> getDestinationKey)
            {
                var result = new List<TDestination>();
                var matches = FindMatches(source, destination, getSourceKey, getDestinationKey);

                foreach (var match in matches)
                    if (match.Source != null)
                        result.Add(Mapper.Map(match.Source, match.Destination));
                    else
                        result.Add(match.Destination);

                return result;
            }

            public class Match<TSource, TDestination>
            {
                public TSource Source;
                public TDestination Destination;
            }
        }
    }
}