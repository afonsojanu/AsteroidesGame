using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Asteroides
{
    public partial class Game : Form
    {
        static Game game;
        private Timer graphicsTimer;
        public List<Bullet> bullets = new();
        private List<Asteroid> asteroids = new();
        private Player player;
        private int score = 0;
        private bool gameOver = false;
        public const double TO_RAD = Math.PI / 180;
        public const int FPS = 30;
        public DateTimeOffset dto = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
                scoreLabel.Text = "Score: " + score;
            }
        }
        public Game()
        {
            InitializeComponent();
            game = this;
        }

        public static Game GetInstance()
        {
            return game;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            player = new Player();
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,
            true);

            gameOverLabel.Visible = false;
            Reset_Asteroids();
            this.KeyPreview = true;
            graphicsTimer = new Timer
            {
                Interval = 2
            };
            graphicsTimer.Tick += GraphicsTimer_Tick;

            GameLoop();
            this.KeyUp += player.KeyUp;
            this.KeyDown += player.KeyDown;
            this.KeyDown += Game_KeyDown;
            graphicsTimer.Start();
            this.Width = 800;
            this.Height = 500;
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Space && gameOver)
            {
                player.Reset();
                player.SetLives(3);
                Game.GetInstance().Reset_Asteroids();
                gameOver = false;
                gameOverLabel.Visible = false;
                Score = 0;
            }
        }

        public void Reset_Asteroids()
        {
            asteroids.Clear();
            for (int i = 0; i < 5; i++)
            {
                Create_Asteroid();
            }
        }
        public void Create_Asteroid()
        {
            asteroids.Add(new Asteroid());
        }
        private async void GameLoop()
        {
            bool Running = true;

            while (Running)
            {
                player.Update();
                
                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    Bullet bullet = bullets[i];
                    bullet.Update();
                    if (CheckCollision(bullet.GetCenterPosition(), player.GetCenterPosition(), Bullet.BULLET_SIZE, Player.SHIP_SIZE*2))
                    {
                        bullets.RemoveAt(i);
                        continue;
                    }
                    for (int k = asteroids.Count - 1; k >= 0; k--)
                    {
                        Asteroid asteroid = asteroids[k];
                        if (CheckCollision(bullet.GetCenterPosition(), asteroid.GetCenterPosition(), Bullet.BULLET_SIZE, asteroid.GetSize()*10*2))
                        {
                            bullets.RemoveAt(i);
                            
                            if (asteroid.GetSize()-1 > 0)
                            {
                                asteroids.Add(new Asteroid(asteroid.GetSize() - 1, asteroid.GetPosition()));
                                asteroids.Add(new Asteroid(asteroid.GetSize() - 1, asteroid.GetPosition()));
                            }
                            
                            if (asteroid.GetSize() == 1)
                            {
                                Score += 3;
                            }
                            else if (asteroid.GetSize() == 2)
                            {
                                Score += 2;
                            }
                            else if (asteroid.GetSize() == 3)
                            {
                                Create_Asteroid();
                                Score += 1;
                            }
                            asteroids.RemoveAt(k);
                            break;
                        }
                    }
                }
                for (int i = asteroids.Count - 1; i >= 0; i--)
                {
                    Asteroid asteroid = asteroids[i];
                    asteroid.Update();
                    if (CheckCollision(asteroid.GetCenterPosition(), player.GetCenterPosition(), asteroid.GetSize()*10*2, Player.SHIP_SIZE*2))
                    {
                        player.Hit();
                        asteroids.RemoveAt(i);
                        continue;
                    }
                }
                
                // Update Game at 60fps
                await Task.Delay(1000/FPS);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.Clear(Color.Black);
            player.Render(g);
            foreach(var bullet in bullets)
            {
                bullet.Render(g);
            }
            foreach(var asteroid in asteroids)
            {
                asteroid.Render(g);
            }
        }

        public static bool CheckCollision(Point object1, Point object2, int size1, int size2)
        {
            return Math.Abs(object1.X - object2.X) < size1 / 2 + size2 / 2 &&
                   Math.Abs(object1.Y - object2.Y) < size1 / 2 + size2 / 2;

        }

        void GraphicsTimer_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        public void GameOver()
        {
            gameOverLabel.Text = "GAME OVER \n YOUR SCORE: " + score + " POINTS \n PRESS SPACE TO RESTART";
            gameOverLabel.Location = new Point(this.Width / 2 - gameOverLabel.Width / 2, this.Height / 2 - gameOverLabel.Height / 2);
            gameOverLabel.Visible = true;
            gameOver = true;
        }
            
    }
}
