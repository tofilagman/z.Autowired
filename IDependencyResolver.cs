 

namespace z.Autowire
{
    public interface IDependencyResolver
    {
        public abstract object? Resolve(IServiceProvider serviceProvider, Type type);
    }
}
