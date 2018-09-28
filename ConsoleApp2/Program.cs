using FakeDI;
using System;
using System.Collections.Generic;
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
            var cat = new Cat();
            cat.Register<IListener,Listener>();
            cat.Register<IControllerActivator,SingletonControllerActivator>();
            cat.Register<IControllerExecutor,ControllerExecutor>();
            cat.Register<IViewRenderer,ViewRenderer>();
            var mvcEngine = new MvcEngine(cat);


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
        /// <summary>
        /// service container
        /// </summary>
        private readonly Cat _cat;

        public MvcEngine(Cat cat)
        {
            _cat = cat;
        }
        public void Start(Uri address)
        {
            while (true)
            {
                Request req = _cat.GetService<IListener>().Listen(address);
                Task.Run(() =>
                {
                    var controller = _cat.GetService<IControllerActivator>().ActiveController(req);
                    var view = _cat.GetService<IControllerExecutor>().ExecuteController(controller);
                    _cat.GetService<IViewRenderer>().RenderView(view);
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



    public class Listener : IListener
    {
        public virtual Request Listen(Uri address)
        {
            // do something
            Console.WriteLine($"executing {nameof(Listen)}...");
            return new Request();
        }
    }

    public class ControllerActivator : IControllerActivator
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

    public class ControllerExecutor : IControllerExecutor
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

    public class ViewRenderer : IViewRenderer
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


    public class SingletonControllerActivator : IControllerActivator
    {
        public Controller ActiveController(Request request)
        {
            Console.WriteLine("use singleton controller activator to active controller");
            return new Controller();

        }
    }





}
