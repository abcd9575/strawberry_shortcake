using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Strawberry_Shortcake.Models
{
    public class User
    {
        /// <summary>
        /// 사용자 번호
        /// </summary>
        [Key]// PK 설정
        public int UserNo { get; set; }
        /// <summary>
        /// 사용자 e-mail겸 아이디
        /// </summary>
        
        [Required(ErrorMessage = "Required")]
        [Display(Name ="e-mail")]
        [EmailAddress]
        public string UserEmail { get; set; }
        
        /// <summary>
        /// 사용자 비밀번호
        /// </summary>
        [Required(ErrorMessage = "Required")]
        public string UserPw { get; set; }

        /// <summary>
        /// 사용자 로닌지갑
        /// </summary>
        [Required(ErrorMessage = "Required")]
        public string UserName { get; set; }

        /// <summary>
        /// 사용자 계정 활성화
        /// </summary>
        [Required(ErrorMessage = "Required")]
        public bool Activation { get; set; }

    }
}
