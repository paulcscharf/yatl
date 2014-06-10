using amulware.Graphics;
using OpenTK.Graphics.OpenGL;

namespace yatl.Rendering
{
    sealed class DeferredBuffer : SurfaceSetting
    {
        private Texture positionTexture;
        private Texture normalTexture;
        private readonly TextureUniform positionUniform;
        private readonly TextureUniform normalUniform;

        private int depthHandle;

        private readonly RenderTarget target;

        public DeferredBuffer()
        {
            this.positionUniform = new TextureUniform("positionTexture", null, TextureUnit.Texture0);
            this.normalUniform = new TextureUniform("normalTexture", null, TextureUnit.Texture1);

            this.target = new RenderTarget();
        }

        public void Resize(int w, int h)
        {
            if (this.positionTexture != null)
            {
                this.positionTexture.Dispose();
                this.normalTexture.Dispose();
                GL.DeleteRenderbuffer(this.depthHandle);
            }

            this.positionTexture = makeTexture(w, h, PixelInternalFormat.Rgba32f);
            this.normalTexture = makeTexture(w, h, PixelInternalFormat.Rgb);

            this.positionUniform.Texture = this.positionTexture;
            this.normalUniform.Texture = this.normalTexture;
            
            this.target.Attach(FramebufferAttachment.ColorAttachment0,
                this.positionTexture, TextureTarget.Texture2D);
            this.target.Attach(FramebufferAttachment.ColorAttachment1,
                this.normalTexture, TextureTarget.Texture2D);



            this.depthHandle = GL.GenRenderbuffer();

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.depthHandle);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, w, h);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, this.target);
            GL.FramebufferRenderbuffer(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, this.depthHandle);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }

        private static Texture makeTexture(int width, int height, PixelInternalFormat format)
        {
            var texture = new Texture();
            texture.SetParameters(TextureMinFilter.Linear, TextureMagFilter.Linear,
                TextureWrapMode.ClampToBorder, TextureWrapMode.ClampToBorder);
            texture.Resize(width, height, format);
            return texture;
        }

        private static readonly DrawBuffersEnum[] drawBufferBindings =
            { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1 };

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, this.target);
            GL.DrawBuffers(2, DeferredBuffer.drawBufferBindings);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
        }

        public override void Set(ShaderProgram program)
        {
            this.positionUniform.Set(program);
            this.normalUniform.Set(program);
        }
    }
}
