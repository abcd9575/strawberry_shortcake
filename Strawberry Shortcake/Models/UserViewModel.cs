using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Strawberry_Shortcake.Models
{
    public class UserViewModel: User
    {
        public new int RoleId;
        public new List<SelectListItem> Role { get; set; }
    }
}
