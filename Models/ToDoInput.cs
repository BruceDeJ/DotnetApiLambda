namespace ToDoAPI.Models
{
    public class ToDoInput
    {
        public string Name { get; set; } = string.Empty;
        public bool? Complete { get; set; } = false;
    }
}
