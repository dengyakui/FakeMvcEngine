using System;

namespace FakeMvcEngine
{
    public interface IListener
    {
        Request Listen(Uri address);
    }
}