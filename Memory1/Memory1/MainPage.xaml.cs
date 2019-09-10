using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Memory;
using Xamarin.Forms.Internals;

namespace Memory1
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private CardType[] cardTypes;
        private bool move1 = true;

        //Aktualny czas na stoperze
        string currentTime = string.Empty;

        //lista znakow, ktore beda na kartach
        private List<char> signs;

        private bool stopTime = false;

        public MainPage()
        {
           
            InitializeComponent();
            NewGame();
        }

        //Tworzenie nowej gry
        private void NewGame()
        {
            //Tablica z czystymi kartami
            cardTypes = new CardType[20];

            for (int i = 0; i < cardTypes.Length; i++)
                cardTypes[i] = CardType.free;

            //Domyslny kolor kart
            Board_grid.Children.Where(x => x.GetType() == typeof(Button) && x != x.FindByName("startBtn")).ForEach(button =>
            {
                ((Button)button).Text = string.Empty;
                ((Button)button).BackgroundColor = Color.Gray;
                ((Button)button).BorderColor = Color.Gray;
                ((Button)button).FontSize = 70;
                ((Button)button).IsEnabled = false;
            });

        }

        /// <summary>
        /// Przypisywanie znakow do losowych kart
        /// </summary>
        private void RandomInit()
        {

            var random = new Random();
            signs = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };
            cardTypes = new CardType[20];
            move1 = true;
            currentTime = string.Empty;

            Board_grid.Children.Where(x => x.GetType() == typeof(Button) && x != x.FindByName("startBtn")).ForEach(button =>
            {
                int current = random.Next(signs.Count);
                ((Button)button).Text = signs[current].ToString();
                signs.RemoveAt(current);
                ((Button)button).BackgroundColor = Color.White;
                ((Button)button).TextColor = Color.White;
                ((Button)button).FontSize = 70;
                ((Button)button).BorderColor = Color.Gray;
                ((Button)button).IsEnabled = true;
            });
        }

        /// <summary>
        /// Odsiwezanie planszy z zachowaniem odkrytych par
        /// </summary>
        private void RefreshPlay()
        {

            Board_grid.Children.Where(x => x.GetType() == typeof(Button) && x != x.FindByName("startBtn")).ForEach(button =>
            {
                var column = Grid.GetColumn(button);
                var row = Grid.GetRow(button);

                if (cardTypes[(column + row * 5) - 5] == CardType.click)
                {
                    cardTypes[(column + row * 5)-5] = CardType.signed;
                    ((Button)button).BackgroundColor = Color.White;
                    ((Button)button).TextColor = Color.White;
                }
            });


        }

        /// <summary>
        /// Sprawdzanie czy zostala odkryta para
        /// </summary>
        private void CheckHit()
        {
            //lista z kliknietymi buttonami (dlugosc listy 2 elementy)
            List<Button> hittedbtn = new List<Button>(2);
            //lista z indeksami kliknietcyh buttonow w CardTypes (dlugosc listy 2 elementy)
            List<int> cards = new List<int>(2);

            Board_grid.Children.Where(x => x.GetType() == typeof(Button) && x != x.FindByName("startBtn")).ForEach(button =>
            {
                var column = Grid.GetColumn(button);
                var row = Grid.GetRow(button);


                
                if (cardTypes[(column + row * 5) - 5] == CardType.click)
                {
                    hittedbtn.Add(((Button)button));
                    cards.Add((column + row * 5)-5);
                }
                
            });

            //Przy pierwszym ruchu w nowej grze hittedbtn.Count = 0
            //Przy przypadku dwukrotnego klikniecia w taka sama karte hittedbtn.Count = 1
            if (hittedbtn.Count == 2)
            {
                if ((hittedbtn[0].Text.ToString() == hittedbtn[1].Text.ToString()) && (cards[0] != cards[1]))
                {
                    //zmiana typu karty na trafiona
                    cardTypes[cards[0]] = CardType.hit;
                    cardTypes[cards[1]] = CardType.hit;
                    //zablokowanie klikania kart
                    hittedbtn[0].IsEnabled = false;
                    hittedbtn[1].IsEnabled = false;

                }



            }


        }

        private async void GameStatus()
        {
            //ilosc trafionych elementow
            int hittedCount = 2; //zalozenie, ze na poczatku trafiono pare, aby licznik prawidlowo funkcjionowal

            for (int i = 0; i < cardTypes.Length; i++)
            {
                if (cardTypes[i] == CardType.hit)
                    hittedCount++;
            }

            //Dialog YES/ NO po zakonczonej grze  
            if (hittedCount == cardTypes.Length)
            {
                
                var action = await DisplayAlert("Koniec gry! Twój wynik: "+currentTime, "Rozpocząć nową grę?", "Yes", "No");
                if (action)
                {
                    NewGame();
                    RandomInit();

                }
                else
                {
                    NewGame();
                }
               

            }


        }


        private void Cardbtn(object sender, EventArgs e)
        {
            //Sprawdzanie trafienia i odsiwezanie planszy co 2 ruchy
            //Przy pierwszym ruchu w nowej grze warunek spełniony
            if (move1)
            {

                CheckHit();
                RefreshPlay();

            }
            //Co drugi ruch sprawdzane czy jest wygrana
            else
            {
               GameStatus();
            }
            move1 ^= true;



            //Klikniety button
            Button bt1 = (Button)sender;

            //Wspolzedne kliknietego button
            var column = Grid.GetColumn(bt1);

            var row = Grid.GetRow(bt1);

            cardTypes[(column + row * 5)-5] = CardType.click;

            bt1.BackgroundColor = Color.Gray;

        }

        /// <summary>
        /// Obsluga stopera 
        /// </summary>
       


        private void GameStart(object sender, EventArgs e)
        {
            RandomInit();
        }

        private void ChangeColor(object sender, FocusEventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackgroundColor = Color.Red;
        }
    }
}
