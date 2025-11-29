using System;
using System.Data;
using System.Data.SqlClient;
using RuppinResearchBudget.DAL;
using RuppinResearchBudget.Models;

namespace RuppinResearchBudget.Dal
{
    public class UsersDal : DBServices
    {
        public Users RegisterUser(Users user)
        {
            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spRegisterUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdNumber", user.IdNumber);
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new Users
                    {
                        IdNumber = reader["IdNumber"].ToString(),
                        UserName = reader["UserName"].ToString(),
                        Email = reader["Email"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        IsActive = (bool)reader["IsActive"],
                        CreatedAt = (DateTime)reader["CreatedAt"]
                    };
                }
            }
        }

        public Users Login(string userName, string passwordHash)
        {
            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spLoginUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new Users
                    {
                        IdNumber = reader["IdNumber"].ToString(),
                        UserName = reader["UserName"].ToString(),
                        Email = reader["Email"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        IsActive = (bool)reader["IsActive"],
                        CreatedAt = (DateTime)reader["CreatedAt"]
                    };
                }
            }
        }

        public UserWithRoles GetUserWithRoles(string userName)
        {
            using (SqlConnection conn = connect("DefaultConnection"))
            using (SqlCommand cmd = new SqlCommand("spGetUserWithRoles", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", userName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    UserWithRoles result = null;

                    while (reader.Read())
                    {
                        if (result == null)
                        {
                            result = new UserWithRoles
                            {
                                IdNumber = reader["IdNumber"].ToString(),
                                UserName = reader["UserName"].ToString(),
                                Email = reader["Email"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                IsActive = (bool)reader["IsActive"]
                            };
                        }

                        if (reader["RoleName"] != DBNull.Value)
                        {
                            string roleName = reader["RoleName"].ToString();
                            if (!result.Roles.Contains(roleName))
                                result.Roles.Add(roleName);
                        }
                    }

                    return result;
                }
            }
        }
    }
}