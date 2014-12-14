using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Pong
{
    public partial class PongForm : Form
    {
        private Game game;
        private Thread gameThread;

        public PongForm()
        {
            InitializeComponent();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            game = new Game(this);

            gameThread = new Thread(new ThreadStart(game.start));
            gameThread.Start();
        }

        private void PongForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            gameThread.Abort();
        }

        private void PongForm_KeyDown(object sender, KeyEventArgs e)
        {
            game.setKey(e, true);
        }

        private void PongForm_KeyUp(object sender, KeyEventArgs e)
        {
            game.setKey(e, false);
        }
    }

    public class Game
    {
        private int score1 = 0,
                    score2 = 0;

        private int width = 600,
                    height = 400,
                    maxTop, maxBottom,
                    speed = 3,
                    waitUpdates = 10;

        public bool needsRestart = false;

        private int updateFrame = 0;

        private Paddle paddle1, paddle2;
        private Ball ball;

        private PongForm form;
        private Drawer drawer;
        private Controls controls = new Controls();

        public Game(PongForm gameForm)
        {
            form = gameForm;

            paddle1 = new Paddle(10, height / 2, 80);
            paddle2 = new Paddle(width - 20, height / 2, 80);

            ball = new Ball(width / 2, height / 2, 0.5);

            maxTop = 10;
            maxBottom = height - 10;
        }

        private void win1()
        {
            ++score1;

            if(score1 % 5 == 0) paddle1 = new Paddle(10, (int)paddle1.pos.y - 5, paddle1.size.height + 10);

            needsRestart = true;
            waitUpdates += 100;
        }
        private void win2() 
        {
            ++score2;

            if(score2 % 5 == 0) paddle2 = new Paddle(width - 20, (int)paddle2.pos.y - 5, paddle2.size.height + 10);

            needsRestart = true;
            waitUpdates += 100;
        }

        public void start()
        {
            drawer = new Drawer(form.Canvas, width, height);
            startTimer();
        }

        private void update()
        {
            ++updateFrame;

            if (waitUpdates > 0)
            {
                --waitUpdates;
            }
            else
            {
                ball.update();
            }

            if (ball.vel.y > 0)
            {
                if (ball.pos.y + ball.size.height >= maxBottom) ball.vel.y *= -1;
            }
            else
            {
                if (ball.pos.y <= maxTop) ball.vel.y *= -1;
            }

            if (ball.vel.x > 0)
            {
                if (AABB(ball, paddle2)) ball.hit((ball.pos.y - paddle2.pos.y) / paddle2.size.height);
                if (ball.pos.x + ball.size.width >= width) win1();
            }
            else
            {
                if (AABB(ball, paddle1)) ball.hit((ball.pos.y - paddle1.pos.y) / paddle1.size.height);
                if (ball.pos.x + ball.size.width <= 0) win2();
            }

            if (updateFrame % 5 == 0)
            {
                if (controls.down1 && paddle1.pos.y + paddle1.size.height < maxBottom) paddle1.pos.y += speed;
                else if (controls.up1 && paddle1.pos.y > maxTop) paddle1.pos.y -= speed;

                if (controls.down2 && paddle2.pos.y + paddle2.size.height < maxBottom) paddle2.pos.y += speed;
                else if (controls.up2 && paddle2.pos.y > maxTop) paddle2.pos.y -= speed;

                if (controls.auto1)
                {
                    if (ball.pos.y < paddle1.pos.y + paddle1.size.height / 2 &&
                        paddle1.pos.y >= maxTop) paddle1.pos.y -= speed;
                    if (ball.pos.y > paddle1.pos.y + paddle1.size.height / 2 &&
                        paddle1.pos.y + paddle1.size.height <= maxBottom) paddle1.pos.y += speed;
                }

                if (controls.auto2)
                {
                    if (ball.pos.y < paddle2.pos.y + paddle2.size.height / 2 &&
                        paddle2.pos.y >= maxTop) paddle2.pos.y -= speed;
                    if (ball.pos.y > paddle2.pos.y + paddle2.size.height / 2 &&
                        paddle2.pos.y + paddle2.size.height <= maxBottom) paddle2.pos.y += speed;
                }
            }

            if (needsRestart)
            {
                needsRestart = false;

                if (score2 > score1)
                {
                    ball = new Ball((int)(paddle1.pos.x + 11), (int)(paddle1.pos.y + paddle1.size.height / 2), 1);
                }
                else
                {
                    ball = new Ball((int)(paddle2.pos.x - 6), (int)(paddle2.pos.y + paddle2.size.height / 2), -1);
                }
            }
        }

        private void render()
        {
            drawer.clearScreen();

            drawer.drawScore(score1, score2);

            drawer.drawBall(ball);
            drawer.drawPaddle(paddle1);
            drawer.drawPaddle(paddle2);
        }

        public void setKey(KeyEventArgs e, bool value)
        {
            switch (e.KeyCode)
            {
                case Keys.Up: controls.up2 = value; break;
                case Keys.W: controls.up1 = value; break;
                case Keys.Down: controls.down2 = value; break;
                case Keys.S: controls.down1 = value; break;

                case Keys.J: controls.auto1 = value; break;
                case Keys.K: controls.auto2 = value; break;
            }
        }

        private bool AABB(IEntity a, IEntity b)
        {
            Vec aPos = a.getPos(),
                bPos = b.getPos();

            Size aSize = a.getSize(),
                bSize = b.getSize();

            return !(
                    aPos.x > bPos.x + bSize.width ||
                    aPos.x + aSize.width < bPos.x ||
                    aPos.y > bPos.y + bSize.height ||
                    aPos.y + aSize.height < bPos.y
                );
        }

        //timer stuff

        private DateTime lastTime;
        private TimeSpan elapsedTime = new TimeSpan(0, 0, 0);
        private TimeSpan updateTime = new TimeSpan(0, 0, 0, 0, 3);

        private void startTimer()
        {
            lastTime = DateTime.Now;

            //this is in a thread, so it won't completely stop the program
            while (true)
            {
                DateTime now = DateTime.Now;
                elapsedTime += now - lastTime;

                while (elapsedTime > updateTime)
                {
                    elapsedTime -= updateTime;
                    update();
                }

                render();

                Thread.Sleep(4);

                lastTime = now;
            }
        }
    }

    public interface IEntity
    {
        Vec getPos();
        Size getSize();
    }

    public class Controls
    {
        public bool
            down1 = false,
            down2 = false,
            up1 = false,
            up2 = false,
            auto1 = true,
            auto2 = false;
    }

    public class Drawer
    {
        private Graphics ctx;
        private SolidBrush brush;
        private Font font = new Font("Verdana", 20);

        private int width, height;

        public Drawer(Panel canvas, int widthValue, int heightValue)
        {
            ctx = canvas.CreateGraphics();

            width = widthValue;
            height = heightValue;
        }

        public void clearScreen()
        {
            brush = new SolidBrush(Color.FromArgb(0x40000000));

            ctx.FillRectangle(brush, 0, 0, width, height);

            //draw borders
            brush = new SolidBrush(Color.LightGray);

            ctx.FillRectangle(brush, 0, 5, width, 5);
            ctx.FillRectangle(brush, 0, height - 10, width, 5);
        }

        public void drawPaddle(Paddle paddle)
        {
            brush = new SolidBrush(Color.White);

            ctx.FillRectangle(brush, (int)paddle.pos.x, (int)paddle.pos.y, paddle.size.width, paddle.size.height);
        }

        public void drawBall(Ball ball)
        {
            brush = new SolidBrush(Color.Red);

            ctx.FillRectangle(brush, (int)ball.pos.x, (int)ball.pos.y, ball.size.width, ball.size.height);
        }

        public void drawScore(int s1, int s2)
        {
            brush = new SolidBrush(Color.Gray);

            ctx.DrawString(s1 + " ", font, brush, width / 2 - 40, 10);
            ctx.DrawString(s2 + " ", font, brush, width / 2 + 20, 10);
        }
    }

    public class Paddle : IEntity
    {
        public Size size;
        public Vec pos;

        public Paddle(int x, int y, int h)
        {
            size = new Size(10, h);
            pos = new Vec(x, y);
        }

        public Vec getPos()
        {
            return pos;
        }
        public Size getSize()
        {
            return size;
        }
    }

    public class Ball : IEntity
    {
        public Size size = new Size(5, 5);
        public Vec pos;
        public Vec vel;

        public Ball(int x, int y, double dir)
        {
            pos = new Vec(x, y);
            vel = new Vec(dir, .5);
        }

        public void update()
        {
            pos.x += vel.x;
            pos.y += vel.y;
        }

        public void hit(double perc)
        {
            vel.y += (perc - .5) / 2;

            vel.x *= -1.05;
        }

        public Vec getPos()
        {
            return pos;
        }
        public Size getSize()
        {
            return size;
        }
    }

    public class Vec
    {
        public double x, y;

        public Vec(double valueX, double valueY)
        {
            x = valueX;
            y = valueY;
        }
    }

    public class Size
    {
        public int width, height;

        public Size(int valueW, int valueH)
        {
            width = valueW;
            height = valueH;
        }
    }
}
