namespace FakeMvcEngine
{
    public interface IControllerActivator
    {
        Controller ActiveController(Request request);
    }
}