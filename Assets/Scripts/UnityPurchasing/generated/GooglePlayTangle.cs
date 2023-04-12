// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("JSr2pSEAG2JmP4ECQwBB0t9tekdc7m1OXGFqZUbqJOqbYW1tbWlsb3wr6T9WteqKQmvparItjgu/kCH6jTMP7lxqQZFIOhb8pQcES0q8ln7ubWNsXO5tZm7ubW1s8h/LMdnHbI9YuBeNqCHAWNaIyzicaZJ89Jc0AqF8CM7OpcrBg0TfmBbhnjsnUTCFwymdzmITGYdL11fCindKA6ZKbaNtxaxYSZCNt7soUBDdJ0b+5lsTm91dvOEjjYv/8IBuWj0yHXoJ2NlvFZnhNu6e4aHCpuYhDyanbBICa8xquaniQ7YeA2LobPARX6w/77agEgrHrBMufxzNSzXAiveCqnmo5wm64IUZi3SUs5Fb2WSVmstmD7p+MUZ/KR1lT7C4725vbWxt");
        private static int[] order = new int[] { 2,2,6,7,6,9,10,8,9,9,11,12,13,13,14 };
        private static int key = 108;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
