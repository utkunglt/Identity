using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Models
{
    public class UserDetailViewModels
    {
        [Display(Name = "User Id")]
        public long Id { get; set; }

        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }


        [Display(Name = "User Role")]
        public string RolesList { get; set; }

    }

    public class EditUserViewModels
    {
        [Display(Name = "User Id")]
        public long Id { get; set; }

        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }


        [Display(Name = "User Role")]
        public IEnumerable<SelectListItem> RolesList { get; set; }

    }

    public class CreateUserViewModels
    {

        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "User Role")]
        public IEnumerable<SelectListItem> RolesList { get; set; }
    }

}