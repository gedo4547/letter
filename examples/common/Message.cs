namespace common
{
    public static class Message
    {
        public const string data_16 = "abc_abc_abc_abc_";
        public const string data_32 = data_16 + data_16;
        public const string data_64 = data_32 + data_32;
        public const string data_128 = data_64 + data_64;
        public const string data_256 = data_128 + data_128;
        public const string data_512 = data_256 + data_256;
        public const string data_1024 = data_512 + data_512;
        public const string data_2048 = data_1024 + data_1024;
        public const string data_4096 = data_2048 + data_2048;
        public const string data_8192 = data_4096 + data_4096;
    }
}