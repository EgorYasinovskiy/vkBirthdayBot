# VKBirtdayBot
## About VKBirthdayBot
**VkBirthdayBot** Console *(so far)* application in **[C#](https://github.com/trending/c%23)** to congratulate your friends in **[VK](https://vk.com)**. Daily, at 00:00, bot checks the list of your friends, and finds out when their birthday, and if the date is the same as today,it congratulates them with special template, different for male and female.
## Comands
- [**setuser**] - sets the login and password to api, trying to auth.
- [**start**] - Starts the programm, checking the friends list
- [**stop**] - Stops the checking.
- [**exit**] - Closes the application
- [**help**] - Writes all comands to the screen
- [**cls**] - Clears screen
- [**auth**] - Command using when you set user but you are somehow logged out

## In the future
- Add **[ignore]** function. If any friend from the list of your friends will be listed in the ignore list then he will not be congratulated by the bot
- Add **[addgreeting]** function. For each friend, you can write your personal greeting that will be sent to him instead of a template text.
- Connect the application to database and server. For every user create table with him friends. For every friends in user friendlist add the text field. Default it will be filled with template text. If user added greeting to this friend it will be rewritten.
- Create the Windows Forms app with sources of this project 
- Create the Android app with sources of this project using **[Xamarin](https://en.wikipedia.org/wiki/Xamarin)**
