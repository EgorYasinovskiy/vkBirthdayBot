using System;
using System.Collections.Generic;

namespace VkBirthdayApp
{
    class Program
    {
        static bool IsStarted = false;
        static bool IsAuthed = false;
        static VKBysLogic Vk;
        static Dictionary<string, Action> Commands = new Dictionary<string, Action>
        {
            { "setuser" ,SetUser},
            { "auth",Auth },
            { "start",Start},
            { "stop",Stop },
            { "exit",(()=>Environment.Exit(0))},
            { "cls",(()=>Console.Clear())},
            { "help",Help }
        };

        static void Main(string[] args)
        {
            while(true)
            {
                string command;
                string path = Environment.CurrentDirectory;
                Console.Write("_VKBirthdayApp>");
                command = Console.ReadLine();
                if (Commands.ContainsKey(command))
                {
                    Console.WriteLine("");
                    Commands[command]();
                }
                else
                {
                    Console.WriteLine("Неизвестная команда: \"{0}\"\nНапишите\"help\" Для просмотра команд\n",command);
                }
            }



        }
        public static void  Help()
        {
            string commands= "" +
                "[setuser] - sets the login and password to api, trying to auth.\n" +
                "[start] - Starts the programm, checking the friends list\n" +
                "[stop] - Stops the checking.\n" +
                "[exit] - Closes the application\n" +
                "[help] - Writes all comands to the screen\n" +
                "[cls] - Clears screen\n" +
                "[auth] - Command using when you set user but you are somehow logged out \n";
           
            Console.WriteLine(commands);
        }
        public static void SetUser()
        {
            string Login;
            string Pass;
            
            Console.Write("_VKBirthdayApp> Введите логин: ");
            while(string.IsNullOrEmpty(Login = Console.ReadLine()))
            {
                Console.Write("_VKBirthdayApp> Введите корректный логин: ");
            }
            Console.Write("_VKBirthdayApp> Введите пароль: ");
            while (string.IsNullOrEmpty(Pass=Console.ReadLine()))
            {
                Console.Write("_VKBirthdayApp> Введите корректный пароль: ");
            }
            Vk = new VKBysLogic(Pass, Login);
            Auth();
            



        }
        public static void Auth()
        {
            if (Vk == null)
            {
                Console.WriteLine("Сначала войдите используя команду  \"setuser\"\n");
            }
            else
            {
                try
                {
                    Vk.Auth();
                    IsAuthed =Vk.IsAuthed;
                    if(IsAuthed)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Успешный вход\n");
                        Console.ResetColor();
                    }
                }
                catch
                {   //TODO : Проверка соединения с интернетом
                    if(Vk.CheckConnect())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Неверный логин или пароль.");
                        Console.ResetColor();
                        Console.WriteLine("Попробуйте снова\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Нет соединения с интернетом!");
                        Console.ResetColor();
                        Console.WriteLine("Проверьте настройки сетевого адаптера\n");
                    }
                }
            }
        }
        public static void Start()
        {
            if (Vk == null)
            {
                Console.WriteLine("Сначала войдите используя команду \"setuser\"\n");
            }
            else if(!IsAuthed)
            {
                Console.WriteLine("Сначала авторизуйтесь используя команду \"auth\"\n");
            }
            else if(!IsStarted)
            {
                IsStarted = true;
                
                Vk.CheckBirthday(IsStarted);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Проверка уже запущена!");
                Console.ResetColor();
            }
                
            
        }
        public static void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                Console.WriteLine("Проверка на дни рождения остановлена\n");
            }
            else
            {
                Console.WriteLine("Проверка не была запущена!!!\n");
            }
        }
        

    }
}
