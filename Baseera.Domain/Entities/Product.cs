using Baseera.Domain.Enums;

namespace Baseera.Domain.Entities
{
    public class Product : BaseEntity
    {
        public ProductType ProductType { get; set; }
    }
}
