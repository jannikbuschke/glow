using MediatR;

namespace Glow.Configurations
{
    public class UpdateRaw<T> : IRequest
    {
        public string ConfigurationId { get; set; }
        public T Value { get; set; }
    }
}
