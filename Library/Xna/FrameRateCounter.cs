using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jellyfish.Library
{
    public sealed class FrameRateCounter : DrawableGameComponent
    {
        public FrameRateCounter(GameBase game) : 
            base(game)
        {
            FontColor = Color.White;
            FontName = "Default";

            //game.IsFixedTimeStep = true; // fixed (default)
            //game.TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);

            //game.IsFixedTimeStep = false; // flatout
            //game.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>(FontName);

            Rectangle titleSafeArea = Game.GraphicsDevice.DisplayMode.TitleSafeArea;
            Position = new Vector2(titleSafeArea.X, titleSafeArea.Y);
        }

        public override void Draw(GameTime gameTime)
        {
            _frameCount++;

            _frameRateBuilder.Length = 0;
            _frameRateBuilder.AppendWithoutGarbage(_frameRate).Append(" fps");

            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            //_spriteBatch.DrawString(_spriteFont, fps, Position - Vector2.UnitX, Color.Black); // rough outline
            //_spriteBatch.DrawString(_spriteFont, fps, Position + Vector2.UnitX, Color.Black);
            //_spriteBatch.DrawString(_spriteFont, fps, Position - Vector2.UnitY, Color.Black);
            //_spriteBatch.DrawString(_spriteFont, fps, Position + Vector2.UnitY, Color.Black);
            _spriteBatch.DrawString(_spriteFont, _frameRateBuilder, Position, FontColor);
            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime.Ticks;

            if (_elapsedTime >= TimeSpan.TicksPerSecond)
            {
                _elapsedTime -= TimeSpan.TicksPerSecond;
                _frameRate = _frameCount;
                _frameCount = 0;
            }
        }

        public Color FontColor { get; set; }
        public string FontName { get; set; }
        public Vector2 Position { get; set; }

        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private long _elapsedTime;
        private int _frameCount;
        private int _frameRate;
        private StringBuilder _frameRateBuilder = new StringBuilder(); // cache builder; avoids garbage
    }
}
