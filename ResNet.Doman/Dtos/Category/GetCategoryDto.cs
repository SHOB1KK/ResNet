namespace ResNet.Domain.Dtos
{
    public class GetCategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}
