using System;
using System.Collections.Generic;
using System.Text;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Enums.Filters;
using VkNet.Categories;

namespace VkBirthdayApp
{
    public class VKBysLogic
    {
        private string _password;
        private string _login;
        private ulong _appKey;
        

        public string Password { get => _password; set => _password = value; }
        public string Login { get => _login; set => _login = value; }
        public ulong AppKey { get => _appKey; set => _appKey = value; }
        
        public VkApi api = new VkApi();

        public void Auth()
        {
            api.Authorize(new VkNet.Model.ApiAuthParams
            {
                Login = _login,
                Password = _password,
                ApplicationId = _appKey,
                Settings = VkNet.Enums.Filters.Settings.All
            });
        }
        public List<User> GetFriendList()
        {
            var friends = api.Friends.Get(new FriendsGetParams { });
            List<User> FriendList = new List<User>();
            int i = 0;
            foreach(var item in friends)
            {
                if(item.BirthDate == null)
                {
                    continue;
                }
                else
                { 
                    User friend = new User();
                    friend.BirthDate = item.BirthDate.Substring(0, 5);
                    friend.FirstName = item.FirstName;
                    FriendList.Add(friend);
                }
            }
            return FriendList;

            
        }
        
        
        
    }
}
