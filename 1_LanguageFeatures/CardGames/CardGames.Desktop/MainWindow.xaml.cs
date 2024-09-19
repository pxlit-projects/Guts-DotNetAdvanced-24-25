using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CardGames.Domain;

namespace CardGames.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly HigherLowerGame _game;

        public MainWindow()
        {
            InitializeComponent();
            _game = new HigherLowerGame(new CardDeck() as ICardDeck, 3, CardRank.Five);

            UpdateWindow();
        }

        private void HigherButton_Click(object sender, RoutedEventArgs e)
        {
            _game.MakeGuess(higher:true);
            UpdateWindow();
        }

        private void LowerButton_Click(object sender, RoutedEventArgs e)
        {
            _game.MakeGuess(higher: false);
            UpdateWindow();
        }

        private void UpdateWindow()
        {
            CurrentCardTextBlock.Text = _game.CurrentCard.ToString();
            if (_game.PreviousCard is not null)
            {
                PreviousCardTextBlock.Text = _game.PreviousCard.ToString();
            }

            if (_game.HasWon)
            {
                MessageTextBlock.Text = "You won!";
            }
            else if (!string.IsNullOrEmpty(_game.Motivation))
            {
                MessageTextBlock.Text = _game.Motivation;
            }
            else
            {
                MessageTextBlock.Text = "";
            }
        }
    }
}
