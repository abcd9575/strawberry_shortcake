using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Strawberry_Shortcake.ViewModel
{
    public class UserViewModel: Models.User
    {
        [DataType(DataType.Password)]
        public new string UserPw { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }
}
