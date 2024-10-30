using System.Numerics;

namespace CounterStrike2
{
    public static class Calculate
    {
        public static Vector2 WorldToScreen(float[] matrix, Vector3 pos, Vector2 windowSize){
            //calcula tela W
            float screenW = (matrix[12] * pos.X) + (matrix[13] * pos.Y) + (matrix[14] * pos.Z) + matrix[15];

            //se Entity está na frente da câmera
            if(screenW > 0.001f){
                //calcula x e y da tela
                float screenX = (matrix[0] * pos.X) + (matrix[1] * pos.Y) + (matrix[2] * pos.Z) + matrix[3];
                float screenY = (matrix[4] * pos.X) + (matrix[5] * pos.Y) + (matrix[6] * pos.Z) + matrix[7];

                //divisão de perspectiva
                float X = (windowSize.X / 2) + (windowSize.X / 2) * screenX / screenW;
                float Y = (windowSize.Y / 2) - (windowSize.Y / 2) * screenY / screenW;

                //retorna coordenadas
                return new Vector2(X, Y);
            }
            else{
                //retorna -99, -99 se Entity está atrás da câmera
                return new Vector2(-99, -99);
            }
        }

        public static Vector2 WorldToScreen2(viewMatrix matrix, Vector3 pos, Vector2 windowSize){
            //calcula tela W
            float screenW = (matrix.m41 * pos.X) + (matrix.m42 * pos.Y) + (matrix.m43 * pos.Z) + matrix.m44;

            //se Entity está na frente da câmera
            if(screenW > 0.001f){
                //calcula x e y da tela
                float screenX = (matrix.m11 * pos.X) + (matrix.m12 * pos.Y) + (matrix.m13 * pos.Z) + matrix.m14;
                float screenY = (matrix.m21 * pos.X) + (matrix.m22 * pos.Y) + (matrix.m23 * pos.Z) + matrix.m24;

                //divisão de perspectiva
                float X = (windowSize.X / 2) + (windowSize.X / 2) * screenX / screenW;
                float Y = (windowSize.Y / 2) - (windowSize.Y / 2) * screenY / screenW;

                //retorna coordenadas
                return new Vector2(X, Y);
            }
            else{
                //retorna -99, -99 se Entity está atrás da câmera
                return new Vector2(-99, -99);
            }
        }

        public static Vector2 CalculateAngles(Vector3 from, Vector3 destination){
            float yaw;
            float pitch;

            float deltaX = destination.X - from.X;
            float deltaY = destination.Y - from.Y;
            yaw = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI);

            float deltaZ = destination.Z - from.Z;
            double distance = Math.Sqrt(Math.Pow(deltaX,2) + Math.Pow(deltaY,2));
            pitch = -(float)(Math.Atan2(deltaZ, distance) * 180 / Math.PI);

            return new Vector2(yaw, pitch);
        }

        public static float CalculatePixelDistance(Vector2 v1, Vector2 v2){

            return (float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2));
        }

        /* public static float CalculateMagnitude(Vector3 v1, Vector3 v2){
            return (float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2) + Math.Pow(v2.Z - v1.Z, 2));
        } */
    }
}   