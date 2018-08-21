namespace RabbitMQ.Client.Impl
{
    public abstract class RecordedEntity
    {
        public RecordedEntity(AutorecoveringModel model)
        {
            Model = model;
        }

        public AutorecoveringModel Model { get; protected set; }

        protected IModel ModelDelegate
        {
            get { return Model.Delegate; }
        }
    }
}
