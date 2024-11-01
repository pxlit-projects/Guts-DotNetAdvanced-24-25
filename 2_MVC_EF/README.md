# Exercises - Part 2 - ASP.NET Core MVC + Entity Framework + NUnit

## MVC & Entity Framework - Quiz Application
In this MVC web app we will make a website that contains questions for a quiz. The questions are sorted in categories and for each question multiple correct and incorrect answers are given. 

![Home screen](images/QuizApplication_home.png)

The solution contains a [layered architecture](https://www.oreilly.com/library/view/software-architecture-patterns/9781491971437/ch01.html) (see .NET Expert next semester!) containing 5 projects:

* **AppLogic**: this project contains the application/'business' logic of the webapp. In the folder Contracts you can find the interfaces that are used in the application. 
QuizService is an important class as it will be used by the controllers in the MVC WebApp to get the necessary data.

* **Domain**: Answer, Question and Category are the domain classes of this solution. 
* **Infrastructure**: This project contains the *implementation* of the repositories and DbContext of the application. In these repositories an actual database should be used for the questions and answers. The categories are saved in an in-memory list. 
* **Tests**: The GUTS unit (and integration) tests.
* **Web**: The .NET Core MVC project that should generate the website by using MVC and the Domain and AppLogic (QuizService) projects.

When the website is visited and the user clicks on 'Quiz questions', the following page should show: 
![Categories screen](images/QuizApplication_categories.png)

The categories are fetched from an in-memory repository containing the categories.

After selecting a category:
![Questions screen](images/QuizApplication_questions.png)

The questions are fetched from a SQL Server using Entity Framework. 

Finally, after clicking a question this page is shown:
![Questions screen](images/QuizApplication_answers.png)

The user can click on answers and the color indicates if the guess was correct.
Important: the last answer **'None of the answers is correct.'** is not present in the dataset/database and should be **generated** in the service that is responsible for providing the questions and answers (QuizService).

**Let the automatic tests guide you into understanding the intent of the code and into completing the code.**