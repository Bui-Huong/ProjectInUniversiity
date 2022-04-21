using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssetManagementWebApi.Repositories.Entities.Enum
{
    public enum CategoryErrorState
    {
        CATEGORY_NAME_EXIST,
        CATEGORY_PREFIX_EXIST,
        CATEGORY_CREATE_SUCCESS,
        CATEGORY_CREATE_FAIL
    }
}