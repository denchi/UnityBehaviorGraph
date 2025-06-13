namespace Behaviours.Graph
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TaskAttribute : System.Attribute
    {
        public string Title;
        public string Category;
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class InputAttribute : System.Attribute
    {
        public string Title;
        public ValueType Type =  ValueType.None;
        public int Order = 1;

        public InputAttribute()
        {
            Type = ValueType.None;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class OutputAttribute : System.Attribute
    {
        public string Title;
        public ValueType Type = ValueType.None;
        public int Order = 1;

        public OutputAttribute()
        {
            Type = ValueType.None;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class FuncAttribute : System.Attribute
    {
        public string Title;
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class EventAttribute : System.Attribute
    {
        public string Title;
    }
}