using System;
using System.Threading;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model.RequestParams;
using VkNet.Enums.Filters;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using System.Net.Sockets;

namespace VkBirthdayApp
{
    public class VKBysLogic
    {
        private string _password;
        private string _login;
        private VkNet.Utils.VkCollection<User> _friendList;
        public bool IsAuthed { get; private set; }
        static ServiceCollection services = new ServiceCollection();
        private static IVkApi api;
        /// <summary>
        /// Заходит в вк используя логин и пароль
        /// </summary>
        public void Auth()
        {
            services.AddAudioBypass();
            api = new VkApi(services);
            Console.WriteLine("Авторизация...\n");
            try
            {
                api.Authorize(new VkNet.Model.ApiAuthParams
                {
                    Login = _login,
                    Password = _password,
                    TwoFactorAuthorization = () =>
                    {
                        Console.WriteLine(" > Код двухфакторной аутентификации:");
                        return Console.ReadLine();
                    }
                });
                IsAuthed = api.IsAuthorized;
            }
            catch
            {   // TODO: Проверка подключение к интернету. - Сделано
                if(CheckConnect())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Неверный логин или пароль!\n");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Нет соединения с интернетом.");
                    Console.ResetColor();
                    Console.WriteLine("Проверьте настройки вашего сетевого подключения\n");
                }
                
            }
        }
        /// <summary>
        /// Получение списка друзей и их имен, даты рождения, айди
        /// </summary>
        /// <returns> Список user</returns>
        public VkNet.Utils.VkCollection<User> GetFriendList()
        {
            Console.WriteLine("Получаю список друзей\n");

            return api.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams { UserId = api.UserId, Fields = ProfileFields.BirthDate | ProfileFields.FirstName | ProfileFields.Sex });


        }
        /// <summary>
        /// Формирует сообщение перед отправкой на основе пола
        /// </summary>
        /// <param name="friend">Экземпляр класса User для получения пола и имени </param>
        /// <returns>Поздравительное сообщение</returns>
        private string GetGreetingMessage(User friend)
        {
            if (friend.Sex == VkNet.Enums.Sex.Female)
            {
                return string.Format("Дорогая {0}, поздравляю тебя с днем рождения," +
                    " желаю тебе всегда улыбаться и быть счастливой." +
                    "Хочу чтобы в твоей жизни были только хорошие моменты, а" +
                    " сегодняшний день ты запомнила навсегла как один из лучших твоих дней!!!",
                    friend.FirstName);
            }
            else
            {
                return string.Format("С днем рождения,{0}!!!Хочу пожелать тебе всего наилучшего," +
                    " побольше счастья, здоровья и конечно же денег. Будь счастлив в этот день!", friend.FirstName);
            }
        }
        /// <summary>
        /// Отправляет сообщение именнинику
        /// </summary>
        /// <param name="friend">Экземпляр класса User для получения id, перед отправкой сообщения  </param>
        public void SendGreetings(User friend)
        {
            api.Messages.Send(new MessagesSendParams
            {
                UserId = friend.Id,
                RandomId = new Random().Next(),
                Message = GetGreetingMessage(friend)

            });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Сообщение отправлено\n\n");
            Console.ResetColor();
        }
        /// <summary>
        /// Каждый день проверяет, наступил ли у кого день рождения
        /// Если наступил - отправляет поздравительное сообщение
        /// </summary>
        public async void CheckBirthday(bool IsStarted)
        {
            while (IsStarted)
            {
                _friendList = GetFriendList();
                Console.WriteLine(_friendList.Count + " друзей");
                int greeted = 0;
                foreach (var friend in _friendList)
                {
                    if (friend.BirthDate == DateTime.Now.ToShortDateString().Substring(0, 5))
                    {
                        SendGreetings(friend);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Друг {0} поздравлен!\n", friend.FirstName);
                        Console.ResetColor();
                        greeted++;
                    }
                }
                if (greeted == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Никто сегодня не празднует ДР :(\n");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Сегодня поздравили {0} человек(а)\n", greeted);
                    Console.ResetColor();
                }

                var Time = (DateTime.Now.AddDays(1).Date - DateTime.Now).TotalMilliseconds;

                Console.Write("Жду до следующего дня ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(Math.Round((Time / (1000 * 60 * 60)), MidpointRounding.AwayFromZero));
                Console.ResetColor();
                Console.Write(" часов\n\n");
                await Task.Run(() => Thread.Sleep(Convert.ToInt32(Time)));
            }
        }

        public VKBysLogic(string Password, string Login)
        {
            _password = Password;
            _login = Login;
        }
        public bool CheckConnect()
        {

            try
            {
                System.Net.HttpWebRequest reqFP = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://vk.com");
                System.Net.HttpWebResponse rspFP = (System.Net.HttpWebResponse)reqFP.GetResponse();
                if (rspFP.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    rspFP.Close();
                    return true;
                }
                else
                {
                    rspFP.Close();
                    return false;
                }
            }
            catch (System.Net.WebException)
            {
                return false;
            }
        }
    }
}

