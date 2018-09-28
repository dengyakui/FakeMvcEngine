namespace FakeMvcEngine
{
    public interface IControllerExecutor
    {
        View ExecuteController(Controller controller);
    }
}