using System;
using System.Diagnostics.CodeAnalysis;
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
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>(FontName);

            var titleSafeArea = Game.GraphicsDevice.DisplayMode.TitleSafeArea;
            Position = new Vector2(titleSafeArea.X, titleSafeArea.Y);
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public override void Draw(GameTime gameTime)
        {
            _frameCount++;

            _frameRateBuilder.Clear().AppendWithoutGarbage(_frameRate).Append(" fps");

            _spriteBatch.Begin();
            //_spriteBatch.DrawString(_spriteFont, _frameRateBuilder, Position - Vector2.UnitX, Color.Black); // rough outline
            //_spriteBatch.DrawString(_spriteFont, _frameRateBuilder, Position + Vector2.UnitX, Color.Black);
            //_spriteBatch.DrawString(_spriteFont, _frameRateBuilder, Position - Vector2.UnitY, Color.Black);
            //_spriteBatch.DrawString(_spriteFont, _frameRateBuilder, Position + Vector2.UnitY, Color.Black);
            _spriteBatch.DrawString(_spriteFont, _frameRateBuilder, Position, FontColor);
            _spriteBatch.End();
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public override void Update(GameTime gameTime)
        {
            if (gameTime == null)
            {
                throw new ArgumentNullException("gameTime");
            }

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
