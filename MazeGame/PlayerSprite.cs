using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Maze;

/// <summary>
/// The PlayerSprite class is responsible for displaying the player on the game screen.
/// </summary>
public class PlayerSprite : DrawableGameComponent
{
    private readonly IPlayer _player;
    private readonly Texture2D _playerTexture;
    private readonly SpriteBatch _spriteBatch;

    /// <summary>
    /// Constructor for the PlayerSprite class.
    /// </summary>
    /// <param name="game">The game instance.</param>
    /// <param name="player">An instance of the IPlayer interface.</param>
    /// <param name="playerTexture">The texture to be used for the player sprite.</param>
    public PlayerSprite(Game game, IPlayer player, Texture2D playerTexture) : base(game)
    {
        _player = player;
        _playerTexture = playerTexture;
        _spriteBatch = new SpriteBatch(game.GraphicsDevice);
    }

    /// <summary>
    /// Updates the player sprite.
    /// </summary>
    /// <param name="gameTime">A GameTime object containing timing information.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    /// <summary>
    /// Draws the player sprite to the game screen.
    /// </summary>
    /// <param name="gameTime">A GameTime object containing timing information.</param>
    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();

        float rotation = _player.GetRotation();
        Vector2 origin = new Vector2(_playerTexture.Width / 2, _playerTexture.Height / 2);
        Vector2 position = new Vector2(_player.Position.X * 32 + 16, _player.Position.Y * 32 + 16);

        _spriteBatch.Draw(_playerTexture, position, null, Color.White, rotation, origin, 1.0f, SpriteEffects.None, 0);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
