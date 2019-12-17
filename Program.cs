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
                command = Console.ReadLine();
                if (Commands.ContainsKey(command))
                {
                    Console.WriteLine("\n");
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
            string commands="";
            foreach(var command in Commands)
            {
                commands += string.Format("{0}, ", command.Key);
            }
            Console.WriteLine(commands);
        }
        public static void SetUser()
        {
            string Login;
            string Pass;
            
            Console.WriteLine("> Введите логин \n");
            while(string.IsNullOrEmpty(Login = Console.ReadLine()))
            {
                Console.WriteLine(">Введите корректный логин \n");
            }
            Console.WriteLine(">Введите пароль\n");
            while (string.IsNullOrEmpty(Pass=Console.ReadLine()))
            {
                Console.WriteLine(">Введите корректный пароль\n");
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
                        Console.WriteLine("Успешный вход\n");
                    }
                }
                catch
                {
                    Console.WriteLine("Что-то пошло не так. Проверьте ваш логин и пароль, а так же подключение к интернету\n");
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
            else
            {
                IsStarted = true;
                
                Vk.CheckBirthday(IsStarted);
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
