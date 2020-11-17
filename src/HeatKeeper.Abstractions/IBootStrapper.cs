using System.Threading.Tasks;

namespace HeatKeeper.Abstractions
{
    /// <summary>
    /// Implementations of this interface should be called at application startup.
    /// </summary>
    public interface IBootStrapper
    {
        Task Execute();
    }
}
