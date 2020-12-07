using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Linq;
using System.Data.Entity;

namespace PoopCollecting
{
    /// <summary>
    /// Interaction logic for MainGame.xaml
    /// </summary>
    public partial class MainGame : Window
    {
        public bool keyleft;
        public bool keyright;
        public bool chickenIsLeft=false;
        public string username;
        static int score = 0;
        static int live = 5;


        //static bool isWin = false;
        static int Poopscount = -1;
        static bool isBanana = false;
        static bool isSave = false;
        static int Bombscount = 0;
        static int Bananacount = 0;
        static int vpop = 0;
        static int vban = 0;
        static int vbom = 0;
        static int speed = 8;
        static int normal_speed = 10;
        static int banana_speed = 2;


        const string bg = "#FFB7B2";

        DispatcherTimer Timer = new DispatcherTimer();
        //DispatcherTimer SecTimer = new DispatcherTimer();
        DispatcherTimer DropTimer = new DispatcherTimer();
        DispatcherTimer BananaTimer = new DispatcherTimer();
        

        Rectangle[] rectangles = new Rectangle[1000000];
        
        
        List<Poop> poops = new List<Poop>();
        List<Banana> ban = new List<Banana>();
        List<Bomb> bom = new List<Bomb>();

        public MainGame()
        {
            InitializeComponent();
        }

