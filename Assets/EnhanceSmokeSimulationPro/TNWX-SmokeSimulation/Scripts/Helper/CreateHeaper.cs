using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace TNWX.Tools
{
    public static class CreateHeaper
    {
        public enum ColorStatus
        {
            none,
            randomColor,
            randomColorFul
        }

        public static Mesh CreateRenderMesh(int _width, int _height)
        {
            Mesh tmp_Mesh = new Mesh();
            float aspect = (float)_width / (float)_height;
            tmp_Mesh.vertices = new[] {
                new Vector3(-1*aspect,-1),new Vector3( 1*aspect,-1),
                new Vector3( 1*aspect, 1),new Vector3(-1*aspect, 1)
            };
            tmp_Mesh.uv = new Vector2[4] {
                    Vector2.zero,Vector2.right,
                    Vector2.one, Vector2.up
            };
            tmp_Mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            return tmp_Mesh;
        }

        public static RenderTexture CreateRenderBuffer(int _width, int _height, RenderTextureFormat _format, string _name = "Buffer")
        {
            RenderTexture tmp_Buffer = new RenderTexture(_width, _height, 0, _format, RenderTextureReadWrite.sRGB)
            {
                name = _name,
                autoGenerateMips = false,
                useMipMap = false,
                filterMode = FilterMode.Trilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            tmp_Buffer.Create();
            tmp_Buffer.Release();
            return tmp_Buffer;
        }

        public static RenderTexture[] CreateDoubleRenderBuffer(int _width, int _height, RenderTextureFormat _format, string _name = "Buffer")
        {
            RenderTexture[] tmp_BufferArray = new RenderTexture[2];
            tmp_BufferArray[0] = CreateRenderBuffer(_width, _height, _format, _name + "0");
            tmp_BufferArray[1] = CreateRenderBuffer(_width, _height, _format, _name + "1");
            return tmp_BufferArray;
        }

        public static void Copy2RenderBuffer(this RenderTexture[] _rts)
        {
            Graphics.Blit(_rts[0],_rts[1]);
        }

        public static void SwapBuffer(this RenderTexture[] _rts)
        {
            RenderTexture tmp = _rts[0];
            _rts[0] = _rts[1];
            _rts[1] = tmp;
        }

        public static void Blit(this Material _mat)
        {
            Graphics.Blit(null,_mat,0);
        }

        public static void Blit(this Material _mat, RenderTexture _dst, RenderTexture _passIn = null)
        {
            Graphics.Blit(_passIn, _dst, _mat, 0);
        }

        public static void BlitAndSwapBuffer(this Material _mat,RenderTexture[] _rts)
        {
            Graphics.Blit(Texture2D.blackTexture, _rts[0], _mat, 0);
            _rts.SwapBuffer();
        }

        public static void SavePicture(Texture2D _tex,string _path)
        {
            byte[] img_Bytes = _tex.EncodeToPNG();
            File.WriteAllBytes(_path + _tex.name, img_Bytes);
        }

        public static Texture2D RenderTex2Texture(RenderTexture _rt)
        {
            Texture2D tmp = new Texture2D(_rt.width,_rt.height,TextureFormat.ARGB32,false,false);
            tmp.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            return tmp;
        }

    }
}
