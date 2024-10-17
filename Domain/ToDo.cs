namespace ToDoAPI.Domain
{
    public class ToDo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool Complete { get; set; }

        public void CompleteToDo () {
            Complete = true;
            CompletedDate = DateTime.Now;
        }

        public ToDo() {}

        public ToDo(string name)
        {
            Name = name;
        }
    }
}
