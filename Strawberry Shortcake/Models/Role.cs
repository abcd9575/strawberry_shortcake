using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Strawberry_Shortcake.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string RoleName { get; set; }

        public string Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
