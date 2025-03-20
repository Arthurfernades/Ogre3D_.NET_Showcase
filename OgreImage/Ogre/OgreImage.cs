using System;
using System.Windows;
using System.Windows.Interop;
using static OgreEngine.Variaveis;

namespace OgreEngine
{
    public class OgreImage : D3DImage
    {
        public bool isRendering = false;

        public void InitOgre()
        {
            try
            {
                myOgre = new Ogre();

                myOgre.initApp(); //Inicialização automática do Ogre (setup).

            }
            catch (Exception ex)
            {
                MessageBox.Show("InitOgre.Erro: " + ex.Message.ToString());
            }
        }

        public void Resize(int width, int height)
        {
            Lock();

            myOgre.TextureHeight = height;
            myOgre.TextureWidth = width;

            myOgre.CreateTexture11();

            AttachRenderTargert();

            Unlock();
        }

        private void AttachRenderTargert()
        {
            if (IsFrontBufferAvailable)
                DetachRenderTarget();

            try
            {
                Lock();

                SetBackBuffer(D3DResourceType.IDirect3DSurface9, myOgre.SharedHandle, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("AttachRenderTargert.Erro: " + ex.Message.ToString());
            }
            finally
            {
                Unlock();
            }
        }

        private void DetachRenderTarget()
        {
            try
            {
                Lock();

                SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("DetachRenderTarget.Erro: " + ex.Message.ToString());
            }
            finally
            {
                Unlock();
            }
        }

        /// <summary>
        /// Renderização manual do Ogre.
        /// </summary>
        public void RenderOneFrame()
        {
            try
            {
                Lock();

                myOgre.root.renderOneFrame();

                myOgre.renderTarget.doFlush();

                AddDirtyRect(new Int32Rect(0, 0, myOgre.TextureWidth, myOgre.TextureHeight));
            }
            catch (Exception ex)
            {
                MessageBox.Show("RenderOneFrame.Erro: " + ex.Message.ToString());
            }
            finally
            {
                Unlock();
            }
        }

        public void DisposeOgre()
        {
            try
            {
                myOgre.DisposeOgre();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DisposeOgre.Erro: " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// Escreve o arquivo no disco na caminho passado.
        /// </summary>
        /// <param name="vArquivo"></param>
        public void SalvaRender(string vArquivo)
        {
            try
            {
                myOgre.SalvaRender(vArquivo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("SalvaRender.Erro: " + ex.Message.ToString());
            }
        }
    }
}
