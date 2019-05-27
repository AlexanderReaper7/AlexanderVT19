using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class QuadRenderer
{

    #region fields

    private VertexPositionTexture[] _vertexData;
    private short[] _indexData;
    private GraphicsDevice _graphicsDevice;

    #endregion

    #region constructors

    public QuadRenderer(GraphicsDevice graphicsDevice)
    {
        this._graphicsDevice = graphicsDevice;

        // texture coordinates semantic not used or needed
        _vertexData = new[]
        {
            new VertexPositionTexture(new Vector3(1, -1, 0), Vector2.One),
            new VertexPositionTexture(new Vector3(-1, -1, 0), Vector2.UnitY),
            new VertexPositionTexture(new Vector3(-1, 1, 0), Vector2.Zero),
            new VertexPositionTexture(new Vector3(1, 1, 0), Vector2.UnitX)
        };

        _indexData = new short[] { 0, 1, 2, 2, 3, 0 };
    }

    #endregion

    #region methods

    public void Render(Effect effect)
    {
        foreach (EffectPass p in effect.CurrentTechnique.Passes)
        {
            p.Apply();
            this.Render();
        }
    }

    private void Render()
    {
        this._graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
            this._vertexData, 0, 4,
            this._indexData, 0, 2);
    }

    #endregion

}