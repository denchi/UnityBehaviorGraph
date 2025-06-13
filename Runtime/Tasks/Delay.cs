namespace Behaviours.Graph
{
    [Task(Title = "Delay")]
    [Input(Title = "Sync")]
    [Output(Title = "Sync")]
    public class Delay : Task
    {
        public float time;
    }
}