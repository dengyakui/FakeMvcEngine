using System;
using System.Threading;
using System.Threading.Tasks;

namespace FakeMvcEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Uri address = new Uri("http://localhost/mvcapp");
            var mvcEngine = new MvcEngine(new FoobarEngineFactory());


            //// customise controller activation
            //mvcEngine.ControllerActivating += (sender, activationArgs) =>
            //{
            //    Console.WriteLine($"custom logic in {nameof(mvcEngine.ControllerActivating)}");
            //};
            //mvcEngine.ControllerActivated += (sender, activationArgs) =>
            //{
            //    Console.WriteLine($"custom logic in {nameof(mvcEngine.ControllerActivated)}");
            //};

            //// customise controller exectuion
            //mvcEngine.ControllerExecuting += (sender, executionArgs) => { Console.WriteLine($"custom logic in {nameof(mvcEngine.ControllerExecuting)}"); };
            //mvcEngine.ControllerExecuted += (sender, executionArgs) =>
            //{
            //    Console.WriteLine($"custom logic in {nameof(mvcEngine.ControllerExecuted)}");
            //};

            //// customise view render
            //mvcEngine.ViewRendering += (sender, renderArgs) =>
            //{
            //    Console.WriteLine($"custom logic in {nameof(mvcEngine.ViewRendering)}");
            //};
            //mvcEngine.ViewRendered += (sender, renderArgs) => { Console.WriteLine($"custom logic in {nameof(mvcEngine.ViewRendered)}"); };

            mvcEngine.Start(address);
        }

    }



    public class View
    {
    }

    public class Controller
    {
    }

    public class Request
    {
    }

    public class MvcEngine
    {
        private readonly EngineFactory _factory;

        public MvcEngine(EngineFactory factory = null)
        {
            _factory = factory?? new EngineFactory();
        }
        public void Start(Uri address)
        {
            while (true)
            {
                Request req = _factory.GetListener().Listen(address);
                Task.Run(() =>
                {
                    var controller = _factory.GetControllerActivator().ActiveController(req);
                    var view = _factory.GetControllerExecutor().ExecuteController(controller);
                    _factory.GetViewRenderer().RenderView(view);
                });
                Thread.Sleep(5000);
            }
        }




    }

    public class ViewRenderArgs
    {
    }

    public class ControllerExecutionArgs
    {
    }

    public class ControllerActivationArgs
    {
    }

    public class FoobarEngineFactory : EngineFactory
    {
        public override ControllerActivator GetControllerActivator()
        {
            return new SingletonControllerActivator();
        }
    }

    public class Listener
    {
        public virtual Request Listen(Uri address)
        {
            // do something
            Console.WriteLine($"executing {nameof(Listen)}...");
            return new Request();
        }
    }

    public class ControllerActivator
    {
        public virtual Controller ActiveController(Request request)
        {
            // before logic
            this.ControllerActivating?.Invoke(new { }, new ControllerActivationArgs());

            // do something
            Console.WriteLine($"executing {nameof(ActiveController)}...");

            // after logic
            this.ControllerActivated?.Invoke(new { }, new ControllerActivationArgs());

            return new Controller();
        }

        // before controller activation
        public EventHandler<ControllerActivationArgs> ControllerActivating;

        // after controller activation
        public EventHandler<ControllerActivationArgs> ControllerActivated;

    }

    public class ControllerExecutor
    {
        public virtual View ExecuteController(Controller controller)
        {

            // before logic
            this.ControllerExecuting?.Invoke(new { }, new ControllerExecutionArgs());

            // do something
            Console.WriteLine($"executing {nameof(ExecuteController)}...");

            // after logic
            this.ControllerExecuted?.Invoke(new { }, new ControllerExecutionArgs());

            return new View();
        }

        // before controller execution
        public EventHandler<ControllerExecutionArgs> ControllerExecuting;

        // after controller execution
        public EventHandler<ControllerExecutionArgs> ControllerExecuted;


    }

    public class ViewRenderer
    {
        public virtual void RenderView(View view)
        {
            // before logic
            this.ViewRendering?.Invoke(new { }, new ViewRenderArgs());

            // do something
            Console.WriteLine($"executing {nameof(RenderView)}...");

            // after logic
            this.ViewRendered?.Invoke(new { }, new ViewRenderArgs());
        }

        // before view render
        public EventHandler<ViewRenderArgs> ViewRendering;

        // after view render
        public EventHandler<ViewRenderArgs> ViewRendered;
    }


    public class SingletonControllerActivator : ControllerActivator
    {
        public override Controller ActiveController(Request request)
        {
            Console.WriteLine("use singleton controller activator to active controller");
            return new Controller();

        }
    }

    public class EngineFactory
    {
        public virtual Listener GetListener()
        {

            return new Listener();
        }

        public virtual ControllerActivator GetControllerActivator()
        {
            return new ControllerActivator();
        }

        public virtual ControllerExecutor GetControllerExecutor()
        {
            return new ControllerExecutor();
        }

        public virtual ViewRenderer GetViewRenderer()
        {
            return new ViewRenderer();
        }
    }
}
