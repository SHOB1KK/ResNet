using ResNet.Domain.Dtos;

public class GetMenuDto
{
    public List<GetCategoryDto> Categories { get; set; } = [];
    public List<GetProductDto> Products { get; set; } = [];
}
