namespace Application.Common.Mapping;

public interface IMapper
{
    TDestination Map<TSource, TDestination>(TSource source);
}