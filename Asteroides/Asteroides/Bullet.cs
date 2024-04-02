using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Asteroides
{
    public class Bullet
    {
        public const int BULLET_SIZE = 10;
        private Point position;
        float speedX, speedY;

        
        public Bullet(int x, int y, float angle)
        {
            this.position.X = x-(BULLET_SIZE/2);
            this.position.Y = y;
            angle = -angle-90;
            
            float rad = (float)(angle * Game.TO_RAD);
            speedX = (float)Math.Cos(rad) * 500 / Game.FPS;
            speedY = (float)-Math.Sin(rad) * 500 / Game.FPS;

            this.position.X = x + (int)(Math.Cos(rad) * 10);
            this.position.Y = y - (int)(Math.Sin(rad) * 10);
        }
        public double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }
        public void Update()
        {

            this.position.X += (int)speedX;
            this.position.Y += (int)speedY;
            if (this.position.X > Game.GetInstance().Width)
            {
                this.position.X = Game.GetInstance().Width - this.position.X;
            }
            else if (this.position.X < 0)
            {
                this.position.X = Game.GetInstance().Width - Math.Abs(0 - this.position.X);
            }
            if (this.position.Y > Game.GetInstance().Height)
            {
                this.position.Y = Game.GetInstance().Height - this.position.Y;
            }
            else if (this.position.Y < 0)
            {
                this.position.Y = Game.GetInstance().Height - Math.Abs(0 - this.position.Y);
            }
        }
        public void Render(Graphics g)
        {
            g.DrawEllipse(Pens.Red, this.position.X, this.position.Y, BULLET_SIZE, BULLET_SIZE);
        }
        public Point GetPosition()
        {
            return this.position;
        }
        public Point GetCenterPosition()
        {
            return new Point(this.position.X + BULLET_SIZE / 2, this.position.Y + BULLET_SIZE / 2);
        }
    }
}
