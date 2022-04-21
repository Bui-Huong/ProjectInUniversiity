using System.ComponentModel.DataAnnotations;

namespace AssetManagementWebApi.Repositories.Requests;

public class PagingRequest 
{
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
    public int? TotalRecords { get; set; }
    public int? PageCount
    {
        get
        {
            if(PageSize > 0){
                var pageCount = (double)TotalRecords/PageSize;
                return (int)Math.Ceiling((decimal)pageCount);
            }else{
                return 1;
            }
        }
    }
}