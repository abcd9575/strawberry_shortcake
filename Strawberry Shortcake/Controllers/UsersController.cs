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

        /* TODO
         * - Email 중복시 Error page 생성
         */
        public UsersController(Strawberry_ShortcakeContext context)
        {
            db = context;
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
                model.Role = Role.User;
                db.Users.Add(model);
                await db.SaveChangesAsync();
                return RedirectToAction("Login", "Users");
            }

            return View(model);

        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) // 이미 Login 했을 때
                return RedirectToAction("LoginSuccess", "Home");

            return View(new LoginViewModel
            {
                ReturnUrl = Request.Query["ReturnUrl"].FirstOrDefault()
            });
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var Users = db.Users
                                .Where(u => u.UserEmail == model.UserEmail).ToList();

                var user = Users.SingleOrDefault(u => BC.Verify(model.UserPw, u.UserPw));

                if (user != null)
                {
                    var claims = new Claim[] {
                        new Claim(ClaimTypes.Name, user.UserEmail),
                        new Claim(ClaimTypes.Email, user.UserEmail),
                        new Claim(ClaimTypes.Role, Enum.GetName<Role>(user.Role))
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity));
                    return RedirectToReturnUrl(model.ReturnUrl);
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

        private IActionResult RedirectToReturnUrl(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl) == false)
                returnUrl = Url.Action("LoginSuccess", "Home");

            return Redirect(returnUrl);
        }
    }
}
