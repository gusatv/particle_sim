using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyGame;

enum Particle
{
    Air,
    Water,
    Sand,
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private List<List<Particle>> particles;
    private int width;
    private int height;
    private int size_per_pixel;
    private Texture2D sandTile;
    private Texture2D waterTile;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        particles = new List<List<Particle>>();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        width = 400;
        height = 300;
        size_per_pixel = 2;
        _graphics.PreferredBackBufferWidth = width * size_per_pixel;
        _graphics.PreferredBackBufferHeight = height * size_per_pixel;
        _graphics.ApplyChanges();
        for (int i = 0; i < height; i++) 
        {
            particles.Add(new List<Particle>());
            for (int j = 0; j < width; j++)
            {
                if (i % 2 == 0)
                {
                    particles[i].Add(Particle.Sand);
                }else if (i % 2 == 1){
                    particles[i].Add(Particle.Water);
                }else{ 
                    particles[i].Add(Particle.Air);
                }
            }
        }
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        sandTile = new Texture2D(_graphics.GraphicsDevice, size_per_pixel, size_per_pixel);
        Color[] sand_color = new Color[size_per_pixel * size_per_pixel];
        for (int i = 0; i < sand_color.Length; i++)
        {
            if (i == 2 || i == 3)
            {
                sand_color[i] = Color.SandyBrown;
            }
            else
            {
                sand_color[i] = Color.RosyBrown;
            }
        }
        
        sandTile.SetData(sand_color);
        waterTile = new Texture2D(_graphics.GraphicsDevice, size_per_pixel, size_per_pixel);
        Color[] water_color = new Color[size_per_pixel * size_per_pixel];
        for (int i = 0; i < water_color.Length; i++)
        {
            if (i == 2 || i == 3)
            {
                water_color[i] = Color.Blue;
            }
            else
            {
                water_color[i] = Color.DodgerBlue;
            }
        }
        waterTile.SetData(water_color);
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        for (int i = particles.Count-1; i > 0; i--)
        {
            for (int j = particles[i].Count-1; j > 0; j--)
            {
                if (particles[i][j] == Particle.Sand)
                {
                    if (check_pos(j, i, 0, 1))
                    {
                        particles[i + 1][j] = Particle.Sand;
                        particles[i][j] = Particle.Air;
                    }else if (check_pos(j, i, -1, 1))
                    {
                        particles[i + 1][j - 1] = Particle.Sand;
                        particles[i][j] = Particle.Air;
                    }else if (check_pos(j, i, 1, 1))
                    {
                        particles[i + 1][j + 1] = Particle.Sand;
                        particles[i][j] = Particle.Air;
                    }
                }else if (particles[i][j] == Particle.Water)
                {
                    if (check_pos(j, i, 0, 1))
                    {
                        particles[i + 1][j] = Particle.Water;
                        particles[i][j] = Particle.Air;
                    }else if (check_pos(j, i, -1, 1))
                    {
                        particles[i + 1][j - 1] = Particle.Water;
                        particles[i][j] = Particle.Air;
                    }else if (check_pos(j, i, 1, 1))
                    {
                        particles[i + 1][j + 1] = Particle.Water;
                        particles[i][j] = Particle.Air;
                    }else if (check_pos(j, i, -1, 0))
                    {
                        particles[i][j - 1] = Particle.Water;
                        particles[i][j] = Particle.Air;
                    }else if (check_pos(j, i, 1, 0))
                    {
                        particles[i][j + 1] = Particle.Water;
                        particles[i][j] = Particle.Air;
                    }
                }
            }
        }
        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _graphics.GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();
        Vector2 tilePosition = Vector2.Zero;
        
        for (int i = 0; i < particles.Count; i++)
        {
            for (int j = 0; j < particles[i].Count; j++)
            {
                if (particles[i][j] == Particle.Sand)
                {
                    tilePosition.Y = i*size_per_pixel;
                    tilePosition.X = j*size_per_pixel;
                    
                    _spriteBatch.Draw(sandTile, tilePosition, Color.White);
                } else if (particles[i][j] == Particle.Water)
                {
                    tilePosition.Y = i*size_per_pixel;
                    tilePosition.X = j*size_per_pixel;
                    
                    _spriteBatch.Draw(waterTile, tilePosition, Color.White);
                }
            }
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    bool check_pos(int x, int y, int dirx, int diry)
    {
        if (x + dirx > 0 && x + dirx < particles[y].Count && y + diry > 0 && y + diry < particles.Count)
        {
            if (particles[y][x] == Particle.Sand)
            {
                if (particles[y + diry][x + dirx] != Particle.Sand)
                {
                    return true;
                }
            }
            else if (particles[y][x] == Particle.Water)
            {
                if (particles[y + diry][x + dirx] == Particle.Air)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
