using servicecontrolhub.storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace alertbot.users
{
    public class UserManager
    {
        #region vars        
        IStorage<List<User>> storage;
        List<User> users = new();   
        #endregion

        public UserManager()
        {
            storage = new Storage<List<User>>("users", "users", users);
            users = storage.load(); 
        }

        public void Add(long tg_id, string? un = null, string? fn = null, string? ln = null, bool? is_admin = false ) {

            var found = users.FirstOrDefault(u => u.tg_id == tg_id);
            if (found == null)
            {
                users.Add(new User
                {
                    tg_id = tg_id,
                    un = un,
                    fn = fn,
                    ln = ln,
                    is_admin = false
                });

                storage.save(users);
            } else
                if (is_admin == true)
            {
                found.is_admin = true;
                storage.save(users);
            }
        }

        public List<User> Get()
        {
            return users;   
        }
    }

    public class User
    {
        public long tg_id { get; set; }
        public string un { get; set;}
        public string fn { get; set; }
        public string ln { get; set; }
        public bool is_admin { get; set; }

    }
}
