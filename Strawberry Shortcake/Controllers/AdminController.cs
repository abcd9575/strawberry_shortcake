using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Strawberry_Shortcake.Data;
using Strawberry_Shortcake.Models;
using Strawberry_Shortcake.ViewModel;
using BC = BCrypt.Net.BCrypt;

namespace Strawberry_Shortcake.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        /* TODO
         */
        private readonly Strawberry_ShortcakeContext db;

        public AdminController(Strawberry_ShortcakeContext context)
        {
            db = context;
        }
        private List<SelectListItem> GetAvailableRoles(User user = null)
        {
            /* 지금 로그인 한 계정의 회원등급 가져오기 */
            var CurrentRoleName = User.Claims.Single(r => r.Type == ClaimTypes.Role).Value;
            var CurrentRole = Enum.Parse(typeof(Role), CurrentRoleName);

            return Enum.GetValues<Role>().ToList().Select(r=> new SelectListItem
            {
                Text = Enum.GetName<Role>(r), // Enum 타입에서 string 타입으로
                Value = Enum.GetName<Role>(r),
                Disabled = 
                    r == Role.Administrator // 새로운 관리자 계정 만들기 금지
                    && user?.Role != Role.Administrator // 지금 로그인한 계정이 관리자 계정이 아닐 경우
                    && (uint)r >= (uint)CurrentRole, // 지금 로그인 한 계정보다 높은 등급은 설정 못하게 방지
            }).ToList();
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            return View(await db.Users.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await db.Users
                .FirstOrDefaultAsync(m => m.UserNo == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            UserViewModel model = new UserViewModel {
                Role = Role.User, //기본 회원등급: User
                Roles = GetAvailableRoles()
            };
            return View(model);
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserNo,UserEmail,UserPw,UserName,Activation,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                user.UserPw = BC.HashPassword(user.UserPw);
                db.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            UserViewModel model = new UserViewModel() {
                UserName= user.UserName,
                UserNo = user.UserNo,
                UserEmail = user.UserEmail,
                Activation = user.Activation,
                Role = user.Role,
                Roles = GetAvailableRoles(user)
            };
            return View(model);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserNo,UserEmail,UserPw,UserName,Activation,Role")] User user)
        {
            if (id != user.UserNo)
            {
                return NotFound();
            }


            if (String.IsNullOrWhiteSpace(user.UserPw) == false)
            {
                /* 만약 비밀번호 칸이 빈칸이 아니라면, 비밀번호를 변경해 줌*/
                user.UserPw = BC.HashPassword(user.UserPw);
            }
            else
            {
                /* 빈칸이라면 이전 비밀번호 유지 */
                User old = await db.Users
                            .AsNoTracking() // LinQ가 값 변경 추적 안하도록
                            .SingleAsync(u => u.UserNo == id);
                user.UserPw = old.UserPw;
            }

            /* 수정된 UserPw를 위해 ModelState 초기화 후 다시 검증 */
            ModelState.Clear();
            TryValidateModel(user);

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(user);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await db.Users
                .FirstOrDefaultAsync(m => m.UserNo == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return db.Users.Any(e => e.UserNo == id);
        }
    }
}