        private void MoveEvent(object sender, EventArgs e, int speed)
        {
            if (keyleft == true && Canvas.GetLeft(Player) > 5) 
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - speed);           
            }
            if (keyright == true && Canvas.GetLeft(Player)+2*Player.Width < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + speed);
            }
        }

        private void ChickenEvent(object sender, EventArgs e)
        {
            if (chickenIsLeft == true)
                Canvas.SetLeft(Chicken, Canvas.GetLeft(Chicken) - 10);
            if (chickenIsLeft == false)
                Canvas.SetLeft(Chicken, Canvas.GetLeft(Chicken) + 10);
            if (Canvas.GetLeft(Chicken) < 5)
                chickenIsLeft = false;
            if (Canvas.GetLeft(Chicken) + 2 * Chicken.Width > Application.Current.MainWindow.Width)
                chickenIsLeft = true;
        }

        private void Canvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                keyleft = false;
            if (e.Key == Key.Right)
                keyright = false;
        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                keyleft = true;
            if (e.Key == Key.Right)
                keyright = true;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ApearIn4(PlayButton,myCanvas,labelEnter,Entry,Gobutton);
        }

        public void Apear(Image chick, Image player, Canvas canvas)
        {
            chick.Opacity = 1;
            player.Opacity = 1;
            var bc = new BrushConverter();
            canvas.Background = (Brush)bc.ConvertFrom("#FFB7B2");
        }
        public void ApearIn4(Button PlayButton, Canvas canvas, Label lb, TextBox tb, Button bt)
        {
            canvas.Children.Remove(PlayButton);
            lb.Opacity = 1;
            tb.Opacity = 1;
            bt.Opacity = 1;
        }
        public void StartGame()
        {
            myCanvas.Focus();
            Timer.Interval = TimeSpan.FromMilliseconds(20);
            DropTimer.Interval = TimeSpan.FromMilliseconds(1000);
            BananaTimer.Interval = TimeSpan.FromSeconds(3);

            BananaTimer.Tick += BananaTimer_Tick;
           
            if (isBanana == true)
                Timer.Tick += (object sender, EventArgs e) => MoveEvent(sender, e, banana_speed);
            else
                Timer.Tick += (object sender, EventArgs e) => MoveEvent(sender, e, speed);
            Timer.Tick += ChickenEvent;

            //DropTimer.Tick += PoopsInitializeEvent;
            //Timer.Tick += PoopsDropEventt;
            Random random = new Random();
            int ran = random.Next(1, 5);
            switch(ran)
            {
                case 1:
                    DropTimer.Tick += PoopsInitializeEvent;
                    
                    break;
                case 2:
                    DropTimer.Tick += PoopsInitializeEvent;
                    
                    break;
                case 3:
                    DropTimer.Tick += PoopsInitializeEvent;
                   
                    break;
                case 4:
                    DropTimer.Tick += BananaInitializeEvent;
                  
                    break;
                case 5:
                    DropTimer.Tick += BombsInitializeEvent;
                   
                    break;
            }
            Timer.Tick += PoopsDropEventt;
            Timer.Tick += BananasDropEventt;
            Timer.Tick += BombsDropEventt;
            Timer.Tick += CheckEventt;
            Timer.Tick += ScoreEvent;
            Timer.Tick += GameOver;

            BananaTimer.Start();
            Timer.Start();
            DropTimer.Start();
            
        }

        private void GameOver(object sender, EventArgs e)
        {
            if ((live == 0)&&(isSave==false))
            {
                Timer.Stop();
                DropTimer.Stop();
                BananaTimer.Stop();
                using (var c = new Context())
                {
                    var p = new Player
                    {
                        Username = username,
                        Score = score
                    };
                    c.Players.Add(p);
                    c.SaveChanges();
                };
                isSave = true;
            }
        }

        

        private void BombsDropEventt(object sender, EventArgs e)
        {
            int ii = vbom;
            while (ii <= Bombscount)
            {
                bom[ii].Move();
                ++ii;
            }
        }

        private void BombsInitializeEvent(object sender, EventArgs e)
        {
            ++Bombscount;
            Rectangle rec = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("C:\\Users/tudan/Desktop/Project OOP/PoopCollecting/Image/bom.png")) }
            };

            Bomb b = new Bomb(Bombscount, Canvas.GetLeft(Chicken) + (Chicken.Width / 2.0), Canvas.GetTop(Chicken) + Chicken.Height + 3.0, rec);

            bom.Add(b);


            Canvas.SetLeft(bom[Bombscount].re, bom[Bombscount].locationX);
            Canvas.SetTop(bom[Bombscount].re, bom[Bombscount].locationY);
            myCanvas.Children.Add(bom[Bombscount].re);
        }

        private void BananaTimer_Tick(object sender, EventArgs e)
        {
            isBanana = false;
        }

        private void BananasDropEventt(object sender, EventArgs e)
        {
            int ii = vban;
            while (ii <= Bananacount)
            {
                ban[ii].Move();

                ++ii;

            }
        }

        private void BananaInitializeEvent(object sender, EventArgs e)
        {
            ++Bananacount;
            Rectangle rec = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("C:\\Users/tudan/Desktop/Project OOP/PoopCollecting/Image/chuoi.png")) }
            };

            Banana b = new Banana(Bananacount, Canvas.GetLeft(Chicken) + (Chicken.Width / 2.0), Canvas.GetTop(Chicken) + Chicken.Height + 3.0, rec);

            ban.Add(b);


            Canvas.SetLeft(ban[Bananacount].re, ban[Bananacount].locationX);
            Canvas.SetTop(ban[Bananacount].re, ban[Bananacount].locationY);
            myCanvas.Children.Add(ban[Bananacount].re);
        }

        private void CheckEventt(object sender, EventArgs e)
        {
            int ii = vpop;
            while (ii <= Poopscount)
            {
                if (poops[ii].checkColli(Player) == 1)
                {
                    myCanvas.Children.Remove(poops[ii].re);
                    ++vpop;
                    poops[ii].isCollected = true;
                    
                }

                if (poops[ii].checkColli(Player) == 2)
                { 
                    myCanvas.Children.Remove(poops[ii].re);
                    ++vpop;
                    poops.RemoveAt(ii);
                }
                ++ii;
            }
            int iii = vban;
            while (iii <= Bananacount)
            {
                if (ban[iii].checkColli(Player) == 1)
                {
                    myCanvas.Children.Remove(ban[iii].re);
                    ++vban;
                    isBanana = true;
                    ban.RemoveAt(iii);
                }

                if (ban[iii].checkColli(Player) == 2)
                {
                    myCanvas.Children.Remove(ban[iii].re);
                    ++vban;
                    ban.RemoveAt(iii);
                }
                ++iii;
            }
            int i = vbom;
            while (i <= Bombscount)
            {
                if (bom[i].checkColli(Player) == 1)
                {
                    myCanvas.Children.Remove(bom[i].re);
                    ++vbom;
                    --live;
                    score -= 15;
                    bom[i].isCollected = true;
                    
                }

                if (bom[i].checkColli(Player) == 2)
                {
                    myCanvas.Children.Remove(bom[i].re);
                    ++vbom;
                    bom.RemoveAt(i);
                }
                ++i;
            }
        }

        private void PoopsDropEventt(object sender, EventArgs e)
        {
            int ii = vpop;
            while (ii <= Poopscount)
            {
                poops[ii].Move();
                ++ii;   
            }
        }

        private void Gobutton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new Context())
            {}
            username = Entry.Text;
            myCanvas.Children.Remove(Gobutton);
            myCanvas.Children.Remove(Entry);
            myCanvas.Children.Remove(labelEnter);
            Apear(Chicken, Player, myCanvas);
            StartGame();
        }
        private void ScoreEvent(object sender, EventArgs e)
        {
            for (int i = vpop; i <= Poopscount; ++i)
                if ((poops[i].isCollected == true) && (poops[i].isSure == false))
                {
                    score += 10;
                    poops[i].isSure = true;
                    poops.RemoveAt(i);
                }
            for (int i = vbom; i <= Bombscount; ++i)
                if ((bom[i].isCollected == true) && (bom[i].isSure == false))
                {
                    score -= 15;
                    bom[i].isSure = true;
                    bom.RemoveAt(i);
                }
            Score.Text = String.Format("Score: {0}", score);
        }
        private void PoopsInitializeEvent(object sender, EventArgs e)
        {
            ++Poopscount;
            Rectangle rec = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("C:\\Users/tudan/Desktop/Project OOP/PoopCollecting/Image/cut.png")) }
            };
            
            Poop p = new Poop(Poopscount, Canvas.GetLeft(Chicken) + (Chicken.Width / 2.0), Canvas.GetTop(Chicken) + Chicken.Height + 3.0, rec);
            
            poops.Add(p);


            Canvas.SetLeft(poops[Poopscount].re, poops[Poopscount].locationX); 
            Canvas.SetTop(poops[Poopscount].re, poops[Poopscount].locationY);
            myCanvas.Children.Add(poops[Poopscount].re);

        }


    }
    public class Poop
    {
        public int id;
        public double speed = 5;
        public double locationX;
        public double locationY;
        public Rectangle re;
        
        public int height = 50;
        public int width = 50;
        public bool isCollected = false;
        public bool isSure = false;

        DispatcherTimer t = new DispatcherTimer();

        public Poop(int id, double location, double loca, Rectangle r)
        {
            this.id = id;
            this.locationX = location;
            this.locationY = loca;
            this.re = r;
            //timer.Interval = TimeSpan.FromMilliseconds(20);

            t.Interval = TimeSpan.FromMilliseconds(20);
        
            t.Start();
        }
        
        
        public void Move()
        { 
            t.Tick += Time_Tick;
            locationY += 5.0;
        }

        private void Time_Tick(object sender, EventArgs e)
        {
            Canvas.SetTop(re, locationY + 5.0);
        }
        public int checkColli(Image player)
        {
            if ((locationX > Canvas.GetLeft(player)) && (locationX < Canvas.GetLeft(player)+player.Width) && (locationY >= Canvas.GetTop(player)))
                return 1;
            else if (locationY > Canvas.GetTop(player)+player.Height)
                return 2;
            else return 0;
        }
    }
    public class Banana
    {
        public int id;
        public double speed = 5;
        public double locationX;
        public double locationY;
        public Rectangle re;

        public int height = 50;
        public int width = 50;

        DispatcherTimer tt = new DispatcherTimer();

        public Banana(int id, double location, double loca, Rectangle r)
        {
            this.id = id;
            this.locationX = location;
            this.locationY = loca;
            this.re = r;
            //timer.Interval = TimeSpan.FromMilliseconds(20);

            tt.Interval = TimeSpan.FromMilliseconds(20);

            tt.Start();
        }


        public void Move()
        {
            tt.Tick += Time_Tick;
            locationY += 5.0;
        }

        private void Time_Tick(object sender, EventArgs e)
        {
            Canvas.SetTop(re, locationY + 5.0);
        }
        public int checkColli(Image player)
        {
            if ((locationX > Canvas.GetLeft(player)) && (locationX < Canvas.GetLeft(player) + player.Width) && (locationY >= Canvas.GetTop(player)))
                return 1;
            else if (locationY > Canvas.GetTop(player) + player.Height)
                return 2;
            else return 0;
        }
    }
    public class Bomb
    {
        public int id;
        public double speed = 5;
        public double locationX;
        public double locationY;
        public Rectangle re;

        public int height = 50;
        public int width = 50;
        public bool isCollected = false;
        public bool isSure = false;

        DispatcherTimer ti = new DispatcherTimer();

        public Bomb(int id, double location, double loca, Rectangle r)
        {
            this.id = id;
            this.locationX = location;
            this.locationY = loca;
            this.re = r;
            //timer.Interval = TimeSpan.FromMilliseconds(20);

            ti.Interval = TimeSpan.FromMilliseconds(20);

            ti.Start();
        }


        public void Move()
        {
            ti.Tick += Time_Tick;
            locationY += 5.0;
        }

        private void Time_Tick(object sender, EventArgs e)
        {
            Canvas.SetTop(re, locationY + 5.0);
        }
        public int checkColli(Image player)
        {
            if ((locationX > Canvas.GetLeft(player)) && (locationX < Canvas.GetLeft(player) + player.Width) && (locationY >= Canvas.GetTop(player)))
                return 1;
            else if (locationY > Canvas.GetTop(player) + player.Height)
                return 2;
            else return 0;
        }
    }
}

