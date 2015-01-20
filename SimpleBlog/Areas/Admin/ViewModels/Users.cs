using System.Collections.Generic;
using SimpleBlog.Models;
using System.ComponentModel.DataAnnotations;
namespace SimpleBlog.Areas.Admin.ViewModels
{
    public class UsersIndex
    {
        public IEnumerable<User> Users { get; set; }
    }

    public class UsersNew
    {
        [Required,MaxLength(126)]
        public string Username { get; set; }

        [Required,DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, MaxLength(256),DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }

    public class UsersEdit
    {
        [Required, MaxLength(126)]
        public string Username { get; set; }

        [Required, MaxLength(256), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }

    public class UsersResetPassword
    {
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}