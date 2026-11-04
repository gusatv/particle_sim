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
    Smoke,
    Rock
}



public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private List<List<Particle>> particles;
    private int width = 200;
    private int height = 150;
    private int size_per_pixel = 4;
    private Texture2D sandTile;
    private Texture2D waterTile;
    private Texture2D gasTile;
    private Texture2D rockTile;
    private Particle selected_particle = Particle.Sand;
    private int brush_size = 0;
    private Dictionary<Vector2, bool> updatable; 
    private List<List<Particle>> movables = [
        [], // Air
        [Particle.Air],  // Water
        [Particle.Air, Particle.Water, Particle.Smoke], // Sand
        [Particle.Air, Particle.Water], // Smoke
        [], // Rock
    ];
    private List<Texture2D> textures;
    private bool paused = false;
    private KeyboardState oldState;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        particles = new List<List<Particle>>();
        textures = new List<Texture2D>();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
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
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        waterTile = new Texture2D(_graphics.GraphicsDevice, size_per_pixel, size_per_pixel);
        create_texture(waterTile, Color.Blue, Color.DodgerBlue);
        
        sandTile = new Texture2D(_graphics.GraphicsDevice, size_per_pixel, size_per_pixel);
        create_texture(sandTile, Color.SandyBrown, Color.RosyBrown);
        
        gasTile = new Texture2D(_graphics.GraphicsDevice, size_per_pixel, size_per_pixel);
        create_texture(gasTile, Color.Gray, Color.DarkGray);
        
        rockTile = new Texture2D(_graphics.GraphicsDevice, size_per_pixel, size_per_pixel);
        create_texture(rockTile, Color.DarkSlateGray, Color.DimGray);
        // TODO: use this.Content to load your game content here
    }
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        if (Mouse.GetState().LeftButton == ButtonState.Pressed && this.IsActive)
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

        if (!paused)
        {
            updatable = new Dictionary<Vector2, bool>();

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                for (int j = particles[i].Count - 1; j >= 0; j--)
                {
                    if (!updatable.ContainsKey(new Vector2(j, i)))
                    {
                        Random random = new Random();
                        int random_dir = random.Next(0, 2);
                        if (random_dir == 0)
                        {
                            random_dir = -1;
                        }

                        if (particles[i][j] == Particle.Sand)
                        {
                            if (check_pos(j, i, 0, 1, updatable))
                            {
                                move(j, i, 0, 1);
                            }
                            else if (check_pos(j, i, random_dir, 1, updatable))
                            {
                                move(j, i, random_dir, 1);
                            }
                            else if (check_pos(j, i, -random_dir, 1, updatable))
                            {
                                move(j, i, -random_dir, 1);
                            }
                        }
                        else if (particles[i][j] == Particle.Water)
                        {
                            if (check_pos(j, i, 0, 1, updatable))
                            {
                                move(j, i, 0, 1);
                            }
                            else if (check_pos(j, i, random_dir, 1, updatable))
                            {
                                move(j, i, random_dir, 1);
                            }
                            else if (check_pos(j, i, -random_dir, 1, updatable))
                            {
                                move(j, i, -random_dir, 1);
                            }
                            else if (check_pos(j, i, random_dir, 0, updatable))
                            {
                                move(j, i, random_dir, 0);
                            }
                            else if (check_pos(j, i, -random_dir, 0, updatable))
                            {
                                move(j, i, -random_dir, 0);
                            }
                        }
                        else if (particles[i][j] == Particle.Smoke)
                        {
                            if (check_pos(j, i, 0, -1, updatable))
                            {
                                move(j, i, 0, -1);
                            }
                            else if (check_pos(j, i, random_dir, -1, updatable))
                            {
                                move(j, i, random_dir, -1);
                            }
                            else if (check_pos(j, i, -random_dir, -1, updatable))
                            {
                                move(j, i, -random_dir, -1);
                            }
                            else if (check_pos(j, i, random_dir, 0, updatable))
                            {
                                move(j, i, random_dir, 0);
                            }
                            else if (check_pos(j, i, -random_dir, 0, updatable))
                            {
                                move(j, i, -random_dir, 0);
                            }
                        }
                    }
                    //else
                    //{
                    //    updatable.Remove(new Vector2(j, i));
                    //}
                }
            }
        }
        // TODO: Add your update logic here
        update_input();
        
        base.Update(gameTime);
    }

    private void update_input()
    {
        KeyboardState newState = Keyboard.GetState();
        if (newState.IsKeyDown(Keys.Space))
        {
            if (!oldState.IsKeyDown(Keys.Space))
            {
                paused = !paused;
            }
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Up))
        {
            if (!oldState.IsKeyDown(Keys.Up))
            {
                brush_size += 1;
            }
        }else if (Keyboard.GetState().IsKeyDown(Keys.Down))
        {
            if (!oldState.IsKeyDown(Keys.Down))
            {
                if (brush_size > 0)
                {
                    brush_size -= 1;
                }
            }
        }
        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            if (!oldState.IsKeyDown(Keys.S))
            {
                selected_particle = Particle.Sand;
            }
        }else if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            if (!oldState.IsKeyDown(Keys.A))
            {
                selected_particle = Particle.Air;
            }
        }else if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            if (!oldState.IsKeyDown(Keys.W))
            {
                selected_particle = Particle.Water;
            }
        }else if (Keyboard.GetState().IsKeyDown(Keys.G))
        {
            if (!oldState.IsKeyDown(Keys.G))
            {
                selected_particle = Particle.Smoke;
            }
        }else if (Keyboard.GetState().IsKeyDown(Keys.R))
        {
            if (!oldState.IsKeyDown(Keys.R))
            {
                selected_particle = Particle.Rock;
            }
        }
        oldState = newState;
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
                if (particles[i][j] != Particle.Air)
                {
                    tilePosition.Y = i * size_per_pixel;
                    tilePosition.X = j * size_per_pixel;

                    _spriteBatch.Draw(textures[(int)particles[i][j]-1], tilePosition, Color.White);
                }
            }
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    bool check_pos(int x, int y, int dirx, int diry, Dictionary<Vector2, bool> updatable)
    {
        if (x + dirx > 0 && x + dirx < particles[y].Count && y + diry > 0 && y + diry < particles.Count)
        {
            if (!updatable.ContainsKey(new Vector2(x + dirx, y + diry)))
            {
                if (movables[(int)particles[y][x]].Contains(particles[y + diry][x + dirx]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void move(int x, int y, int dirx, int diry)
    {
        Particle particle = particles[y][x];
        particles[y][x] = particles[y + diry][x + dirx];
        particles[y + diry][x + dirx] = particle;
        updatable.Remove(new Vector2(x + dirx, y + diry));
        updatable.Add(new Vector2(x + dirx, y + diry), true);
        if (particles[y][x] != Particle.Air)
        {
            updatable.Remove(new Vector2(x, y));
            updatable.Add(new Vector2(x, y), true);
        }
    }

    void create_texture(Texture2D tile, Color color1, Color color2)
    {
        bool lines = true;
        Color[] texture_color = new Color[size_per_pixel * size_per_pixel];
        for (int i = 0; i < texture_color.Length; i++)
        {
            if (i % size_per_pixel == 0)
            {
                lines = !lines;
            }
            if (lines)
            {
                texture_color[i] = color1;
            }
            else
            {
                texture_color[i] = color2;
            }
        }
        tile.SetData<Color>(texture_color);
        textures.Add(tile);
    }
}

