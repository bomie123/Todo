namespace TodoApp.Models.DatabaseModels
{
    public abstract class BaseDataModel 
    {
        public long Id { get; set; }
        public long RowId { get; set; }
        public bool Active { get; set; } = true;
    }
}
