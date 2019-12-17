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
            Console.WriteLine("Авторизация...\n\n");
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
            {   // TODO: Проверка подключение к интернету.
                Console.WriteLine("Ошибка. Попытайтесь снова");
            }
        }
        /// <summary>
        /// Получение списка друзей и их имен, даты рождения, айди
        /// </summary>
        /// <returns> Список user</returns>
        public VkNet.Utils.VkCollection<User> GetFriendList()
        {
            Console.WriteLine("Getting friendlist\n\n");

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
                return string.Format("С днем рождения,{0}!!!Хочу пожелать тебе всео наилучшего," +
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
            Console.WriteLine("Сообщение отправлено\n\n");
        }
        /// <summary>
        /// Каждый день проверяет, наступил ли у кого день рождения
        /// Если наступил - отправляет поздравительное сообщение
        /// </summary>
        public async void CheckBirthday(bool IsStarted)
        {
            while (IsStarted)
            {
                Console.WriteLine("Начало проверки\n\n");
                _friendList = GetFriendList();
                Console.WriteLine(_friendList.Count);
                Console.WriteLine(_friendList);
                int greeted = 0;
                foreach (var friend in _friendList)
                {
                    if (friend.BirthDate == DateTime.Now.ToShortDateString().Substring(0, 5))
                    {
                        SendGreetings(friend);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Друг {0} поздравлен!", friend.FirstName);
                        Console.ResetColor();
                        greeted++;
                    }
                }
                if (greeted == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Никто сегодня не празднует ДР :(");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Сегодня поздравили {0} человек(а)", greeted);
                    Console.ResetColor();
                }

                var Time = (DateTime.Now.AddDays(1).Date - DateTime.Now).TotalMilliseconds;
                
                Console.WriteLine("Жду до следующего дня {0} часов", Time/(1000*60*60));
                await Task.Run(() => Thread.Sleep(Convert.ToInt32(Time)));
            }
        }

        public VKBysLogic(string Password, string Login)
        {
            _password = Password;
            _login = Login;
        }
    }
}

