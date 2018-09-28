using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;

namespace FakeDI
{
    class Program
    {
        static void Main(string[] args)
        {
            Cat cat = new Cat();
            cat.Register<IFoo, Foo>();
            cat.Register<IBar, Bar>();
            cat.Register<IBaz, Baz>();
            cat.Register<IQux, Qux>();


            var service = cat.GetService<IFoo>();
            Foo foo = (Foo)service;
            Baz baz = (Baz)foo.Baz;

            Console.WriteLine($"cat.GetService<IFoo>(): {service}");
            Console.WriteLine($"cat.GetService<IFoo>().Bar: {foo.Bar}");
            Console.WriteLine($"cat.GetService<IFoo>().Baz: {foo.Baz}");
            Console.WriteLine($"cat.GetService<IFoo>().Baz.Qux: {baz.Qux}");

            Console.WriteLine("Hello World!");
        }
    }

    public interface IFoo { }
    public interface IBar { }
    public interface IBaz { }
    public interface IQux { }


    public class Foo : IFoo
    {
        public IBar Bar { get; }



        [Injection]
        public IBaz Baz { get; set; }

        public Foo()
        {

        }

        [Injection]
        public Foo(IBar bar)
        {
            this.Bar = bar;
        }
    }

    public class Bar : IBar { }

    public class Baz : IBaz
    {
        public IQux Qux { get; private set; }

        [Injection]
        public void Initilize(IQux qux)
        {
            this.Qux = qux;
        }
    }

    public class Qux : IQux { }

    public class InjectionAttribute : Attribute { }


    public class Cat
    {
        private ConcurrentDictionary<Type, Type> typeMapping = new ConcurrentDictionary<Type, Type>();

        public void Register(Type from, Type to)
        {
            typeMapping[from] = to;
        }

        public void Register<TFrom, TTo>()
        {
            this.Register(typeof(TFrom),typeof(TTo));
        }
        public object GetService(Type servieType)
        {
            Type type;
            if (!typeMapping.TryGetValue(servieType, out type))
            {
                type = servieType;
            }

            if (type.IsInterface || type.IsAbstract)
            {
                return null;
            }

            ConstructorInfo constructor = this.GetConstructor(type);

            if (constructor == null)
            {
                return null;
            }

            object[] arguments = constructor.GetParameters().Select(p => this.GetService(p.ParameterType)).ToArray();
            object service = constructor.Invoke(arguments);
            InitializeInjectedProperties(service);
            InvokeInjectedMethods(service);

            return service;


        }

        public T GetService<T>() where T : class
        {
            return this.GetService(typeof(T)) as T;
        }
        protected virtual ConstructorInfo GetConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            return constructors.FirstOrDefault(c => c.GetCustomAttribute<InjectionAttribute>() != null) ?? constructors.FirstOrDefault();
        }

        protected virtual void InitializeInjectedProperties(object service)
        {
            PropertyInfo[] properties = service.GetType().GetProperties()
                .Where(p => p.CanWrite && p.GetCustomAttribute<InjectionAttribute>() != null).ToArray();

            Array.ForEach(properties, p => p.SetValue(service, this.GetService(p.PropertyType)));
        }

        protected virtual void InvokeInjectedMethods(object service)
        {
            MethodInfo[] methods = service.GetType().GetMethods().Where(m => m.GetCustomAttribute<InjectionAttribute>() != null).ToArray();
            Array.ForEach(methods, m =>
            {
                object[] arguments = m.GetParameters().Select(p => this.GetService(p.ParameterType)).ToArray();
                m.Invoke(service, arguments);
            });
        }
    }
}
