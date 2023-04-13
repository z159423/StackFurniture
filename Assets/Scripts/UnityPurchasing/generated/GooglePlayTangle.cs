// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("UPYlNX7fKoKf/nTwbI3DMKNzKjwHQcEgfb8RF2NsHPLGoa6B5pVERSZ8GYUX6AgvDcdF+AkGV/qTJuKtjpZbMI+y44BR16lcFmseNuU0e5XziQV9qnICfT1eOnq9k7o78I6e9xGvk3LA9t0N1KaKYDmbmNfWIAriE8QkixE0vVzEShRXpAD1DuBoC6hy8f/wwHLx+vJy8fHwboNXrUVb8OC3daPKKXYW3vd19i6xEpcjDL1mGV+1AVL+j4Ub10vLXhbr1p861vHAcvHSwP32+dp2uHYH/fHx8fXw85494JRSUjlWXR/YQwSKfQKnu82subZqOb2ch/76ox2e35zdTkPx5ts/8VkwxNUMESsntMyMQbvaYnrHj9rjtYH50ywkc/Lz8fDx");
        private static int[] order = new int[] { 12,3,4,13,11,7,9,13,9,9,12,12,12,13,14 };
        private static int key = 240;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
