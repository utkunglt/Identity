﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.App_Start;
using WebApplication.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
namespace WebApplication.Controllers
{
    public class UserAdminController : Controller
    {
        public UserAdminController() {}
        private BUS_DBEntities db = new BUS_DBEntities();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        public UserAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }
        // GET: /UserAdmin/
        public ActionResult Index()
        {
            var result = from user in db.Users
                         from role in db.Roles
                         where role.Users.Any(r => r.Id == user.Id) && !user.Id.Equals(2)
                         select new UserDetailViewModels
                         {
                             Id = user.Id,
                             RolesList = role.Name,
                             UserName = user.UserName
                         };
            return View(result.ToList());
        }

        // GET: /UserAdmin/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserDetailViewModels userDetail = (from user in db.Users
                                               from role in db.Roles
                                               where role.Users.Any(r => r.Id == id.Value)
                                               select new UserDetailViewModels
                                               {
                                                   Id = user.Id,
                                                   Email = user.Email,
                                                   RolesList = role.Name
                                               }).FirstOrDefault();
            if (userDetail == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(userDetail);
        }

        // GET: /UserAdmin/Create
        public async Task<ActionResult> Create()
        {
            CreateUserViewModels user = new CreateUserViewModels();
            ViewBag.RolesList = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name", "Admin");
            return View(user);
        }

        // POST: /UserAdmin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserViewModels userViewModels, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var appDbContext = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
                using (var transaction = appDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var user = new BusUser
                        {
                            UserName = userViewModels.UserName,
                            Email = userViewModels.Email,
                            PasswordHash = userViewModels.Password,
                            PhoneNumber = userViewModels.PhoneNumber
                        };
                        var adminresult = await UserManager.CreateAsync(user, userViewModels.Password);
                        if (adminresult.Succeeded)
                        {
                            if (selectedRoles != null)
                            {
                                var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                                if (!result.Succeeded)
                                {
                                    ModelState.AddModelError("", "Failed to add roles");
                                    ViewBag.RolesList = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name", "Admin");
                                    transaction.Rollback();
                                    return View();
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "UserName is exists");
                            ViewBag.RolesList = new SelectList(RoleManager.Roles, "Name", "Name", "Admin");
                            transaction.Rollback();
                            return View();

                        }
                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Have an error when created user");
                        transaction.Rollback();
                        return null;
                    }
                }
            }
            ViewBag.RolesList = new SelectList(RoleManager.Roles, "Name", "Name", "Admin");
            return View();
        }

        // GET: /UserAdmin/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id.Value);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userRoles = await UserManager.GetRolesAsync(user.Id);


            EditUserViewModels userDetail = new EditUserViewModels();
            userDetail.Id = user.Id;
            userDetail.UserName = user.UserName;
            userDetail.Email = user.Email;
            userDetail.PhoneNumber = user.PhoneNumber;
            userDetail.RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
            {
                Selected = userRoles.Contains(x.Name),
                Text = x.Name,
                Value = x.Name
            });
            return View(userDetail);
        }

        // POST: /UserAdmin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email,PhoneNumber")] EditUserViewModels editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.Email = editUser.Email;
                user.PhoneNumber = editUser.PhoneNumber;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: /UserAdmin/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: /UserAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
