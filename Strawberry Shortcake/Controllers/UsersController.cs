using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Strawberry_Shortcake.Data;
using Strawberry_Shortcake.Models;
using Strawberry_Shortcake.ViewModel;

namespace Strawberry_Shortcake.Controllers
{
    public class UsersController : Controller
    {
        private readonly Strawberry_ShortcakeContext db;

        public UsersController(Strawberry_ShortcakeContext context)
        {
            db = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await db.User.ToListAsync());
        }

        // 회원가입
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                db.User.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.User.FirstOrDefault(u => u.UserEmail == model.UserEmail && u.UserPw == model.UserPw);
                
                if (user != null)
                {
                    if (user.UserEmail == "abcd95751@gmail.com")
                    {
                        HttpContext.Session.SetInt32("MASTER_LOGIN_KEY", user.UserNo);
                        return RedirectToAction("Index", "Users");
                    }
                    // 로그인에 성공했을때
                    //HttpContext.Session.SetInt32(key,value);
                    HttpContext.Session.SetInt32("USER_LOGIN_KEY",user.UserNo);
                    return RedirectToAction("LoginSuccess", "Home");
                    
                }
                //로그인에 실패했을 때 "문자열 아무것도 없을때 = string.empty"
                ModelState.AddModelError(string.Empty, "Incorrect username or password.");
                
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("USER_LOGIN_KEY");
            HttpContext.Session.Remove("MASTER_LOGIN_KEY");
            return RedirectToAction("Index", "Home");
        }
        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await db.User
                .FirstOrDefaultAsync(m => m.UserNo == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserNo,UserEmail,UserPw,UserName,Activation")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await db.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserNo,UserEmail,UserPw,UserName,Activation")] User user)
        {
            if (id != user.UserNo)
            {
                return NotFound();
            }

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

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await db.User
                .FirstOrDefaultAsync(m => m.UserNo == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await db.User.FindAsync(id);
            db.User.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return db.User.Any(e => e.UserNo == id);
        }
    }
}
