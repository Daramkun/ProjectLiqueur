using System;

namespace Daramkun.Liqueur.IO.Compression
{
    static class ZlibConstants
    {
        public const int WindowBitsMax = 15;
        public const int WindowBitsDefault = WindowBitsMax;

        public const int Z_OK = 0;
        public const int Z_STREAM_END = 1;
        public const int Z_NEED_DICT = 2;
        public const int Z_STREAM_ERROR = -2;
        public const int Z_DATA_ERROR = -3;
        public const int Z_BUF_ERROR = -5;

#if NETCF        
        public const int WorkingBufferSizeDefault = 8192;
#else
        public const int WorkingBufferSizeDefault = 16384; 
#endif
        public const int WorkingBufferSizeMin = 1024;
    }

}
