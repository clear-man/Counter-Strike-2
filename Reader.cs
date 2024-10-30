using System.Numerics;
using Swed64;

namespace CounterStrike2
{
    public class Reader
    {
        Swed swed;
        public Reader(Swed swed) { this.swed = swed; }

        public List<Vector3> ReadBones(IntPtr boneAddress)
        {
            byte[] boneBytes = Swed.ReadBytes(boneAddress, 27 * 32 + 16);
            List<Vector3> bones = new List<Vector3>();
            foreach (var boneId in Enum.GetValues(typeof(BoneIds)))
            {
                float x = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 0);
                float y = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 4);
                float z = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 8);
                Vector3 currentBone = new Vector3(x, y, z);
                bones.Add(currentBone);
            }
            return bones;
        }
        public List<Vector2> ReadBones2d(List<Vector3> bones, viewMatrix viewmatrix, Vector2 screenSize)
        {
            List<Vector2> bones2d = new List<Vector2>();
            foreach (Vector3 bone in bones)
            {
                Vector2 bone2d = Calculate.WorldToScreen2(viewmatrix, bone, screenSize);
                bones2d.Add(bone2d);
            }
            return bones2d;
        }

        public viewMatrix ReadMatrix(IntPtr matrixAddress)
        {
            var viewmatrix = new viewMatrix();
            var matrix = swed.ReadMatrix(matrixAddress);
            viewmatrix.m11 = matrix[0];
            viewmatrix.m12 = matrix[1];
            viewmatrix.m13 = matrix[2];
            viewmatrix.m14 = matrix[3];

            viewmatrix.m21 = matrix[4];
            viewmatrix.m22 = matrix[5];
            viewmatrix.m23 = matrix[6];
            viewmatrix.m24 = matrix[7];

            viewmatrix.m31 = matrix[8];
            viewmatrix.m32 = matrix[9];
            viewmatrix.m33 = matrix[10];
            viewmatrix.m34 = matrix[11];

            viewmatrix.m41 = matrix[12];
            viewmatrix.m42 = matrix[13];
            viewmatrix.m43 = matrix[14];
            viewmatrix.m44 = matrix[15];

            return viewmatrix;
        }
    }
}