namespace OAuth_Taskk.Helpers
{
    public class Respons<T> where T : class
    {
        public T Data { get; set; }
        public string Error { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPages { get; set; }

    }
}
