using Nelibur.ObjectMapper;

namespace Application.Common.Mapping;

internal class MappingHelper : IMapper
{
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        TinyMapper.Bind<TSource, TDestination>();
        return TinyMapper.Map<TSource, TDestination>(source);
    }
}