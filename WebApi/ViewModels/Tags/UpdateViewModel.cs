namespace WebApi.ViewModels.Tags
{
    public class UpdateViewModel
    {
        public required Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
