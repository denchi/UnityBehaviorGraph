namespace Behaviours.Graph
{
    [Task(Title = "If")]
    [Input(Title = "Sync", Order = 0)]
    [Input(Title = "Expression", Type = ValueType.Bool, Order = 1)]
    [Output(Title = "OnTrue", Order = 0)]
    [Output(Title = "OnFalse", Order = 1)]
    public class If : Task
    {
        
    }
}