namespace AssetManagementWebApi.Repositories.Requests;

public class PagingResult<T> : PagingRequest
{
    public List<T>? Items { get; set; }
}