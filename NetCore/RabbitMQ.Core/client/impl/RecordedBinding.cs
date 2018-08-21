using System;
using System.Collections.Generic;

namespace RabbitMQ.Client.Impl
{
    public abstract class RecordedBinding : RecordedEntity
    {
        public RecordedBinding(AutorecoveringModel model) : base(model)
        {
        }

        public IDictionary<string, object> Arguments { get; protected set; }
        public string Destination { get; set; }
        public string RoutingKey { get; protected set; }
        public string Source { get; protected set; }
		
        public bool Equals(RecordedBinding other)
        {
            return other != null && 
                (Source.Equals(other.Source)) &&
                (Destination.Equals(other.Destination)) &&
                (RoutingKey.Equals(other.RoutingKey)) &&
                (Arguments == other.Arguments);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

			var other = obj as RecordedBinding;
			
			return Equals(other);
        }

        public override int GetHashCode()
        {
            return Source.GetHashCode() ^
                   Destination.GetHashCode() ^
                   RoutingKey.GetHashCode() ^
                   (Arguments != null ? Arguments.GetHashCode() : 0);
        }

        public virtual void Recover()
        {
        }

        public override string ToString()
        {
            return String.Format("{0}: source = '{1}', destination = '{2}', routingKey = '{3}', arguments = '{4}'",
                GetType().Name, Source, Destination, RoutingKey, Arguments);
        }

        public RecordedBinding WithArguments(IDictionary<string, object> value)
        {
            Arguments = value;
            return this;
        }

        public RecordedBinding WithDestination(string value)
        {
            Destination = value;
            return this;
        }

        public RecordedBinding WithRoutingKey(string value)
        {
            RoutingKey = value;
            return this;
        }

        public RecordedBinding WithSource(string value)
        {
            Source = value;
            return this;
        }
    }


    public class RecordedQueueBinding : RecordedBinding
    {
        public RecordedQueueBinding(AutorecoveringModel model) : base(model)
        {
        }

        public override void Recover()
        {
            ModelDelegate.QueueBind(Destination, Source, RoutingKey, Arguments);
        }
    }


    public class RecordedExchangeBinding : RecordedBinding
    {
        public RecordedExchangeBinding(AutorecoveringModel model) : base(model)
        {
        }

        public override void Recover()
        {
            ModelDelegate.ExchangeBind(Destination, Source, RoutingKey, Arguments);
        }
    }
}
