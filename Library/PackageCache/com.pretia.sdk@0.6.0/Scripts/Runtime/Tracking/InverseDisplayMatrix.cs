using UnityEngine;

namespace PretiaArCloud
{
    public class InverseDisplayMatrix
    {
        private Vector3 matX = Vector3.zero;
        private Vector3 matY = Vector3.zero;
        private ScreenOrientation orientation = ScreenOrientation.AutoRotation;
        private float imageWidthInv = 0;
        private float imageHeightInv = 0;

        public void SetImageResolution(float width, float height)
        {
            imageWidthInv = 1.0f / width;
            imageHeightInv = 1.0f / height;
        }

        public void UpdateMatrix(Matrix4x4? displayMatrix)
        {
            if (Screen.orientation != orientation && displayMatrix != null)
            {
                orientation = Screen.orientation;

#if UNITY_ANDROID
                {
                    var mat = displayMatrix.Value;

                    var detInv = 1.0f / (mat[1,1] * mat[0,0] - mat[0,1] * mat[1,0]);

                    // PIx = m00 PSx + m01 (1-PSy) + m02
                    // PIy = m10 PSx + m11 (1-PSy) + m12

                    // PSx = (PIx - m02 - m01 (1-PSy)) / m00
                    // PSy = 1 - (PIy - m12 - m10 PSx) / m11

                    // PSx = (m01 m12 - m01 PIy - m11 m02 + PIx m11) / (m11 m00 - m01 m10)
                    // PSy = (m00 m12 - m00 PIy + m11 m00 - m01 m10 - m02 m10 + PIx m10) / (m11 m00 - m01 m10)

                    var minv00 = mat[1,1] * detInv;
                    var minv01 = -mat[0,1] * detInv;
                    var minv02 = (mat[0,1] * mat[1,2] - mat[1,1] * mat[0,2]) * detInv;
                    matX = new Vector3(minv00, minv01, minv02);

                    var minv10 = mat[1,0] * detInv;
                    var minv11 = -mat[0,0] * detInv;
                    var minv12 = (mat[0,0] * mat[1,2] + mat[1,1] * mat[0,0] - mat[0,1] * mat[1,0] - mat[0,2] * mat[1,0]) * detInv;
                    matY = new Vector3(minv10, minv11, minv12);
                }
#elif UNITY_IOS
                {
                    var mat = displayMatrix.Value.transpose;

                    var detInv = 1.0f / (mat[1,1] * mat[0,0] - mat[0,1] * mat[1,0]);

                    // PIx = m00 PSx + m01 PSy + m02
                    // PIy = m10 PSx + m11 PSy + m12

                    // PSx = (PIx - m02 - m01 PSy) / m00
                    // PSy = (PIy - m12 - m10 PSx) / m11

                    // PSx = (m01 m12 - m01 PIy - m11 m02 + PIx m11) / (m11 m00 - m01 m10)
                    // PSy = (-m00 m12 + m00 PIy + m02 m10 - PIx m10) / (m11 m00 - m01 m10)

                    var minv00 = mat[1,1] * detInv;
                    var minv01 = -mat[0,1] * detInv;
                    var minv02 = (mat[0,1] * mat[1,2] - mat[1,1] * mat[0,2]) * detInv;
                    matX = new Vector3(minv00, minv01, minv02);

                    var minv10 = -mat[1,0] * detInv;
                    var minv11 = mat[0,0] * detInv;
                    var minv12 = (-mat[0,0] * mat[1,2] + mat[0,2] * mat[1,0]) * detInv;
                    matY = new Vector3(minv10, minv11, minv12);
                }
#endif
            }
        }

        public void ComputeScreenPoint(
            float imagePointX, float imagePointY,
            out float screenPointX, out float screenPointY)
        {
            var imagePointNormalized = new Vector3(imagePointX * imageWidthInv, imagePointY * imageHeightInv, 1);

            screenPointX = Vector3.Dot(imagePointNormalized, matX) * Screen.width;
            screenPointY = Vector3.Dot(imagePointNormalized, matY) * Screen.height;
        }
    }
}
