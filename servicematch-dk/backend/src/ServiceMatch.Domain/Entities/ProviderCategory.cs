namespace ServiceMatch.Domain.Entities;

public class ProviderCategory
{
    public Guid ServiceProviderId { get; private set; }
    public int ServiceCategoryId { get; private set; }

    private ProviderCategory() { }

    public ProviderCategory(Guid serviceProviderId, int serviceCategoryId)
    {
        ServiceProviderId = serviceProviderId;
        ServiceCategoryId = serviceCategoryId;
    }
}
