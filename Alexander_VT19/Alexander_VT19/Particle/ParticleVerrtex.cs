/*
 * A struct cannot inherit from another struct or class,
 * and it cannot be the base of a class.
 * ... In C#, classes and structs are semantically different.
 * A struct is a value type, while a class is a reference type.
 * //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-structs
 *
 *      En struct kan ej ärva eller bli ärvd.
 *
 * ...difference between reference types and value types
 * ...is that reference types are allocated on the heap and garbage-collected,
 * whereas value types are allocated either on the stack or inline in
 * containing types and deallocated when the stack unwinds or when their containing type gets deallocated.
 *
 * Therefore... value types are in general cheaper than... reference types.
 *
 * ...reference type assignments copy the reference,
 * whereas value type assignments copy the entire value.
 * Therefore, assignments of large reference types are cheaper than assignments of large value types.
 * //https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
 *
 *      structs allokeras på stack och klasser på heap:en, dvs, på en maskin med lite RAM 
 *      så är det bättre att använda klasser då heap:en är dynamiskt allokerad.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alexander_VT19
{
    struct ParticleVertex : IVertexType
    {
        private Vector3 _startPosition;
        private Vector2 _uv;
        private Vector3 _direction;
        private float _speed;
        private float _startTime;
        private VertexDeclaration _vertexDeclaration;

        /// <summary>
        /// Starting position of that particle
        /// </summary>
        public Vector3 StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }

        /// <summary>
        /// UV coordinate, used for texturing and to offset vertex in shader
        /// </summary>
        public Vector2 UV
        {
            get { return _uv; }
            set { _uv = value; }
        }

        /// <summary>
        /// Movement direction of the particle
        /// </summary>
        public Vector3 Direction
        
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// Speed of the particle in units/second
        /// </summary>
        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        /// <summary>
        /// The time since the particle system was created that this particle came into use 
        /// </summary>
        public float StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public ParticleVertex(Vector3 startPosition, Vector2 uv, Vector3 direction, float speed, float startTime) : this()
        {
            _startPosition = startPosition;
            _uv = uv;
            _direction = direction;
            _speed = speed;
            _startTime = startTime;
        }



        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), // Start position
            new VertexElement(12,VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0), // UV coordinates
            new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1), // Movement Direction
            new VertexElement(32, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2), // Movement speed
            new VertexElement(36, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3) // Start time
            );

        VertexDeclaration IVertexType.VertexDeclaration => _vertexDeclaration;
    }
}
