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

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class OrderAttribute : System.Attribute
    {
        public OrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }
}
