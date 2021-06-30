using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Client.Models
{
    public class UserInfoVM
    {
        public Dictionary<string, string> UserInforDictionary { get; private set; }

        public UserInfoVM(Dictionary<string, string> UserInforDictionary)
        {
            this.UserInforDictionary = UserInforDictionary;
        }
    }
}
