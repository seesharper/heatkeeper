using System;
using System.Linq;
using AutoMapper;
using HeatKeeper.Server.Measurements;

namespace HeatKeeper.Server.Mapping
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);
    }

    public class Mapper : IMapper
    {
        private readonly AutoMapper.IMapper autoMapper;

        public Mapper()
        {
            autoMapper = new MapperConfiguration(config =>
            {
                config.AddProfiles(AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith("HeatKeeper",StringComparison.OrdinalIgnoreCase)));
            }).CreateMapper();
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return autoMapper.Map<TSource,TDestination>(source);
        }
    }

    public class ServerMappingProfile : Profile
    {
        public ServerMappingProfile()
        {

        }
    }


}