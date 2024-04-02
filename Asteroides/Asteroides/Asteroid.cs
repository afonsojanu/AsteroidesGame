using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroides
{
    public class Asteroid
    {
        private int size = 3;
        private int speedX, speedY;
        private Point position;
        readonly Point[] points = new Point[10];
        public Asteroid()
        {
            Game game = Game.GetInstance();
            Random r = new Random();
            do
            {
                position = new Point(r.Next(0, game.Width), r.Next(0, game.Height));
            } while (position.X > 0 && position.X < game.Width && position.Y > 0 && position.Y < game.Height);

            while (speedX == 0 && speedY == 0)
            {
                speedX = r.Next(-80, 80) / Game.FPS;
                speedY = r.Next(-80, 80) / Game.FPS;
            }
        }
        public Asteroid(int size, Point position) : this()
        {
            this.size = size;
            this.position = position;
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
            for (int i = 0; i < 10; i++)
            {
                points[i] = new Point(position.X + (int)(Math.Cos(i * 2 * Math.PI / 10) * size*10), position.Y + (int)(Math.Sin(i * 2 * Math.PI / 10) * size*10));
            }
            g.DrawPolygon(new Pen(Color.White, 2), points);

        }


        public Point GetPosition()
        {
            return position;
        }
        public Point GetCenterPosition()
        {
            return new Point(position.X + size/2, position.Y + size / 2);
        }
        public int GetSize()
        {
            return size;
        }
    }
}
