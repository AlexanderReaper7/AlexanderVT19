using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    class ParticleSystem
    {
        private VertexBuffer _verts;
        private IndexBuffer _ints;

        private GraphicsDevice _graphics;
        private Effect _effect;

        private int _nParticles;
        private Vector2 _particleSize;
        private float _lifespan = 1;
        private Vector3 _wind;
        private Texture2D _texture;
        private float _fadeInTime;

        private ParticleVertex[] _particles;
        private int[] _indices;

        private int _activeStart = 0, _nActive = 0;

        private DateTime _start;

        public ParticleSystem(GraphicsDevice graphicsDevice, ContentManager content, Texture2D texture, int nParticles, Vector2 particleSize, float lifespan, Vector3 wind, float fadeInTime)
        {
            _graphics = graphicsDevice;
            _texture = texture;
            _nParticles = nParticles;
            _particleSize = particleSize;
            _lifespan = lifespan;
            _wind = wind;
            _fadeInTime = fadeInTime;

            // Create vertex and index buffers to accomodate all particles
            _verts = new VertexBuffer(graphicsDevice, typeof(ParticleVertex), nParticles * 4, BufferUsage.WriteOnly);

            _ints = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits,_nParticles * 6, BufferUsage.WriteOnly);

            GenerateParticles();

            _effect = content.Load<Effect>("Effects/ParticleEffect");

            _start = DateTime.Now;
        }

        private void GenerateParticles()
        {
            // Create new particle and index arrays
            _particles = new ParticleVertex[_nParticles * 4];
            _indices = new int[_nParticles * 6];

            Vector3 z = Vector3.Zero;

            int x = 0;

            // initialize particle settings and fill index and vertex arrays
            for (int i = 0; i < _nParticles; i += 4)
            {
                _particles[i + 0] = new ParticleVertex(z, new Vector2(0, 0), z, 0, -10);
                _particles[i + 1] = new ParticleVertex(z, new Vector2(0, 1), z, 0, -10);
                _particles[i + 2] = new ParticleVertex(z, new Vector2(1, 1), z, 0, -10);
                _particles[i + 3] = new ParticleVertex(z, new Vector2(1, 0), z, 0, -10);

                _indices[x++] = i + 0;
                _indices[x++] = i + 3;
                _indices[x++] = i + 2;
                _indices[x++] = i + 2;
                _indices[x++] = i + 1;
                _indices[x++] = i + 0;
            }
        }

        private void AddParticle(Vector3 position, Vector3 direction, float speed)
        {
            // if there are no available particles, give up
            if (_nActive + 4 == _nParticles * 4) return;

            // Determine the index at which this particle should be created
            int index = OffsetIndex(_activeStart, _nActive);
            _nActive += 4;

            // Determine the start time
            float startTime = (float) (DateTime.Now - _start).TotalSeconds;

            // Set the particle settings to each of the particle´s vertices
            for (int i = 0; i < 4; i++)
            {
                _particles[index + i].StartPosition = position;
                _particles[index + i].Direction = direction;
                _particles[index + i].Speed = speed;
                _particles[index + i].StartTime = startTime;
            }
        }

        private int OffsetIndex(int start, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (++start == _particles.Length) start = 0;
            }

            return start;
        }

        public void Update()
        {
            float now = (float) (DateTime.Now - _start).TotalSeconds;

            int startIndex = _activeStart;
            int end = _nActive;

            // Foreach particle marked as active..
            for (int i = 0; i < end; i++)
            {
                // if this particle has gotten older than _lifespan...
                if (_particles[_activeStart].StartTime < now - _lifespan)
                {
                    // Advance the active particle start position past the particle´s index and reduce the number of active particles by 1
                    _activeStart++;
                    _nActive--;

                    if (_activeStart == _particles.Length) _activeStart = 0;
                }
            }

            // Update the vertex and index buffers
            _verts.SetData(_particles);
            _ints.SetData(_indices);
        }

        public void Draw(Matrix view, Matrix projection, Vector3 up, Vector3 right)
        {
            // Set the vertex and index buffer to the graphics card
            _graphics.SetVertexBuffer(_verts);
            _graphics.Indices = _ints;

            // set effect parameters
            _effect.Parameters["ParticleTexture"].SetValue(_texture);
            _effect.Parameters["View"].SetValue(view);
            _effect.Parameters["Projection"].SetValue(projection);
            _effect.Parameters["Time"].SetValue((float)(DateTime.Now - _start).TotalSeconds);
            _effect.Parameters["Lifespan"].SetValue(_lifespan);
            _effect.Parameters["Wind"].SetValue(_wind);
            _effect.Parameters["Size"].SetValue(_particleSize / 2f);
            _effect.Parameters["Up"].SetValue(up);
            _effect.Parameters["Side"].SetValue(right);
            _effect.Parameters["FadeInTime"].SetValue(_fadeInTime);

            // Enable blending render states
            _graphics.BlendState = BlendState.AlphaBlend;
            _graphics.DepthStencilState = DepthStencilState.DepthRead;

            // Apply the effect
            _effect.CurrentTechnique.Passes[0].Apply();

            // Draw the billboards
            _graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,0, _nParticles * 4, 0, _nParticles * 2);

            // Un-set the buffers
            _graphics.SetVertexBuffer(null);
            _graphics.Indices = null;

            // Reset render states
            _graphics.BlendState = BlendState.Opaque;
            _graphics.DepthStencilState = DepthStencilState.Default;
        }
    }
}
