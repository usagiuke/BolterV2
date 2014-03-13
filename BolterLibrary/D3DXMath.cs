using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BolterLibrary
{
    public class NativeDx : GameInput
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct D3DXMATRIX
        {
            private Single _11;
            private Single _12;
            private Single _13;
            private Single _14;
            private Single _21;
            private Single _22;
            private Single _23;
            private Single _24;
            public Single _31;
            private Single _32;
            public Single _33;
            private Single _34;
            private Single _41;
            private Single _42;
            private Single _43;
            private Single _44;
        }

        [DllImport("d3dx9_43.dll", EntryPoint = "D3DXMatrixRotationY", CallingConvention = CallingConvention.StdCall,
            SetLastError = false)]
        private static extern IntPtr D3DXMatrixRotationY(
             out D3DXMATRIX pOut,
             Single Angle
            );

        [DllImport("d3dx9_43.dll", EntryPoint = "D3DXMatrixTranslation", CallingConvention = CallingConvention.StdCall,
            SetLastError = false)]
        private static extern IntPtr D3DXMatrixTranslation(
             out D3DXMATRIX pOut,
             Single x,
             Single y,
             Single z
            );

        [DllImport("d3dx9_43.dll", EntryPoint = "D3DXMatrixMultiply", CallingConvention = CallingConvention.StdCall,
            SetLastError = false)]
        private static extern IntPtr D3DXMatrixMultiply(
             out D3DXMATRIX pOut,
             ref D3DXMATRIX pM1,
             ref D3DXMATRIX pM2
            );

        protected static D3DXVECTOR2 GetNewVector(float heading)
        {
            D3DXMATRIX updatedMatrix;
            D3DXMATRIX transMatrix;
            D3DXMATRIX yRotationMatrix;

            //Set up the rotation matrix for the player model
            D3DXMatrixRotationY(out yRotationMatrix, heading / 2);

            //Set up the translation matrix 
            D3DXMatrixTranslation(out transMatrix, 0.0f, 0.0f, 0.0f);

            //Combine out matrices
            D3DXMatrixMultiply(out updatedMatrix, ref yRotationMatrix, ref transMatrix);

            //Return new vector for player matrix.
            return new D3DXVECTOR2(updatedMatrix._31, updatedMatrix._33);

        }

    }

    [Serializable]
    public class D3DXVECTOR2
    {
        [XmlAttribute("X")]
        public float x;
        [XmlAttribute("Y")]
        public float y;

        public D3DXVECTOR2()
        {

        }

        public D3DXVECTOR2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

    }
}
