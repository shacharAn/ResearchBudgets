using System;
using System.Security.Cryptography;
using System.Text;
using RuppinResearchBudget.DAL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.BL
{
    public class UsersBl 
    {
        private readonly UsersDal _usersDal;

        public UsersBl()
        {
            _usersDal = new UsersDal();
        }

        public Users RegisterUser(
            string idNumber,
            string userName,
            string email,
            string plainPassword,
            string confirmPassword,
            string firstName,
            string lastName)
        {
            if (string.IsNullOrWhiteSpace(idNumber) || idNumber.Length != 9)
                throw new ArgumentException("תעודת זהות חייבת להיות 9 ספרות");

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("שם משתמש הוא שדה חובה");

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("כתובת מייל אינה תקינה");

            if (string.IsNullOrWhiteSpace(plainPassword) || plainPassword.Length < 6)
                throw new ArgumentException("סיסמה חייבת להיות לפחות 6 תווים");

            if (plainPassword != confirmPassword)
                throw new ArgumentException("הסיסמאות אינן תואמות");

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("שם פרטי ושם משפחה הם שדות חובה");

            string hash = HashPassword(plainPassword);

            var user = new Users
            {
                IdNumber = idNumber,
                UserName = userName,
                Email = email,
                PasswordHash = hash,
                FirstName = firstName,
                LastName = lastName
            };

            var created = _usersDal.RegisterUser(user);
            if (created == null)
            {
                throw new Exception("נכשלה יצירת המשתמש במערכת");
            }

            return created;
        }

        public Users Login(string userName, string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("שם משתמש וסיסמה הם שדות חובה");

            string hash = HashPassword(plainPassword);
            var user = _usersDal.Login(userName, hash);

            if (user == null)
                throw new Exception("שם משתמש או סיסמה שגויים");

            if (!user.IsActive)
                throw new Exception("המשתמש אינו פעיל במערכת");

            return user;
        }

        public UserWithRoles GetUserWithRoles(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("שם משתמש הוא שדה חובה");

            var result = _usersDal.GetUserWithRoles(userName);
            if (result == null)
                throw new Exception("המשתמש לא נמצא או שאין לו תפקידים במערכת");

            return result;
        }


        private string HashPassword(string plainPassword)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(plainPassword);
                byte[] hashBytes = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
