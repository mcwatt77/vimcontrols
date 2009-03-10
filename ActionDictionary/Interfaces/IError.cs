using System;

namespace ActionDictionary.Interfaces
{
    public interface IError
    {
        void Report(Exception e);
    }
}
