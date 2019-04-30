using System;
namespace Questions.Interfaces
{
    public interface IModelMapper<in TSource, out TTarget> where TSource : class where TTarget : class
    {
        TTarget Map(TSource source);
    }
}
