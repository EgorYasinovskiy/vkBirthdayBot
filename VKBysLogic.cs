using System;
using System.Collections.Generic;
using System.Threading;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Enums.Filters;
using VkNet.Categories;
using System.Threading.Tasks;

namespace VkBirthdayApp
{
    public class VKBysLogic
    {
        private string _password;
        private string _login;
        private ulong _appKey;
        private List<User> _friendList;
        
        private VkApi api = new VkApi();

        /// <summary>
        /// Makes auth in vk.com using AppID, Login and Password
        /// </summary>
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
        /// <summary>
        /// Parsing your friendlist from vk, takes it's birthdate ,firstname and ID to send they greetings messages
        /// </summary>
        /// <returns> FriendLsit with birthdate, firstname, ID</returns>
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
                    friend.Id = item.Id;
                    FriendList.Add(friend);
                }
            }
            return FriendList;
        }
        /// <summary>
        /// Refreshing friendlist for know if user added new friends or deleted someone
        /// </summary>
        public void RefreshFriendList()
        {
           _friendList = GetFriendList(); 
        }
        /// <summary>
        /// Forming the greetings messages using your friend name
        /// </summary>
        /// <param name="friend">Object of User class</param>
        /// <returns>Greeting message</returns>
        private string GetGreetingMessage(User friend)
        {
            if(friend.Sex==VkNet.Enums.Sex.Female)
            {
                return string.Format("Дорогая {0}, поздравляю тебя с днем рождения," +
                    " желаю тебе всегда улыбаться и быть счастливой." +
                    "Хочу чтобы в твоей жизни были только хорошие моменты, а" +
                    " сегодняшний день ты запомнила навсегла как один из лучших твоих дней!!!",
                    friend.FirstName);
            }
            else
            {
                return string.Format("С днем рождени,{0}!!!Хочу пожелать тебе всео наилучшего," +
                    " побольше счастья, здоровья и конечно же денег. Будь счастлив в этот день!", friend.FirstName);
            }
        }
        /// <summary>
        /// Sends your friend the greeting message
        /// </summary>
        /// <param name="friend">Object of User class, who should get the greeting message </param>
        public void SendGreetings(User friend)
        { 
            api.Messages.Send(new MessagesSendParams
            {
                UserId = friend.Id,
                RandomId = new Random().Next(),
                Message = GetGreetingMessage(friend)
            });
        }
        /// <summary>
        /// Everyday checking if someone has birthday today. If someone has - sends greeting, wait for next day;
        /// </summary>
        public async void  CheckBirthday()
        {
            RefreshFriendList();
            foreach(var friend in _friendList)
            {
                if (friend.BirthDate == DateTime.Now.ToShortDateString().Substring(0, 5))
                {
                    SendGreetings(friend);
                }
            }
            var Time = (DateTime.Now.AddDays(1) - DateTime.Now).TotalMilliseconds;
            await Task.Run(() => Thread.Sleep(Convert.ToInt32(Time)));
        }

        public VKBysLogic(string Password,string Login, ulong AppKey)
        {
            _password = Password;
            _login = Login;
            _appKey = AppKey;
        }
        
    }
}

