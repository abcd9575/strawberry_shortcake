using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Strawberry_Shortcake.Models
{
    /* Note
     * - 숫자가 낮을 수록 낮은 권한
     * - 이미 존재하는 권한의 이름을 바꾸면 문제가 생길 수 있음
     */
    public enum Role : uint
    {
        User = 1,
        Manager = 2,
        Administrator = 3,
    }
}
