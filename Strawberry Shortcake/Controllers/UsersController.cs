﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Strawberry_Shortcake.Data;
using Strawberry_Shortcake.Models;
using Strawberry_Shortcake.ViewModel;
using BC = BCrypt.Net.BCrypt;

namespace Strawberry_Shortcake.Controllers
{
    [Authorize]
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
        [AllowAnonymous]
        public IActionResult Register() => View();

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                model.UserPw = BC.HashPassword(model.UserPw);
                db.User.Add(model);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(model);

        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) // 이미 Login 했을 때
                return RedirectToAction("LoginSuccess", "Home");

            return View();
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var Users = db.User.Where(u => u.UserEmail == model.UserEmail).ToList();
                var user = Users.SingleOrDefault(u => BC.Verify(model.UserPw, u.UserPw));

                if (user != null)
                {
                    //if (user.UserEmail == "abcd95751@gmail.com")
                    //{
                    //    HttpContext.Session.SetInt32("MASTER_LOGIN_KEY", user.UserNo);
                    //    return RedirectToAction("Index", "Users");
                    //}
                    //// 로그인에 성공했을때
                    ////HttpContext.Session.SetInt32(key,value);
                    //HttpContext.Session.SetInt32("USER_LOGIN_KEY",user.UserNo);
                    //return RedirectToAction("LoginSuccess", "Home");

                    var claims = new Claim[] {
                        new Claim(ClaimTypes.Name, user.UserEmail),
                        new Claim(ClaimTypes.Email, user.UserEmail),
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity));
                    return RedirectToAction("LoginSuccess", "Home");
                }
                else {
                    //로그인에 실패했을 때 "문자열 아무것도 없을때 = string.empty"
                    ModelState.AddModelError(string.Empty, "Incorrect username or password.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
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

        //// GET: Users/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Users/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("UserNo,UserEmail,UserPw,UserName,Activation")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Add(user);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(user);
        //}

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
