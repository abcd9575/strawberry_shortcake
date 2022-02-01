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
using BC = BCrypt.Net.BCrypt;

namespace Strawberry_Shortcake.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        /* TODO
         * 
         * - 회원 등급 상수화 (코딩하기 쉽게)
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
            var CurrentRole = db.Roles.SingleOrDefault(r => r.RoleName.Equals(CurrentRoleName));

            return db.Roles.ToList().Select(r => new SelectListItem
            {
                Text = r.RoleName,
                Value = r.Id.ToString(),
                Selected = user == null ? r.Id == 1 : user?.RoleId == r.Id,
                Disabled = 
                    r.RoleName.Equals("Administrator") // 새로운 관리자 계정 만들기 금지
                    && user?.Role?.RoleName?.Equals("Administrator") != true // 지금 로그인한 계정이 관리자 계정이 아닐 경우
                    && r.Id >= CurrentRole.Id, // 지금 로그인 한 계정보다 높은 등급을 설정 못하게 방지
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
                Role = GetAvailableRoles()
            };
            return View(model);
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserNo,UserEmail,UserPw,UserName,Activation,RoleId")] User user)
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
                UserPw = user.UserPw,
                RoleId = user.RoleId,
                Role = GetAvailableRoles(user)
            };
            return View(model);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserNo,UserEmail,UserPw,UserName,Activation,RoleId")] User user)
        {
            if (id != user.UserNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.UserPw = BC.HashPassword(user.UserPw);
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
