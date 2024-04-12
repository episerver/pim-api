namespace PimApi.ConsoleApp.Renderers.Miscellaneous;

internal class EntityListRenderer : IApiResponseMessageRenderer
{
    public async Task Render(
        ApiResponseMessage apiResponseMessage,
        IJsonSerializer jsonSerializer,
        Action<string> messageWriter
    )
    {
        var response = await apiResponseMessage.GetDataAsync<
            ODataResponse<ICollection<GenericEntityDto>>
        >(jsonSerializer);

        var table = new ConsoleTables.ConsoleTable(
            nameof(BaseEntityDto.Id),
            nameof(BaseEntityDto.CreatedBy),
            nameof(BaseEntityDto.CreatedOn),
            nameof(BaseEntityDto.ModifiedBy),
            nameof(BaseEntityDto.ModifiedOn)
        );

        foreach (var entity in response.Value)
        {
            table.AddRow(
                entity.Id,
                entity.CreatedBy,
                entity.CreatedOn.DisplayValue(),
                entity.ModifiedBy,
                entity.ModifiedOn.DisplayValue()
            );
        }

        messageWriter(table.ToString());
        response.WriteTotalCount(messageWriter);
    }
}
