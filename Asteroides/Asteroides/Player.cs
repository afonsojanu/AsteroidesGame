using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Asteroides
{
    class Player
    {
        public const int SHIP_SIZE = 15;
        public const int SHIP_SPEED = 10;
        public const float SHIP_TURNSPEED = 300;
        private Point position;
        private int angle;
        private float speedX, speedY;
        private float rotateSpeed;
        private bool thrusting;
        private bool firing;
        private Game game;
        private Point[] ship = new Point[3];
        private int lives = 3;
        private long hitTimer = 0;
        private bool frozen = false;


        public Player()
        {
            Reset();
            this.game = Game.GetInstance();
        }

        public void Reset()
        {
            this.frozen = false;
            this.position.X = Game.GetInstance().Width / 2;
            this.position.Y = Game.GetInstance().Height / 2;
            this.speedX = 0;
            this.speedY = 0;
            this.rotateSpeed = 0;
            this.angle = 180;
        }

        public void Update()
        {
            if (frozen)
                return;
            if (thrusting)
            {
                int angleC = -angle - 90;
                float rad = (float)(angleC * Game.TO_RAD);
                speedX += (float)Math.Cos(rad) * 15 / Game.FPS;
                speedY += (float)-Math.Sin(rad) * 15 / Game.FPS;
            }
            this.position.X += (int)Math.Round(speedX);
            this.position.Y += (int)Math.Round(speedY);
            this.angle += (int)Math.Round(rotateSpeed);
            if (this.position.X > game.Width)
            {
                this.position.X = game.Width - this.position.X;
            }
            else if (this.position.X < 0)
            {
                this.position.X = game.Width - Math.Abs(0 - this.position.X);
            }
            if (this.position.Y > game.Height)
            {
                this.position.Y = game.Height - this.position.Y;
            }
            else if (this.position.Y < 0)
            {
                this.position.Y = game.Height - Math.Abs(0 - this.position.Y);
            }

        }
        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                thrusting = true;
            }
            else if (e.KeyCode == Keys.Right)
            {
                rotateSpeed = SHIP_TURNSPEED / Game.FPS;
            }
            else if (e.KeyCode == Keys.Left)
            {
                rotateSpeed = -SHIP_TURNSPEED / Game.FPS;
            }
            else if (e.KeyCode == Keys.Space && !firing)
            {
                firing = true;
                Fire();
            }
        }
        
        public void KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                thrusting = false;
            }
            else if (e.KeyCode == Keys.Right | e.KeyCode == Keys.Left)
            {
                rotateSpeed = 0;
            }
            else if (e.KeyCode == Keys.Space)
            {
                firing = false;
            }
        }
        static Point RotatePoint(Point pivot, Point p, float angle)
        {
            double radians = angle * Math.PI / 180;
            double cosTheta = Math.Cos(radians);
            double sinTheta = Math.Sin(radians);
            return new Point
            {
                X = (int)(cosTheta * (p.X - pivot.X) - sinTheta * (p.Y - pivot.Y) + pivot.X),
                Y = (int)(sinTheta * (p.X - pivot.X) + cosTheta * (p.Y - pivot.Y) + pivot.Y)
            };
        }
        private Point[] RotatePointsShip(Point[] points, float angle)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = RotatePoint(new Point(this.position.X, this.position.Y), points[i], angle);
            }
            return points;
        }
        public void Render(Graphics g)
        {
            if(!frozen || frozen && DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond / 200 % 2 == 0)
            {
                ship[0] = new Point(this.position.X - SHIP_SIZE, this.position.Y - SHIP_SIZE);
                ship[1] = new Point(this.position.X + SHIP_SIZE, this.position.Y - SHIP_SIZE);
                ship[2] = new Point(this.position.X, this.position.Y + SHIP_SIZE);
                ship = RotatePointsShip(ship, angle);
                g.DrawPolygon(Pens.White, ship);
            }
       
            if (thrusting && !frozen)
            {
                Point[] flame = new Point[3];

                flame[0] = new Point(this.position.X + SHIP_SIZE - 5, this.position.Y - SHIP_SIZE);
                flame[1] = new Point(this.position.X - SHIP_SIZE + 5, this.position.Y - SHIP_SIZE);
                flame[2] = new Point(this.position.X, this.position.Y - 35);
                flame = RotatePointsShip(flame, angle);
                g.FillPolygon(Brushes.Yellow, flame);

                flame[0] = new Point(this.position.X + SHIP_SIZE - 8, this.position.Y - SHIP_SIZE);
                flame[1] = new Point(this.position.X - SHIP_SIZE + 8, this.position.Y - SHIP_SIZE);
                flame[2] = new Point(this.position.X, this.position.Y - 30);
                flame = RotatePointsShip(flame, angle);
                g.FillPolygon(Brushes.Red, flame);
            }

            int xLife = 10;
            for(int i=0; i < lives; i++)
            {
                Point[] livesP = new Point[3];
                int heightLife = Game.GetInstance().Height - 50;
                livesP[0] = new Point(xLife, heightLife);
                livesP[1] = new Point(xLife + SHIP_SIZE * 2, heightLife);
                livesP[2] = new Point(xLife + SHIP_SIZE, heightLife - SHIP_SIZE * 2);
                xLife += SHIP_SIZE * 2 + 10;
                if (i == lives - 1 && hitTimer != 0)
                {
                    g.DrawPolygon(Pens.Red, livesP);
                }else
                {
                    g.DrawPolygon(Pens.White, livesP);
                }
            }
            if (hitTimer != 0 && DateTimeOffset.Now.ToUnixTimeSeconds() > hitTimer)
            {
                hitTimer = 0;
                lives--;
                if (this.lives == 0)
                {
                    Game.GetInstance().GameOver();
                }
                else
                {
                    Reset();
                    Game.GetInstance().Reset_Asteroids();
                }
            }            
        }


        public void Fire()
        {
            if (frozen)
                return;
            Point firePoint = ship[2];
            game.bullets.Add(new Bullet(firePoint.X, firePoint.Y, this.angle));
        }
        public Point getPosition()
        {
            return this.position;
        }
        public Point GetCenterPosition()
        {
            return new Point(this.position.X + SHIP_SIZE, this.position.Y + SHIP_SIZE);
        }
        public void Hit()
        {
            if (hitTimer != 0)
                return;
            long unixSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
            hitTimer = unixSeconds + 2;
            frozen = true;
        }
        public void SetX(int x)
        {
            this.position.X = x;
        }
        public void SetY(int y)
        {
            this.position.Y = y;
        }
        public void SetLives(int lives)
        {
            this.lives = lives;
        }
    }
}
