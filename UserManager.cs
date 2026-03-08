// The CMLeonOS Project (https://github.com/Leonmmcoset/CMLeonOS)
// Copyright (C) 2025-present LeonOS 2 Developer Team
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace CMLeonOS
{
    public static class UserManager
    {
        public static List<User> Users
        {
            get
            {
                return UserSystem.GetUsers();
            }
        }

        public static User GetUser(string username)
        {
            foreach (User user in UserSystem.GetUsers())
            {
                if (user.Username == username)
                {
                    return user;
                }
            }
            return null;
        }

        public static bool AddUser(User user)
        {
            return Kernel.userSystem.AddUser($"{user.Username} {user.Password}", user.IsAdmin);
        }

        public static bool AddUser(string username, string password, bool isAdmin)
        {
            return Kernel.userSystem.AddUser($"{username} {password}", isAdmin);
        }

        public static bool RemoveUser(string username)
        {
            return Kernel.userSystem.DeleteUser(username);
        }

        public static bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return Kernel.userSystem.ChangePassword();
        }

        public static bool ChangePassword()
        {
            return Kernel.userSystem.ChangePassword();
        }

        public static bool Authenticate(string username, string password)
        {
            User user = GetUser(username);
            if (user == null)
            {
                return false;
            }
            return user.Authenticate(password);
        }

        public static void ListUsers()
        {
            Kernel.userSystem.ListUsers();
        }

        public static bool HasUsers()
        {
            return Kernel.userSystem.HasUsers;
        }

        public static bool IsAdminSet()
        {
            return Kernel.userSystem.IsAdminSet;
        }

        public static string CurrentUsername()
        {
            return Kernel.userSystem.CurrentUsername;
        }

        public static bool CurrentUserIsAdmin()
        {
            return Kernel.userSystem.CurrentUserIsAdmin;
        }

        public static User CurrentLoggedInUser()
        {
            return UserSystem.CurrentLoggedInUser;
        }
    }
}
