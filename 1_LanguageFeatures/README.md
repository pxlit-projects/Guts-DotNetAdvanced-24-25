# Exercises - Part 1 - Language Features

## Exercise 1 - Card games
In this exercise you will need to complete the inner workings of a WPF application. The applications allows you to play a *Higher-Lower* card game.
The *Higher-Lower* game goes like this:
- A card is dealt from a deck of cards
- You have to guess if the next card will be higher or lower than the current card
- When you have a certain number of correct guesses in a row, you win the game
- When you make a wrong guess, your correct-guess-streak gets reset to zero

![alt text][img_mainwindow]

The solution contains 3 projects:
- **CardGames.Desktop**: this is the WPF application. All code in this project is given. You don't need to change a thing in this project. Study the *MainWindow.xaml.cs* code, to understand how the *HigherLowerGame* class (from the *CardGames.Domain* project) is used. As you can see the main window starts a game with a deck that has all cards below 5 removed (this includes the *Aces* because an *Ace* has value 1). You must have 3 correct guesses in a row to win.
- **CardGames.Domain**: this project is a class library that contains classes that try to capture the domain logic of playing a *Higher-Lower* card game. This is where the real magic happens. This is where you will need to do the work for this exercise. In this project you will find definitions for domain concepts like a card (that has a suit and a rank), a deck of cards (*CardDeck*) and an *Higher-Lower* game (*HigherLowerGame*).
- **CardGames.Domain.Tests**: this project contains the (GUTS) tests that verify the code in the *CardGames.Domain* project. You cannot alter a single line of code here, otherwise your testresults cannot be send to the GUTS system. 

Let the automatic tests guide you into understanding the intent of the code and into completing the code.

Use the WPF app to verify if it all actually works.

[img_mainwindow]:Images/mainwindow.png "MainWindow of the CardGames WPF application"