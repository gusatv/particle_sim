using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace MyGame;

enum Particle
{
    Air,
    Water,
    Sand,
    Smoke
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
    private Texture2D gasTile;
    private Particle selected_particle = Particle.Sand;
    private int brush_size = 0;
    
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
        width = 200;
        height = 150;
        size_per_pixel = 4;
        _graphics.PreferredBackBufferWidth = width * size_per_pixel;
        _graphics.PreferredBackBufferHeight = height * size_per_pixel;
        _graphics.ApplyChanges();
        for (int i = 0; i < height; i++)
        {
            particles.Add(new List<Particle>());
            for (int j = 0; j < width; j++)
            {
                particles[i].Add(Particle.Air);
            }
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        bool lines = true;
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        sandTile = new Texture2D(_graphics.GraphicsDevice, size_per_pixel, size_per_pixel);
        Color[] sand_color = new Color[size_per_pixel * size_per_pixel];
        for (int i = 0; i < sand_color.Length; i++)
        {
            if (i % size_per_pixel == 0)
            {
                lines = !lines;
            }
            if (lines)
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
            if (i % size_per_pixel == 0)
            {
                lines = !lines;
            }
            if (lines)
            {
                water_color[i] = Color.Blue;
            }
            else
            {
                water_color[i] = Color.DodgerBlue;
            }
        }
        waterTile.SetData(water_color);
        
        gasTile = new Texture2D(_graphics.GraphicsDevice, size_per_pixel, size_per_pixel);
        Color[] gas_color = new Color[size_per_pixel * size_per_pixel];
        for (int i = 0; i < gas_color.Length; i++)
        {
            if (i % size_per_pixel == 0)
            {
                lines = !lines;
            }
            if (lines)
            {
                gas_color[i] = Color.Gray;
            }
            else
            {
                gas_color[i] = Color.DarkGray;
            }
        }
        gasTile.SetData(gas_color);
        // TODO: use this.Content to load your game content here
    }
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.Up))
        {
            
            brush_size += 1;
        }else if (Keyboard.GetState().IsKeyDown(Keys.Down))
        {
            if (brush_size > 0)
            {
                brush_size -= 1;
            }
        }

        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            selected_particle = Particle.Sand;
        }else if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            selected_particle = Particle.Air;
        }else if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            selected_particle = Particle.Water;
        }else if (Keyboard.GetState().IsKeyDown(Keys.G))
        {
            selected_particle = Particle.Smoke;
        }
        
        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            int mouse_x = Mouse.GetState().X/size_per_pixel;
            int mouse_y = Mouse.GetState().Y/size_per_pixel;
            if (mouse_x < width && mouse_y < height && mouse_x >= 0 && mouse_y >= 0)
            {
                if (brush_size > 0)
                {
                    for (int l = brush_size; l > 0; l--)
                    {
                        for (int i = -l; i < l; i++)
                        {
                            if (mouse_x + l - Math.Abs(i) < width && mouse_y + i < height && mouse_y + i >= 0)
                            {
                                particles[mouse_y + i][mouse_x + l - Math.Abs(i)] = selected_particle;
                            }

                            if (mouse_x - l + Math.Abs(i) >= 0 && mouse_x + i < width && mouse_y + i < height &&
                                mouse_y + i >= 0)
                            {
                                particles[mouse_y + i][mouse_x - l + Math.Abs(i)] = selected_particle;
                            }

                            if (mouse_y + i >= 0 && mouse_y + i < height)
                            {
                                particles[mouse_y + i][mouse_x] = selected_particle;
                            }

                            if (mouse_x + i >= 0 && mouse_x + i < width)
                            {
                                particles[mouse_y][mouse_x + i] = selected_particle;
                            }
                        }
                    }
                }
                else
                {
                    particles[mouse_y][mouse_x] = selected_particle;
                }
            }
        }   
        
        
        for (int i = particles.Count-1; i >= 0; i--)
        {
            for (int j = particles[i].Count-1; j >= 0; j--)
            {
                if (particles[i][j] == Particle.Sand)
                {
                    if (check_pos(j, i, 0, 1))
                    {
                        particles[i][j] = particles[i + 1][j];
                        particles[i + 1][j] = Particle.Sand;
                    }else if (check_pos(j, i, -1, 1))
                    {
                        particles[i][j] = particles[i + 1][j - 1];
                        particles[i + 1][j - 1] = Particle.Sand;
                    }else if (check_pos(j, i, 1, 1))
                    {
                        particles[i][j] = particles[i + 1][j + 1];
                        particles[i + 1][j + 1] = Particle.Sand;
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
                }else if (particles[i][j] == Particle.Smoke)
                {
                    if (check_pos(j, i, 0, -1))
                    {
                        particles[i - 1][j] = Particle.Smoke;
                        particles[i][j] = Particle.Air;
                    }else if (check_pos(j, i, -1, -1))
                    {
                        particles[i - 1][j - 1] = Particle.Smoke;
                        particles[i][j] = Particle.Air;
                    } else if (check_pos(j, i, 1, -1))
                    {
                        particles[i - 1][j + 1] = Particle.Smoke;
                        particles[i][j] = Particle.Air;
                    }else if (check_pos(j, i, -1, 0))
                    {
                        particles[i ][j - 1] = Particle.Smoke;
                        particles[i][j] = Particle.Air;
                    }else if (check_pos(j, i, 1, 0))
                    {
                        particles[i][j + 1] = Particle.Smoke;
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
                }else if (particles[i][j] == Particle.Smoke)
                {
                    tilePosition.Y = i*size_per_pixel;
                    tilePosition.X = j*size_per_pixel;
                    
                    _spriteBatch.Draw(gasTile, tilePosition, Color.White);
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
            }else if (particles[y][x] == Particle.Water)
            {
                if (particles[y + diry][x + dirx] == Particle.Air)
                {
                    return true;
                }
            }else if (particles[y][x] == Particle.Smoke)
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

