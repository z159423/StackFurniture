// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("pKt3JKCBmuPnvgCDwoHAU17s+8aDIP2JT08kS0ACxV4Zl2AfuqbQsf2qaL7XNGsLw+po6zOsD4o+EaB7GlzcPWCiDAp+cQHv27yznPuIWVjdb+zP3eDr5MdrpWsa4Ozs7Ojt7jthBJgK9RUyENpY5RQbSueOO/+wIuxELdnIEQw2OqnRkVymx39n2pKTi0Ytkq/+nUzKtEELdgMr+ClmiO6UGGC3bx9gIEMnZ6COpybtk4PqDtk5lgwpoEHZVwlKuR3oE/11FrVN6zgoY8I3n4Ljae1xkN4tvm43IW/s4u3db+zn72/s7O1znkqwWEbtDLKOb93rwBDJu5d9JIaFyss9F/8EQqgcT+OSmAbKVtZDC/bLgifL7Mf+qJzkzjE5bu/u7O3s");
        private static int[] order = new int[] { 2,10,6,3,6,6,6,13,11,9,12,12,13,13,14 };
        private static int key = 237;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
