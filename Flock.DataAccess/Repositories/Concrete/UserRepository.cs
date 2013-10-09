﻿

using System.Linq;
using Flock.DataAccess.Base;
using Flock.DataAccess.EntityFramework;
using Flock.DataAccess.Repositories.Interfaces;

namespace Flock.DataAccess.Repositories.Concrete
{
    public class UserRepository : SqlRepository<User>, IUserRepository
    {
        public User GetUserByUserName(string userName)
        {
            var users = base.GetAll();
            return users.FirstOrDefault(user => user.UserName == userName);

        }

        public void SaveUser(User user)
        {
            base.Add(user);
        }
    }
}