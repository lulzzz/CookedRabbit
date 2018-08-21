using RabbitMQ.Client.Exceptions;

namespace RabbitMQ.Client.Framing
{
    public class Protocol : Impl.ProtocolBase
    {
        ///<summary>Protocol major version (= 0)</summary>
        public override int MajorVersion { get { return 0; } }
        ///<summary>Protocol minor version (= 9)</summary>
        public override int MinorVersion { get { return 9; } }
        ///<summary>Protocol revision (= 1)</summary>
        public override int Revision { get { return 1; } }
        ///<summary>Protocol API name (= :AMQP_0_9_1)</summary>
        public override string ApiName { get { return ":AMQP_0_9_1"; } }
        ///<summary>Default TCP port (= 5672)</summary>
        public override int DefaultPort { get { return 5672; } }

        public override Client.Impl.MethodBase DecodeMethodFrom(Util.NetworkBinaryReader reader)
        {
            ushort classId = reader.ReadUInt16();
            ushort methodId = reader.ReadUInt16();

            switch (classId)
            {
                case 10:
                    {
                        switch (methodId)
                        {
                            case 10:
                                {
                                    Impl.ConnectionStart result = new Impl.ConnectionStart();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 11:
                                {
                                    Impl.ConnectionStartOk result = new Impl.ConnectionStartOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 20:
                                {
                                    Impl.ConnectionSecure result = new Impl.ConnectionSecure();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 21:
                                {
                                    Impl.ConnectionSecureOk result = new Impl.ConnectionSecureOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 30:
                                {
                                    Impl.ConnectionTune result = new Impl.ConnectionTune();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 31:
                                {
                                    Impl.ConnectionTuneOk result = new Impl.ConnectionTuneOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 40:
                                {
                                    Impl.ConnectionOpen result = new Impl.ConnectionOpen();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 41:
                                {
                                    Impl.ConnectionOpenOk result = new Impl.ConnectionOpenOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 50:
                                {
                                    Impl.ConnectionClose result = new Impl.ConnectionClose();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 51:
                                {
                                    Impl.ConnectionCloseOk result = new Impl.ConnectionCloseOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 60:
                                {
                                    Impl.ConnectionBlocked result = new Impl.ConnectionBlocked();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 61:
                                {
                                    Impl.ConnectionUnblocked result = new Impl.ConnectionUnblocked();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            default: break;
                        }
                        break;
                    }
                case 20:
                    {
                        switch (methodId)
                        {
                            case 10:
                                {
                                    Impl.ChannelOpen result = new Impl.ChannelOpen();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 11:
                                {
                                    Impl.ChannelOpenOk result = new Impl.ChannelOpenOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 20:
                                {
                                    Impl.ChannelFlow result = new Impl.ChannelFlow();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 21:
                                {
                                    Impl.ChannelFlowOk result = new Impl.ChannelFlowOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 40:
                                {
                                    Impl.ChannelClose result = new Impl.ChannelClose();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 41:
                                {
                                    Impl.ChannelCloseOk result = new Impl.ChannelCloseOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            default: break;
                        }
                        break;
                    }
                case 40:
                    {
                        switch (methodId)
                        {
                            case 10:
                                {
                                    Impl.ExchangeDeclare result = new Impl.ExchangeDeclare();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 11:
                                {
                                    Impl.ExchangeDeclareOk result = new Impl.ExchangeDeclareOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 20:
                                {
                                    Impl.ExchangeDelete result = new Impl.ExchangeDelete();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 21:
                                {
                                    Impl.ExchangeDeleteOk result = new Impl.ExchangeDeleteOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 30:
                                {
                                    Impl.ExchangeBind result = new Impl.ExchangeBind();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 31:
                                {
                                    Impl.ExchangeBindOk result = new Impl.ExchangeBindOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 40:
                                {
                                    Impl.ExchangeUnbind result = new Impl.ExchangeUnbind();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 51:
                                {
                                    Impl.ExchangeUnbindOk result = new Impl.ExchangeUnbindOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            default: break;
                        }
                        break;
                    }
                case 50:
                    {
                        switch (methodId)
                        {
                            case 10:
                                {
                                    Impl.QueueDeclare result = new Impl.QueueDeclare();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 11:
                                {
                                    Impl.QueueDeclareOk result = new Impl.QueueDeclareOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 20:
                                {
                                    Impl.QueueBind result = new Impl.QueueBind();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 21:
                                {
                                    Impl.QueueBindOk result = new Impl.QueueBindOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 50:
                                {
                                    Impl.QueueUnbind result = new Impl.QueueUnbind();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 51:
                                {
                                    Impl.QueueUnbindOk result = new Impl.QueueUnbindOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 30:
                                {
                                    Impl.QueuePurge result = new Impl.QueuePurge();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 31:
                                {
                                    Impl.QueuePurgeOk result = new Impl.QueuePurgeOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 40:
                                {
                                    Impl.QueueDelete result = new Impl.QueueDelete();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 41:
                                {
                                    Impl.QueueDeleteOk result = new Impl.QueueDeleteOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            default: break;
                        }
                        break;
                    }
                case 60:
                    {
                        switch (methodId)
                        {
                            case 10:
                                {
                                    Impl.BasicQos result = new Impl.BasicQos();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 11:
                                {
                                    Impl.BasicQosOk result = new Impl.BasicQosOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 20:
                                {
                                    Impl.BasicConsume result = new Impl.BasicConsume();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 21:
                                {
                                    Impl.BasicConsumeOk result = new Impl.BasicConsumeOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 30:
                                {
                                    Impl.BasicCancel result = new Impl.BasicCancel();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 31:
                                {
                                    Impl.BasicCancelOk result = new Impl.BasicCancelOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 40:
                                {
                                    Impl.BasicPublish result = new Impl.BasicPublish();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 50:
                                {
                                    Impl.BasicReturn result = new Impl.BasicReturn();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 60:
                                {
                                    Impl.BasicDeliver result = new Impl.BasicDeliver();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 70:
                                {
                                    Impl.BasicGet result = new Impl.BasicGet();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 71:
                                {
                                    Impl.BasicGetOk result = new Impl.BasicGetOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 72:
                                {
                                    Impl.BasicGetEmpty result = new Impl.BasicGetEmpty();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 80:
                                {
                                    Impl.BasicAck result = new Impl.BasicAck();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 90:
                                {
                                    Impl.BasicReject result = new Impl.BasicReject();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 100:
                                {
                                    Impl.BasicRecoverAsync result = new Impl.BasicRecoverAsync();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 110:
                                {
                                    Impl.BasicRecover result = new Impl.BasicRecover();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 111:
                                {
                                    Impl.BasicRecoverOk result = new Impl.BasicRecoverOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 120:
                                {
                                    Impl.BasicNack result = new Impl.BasicNack();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            default: break;
                        }
                        break;
                    }
                case 90:
                    {
                        switch (methodId)
                        {
                            case 10:
                                {
                                    Impl.TxSelect result = new Impl.TxSelect();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 11:
                                {
                                    Impl.TxSelectOk result = new Impl.TxSelectOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 20:
                                {
                                    Impl.TxCommit result = new Impl.TxCommit();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 21:
                                {
                                    Impl.TxCommitOk result = new Impl.TxCommitOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 30:
                                {
                                    Impl.TxRollback result = new Impl.TxRollback();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 31:
                                {
                                    Impl.TxRollbackOk result = new Impl.TxRollbackOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            default: break;
                        }
                        break;
                    }
                case 85:
                    {
                        switch (methodId)
                        {
                            case 10:
                                {
                                    Impl.ConfirmSelect result = new Impl.ConfirmSelect();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            case 11:
                                {
                                    Impl.ConfirmSelectOk result = new Impl.ConfirmSelectOk();
                                    result.ReadArgumentsFrom(new Client.Impl.MethodArgumentReader(reader));
                                    return result;
                                }
                            default: break;
                        }
                        break;
                    }
                default: break;
            }
            throw new Client.Impl.UnknownClassOrMethodException(classId, methodId);
        }

        public override Client.Impl.ContentHeaderBase DecodeContentHeaderFrom(Util.NetworkBinaryReader reader)
        {
            ushort classId = reader.ReadUInt16();

            switch (classId)
            {
                case 60: return new BasicProperties();
                default: break;
            }
            throw new Client.Impl.UnknownClassOrMethodException(classId, 0);
        }
    }
    public class Constants
    {
        ///<summary>(= 1)</summary>
        public const int FrameMethod = 1;
        ///<summary>(= 2)</summary>
        public const int FrameHeader = 2;
        ///<summary>(= 3)</summary>
        public const int FrameBody = 3;
        ///<summary>(= 8)</summary>
        public const int FrameHeartbeat = 8;
        ///<summary>(= 4096)</summary>
        public const int FrameMinSize = 4096;
        ///<summary>(= 206)</summary>
        public const int FrameEnd = 206;
        ///<summary>(= 200)</summary>
        public const int ReplySuccess = 200;
        ///<summary>(= 311)</summary>
        public const int ContentTooLarge = 311;
        ///<summary>(= 313)</summary>
        public const int NoConsumers = 313;
        ///<summary>(= 320)</summary>
        public const int ConnectionForced = 320;
        ///<summary>(= 402)</summary>
        public const int InvalidPath = 402;
        ///<summary>(= 403)</summary>
        public const int AccessRefused = 403;
        ///<summary>(= 404)</summary>
        public const int NotFound = 404;
        ///<summary>(= 405)</summary>
        public const int ResourceLocked = 405;
        ///<summary>(= 406)</summary>
        public const int PreconditionFailed = 406;
        ///<summary>(= 501)</summary>
        public const int FrameError = 501;
        ///<summary>(= 502)</summary>
        public const int SyntaxError = 502;
        ///<summary>(= 503)</summary>
        public const int CommandInvalid = 503;
        ///<summary>(= 504)</summary>
        public const int ChannelError = 504;
        ///<summary>(= 505)</summary>
        public const int UnexpectedFrame = 505;
        ///<summary>(= 506)</summary>
        public const int ResourceError = 506;
        ///<summary>(= 530)</summary>
        public const int NotAllowed = 530;
        ///<summary>(= 540)</summary>
        public const int NotImplemented = 540;
        ///<summary>(= 541)</summary>
        public const int InternalError = 541;
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.start".</summary>
    public interface IConnectionStart : IMethod
    {
        byte VersionMajor { get; }
        byte VersionMinor { get; }
        System.Collections.Generic.IDictionary<string, object> ServerProperties { get; }
        byte[] Mechanisms { get; }
        byte[] Locales { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.start-ok".</summary>
    public interface IConnectionStartOk : IMethod
    {
        System.Collections.Generic.IDictionary<string, object> ClientProperties { get; }
        string Mechanism { get; }
        byte[] Response { get; }
        string Locale { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.secure".</summary>
    public interface IConnectionSecure : IMethod
    {
        byte[] Challenge { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.secure-ok".</summary>
    public interface IConnectionSecureOk : IMethod
    {
        byte[] Response { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.tune".</summary>
    public interface IConnectionTune : IMethod
    {
        ushort ChannelMax { get; }
        uint FrameMax { get; }
        ushort Heartbeat { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.tune-ok".</summary>
    public interface IConnectionTuneOk : IMethod
    {
        ushort ChannelMax { get; }
        uint FrameMax { get; }
        ushort Heartbeat { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.open".</summary>
    public interface IConnectionOpen : IMethod
    {
        string VirtualHost { get; }
        string Reserved1 { get; }
        bool Reserved2 { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.open-ok".</summary>
    public interface IConnectionOpenOk : IMethod
    {
        string Reserved1 { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.close".</summary>
    public interface IConnectionClose : IMethod
    {
        ushort ReplyCode { get; }
        string ReplyText { get; }
        ushort ClassId { get; }
        ushort MethodId { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.close-ok".</summary>
    public interface IConnectionCloseOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.blocked".</summary>
    public interface IConnectionBlocked : IMethod
    {
        string Reason { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "connection.unblocked".</summary>
    public interface IConnectionUnblocked : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "channel.open".</summary>
    public interface IChannelOpen : IMethod
    {
        string Reserved1 { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "channel.open-ok".</summary>
    public interface IChannelOpenOk : IMethod
    {
        byte[] Reserved1 { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "channel.flow".</summary>
    public interface IChannelFlow : IMethod
    {
        bool Active { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "channel.flow-ok".</summary>
    public interface IChannelFlowOk : IMethod
    {
        bool Active { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "channel.close".</summary>
    public interface IChannelClose : IMethod
    {
        ushort ReplyCode { get; }
        string ReplyText { get; }
        ushort ClassId { get; }
        ushort MethodId { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "channel.close-ok".</summary>
    public interface IChannelCloseOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "exchange.declare".</summary>
    public interface IExchangeDeclare : IMethod
    {
        ushort Reserved1 { get; }
        string Exchange { get; }
        string Type { get; }
        bool Passive { get; }
        bool Durable { get; }
        bool AutoDelete { get; }
        bool Internal { get; }
        bool Nowait { get; }
        System.Collections.Generic.IDictionary<string, object> Arguments { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "exchange.declare-ok".</summary>
    public interface IExchangeDeclareOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "exchange.delete".</summary>
    public interface IExchangeDelete : IMethod
    {
        ushort Reserved1 { get; }
        string Exchange { get; }
        bool IfUnused { get; }
        bool Nowait { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "exchange.delete-ok".</summary>
    public interface IExchangeDeleteOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "exchange.bind".</summary>
    public interface IExchangeBind : IMethod
    {
        ushort Reserved1 { get; }
        string Destination { get; }
        string Source { get; }
        string RoutingKey { get; }
        bool Nowait { get; }
        System.Collections.Generic.IDictionary<string, object> Arguments { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "exchange.bind-ok".</summary>
    public interface IExchangeBindOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "exchange.unbind".</summary>
    public interface IExchangeUnbind : IMethod
    {
        ushort Reserved1 { get; }
        string Destination { get; }
        string Source { get; }
        string RoutingKey { get; }
        bool Nowait { get; }
        System.Collections.Generic.IDictionary<string, object> Arguments { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "exchange.unbind-ok".</summary>
    public interface IExchangeUnbindOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "queue.declare".</summary>
    public interface IQueueDeclare : IMethod
    {
        ushort Reserved1 { get; }
        string Queue { get; }
        bool Passive { get; }
        bool Durable { get; }
        bool Exclusive { get; }
        bool AutoDelete { get; }
        bool Nowait { get; }
        System.Collections.Generic.IDictionary<string, object> Arguments { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "queue.declare-ok".</summary>
    public interface IQueueDeclareOk : IMethod
    {
        string Queue { get; }
        uint MessageCount { get; }
        uint ConsumerCount { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "queue.bind".</summary>
    public interface IQueueBind : IMethod
    {
        ushort Reserved1 { get; }
        string Queue { get; }
        string Exchange { get; }
        string RoutingKey { get; }
        bool Nowait { get; }
        System.Collections.Generic.IDictionary<string, object> Arguments { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "queue.bind-ok".</summary>
    public interface IQueueBindOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "queue.unbind".</summary>
    public interface IQueueUnbind : IMethod
    {
        ushort Reserved1 { get; }
        string Queue { get; }
        string Exchange { get; }
        string RoutingKey { get; }
        System.Collections.Generic.IDictionary<string, object> Arguments { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "queue.unbind-ok".</summary>
    public interface IQueueUnbindOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "queue.purge".</summary>
    public interface IQueuePurge : IMethod
    {
        ushort Reserved1 { get; }
        string Queue { get; }
        bool Nowait { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "queue.purge-ok".</summary>
    public interface IQueuePurgeOk : IMethod
    {
        uint MessageCount { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "queue.delete".</summary>
    public interface IQueueDelete : IMethod
    {
        ushort Reserved1 { get; }
        string Queue { get; }
        bool IfUnused { get; }
        bool IfEmpty { get; }
        bool Nowait { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "queue.delete-ok".</summary>
    public interface IQueueDeleteOk : IMethod
    {
        uint MessageCount { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.qos".</summary>
    public interface IBasicQos : IMethod
    {
        uint PrefetchSize { get; }
        ushort PrefetchCount { get; }
        bool Global { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.qos-ok".</summary>
    public interface IBasicQosOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.consume".</summary>
    public interface IBasicConsume : IMethod
    {
        ushort Reserved1 { get; }
        string Queue { get; }
        string ConsumerTag { get; }
        bool NoLocal { get; }
        bool NoAck { get; }
        bool Exclusive { get; }
        bool Nowait { get; }
        System.Collections.Generic.IDictionary<string, object> Arguments { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.consume-ok".</summary>
    public interface IBasicConsumeOk : IMethod
    {
        string ConsumerTag { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.cancel".</summary>
    public interface IBasicCancel : IMethod
    {
        string ConsumerTag { get; }
        bool Nowait { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.cancel-ok".</summary>
    public interface IBasicCancelOk : IMethod
    {
        string ConsumerTag { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.publish".</summary>
    public interface IBasicPublish : IMethod
    {
        ushort Reserved1 { get; }
        string Exchange { get; }
        string RoutingKey { get; }
        bool Mandatory { get; }
        bool Immediate { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.return".</summary>
    public interface IBasicReturn : IMethod
    {
        ushort ReplyCode { get; }
        string ReplyText { get; }
        string Exchange { get; }
        string RoutingKey { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.deliver".</summary>
    public interface IBasicDeliver : IMethod
    {
        string ConsumerTag { get; }
        ulong DeliveryTag { get; }
        bool Redelivered { get; }
        string Exchange { get; }
        string RoutingKey { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.get".</summary>
    public interface IBasicGet : IMethod
    {
        ushort Reserved1 { get; }
        string Queue { get; }
        bool NoAck { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.get-ok".</summary>
    public interface IBasicGetOk : IMethod
    {
        ulong DeliveryTag { get; }
        bool Redelivered { get; }
        string Exchange { get; }
        string RoutingKey { get; }
        uint MessageCount { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.get-empty".</summary>
    public interface IBasicGetEmpty : IMethod
    {
        string Reserved1 { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.ack".</summary>
    public interface IBasicAck : IMethod
    {
        ulong DeliveryTag { get; }
        bool Multiple { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.reject".</summary>
    public interface IBasicReject : IMethod
    {
        ulong DeliveryTag { get; }
        bool Requeue { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.recover-async".</summary>
    public interface IBasicRecoverAsync : IMethod
    {
        bool Requeue { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.recover".</summary>
    public interface IBasicRecover : IMethod
    {
        bool Requeue { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.recover-ok".</summary>
    public interface IBasicRecoverOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "basic.nack".</summary>
    public interface IBasicNack : IMethod
    {
        ulong DeliveryTag { get; }
        bool Multiple { get; }
        bool Requeue { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "tx.select".</summary>
    public interface ITxSelect : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "tx.select-ok".</summary>
    public interface ITxSelectOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "tx.commit".</summary>
    public interface ITxCommit : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "tx.commit-ok".</summary>
    public interface ITxCommitOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "tx.rollback".</summary>
    public interface ITxRollback : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "tx.rollback-ok".</summary>
    public interface ITxRollbackOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification method "confirm.select".</summary>
    public interface IConfirmSelect : IMethod
    {
        bool Nowait { get; }
    }
    /// <summary>Autogenerated type. AMQP specification method "confirm.select-ok".</summary>
    public interface IConfirmSelectOk : IMethod
    {
    }
    /// <summary>Autogenerated type. AMQP specification content header properties for content class "basic"</summary>
    public class BasicProperties : Client.Impl.BasicProperties
    {
        private string m_contentType;
        private string m_contentEncoding;
        private System.Collections.Generic.IDictionary<string, object> m_headers;
        private byte m_deliveryMode;
        private byte m_priority;
        private string m_correlationId;
        private string m_replyTo;
        private string m_expiration;
        private string m_messageId;
        private AmqpTimestamp m_timestamp;
        private string m_type;
        private string m_userId;
        private string m_appId;
        private string m_clusterId;

        private bool m_contentType_present = false;
        private bool m_contentEncoding_present = false;
        private bool m_headers_present = false;
        private bool m_deliveryMode_present = false;
        private bool m_priority_present = false;
        private bool m_correlationId_present = false;
        private bool m_replyTo_present = false;
        private bool m_expiration_present = false;
        private bool m_messageId_present = false;
        private bool m_timestamp_present = false;
        private bool m_type_present = false;
        private bool m_userId_present = false;
        private bool m_appId_present = false;
        private bool m_clusterId_present = false;

        public override string ContentType
        {
            get
            {
                return m_contentType;
            }
            set
            {
                m_contentType_present = true;
                m_contentType = value;
            }
        }
        public override string ContentEncoding
        {
            get
            {
                return m_contentEncoding;
            }
            set
            {
                m_contentEncoding_present = true;
                m_contentEncoding = value;
            }
        }
        public override System.Collections.Generic.IDictionary<string, object> Headers
        {
            get
            {
                return m_headers;
            }
            set
            {
                m_headers_present = true;
                m_headers = value;
            }
        }
        public override byte DeliveryMode
        {
            get
            {
                return m_deliveryMode;
            }
            set
            {
                m_deliveryMode_present = true;
                m_deliveryMode = value;
            }
        }
        public override byte Priority
        {
            get
            {
                return m_priority;
            }
            set
            {
                m_priority_present = true;
                m_priority = value;
            }
        }
        public override string CorrelationId
        {
            get
            {
                return m_correlationId;
            }
            set
            {
                m_correlationId_present = true;
                m_correlationId = value;
            }
        }
        public override string ReplyTo
        {
            get
            {
                return m_replyTo;
            }
            set
            {
                m_replyTo_present = true;
                m_replyTo = value;
            }
        }
        public override string Expiration
        {
            get
            {
                return m_expiration;
            }
            set
            {
                m_expiration_present = true;
                m_expiration = value;
            }
        }
        public override string MessageId
        {
            get
            {
                return m_messageId;
            }
            set
            {
                m_messageId_present = true;
                m_messageId = value;
            }
        }
        public override AmqpTimestamp Timestamp
        {
            get
            {
                return m_timestamp;
            }
            set
            {
                m_timestamp_present = true;
                m_timestamp = value;
            }
        }
        public override string Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type_present = true;
                m_type = value;
            }
        }
        public override string UserId
        {
            get
            {
                return m_userId;
            }
            set
            {
                m_userId_present = true;
                m_userId = value;
            }
        }
        public override string AppId
        {
            get
            {
                return m_appId;
            }
            set
            {
                m_appId_present = true;
                m_appId = value;
            }
        }
        public override string ClusterId
        {
            get
            {
                return m_clusterId;
            }
            set
            {
                m_clusterId_present = true;
                m_clusterId = value;
            }
        }

        public override void ClearContentType() { m_contentType_present = false; }
        public override void ClearContentEncoding() { m_contentEncoding_present = false; }
        public override void ClearHeaders() { m_headers_present = false; }
        public override void ClearDeliveryMode() { m_deliveryMode_present = false; }
        public override void ClearPriority() { m_priority_present = false; }
        public override void ClearCorrelationId() { m_correlationId_present = false; }
        public override void ClearReplyTo() { m_replyTo_present = false; }
        public override void ClearExpiration() { m_expiration_present = false; }
        public override void ClearMessageId() { m_messageId_present = false; }
        public override void ClearTimestamp() { m_timestamp_present = false; }
        public override void ClearType() { m_type_present = false; }
        public override void ClearUserId() { m_userId_present = false; }
        public override void ClearAppId() { m_appId_present = false; }
        public override void ClearClusterId() { m_clusterId_present = false; }

        public override bool IsContentTypePresent() { return m_contentType_present; }
        public override bool IsContentEncodingPresent() { return m_contentEncoding_present; }
        public override bool IsHeadersPresent() { return m_headers_present; }
        public override bool IsDeliveryModePresent() { return m_deliveryMode_present; }
        public override bool IsPriorityPresent() { return m_priority_present; }
        public override bool IsCorrelationIdPresent() { return m_correlationId_present; }
        public override bool IsReplyToPresent() { return m_replyTo_present; }
        public override bool IsExpirationPresent() { return m_expiration_present; }
        public override bool IsMessageIdPresent() { return m_messageId_present; }
        public override bool IsTimestampPresent() { return m_timestamp_present; }
        public override bool IsTypePresent() { return m_type_present; }
        public override bool IsUserIdPresent() { return m_userId_present; }
        public override bool IsAppIdPresent() { return m_appId_present; }
        public override bool IsClusterIdPresent() { return m_clusterId_present; }

        public BasicProperties() { }
        public override int ProtocolClassId { get { return 60; } }
        public override string ProtocolClassName { get { return "basic"; } }

        public override void ReadPropertiesFrom(Client.Impl.ContentHeaderPropertyReader reader)
        {
            m_contentType_present = reader.ReadPresence();
            m_contentEncoding_present = reader.ReadPresence();
            m_headers_present = reader.ReadPresence();
            m_deliveryMode_present = reader.ReadPresence();
            m_priority_present = reader.ReadPresence();
            m_correlationId_present = reader.ReadPresence();
            m_replyTo_present = reader.ReadPresence();
            m_expiration_present = reader.ReadPresence();
            m_messageId_present = reader.ReadPresence();
            m_timestamp_present = reader.ReadPresence();
            m_type_present = reader.ReadPresence();
            m_userId_present = reader.ReadPresence();
            m_appId_present = reader.ReadPresence();
            m_clusterId_present = reader.ReadPresence();
            reader.FinishPresence();
            if (m_contentType_present) { m_contentType = reader.ReadShortstr(); }
            if (m_contentEncoding_present) { m_contentEncoding = reader.ReadShortstr(); }
            if (m_headers_present) { m_headers = reader.ReadTable(); }
            if (m_deliveryMode_present) { m_deliveryMode = reader.ReadOctet(); }
            if (m_priority_present) { m_priority = reader.ReadOctet(); }
            if (m_correlationId_present) { m_correlationId = reader.ReadShortstr(); }
            if (m_replyTo_present) { m_replyTo = reader.ReadShortstr(); }
            if (m_expiration_present) { m_expiration = reader.ReadShortstr(); }
            if (m_messageId_present) { m_messageId = reader.ReadShortstr(); }
            if (m_timestamp_present) { m_timestamp = reader.ReadTimestamp(); }
            if (m_type_present) { m_type = reader.ReadShortstr(); }
            if (m_userId_present) { m_userId = reader.ReadShortstr(); }
            if (m_appId_present) { m_appId = reader.ReadShortstr(); }
            if (m_clusterId_present) { m_clusterId = reader.ReadShortstr(); }
        }

        public override void WritePropertiesTo(Client.Impl.ContentHeaderPropertyWriter writer)
        {
            writer.WritePresence(m_contentType_present);
            writer.WritePresence(m_contentEncoding_present);
            writer.WritePresence(m_headers_present);
            writer.WritePresence(m_deliveryMode_present);
            writer.WritePresence(m_priority_present);
            writer.WritePresence(m_correlationId_present);
            writer.WritePresence(m_replyTo_present);
            writer.WritePresence(m_expiration_present);
            writer.WritePresence(m_messageId_present);
            writer.WritePresence(m_timestamp_present);
            writer.WritePresence(m_type_present);
            writer.WritePresence(m_userId_present);
            writer.WritePresence(m_appId_present);
            writer.WritePresence(m_clusterId_present);
            writer.FinishPresence();
            if (m_contentType_present) { writer.WriteShortstr(m_contentType); }
            if (m_contentEncoding_present) { writer.WriteShortstr(m_contentEncoding); }
            if (m_headers_present) { writer.WriteTable(m_headers); }
            if (m_deliveryMode_present) { writer.WriteOctet(m_deliveryMode); }
            if (m_priority_present) { writer.WriteOctet(m_priority); }
            if (m_correlationId_present) { writer.WriteShortstr(m_correlationId); }
            if (m_replyTo_present) { writer.WriteShortstr(m_replyTo); }
            if (m_expiration_present) { writer.WriteShortstr(m_expiration); }
            if (m_messageId_present) { writer.WriteShortstr(m_messageId); }
            if (m_timestamp_present) { writer.WriteTimestamp(m_timestamp); }
            if (m_type_present) { writer.WriteShortstr(m_type); }
            if (m_userId_present) { writer.WriteShortstr(m_userId); }
            if (m_appId_present) { writer.WriteShortstr(m_appId); }
            if (m_clusterId_present) { writer.WriteShortstr(m_clusterId); }
        }

        public override void AppendPropertyDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append("content-type="); sb.Append(m_contentType_present ? (m_contentType == null ? "(null)" : m_contentType.ToString()) : "_"); sb.Append(", ");
            sb.Append("content-encoding="); sb.Append(m_contentEncoding_present ? (m_contentEncoding == null ? "(null)" : m_contentEncoding.ToString()) : "_"); sb.Append(", ");
            sb.Append("headers="); sb.Append(m_headers_present ? (m_headers == null ? "(null)" : m_headers.ToString()) : "_"); sb.Append(", ");
            sb.Append("delivery-mode="); sb.Append(m_deliveryMode_present ? m_deliveryMode.ToString() : "_"); sb.Append(", ");
            sb.Append("priority="); sb.Append(m_priority_present ? m_priority.ToString() : "_"); sb.Append(", ");
            sb.Append("correlation-id="); sb.Append(m_correlationId_present ? (m_correlationId == null ? "(null)" : m_correlationId.ToString()) : "_"); sb.Append(", ");
            sb.Append("reply-to="); sb.Append(m_replyTo_present ? (m_replyTo == null ? "(null)" : m_replyTo.ToString()) : "_"); sb.Append(", ");
            sb.Append("expiration="); sb.Append(m_expiration_present ? (m_expiration == null ? "(null)" : m_expiration.ToString()) : "_"); sb.Append(", ");
            sb.Append("message-id="); sb.Append(m_messageId_present ? (m_messageId == null ? "(null)" : m_messageId.ToString()) : "_"); sb.Append(", ");
            sb.Append("timestamp="); sb.Append(m_timestamp_present ? m_timestamp.ToString() : "_"); sb.Append(", ");
            sb.Append("type="); sb.Append(m_type_present ? (m_type == null ? "(null)" : m_type.ToString()) : "_"); sb.Append(", ");
            sb.Append("user-id="); sb.Append(m_userId_present ? (m_userId == null ? "(null)" : m_userId.ToString()) : "_"); sb.Append(", ");
            sb.Append("app-id="); sb.Append(m_appId_present ? (m_appId == null ? "(null)" : m_appId.ToString()) : "_"); sb.Append(", ");
            sb.Append("cluster-id="); sb.Append(m_clusterId_present ? (m_clusterId == null ? "(null)" : m_clusterId.ToString()) : "_");
            sb.Append(")");
        }
    }
}
namespace RabbitMQ.Client.Framing.Impl
{
    using RabbitMQ.Client.Framing;
    public enum ClassId
    {
        Connection = 10,
        Channel = 20,
        Exchange = 40,
        Queue = 50,
        Basic = 60,
        Tx = 90,
        Confirm = 85,
        Invalid = -1
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionStart : Client.Impl.MethodBase, IConnectionStart
    {
        public const int ClassId = 10;
        public const int MethodId = 10;

        public byte m_versionMajor;
        public byte m_versionMinor;
        public System.Collections.Generic.IDictionary<string, object> m_serverProperties;
        public byte[] m_mechanisms;
        public byte[] m_locales;

        byte IConnectionStart.VersionMajor { get { return m_versionMajor; } }
        byte IConnectionStart.VersionMinor { get { return m_versionMinor; } }
        System.Collections.Generic.IDictionary<string, object> IConnectionStart.ServerProperties { get { return m_serverProperties; } }
        byte[] IConnectionStart.Mechanisms { get { return m_mechanisms; } }
        byte[] IConnectionStart.Locales { get { return m_locales; } }

        public ConnectionStart() { }
        public ConnectionStart(
          byte initVersionMajor,
          byte initVersionMinor,
          System.Collections.Generic.IDictionary<string, object> initServerProperties,
          byte[] initMechanisms,
          byte[] initLocales)
        {
            m_versionMajor = initVersionMajor;
            m_versionMinor = initVersionMinor;
            m_serverProperties = initServerProperties;
            m_mechanisms = initMechanisms;
            m_locales = initLocales;
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 10; } }
        public override string ProtocolMethodName { get { return "connection.start"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_versionMajor = reader.ReadOctet();
            m_versionMinor = reader.ReadOctet();
            m_serverProperties = reader.ReadTable();
            m_mechanisms = reader.ReadLongstr();
            m_locales = reader.ReadLongstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteOctet(m_versionMajor);
            writer.WriteOctet(m_versionMinor);
            writer.WriteTable(m_serverProperties);
            writer.WriteLongstr(m_mechanisms);
            writer.WriteLongstr(m_locales);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_versionMajor); sb.Append(",");
            sb.Append(m_versionMinor); sb.Append(",");
            sb.Append(m_serverProperties); sb.Append(",");
            sb.Append(m_mechanisms); sb.Append(",");
            sb.Append(m_locales);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionStartOk : Client.Impl.MethodBase, IConnectionStartOk
    {
        public const int ClassId = 10;
        public const int MethodId = 11;

        public System.Collections.Generic.IDictionary<string, object> m_clientProperties;
        public string m_mechanism;
        public byte[] m_response;
        public string m_locale;

        System.Collections.Generic.IDictionary<string, object> IConnectionStartOk.ClientProperties { get { return m_clientProperties; } }
        string IConnectionStartOk.Mechanism { get { return m_mechanism; } }
        byte[] IConnectionStartOk.Response { get { return m_response; } }
        string IConnectionStartOk.Locale { get { return m_locale; } }

        public ConnectionStartOk() { }
        public ConnectionStartOk(
          System.Collections.Generic.IDictionary<string, object> initClientProperties,
          string initMechanism,
          byte[] initResponse,
          string initLocale)
        {
            m_clientProperties = initClientProperties;
            m_mechanism = initMechanism;
            m_response = initResponse;
            m_locale = initLocale;
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 11; } }
        public override string ProtocolMethodName { get { return "connection.start-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_clientProperties = reader.ReadTable();
            m_mechanism = reader.ReadShortstr();
            m_response = reader.ReadLongstr();
            m_locale = reader.ReadShortstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteTable(m_clientProperties);
            writer.WriteShortstr(m_mechanism);
            writer.WriteLongstr(m_response);
            writer.WriteShortstr(m_locale);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_clientProperties); sb.Append(",");
            sb.Append(m_mechanism); sb.Append(",");
            sb.Append(m_response); sb.Append(",");
            sb.Append(m_locale);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionSecure : Client.Impl.MethodBase, IConnectionSecure
    {
        public const int ClassId = 10;
        public const int MethodId = 20;

        public byte[] m_challenge;

        byte[] IConnectionSecure.Challenge { get { return m_challenge; } }

        public ConnectionSecure() { }
        public ConnectionSecure(
          byte[] initChallenge)
        {
            m_challenge = initChallenge;
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 20; } }
        public override string ProtocolMethodName { get { return "connection.secure"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_challenge = reader.ReadLongstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteLongstr(m_challenge);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_challenge);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionSecureOk : Client.Impl.MethodBase, IConnectionSecureOk
    {
        public const int ClassId = 10;
        public const int MethodId = 21;

        public byte[] m_response;

        byte[] IConnectionSecureOk.Response { get { return m_response; } }

        public ConnectionSecureOk() { }
        public ConnectionSecureOk(
          byte[] initResponse)
        {
            m_response = initResponse;
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 21; } }
        public override string ProtocolMethodName { get { return "connection.secure-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_response = reader.ReadLongstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteLongstr(m_response);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_response);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionTune : Client.Impl.MethodBase, IConnectionTune
    {
        public const int ClassId = 10;
        public const int MethodId = 30;

        public ushort m_channelMax;
        public uint m_frameMax;
        public ushort m_heartbeat;

        ushort IConnectionTune.ChannelMax { get { return m_channelMax; } }
        uint IConnectionTune.FrameMax { get { return m_frameMax; } }
        ushort IConnectionTune.Heartbeat { get { return m_heartbeat; } }

        public ConnectionTune() { }
        public ConnectionTune(
          ushort initChannelMax,
          uint initFrameMax,
          ushort initHeartbeat)
        {
            m_channelMax = initChannelMax;
            m_frameMax = initFrameMax;
            m_heartbeat = initHeartbeat;
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 30; } }
        public override string ProtocolMethodName { get { return "connection.tune"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_channelMax = reader.ReadShort();
            m_frameMax = reader.ReadLong();
            m_heartbeat = reader.ReadShort();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_channelMax);
            writer.WriteLong(m_frameMax);
            writer.WriteShort(m_heartbeat);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_channelMax); sb.Append(",");
            sb.Append(m_frameMax); sb.Append(",");
            sb.Append(m_heartbeat);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionTuneOk : Client.Impl.MethodBase, IConnectionTuneOk
    {
        public const int ClassId = 10;
        public const int MethodId = 31;

        public ushort m_channelMax;
        public uint m_frameMax;
        public ushort m_heartbeat;

        ushort IConnectionTuneOk.ChannelMax { get { return m_channelMax; } }
        uint IConnectionTuneOk.FrameMax { get { return m_frameMax; } }
        ushort IConnectionTuneOk.Heartbeat { get { return m_heartbeat; } }

        public ConnectionTuneOk() { }
        public ConnectionTuneOk(
          ushort initChannelMax,
          uint initFrameMax,
          ushort initHeartbeat)
        {
            m_channelMax = initChannelMax;
            m_frameMax = initFrameMax;
            m_heartbeat = initHeartbeat;
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 31; } }
        public override string ProtocolMethodName { get { return "connection.tune-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_channelMax = reader.ReadShort();
            m_frameMax = reader.ReadLong();
            m_heartbeat = reader.ReadShort();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_channelMax);
            writer.WriteLong(m_frameMax);
            writer.WriteShort(m_heartbeat);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_channelMax); sb.Append(",");
            sb.Append(m_frameMax); sb.Append(",");
            sb.Append(m_heartbeat);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionOpen : Client.Impl.MethodBase, IConnectionOpen
    {
        public const int ClassId = 10;
        public const int MethodId = 40;

        public string m_virtualHost;
        public string m_reserved1;
        public bool m_reserved2;

        string IConnectionOpen.VirtualHost { get { return m_virtualHost; } }
        string IConnectionOpen.Reserved1 { get { return m_reserved1; } }
        bool IConnectionOpen.Reserved2 { get { return m_reserved2; } }

        public ConnectionOpen() { }
        public ConnectionOpen(
          string initVirtualHost,
          string initReserved1,
          bool initReserved2)
        {
            m_virtualHost = initVirtualHost;
            m_reserved1 = initReserved1;
            m_reserved2 = initReserved2;
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 40; } }
        public override string ProtocolMethodName { get { return "connection.open"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_virtualHost = reader.ReadShortstr();
            m_reserved1 = reader.ReadShortstr();
            m_reserved2 = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShortstr(m_virtualHost);
            writer.WriteShortstr(m_reserved1);
            writer.WriteBit(m_reserved2);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_virtualHost); sb.Append(",");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_reserved2);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionOpenOk : Client.Impl.MethodBase, IConnectionOpenOk
    {
        public const int ClassId = 10;
        public const int MethodId = 41;

        public string m_reserved1;

        string IConnectionOpenOk.Reserved1 { get { return m_reserved1; } }

        public ConnectionOpenOk() { }
        public ConnectionOpenOk(
          string initReserved1)
        {
            m_reserved1 = initReserved1;
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 41; } }
        public override string ProtocolMethodName { get { return "connection.open-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShortstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShortstr(m_reserved1);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionClose : Client.Impl.MethodBase, IConnectionClose
    {
        public const int ClassId = 10;
        public const int MethodId = 50;

        public ushort m_replyCode;
        public string m_replyText;
        public ushort m_classId;
        public ushort m_methodId;

        ushort IConnectionClose.ReplyCode { get { return m_replyCode; } }
        string IConnectionClose.ReplyText { get { return m_replyText; } }
        ushort IConnectionClose.ClassId { get { return m_classId; } }
        ushort IConnectionClose.MethodId { get { return m_methodId; } }

        public ConnectionClose() { }
        public ConnectionClose(
          ushort initReplyCode,
          string initReplyText,
          ushort initClassId,
          ushort initMethodId)
        {
            m_replyCode = initReplyCode;
            m_replyText = initReplyText;
            m_classId = initClassId;
            m_methodId = initMethodId;
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 50; } }
        public override string ProtocolMethodName { get { return "connection.close"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_replyCode = reader.ReadShort();
            m_replyText = reader.ReadShortstr();
            m_classId = reader.ReadShort();
            m_methodId = reader.ReadShort();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_replyCode);
            writer.WriteShortstr(m_replyText);
            writer.WriteShort(m_classId);
            writer.WriteShort(m_methodId);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_replyCode); sb.Append(",");
            sb.Append(m_replyText); sb.Append(",");
            sb.Append(m_classId); sb.Append(",");
            sb.Append(m_methodId);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionCloseOk : Client.Impl.MethodBase, IConnectionCloseOk
    {
        public const int ClassId = 10;
        public const int MethodId = 51;



        public ConnectionCloseOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 51; } }
        public override string ProtocolMethodName { get { return "connection.close-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionBlocked : Client.Impl.MethodBase, IConnectionBlocked
    {
        public const int ClassId = 10;
        public const int MethodId = 60;

        public string m_reason;

        string IConnectionBlocked.Reason { get { return m_reason; } }

        public ConnectionBlocked() { }
        public ConnectionBlocked(
          string initReason)
        {
            m_reason = initReason;
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 60; } }
        public override string ProtocolMethodName { get { return "connection.blocked"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reason = reader.ReadShortstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShortstr(m_reason);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reason);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConnectionUnblocked : Client.Impl.MethodBase, IConnectionUnblocked
    {
        public const int ClassId = 10;
        public const int MethodId = 61;



        public ConnectionUnblocked(
    )
        {
        }

        public override int ProtocolClassId { get { return 10; } }
        public override int ProtocolMethodId { get { return 61; } }
        public override string ProtocolMethodName { get { return "connection.unblocked"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ChannelOpen : Client.Impl.MethodBase, IChannelOpen
    {
        public const int ClassId = 20;
        public const int MethodId = 10;

        public string m_reserved1;

        string IChannelOpen.Reserved1 { get { return m_reserved1; } }

        public ChannelOpen() { }
        public ChannelOpen(
          string initReserved1)
        {
            m_reserved1 = initReserved1;
        }

        public override int ProtocolClassId { get { return 20; } }
        public override int ProtocolMethodId { get { return 10; } }
        public override string ProtocolMethodName { get { return "channel.open"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShortstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShortstr(m_reserved1);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ChannelOpenOk : Client.Impl.MethodBase, IChannelOpenOk
    {
        public const int ClassId = 20;
        public const int MethodId = 11;

        public byte[] m_reserved1;

        byte[] IChannelOpenOk.Reserved1 { get { return m_reserved1; } }

        public ChannelOpenOk() { }
        public ChannelOpenOk(
          byte[] initReserved1)
        {
            m_reserved1 = initReserved1;
        }

        public override int ProtocolClassId { get { return 20; } }
        public override int ProtocolMethodId { get { return 11; } }
        public override string ProtocolMethodName { get { return "channel.open-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadLongstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteLongstr(m_reserved1);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ChannelFlow : Client.Impl.MethodBase, IChannelFlow
    {
        public const int ClassId = 20;
        public const int MethodId = 20;

        public bool m_active;

        bool IChannelFlow.Active { get { return m_active; } }

        public ChannelFlow() { }
        public ChannelFlow(
          bool initActive)
        {
            m_active = initActive;
        }

        public override int ProtocolClassId { get { return 20; } }
        public override int ProtocolMethodId { get { return 20; } }
        public override string ProtocolMethodName { get { return "channel.flow"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_active = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteBit(m_active);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_active);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ChannelFlowOk : Client.Impl.MethodBase, IChannelFlowOk
    {
        public const int ClassId = 20;
        public const int MethodId = 21;

        public bool m_active;

        bool IChannelFlowOk.Active { get { return m_active; } }

        public ChannelFlowOk() { }
        public ChannelFlowOk(
          bool initActive)
        {
            m_active = initActive;
        }

        public override int ProtocolClassId { get { return 20; } }
        public override int ProtocolMethodId { get { return 21; } }
        public override string ProtocolMethodName { get { return "channel.flow-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_active = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteBit(m_active);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_active);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ChannelClose : Client.Impl.MethodBase, IChannelClose
    {
        public const int ClassId = 20;
        public const int MethodId = 40;

        public ushort m_replyCode;
        public string m_replyText;
        public ushort m_classId;
        public ushort m_methodId;

        ushort IChannelClose.ReplyCode { get { return m_replyCode; } }
        string IChannelClose.ReplyText { get { return m_replyText; } }
        ushort IChannelClose.ClassId { get { return m_classId; } }
        ushort IChannelClose.MethodId { get { return m_methodId; } }

        public ChannelClose() { }
        public ChannelClose(
          ushort initReplyCode,
          string initReplyText,
          ushort initClassId,
          ushort initMethodId)
        {
            m_replyCode = initReplyCode;
            m_replyText = initReplyText;
            m_classId = initClassId;
            m_methodId = initMethodId;
        }

        public override int ProtocolClassId { get { return 20; } }
        public override int ProtocolMethodId { get { return 40; } }
        public override string ProtocolMethodName { get { return "channel.close"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_replyCode = reader.ReadShort();
            m_replyText = reader.ReadShortstr();
            m_classId = reader.ReadShort();
            m_methodId = reader.ReadShort();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_replyCode);
            writer.WriteShortstr(m_replyText);
            writer.WriteShort(m_classId);
            writer.WriteShort(m_methodId);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_replyCode); sb.Append(",");
            sb.Append(m_replyText); sb.Append(",");
            sb.Append(m_classId); sb.Append(",");
            sb.Append(m_methodId);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ChannelCloseOk : Client.Impl.MethodBase, IChannelCloseOk
    {
        public const int ClassId = 20;
        public const int MethodId = 41;



        public ChannelCloseOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 20; } }
        public override int ProtocolMethodId { get { return 41; } }
        public override string ProtocolMethodName { get { return "channel.close-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ExchangeDeclare : Client.Impl.MethodBase, IExchangeDeclare
    {
        public const int ClassId = 40;
        public const int MethodId = 10;

        public ushort m_reserved1;
        public string m_exchange;
        public string m_type;
        public bool m_passive;
        public bool m_durable;
        public bool m_autoDelete;
        public bool m_internal;
        public bool m_nowait;
        public System.Collections.Generic.IDictionary<string, object> m_arguments;

        ushort IExchangeDeclare.Reserved1 { get { return m_reserved1; } }
        string IExchangeDeclare.Exchange { get { return m_exchange; } }
        string IExchangeDeclare.Type { get { return m_type; } }
        bool IExchangeDeclare.Passive { get { return m_passive; } }
        bool IExchangeDeclare.Durable { get { return m_durable; } }
        bool IExchangeDeclare.AutoDelete { get { return m_autoDelete; } }
        bool IExchangeDeclare.Internal { get { return m_internal; } }
        bool IExchangeDeclare.Nowait { get { return m_nowait; } }
        System.Collections.Generic.IDictionary<string, object> IExchangeDeclare.Arguments { get { return m_arguments; } }

        public ExchangeDeclare() { }
        public ExchangeDeclare(
          ushort initReserved1,
          string initExchange,
          string initType,
          bool initPassive,
          bool initDurable,
          bool initAutoDelete,
          bool initInternal,
          bool initNowait,
          System.Collections.Generic.IDictionary<string, object> initArguments)
        {
            m_reserved1 = initReserved1;
            m_exchange = initExchange;
            m_type = initType;
            m_passive = initPassive;
            m_durable = initDurable;
            m_autoDelete = initAutoDelete;
            m_internal = initInternal;
            m_nowait = initNowait;
            m_arguments = initArguments;
        }

        public override int ProtocolClassId { get { return 40; } }
        public override int ProtocolMethodId { get { return 10; } }
        public override string ProtocolMethodName { get { return "exchange.declare"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_exchange = reader.ReadShortstr();
            m_type = reader.ReadShortstr();
            m_passive = reader.ReadBit();
            m_durable = reader.ReadBit();
            m_autoDelete = reader.ReadBit();
            m_internal = reader.ReadBit();
            m_nowait = reader.ReadBit();
            m_arguments = reader.ReadTable();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_exchange);
            writer.WriteShortstr(m_type);
            writer.WriteBit(m_passive);
            writer.WriteBit(m_durable);
            writer.WriteBit(m_autoDelete);
            writer.WriteBit(m_internal);
            writer.WriteBit(m_nowait);
            writer.WriteTable(m_arguments);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_exchange); sb.Append(",");
            sb.Append(m_type); sb.Append(",");
            sb.Append(m_passive); sb.Append(",");
            sb.Append(m_durable); sb.Append(",");
            sb.Append(m_autoDelete); sb.Append(",");
            sb.Append(m_internal); sb.Append(",");
            sb.Append(m_nowait); sb.Append(",");
            sb.Append(m_arguments);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ExchangeDeclareOk : Client.Impl.MethodBase, IExchangeDeclareOk
    {
        public const int ClassId = 40;
        public const int MethodId = 11;



        public ExchangeDeclareOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 40; } }
        public override int ProtocolMethodId { get { return 11; } }
        public override string ProtocolMethodName { get { return "exchange.declare-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ExchangeDelete : Client.Impl.MethodBase, IExchangeDelete
    {
        public const int ClassId = 40;
        public const int MethodId = 20;

        public ushort m_reserved1;
        public string m_exchange;
        public bool m_ifUnused;
        public bool m_nowait;

        ushort IExchangeDelete.Reserved1 { get { return m_reserved1; } }
        string IExchangeDelete.Exchange { get { return m_exchange; } }
        bool IExchangeDelete.IfUnused { get { return m_ifUnused; } }
        bool IExchangeDelete.Nowait { get { return m_nowait; } }

        public ExchangeDelete() { }
        public ExchangeDelete(
          ushort initReserved1,
          string initExchange,
          bool initIfUnused,
          bool initNowait)
        {
            m_reserved1 = initReserved1;
            m_exchange = initExchange;
            m_ifUnused = initIfUnused;
            m_nowait = initNowait;
        }

        public override int ProtocolClassId { get { return 40; } }
        public override int ProtocolMethodId { get { return 20; } }
        public override string ProtocolMethodName { get { return "exchange.delete"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_exchange = reader.ReadShortstr();
            m_ifUnused = reader.ReadBit();
            m_nowait = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_exchange);
            writer.WriteBit(m_ifUnused);
            writer.WriteBit(m_nowait);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_exchange); sb.Append(",");
            sb.Append(m_ifUnused); sb.Append(",");
            sb.Append(m_nowait);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ExchangeDeleteOk : Client.Impl.MethodBase, IExchangeDeleteOk
    {
        public const int ClassId = 40;
        public const int MethodId = 21;



        public ExchangeDeleteOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 40; } }
        public override int ProtocolMethodId { get { return 21; } }
        public override string ProtocolMethodName { get { return "exchange.delete-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ExchangeBind : Client.Impl.MethodBase, IExchangeBind
    {
        public const int ClassId = 40;
        public const int MethodId = 30;

        public ushort m_reserved1;
        public string m_destination;
        public string m_source;
        public string m_routingKey;
        public bool m_nowait;
        public System.Collections.Generic.IDictionary<string, object> m_arguments;

        ushort IExchangeBind.Reserved1 { get { return m_reserved1; } }
        string IExchangeBind.Destination { get { return m_destination; } }
        string IExchangeBind.Source { get { return m_source; } }
        string IExchangeBind.RoutingKey { get { return m_routingKey; } }
        bool IExchangeBind.Nowait { get { return m_nowait; } }
        System.Collections.Generic.IDictionary<string, object> IExchangeBind.Arguments { get { return m_arguments; } }

        public ExchangeBind() { }
        public ExchangeBind(
          ushort initReserved1,
          string initDestination,
          string initSource,
          string initRoutingKey,
          bool initNowait,
          System.Collections.Generic.IDictionary<string, object> initArguments)
        {
            m_reserved1 = initReserved1;
            m_destination = initDestination;
            m_source = initSource;
            m_routingKey = initRoutingKey;
            m_nowait = initNowait;
            m_arguments = initArguments;
        }

        public override int ProtocolClassId { get { return 40; } }
        public override int ProtocolMethodId { get { return 30; } }
        public override string ProtocolMethodName { get { return "exchange.bind"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_destination = reader.ReadShortstr();
            m_source = reader.ReadShortstr();
            m_routingKey = reader.ReadShortstr();
            m_nowait = reader.ReadBit();
            m_arguments = reader.ReadTable();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_destination);
            writer.WriteShortstr(m_source);
            writer.WriteShortstr(m_routingKey);
            writer.WriteBit(m_nowait);
            writer.WriteTable(m_arguments);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_destination); sb.Append(",");
            sb.Append(m_source); sb.Append(",");
            sb.Append(m_routingKey); sb.Append(",");
            sb.Append(m_nowait); sb.Append(",");
            sb.Append(m_arguments);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ExchangeBindOk : Client.Impl.MethodBase, IExchangeBindOk
    {
        public const int ClassId = 40;
        public const int MethodId = 31;



        public ExchangeBindOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 40; } }
        public override int ProtocolMethodId { get { return 31; } }
        public override string ProtocolMethodName { get { return "exchange.bind-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ExchangeUnbind : Client.Impl.MethodBase, IExchangeUnbind
    {
        public const int ClassId = 40;
        public const int MethodId = 40;

        public ushort m_reserved1;
        public string m_destination;
        public string m_source;
        public string m_routingKey;
        public bool m_nowait;
        public System.Collections.Generic.IDictionary<string, object> m_arguments;

        ushort IExchangeUnbind.Reserved1 { get { return m_reserved1; } }
        string IExchangeUnbind.Destination { get { return m_destination; } }
        string IExchangeUnbind.Source { get { return m_source; } }
        string IExchangeUnbind.RoutingKey { get { return m_routingKey; } }
        bool IExchangeUnbind.Nowait { get { return m_nowait; } }
        System.Collections.Generic.IDictionary<string, object> IExchangeUnbind.Arguments { get { return m_arguments; } }

        public ExchangeUnbind() { }
        public ExchangeUnbind(
          ushort initReserved1,
          string initDestination,
          string initSource,
          string initRoutingKey,
          bool initNowait,
          System.Collections.Generic.IDictionary<string, object> initArguments)
        {
            m_reserved1 = initReserved1;
            m_destination = initDestination;
            m_source = initSource;
            m_routingKey = initRoutingKey;
            m_nowait = initNowait;
            m_arguments = initArguments;
        }

        public override int ProtocolClassId { get { return 40; } }
        public override int ProtocolMethodId { get { return 40; } }
        public override string ProtocolMethodName { get { return "exchange.unbind"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_destination = reader.ReadShortstr();
            m_source = reader.ReadShortstr();
            m_routingKey = reader.ReadShortstr();
            m_nowait = reader.ReadBit();
            m_arguments = reader.ReadTable();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_destination);
            writer.WriteShortstr(m_source);
            writer.WriteShortstr(m_routingKey);
            writer.WriteBit(m_nowait);
            writer.WriteTable(m_arguments);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_destination); sb.Append(",");
            sb.Append(m_source); sb.Append(",");
            sb.Append(m_routingKey); sb.Append(",");
            sb.Append(m_nowait); sb.Append(",");
            sb.Append(m_arguments);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ExchangeUnbindOk : Client.Impl.MethodBase, IExchangeUnbindOk
    {
        public const int ClassId = 40;
        public const int MethodId = 51;



        public ExchangeUnbindOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 40; } }
        public override int ProtocolMethodId { get { return 51; } }
        public override string ProtocolMethodName { get { return "exchange.unbind-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class QueueDeclare : Client.Impl.MethodBase, IQueueDeclare
    {
        public const int ClassId = 50;
        public const int MethodId = 10;

        public ushort m_reserved1;
        public string m_queue;
        public bool m_passive;
        public bool m_durable;
        public bool m_exclusive;
        public bool m_autoDelete;
        public bool m_nowait;
        public System.Collections.Generic.IDictionary<string, object> m_arguments;

        ushort IQueueDeclare.Reserved1 { get { return m_reserved1; } }
        string IQueueDeclare.Queue { get { return m_queue; } }
        bool IQueueDeclare.Passive { get { return m_passive; } }
        bool IQueueDeclare.Durable { get { return m_durable; } }
        bool IQueueDeclare.Exclusive { get { return m_exclusive; } }
        bool IQueueDeclare.AutoDelete { get { return m_autoDelete; } }
        bool IQueueDeclare.Nowait { get { return m_nowait; } }
        System.Collections.Generic.IDictionary<string, object> IQueueDeclare.Arguments { get { return m_arguments; } }

        public QueueDeclare() { }
        public QueueDeclare(
          ushort initReserved1,
          string initQueue,
          bool initPassive,
          bool initDurable,
          bool initExclusive,
          bool initAutoDelete,
          bool initNowait,
          System.Collections.Generic.IDictionary<string, object> initArguments)
        {
            m_reserved1 = initReserved1;
            m_queue = initQueue;
            m_passive = initPassive;
            m_durable = initDurable;
            m_exclusive = initExclusive;
            m_autoDelete = initAutoDelete;
            m_nowait = initNowait;
            m_arguments = initArguments;
        }

        public override int ProtocolClassId { get { return 50; } }
        public override int ProtocolMethodId { get { return 10; } }
        public override string ProtocolMethodName { get { return "queue.declare"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_queue = reader.ReadShortstr();
            m_passive = reader.ReadBit();
            m_durable = reader.ReadBit();
            m_exclusive = reader.ReadBit();
            m_autoDelete = reader.ReadBit();
            m_nowait = reader.ReadBit();
            m_arguments = reader.ReadTable();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_queue);
            writer.WriteBit(m_passive);
            writer.WriteBit(m_durable);
            writer.WriteBit(m_exclusive);
            writer.WriteBit(m_autoDelete);
            writer.WriteBit(m_nowait);
            writer.WriteTable(m_arguments);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_queue); sb.Append(",");
            sb.Append(m_passive); sb.Append(",");
            sb.Append(m_durable); sb.Append(",");
            sb.Append(m_exclusive); sb.Append(",");
            sb.Append(m_autoDelete); sb.Append(",");
            sb.Append(m_nowait); sb.Append(",");
            sb.Append(m_arguments);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class QueueDeclareOk : Client.Impl.MethodBase, IQueueDeclareOk
    {
        public const int ClassId = 50;
        public const int MethodId = 11;

        public string m_queue;
        public uint m_messageCount;
        public uint m_consumerCount;

        string IQueueDeclareOk.Queue { get { return m_queue; } }
        uint IQueueDeclareOk.MessageCount { get { return m_messageCount; } }
        uint IQueueDeclareOk.ConsumerCount { get { return m_consumerCount; } }

        public QueueDeclareOk() { }
        public QueueDeclareOk(
          string initQueue,
          uint initMessageCount,
          uint initConsumerCount)
        {
            m_queue = initQueue;
            m_messageCount = initMessageCount;
            m_consumerCount = initConsumerCount;
        }

        public override int ProtocolClassId { get { return 50; } }
        public override int ProtocolMethodId { get { return 11; } }
        public override string ProtocolMethodName { get { return "queue.declare-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_queue = reader.ReadShortstr();
            m_messageCount = reader.ReadLong();
            m_consumerCount = reader.ReadLong();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShortstr(m_queue);
            writer.WriteLong(m_messageCount);
            writer.WriteLong(m_consumerCount);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_queue); sb.Append(",");
            sb.Append(m_messageCount); sb.Append(",");
            sb.Append(m_consumerCount);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class QueueBind : Client.Impl.MethodBase, IQueueBind
    {
        public const int ClassId = 50;
        public const int MethodId = 20;

        public ushort m_reserved1;
        public string m_queue;
        public string m_exchange;
        public string m_routingKey;
        public bool m_nowait;
        public System.Collections.Generic.IDictionary<string, object> m_arguments;

        ushort IQueueBind.Reserved1 { get { return m_reserved1; } }
        string IQueueBind.Queue { get { return m_queue; } }
        string IQueueBind.Exchange { get { return m_exchange; } }
        string IQueueBind.RoutingKey { get { return m_routingKey; } }
        bool IQueueBind.Nowait { get { return m_nowait; } }
        System.Collections.Generic.IDictionary<string, object> IQueueBind.Arguments { get { return m_arguments; } }

        public QueueBind() { }
        public QueueBind(
          ushort initReserved1,
          string initQueue,
          string initExchange,
          string initRoutingKey,
          bool initNowait,
          System.Collections.Generic.IDictionary<string, object> initArguments)
        {
            m_reserved1 = initReserved1;
            m_queue = initQueue;
            m_exchange = initExchange;
            m_routingKey = initRoutingKey;
            m_nowait = initNowait;
            m_arguments = initArguments;
        }

        public override int ProtocolClassId { get { return 50; } }
        public override int ProtocolMethodId { get { return 20; } }
        public override string ProtocolMethodName { get { return "queue.bind"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_queue = reader.ReadShortstr();
            m_exchange = reader.ReadShortstr();
            m_routingKey = reader.ReadShortstr();
            m_nowait = reader.ReadBit();
            m_arguments = reader.ReadTable();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_queue);
            writer.WriteShortstr(m_exchange);
            writer.WriteShortstr(m_routingKey);
            writer.WriteBit(m_nowait);
            writer.WriteTable(m_arguments);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_queue); sb.Append(",");
            sb.Append(m_exchange); sb.Append(",");
            sb.Append(m_routingKey); sb.Append(",");
            sb.Append(m_nowait); sb.Append(",");
            sb.Append(m_arguments);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class QueueBindOk : Client.Impl.MethodBase, IQueueBindOk
    {
        public const int ClassId = 50;
        public const int MethodId = 21;



        public QueueBindOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 50; } }
        public override int ProtocolMethodId { get { return 21; } }
        public override string ProtocolMethodName { get { return "queue.bind-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class QueueUnbind : Client.Impl.MethodBase, IQueueUnbind
    {
        public const int ClassId = 50;
        public const int MethodId = 50;

        public ushort m_reserved1;
        public string m_queue;
        public string m_exchange;
        public string m_routingKey;
        public System.Collections.Generic.IDictionary<string, object> m_arguments;

        ushort IQueueUnbind.Reserved1 { get { return m_reserved1; } }
        string IQueueUnbind.Queue { get { return m_queue; } }
        string IQueueUnbind.Exchange { get { return m_exchange; } }
        string IQueueUnbind.RoutingKey { get { return m_routingKey; } }
        System.Collections.Generic.IDictionary<string, object> IQueueUnbind.Arguments { get { return m_arguments; } }

        public QueueUnbind() { }
        public QueueUnbind(
          ushort initReserved1,
          string initQueue,
          string initExchange,
          string initRoutingKey,
          System.Collections.Generic.IDictionary<string, object> initArguments)
        {
            m_reserved1 = initReserved1;
            m_queue = initQueue;
            m_exchange = initExchange;
            m_routingKey = initRoutingKey;
            m_arguments = initArguments;
        }

        public override int ProtocolClassId { get { return 50; } }
        public override int ProtocolMethodId { get { return 50; } }
        public override string ProtocolMethodName { get { return "queue.unbind"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_queue = reader.ReadShortstr();
            m_exchange = reader.ReadShortstr();
            m_routingKey = reader.ReadShortstr();
            m_arguments = reader.ReadTable();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_queue);
            writer.WriteShortstr(m_exchange);
            writer.WriteShortstr(m_routingKey);
            writer.WriteTable(m_arguments);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_queue); sb.Append(",");
            sb.Append(m_exchange); sb.Append(",");
            sb.Append(m_routingKey); sb.Append(",");
            sb.Append(m_arguments);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class QueueUnbindOk : Client.Impl.MethodBase, IQueueUnbindOk
    {
        public const int ClassId = 50;
        public const int MethodId = 51;



        public QueueUnbindOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 50; } }
        public override int ProtocolMethodId { get { return 51; } }
        public override string ProtocolMethodName { get { return "queue.unbind-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class QueuePurge : Client.Impl.MethodBase, IQueuePurge
    {
        public const int ClassId = 50;
        public const int MethodId = 30;

        public ushort m_reserved1;
        public string m_queue;
        public bool m_nowait;

        ushort IQueuePurge.Reserved1 { get { return m_reserved1; } }
        string IQueuePurge.Queue { get { return m_queue; } }
        bool IQueuePurge.Nowait { get { return m_nowait; } }

        public QueuePurge() { }
        public QueuePurge(
          ushort initReserved1,
          string initQueue,
          bool initNowait)
        {
            m_reserved1 = initReserved1;
            m_queue = initQueue;
            m_nowait = initNowait;
        }

        public override int ProtocolClassId { get { return 50; } }
        public override int ProtocolMethodId { get { return 30; } }
        public override string ProtocolMethodName { get { return "queue.purge"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_queue = reader.ReadShortstr();
            m_nowait = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_queue);
            writer.WriteBit(m_nowait);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_queue); sb.Append(",");
            sb.Append(m_nowait);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class QueuePurgeOk : Client.Impl.MethodBase, IQueuePurgeOk
    {
        public const int ClassId = 50;
        public const int MethodId = 31;

        public uint m_messageCount;

        uint IQueuePurgeOk.MessageCount { get { return m_messageCount; } }

        public QueuePurgeOk() { }
        public QueuePurgeOk(
          uint initMessageCount)
        {
            m_messageCount = initMessageCount;
        }

        public override int ProtocolClassId { get { return 50; } }
        public override int ProtocolMethodId { get { return 31; } }
        public override string ProtocolMethodName { get { return "queue.purge-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_messageCount = reader.ReadLong();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteLong(m_messageCount);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_messageCount);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class QueueDelete : Client.Impl.MethodBase, IQueueDelete
    {
        public const int ClassId = 50;
        public const int MethodId = 40;

        public ushort m_reserved1;
        public string m_queue;
        public bool m_ifUnused;
        public bool m_ifEmpty;
        public bool m_nowait;

        ushort IQueueDelete.Reserved1 { get { return m_reserved1; } }
        string IQueueDelete.Queue { get { return m_queue; } }
        bool IQueueDelete.IfUnused { get { return m_ifUnused; } }
        bool IQueueDelete.IfEmpty { get { return m_ifEmpty; } }
        bool IQueueDelete.Nowait { get { return m_nowait; } }

        public QueueDelete() { }
        public QueueDelete(
          ushort initReserved1,
          string initQueue,
          bool initIfUnused,
          bool initIfEmpty,
          bool initNowait)
        {
            m_reserved1 = initReserved1;
            m_queue = initQueue;
            m_ifUnused = initIfUnused;
            m_ifEmpty = initIfEmpty;
            m_nowait = initNowait;
        }

        public override int ProtocolClassId { get { return 50; } }
        public override int ProtocolMethodId { get { return 40; } }
        public override string ProtocolMethodName { get { return "queue.delete"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_queue = reader.ReadShortstr();
            m_ifUnused = reader.ReadBit();
            m_ifEmpty = reader.ReadBit();
            m_nowait = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_queue);
            writer.WriteBit(m_ifUnused);
            writer.WriteBit(m_ifEmpty);
            writer.WriteBit(m_nowait);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_queue); sb.Append(",");
            sb.Append(m_ifUnused); sb.Append(",");
            sb.Append(m_ifEmpty); sb.Append(",");
            sb.Append(m_nowait);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class QueueDeleteOk : Client.Impl.MethodBase, IQueueDeleteOk
    {
        public const int ClassId = 50;
        public const int MethodId = 41;

        public uint m_messageCount;

        uint IQueueDeleteOk.MessageCount { get { return m_messageCount; } }

        public QueueDeleteOk() { }
        public QueueDeleteOk(
          uint initMessageCount)
        {
            m_messageCount = initMessageCount;
        }

        public override int ProtocolClassId { get { return 50; } }
        public override int ProtocolMethodId { get { return 41; } }
        public override string ProtocolMethodName { get { return "queue.delete-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_messageCount = reader.ReadLong();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteLong(m_messageCount);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_messageCount);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicQos : Client.Impl.MethodBase, IBasicQos
    {
        public const int ClassId = 60;
        public const int MethodId = 10;

        public uint m_prefetchSize;
        public ushort m_prefetchCount;
        public bool m_global;

        uint IBasicQos.PrefetchSize { get { return m_prefetchSize; } }
        ushort IBasicQos.PrefetchCount { get { return m_prefetchCount; } }
        bool IBasicQos.Global { get { return m_global; } }

        public BasicQos() { }
        public BasicQos(
          uint initPrefetchSize,
          ushort initPrefetchCount,
          bool initGlobal)
        {
            m_prefetchSize = initPrefetchSize;
            m_prefetchCount = initPrefetchCount;
            m_global = initGlobal;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 10; } }
        public override string ProtocolMethodName { get { return "basic.qos"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_prefetchSize = reader.ReadLong();
            m_prefetchCount = reader.ReadShort();
            m_global = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteLong(m_prefetchSize);
            writer.WriteShort(m_prefetchCount);
            writer.WriteBit(m_global);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_prefetchSize); sb.Append(",");
            sb.Append(m_prefetchCount); sb.Append(",");
            sb.Append(m_global);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicQosOk : Client.Impl.MethodBase, IBasicQosOk
    {
        public const int ClassId = 60;
        public const int MethodId = 11;



        public BasicQosOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 11; } }
        public override string ProtocolMethodName { get { return "basic.qos-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicConsume : Client.Impl.MethodBase, IBasicConsume
    {
        public const int ClassId = 60;
        public const int MethodId = 20;

        public ushort m_reserved1;
        public string m_queue;
        public string m_consumerTag;
        public bool m_noLocal;
        public bool m_noAck;
        public bool m_exclusive;
        public bool m_nowait;
        public System.Collections.Generic.IDictionary<string, object> m_arguments;

        ushort IBasicConsume.Reserved1 { get { return m_reserved1; } }
        string IBasicConsume.Queue { get { return m_queue; } }
        string IBasicConsume.ConsumerTag { get { return m_consumerTag; } }
        bool IBasicConsume.NoLocal { get { return m_noLocal; } }
        bool IBasicConsume.NoAck { get { return m_noAck; } }
        bool IBasicConsume.Exclusive { get { return m_exclusive; } }
        bool IBasicConsume.Nowait { get { return m_nowait; } }
        System.Collections.Generic.IDictionary<string, object> IBasicConsume.Arguments { get { return m_arguments; } }

        public BasicConsume() { }
        public BasicConsume(
          ushort initReserved1,
          string initQueue,
          string initConsumerTag,
          bool initNoLocal,
          bool initNoAck,
          bool initExclusive,
          bool initNowait,
          System.Collections.Generic.IDictionary<string, object> initArguments)
        {
            m_reserved1 = initReserved1;
            m_queue = initQueue;
            m_consumerTag = initConsumerTag;
            m_noLocal = initNoLocal;
            m_noAck = initNoAck;
            m_exclusive = initExclusive;
            m_nowait = initNowait;
            m_arguments = initArguments;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 20; } }
        public override string ProtocolMethodName { get { return "basic.consume"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_queue = reader.ReadShortstr();
            m_consumerTag = reader.ReadShortstr();
            m_noLocal = reader.ReadBit();
            m_noAck = reader.ReadBit();
            m_exclusive = reader.ReadBit();
            m_nowait = reader.ReadBit();
            m_arguments = reader.ReadTable();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_queue);
            writer.WriteShortstr(m_consumerTag);
            writer.WriteBit(m_noLocal);
            writer.WriteBit(m_noAck);
            writer.WriteBit(m_exclusive);
            writer.WriteBit(m_nowait);
            writer.WriteTable(m_arguments);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_queue); sb.Append(",");
            sb.Append(m_consumerTag); sb.Append(",");
            sb.Append(m_noLocal); sb.Append(",");
            sb.Append(m_noAck); sb.Append(",");
            sb.Append(m_exclusive); sb.Append(",");
            sb.Append(m_nowait); sb.Append(",");
            sb.Append(m_arguments);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicConsumeOk : Client.Impl.MethodBase, IBasicConsumeOk
    {
        public const int ClassId = 60;
        public const int MethodId = 21;

        public string m_consumerTag;

        string IBasicConsumeOk.ConsumerTag { get { return m_consumerTag; } }

        public BasicConsumeOk() { }
        public BasicConsumeOk(
          string initConsumerTag)
        {
            m_consumerTag = initConsumerTag;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 21; } }
        public override string ProtocolMethodName { get { return "basic.consume-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_consumerTag = reader.ReadShortstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShortstr(m_consumerTag);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_consumerTag);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicCancel : Client.Impl.MethodBase, IBasicCancel
    {
        public const int ClassId = 60;
        public const int MethodId = 30;

        public string m_consumerTag;
        public bool m_nowait;

        string IBasicCancel.ConsumerTag { get { return m_consumerTag; } }
        bool IBasicCancel.Nowait { get { return m_nowait; } }

        public BasicCancel() { }
        public BasicCancel(
          string initConsumerTag,
          bool initNowait)
        {
            m_consumerTag = initConsumerTag;
            m_nowait = initNowait;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 30; } }
        public override string ProtocolMethodName { get { return "basic.cancel"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_consumerTag = reader.ReadShortstr();
            m_nowait = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShortstr(m_consumerTag);
            writer.WriteBit(m_nowait);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_consumerTag); sb.Append(",");
            sb.Append(m_nowait);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicCancelOk : Client.Impl.MethodBase, IBasicCancelOk
    {
        public const int ClassId = 60;
        public const int MethodId = 31;

        public string m_consumerTag;

        string IBasicCancelOk.ConsumerTag { get { return m_consumerTag; } }

        public BasicCancelOk() { }
        public BasicCancelOk(
          string initConsumerTag)
        {
            m_consumerTag = initConsumerTag;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 31; } }
        public override string ProtocolMethodName { get { return "basic.cancel-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_consumerTag = reader.ReadShortstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShortstr(m_consumerTag);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_consumerTag);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicPublish : Client.Impl.MethodBase, IBasicPublish
    {
        public const int ClassId = 60;
        public const int MethodId = 40;

        public ushort m_reserved1;
        public string m_exchange;
        public string m_routingKey;
        public bool m_mandatory;
        public bool m_immediate;

        ushort IBasicPublish.Reserved1 { get { return m_reserved1; } }
        string IBasicPublish.Exchange { get { return m_exchange; } }
        string IBasicPublish.RoutingKey { get { return m_routingKey; } }
        bool IBasicPublish.Mandatory { get { return m_mandatory; } }
        bool IBasicPublish.Immediate { get { return m_immediate; } }

        public BasicPublish() { }
        public BasicPublish(
          ushort initReserved1,
          string initExchange,
          string initRoutingKey,
          bool initMandatory,
          bool initImmediate)
        {
            m_reserved1 = initReserved1;
            m_exchange = initExchange;
            m_routingKey = initRoutingKey;
            m_mandatory = initMandatory;
            m_immediate = initImmediate;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 40; } }
        public override string ProtocolMethodName { get { return "basic.publish"; } }
        public override bool HasContent { get { return true; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_exchange = reader.ReadShortstr();
            m_routingKey = reader.ReadShortstr();
            m_mandatory = reader.ReadBit();
            m_immediate = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_exchange);
            writer.WriteShortstr(m_routingKey);
            writer.WriteBit(m_mandatory);
            writer.WriteBit(m_immediate);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_exchange); sb.Append(",");
            sb.Append(m_routingKey); sb.Append(",");
            sb.Append(m_mandatory); sb.Append(",");
            sb.Append(m_immediate);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicReturn : Client.Impl.MethodBase, IBasicReturn
    {
        public const int ClassId = 60;
        public const int MethodId = 50;

        public ushort m_replyCode;
        public string m_replyText;
        public string m_exchange;
        public string m_routingKey;

        ushort IBasicReturn.ReplyCode { get { return m_replyCode; } }
        string IBasicReturn.ReplyText { get { return m_replyText; } }
        string IBasicReturn.Exchange { get { return m_exchange; } }
        string IBasicReturn.RoutingKey { get { return m_routingKey; } }

        public BasicReturn() { }
        public BasicReturn(
          ushort initReplyCode,
          string initReplyText,
          string initExchange,
          string initRoutingKey)
        {
            m_replyCode = initReplyCode;
            m_replyText = initReplyText;
            m_exchange = initExchange;
            m_routingKey = initRoutingKey;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 50; } }
        public override string ProtocolMethodName { get { return "basic.return"; } }
        public override bool HasContent { get { return true; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_replyCode = reader.ReadShort();
            m_replyText = reader.ReadShortstr();
            m_exchange = reader.ReadShortstr();
            m_routingKey = reader.ReadShortstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_replyCode);
            writer.WriteShortstr(m_replyText);
            writer.WriteShortstr(m_exchange);
            writer.WriteShortstr(m_routingKey);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_replyCode); sb.Append(",");
            sb.Append(m_replyText); sb.Append(",");
            sb.Append(m_exchange); sb.Append(",");
            sb.Append(m_routingKey);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicDeliver : Client.Impl.MethodBase, IBasicDeliver
    {
        public const int ClassId = 60;
        public const int MethodId = 60;

        public string m_consumerTag;
        public ulong m_deliveryTag;
        public bool m_redelivered;
        public string m_exchange;
        public string m_routingKey;

        string IBasicDeliver.ConsumerTag { get { return m_consumerTag; } }
        ulong IBasicDeliver.DeliveryTag { get { return m_deliveryTag; } }
        bool IBasicDeliver.Redelivered { get { return m_redelivered; } }
        string IBasicDeliver.Exchange { get { return m_exchange; } }
        string IBasicDeliver.RoutingKey { get { return m_routingKey; } }

        public BasicDeliver() { }
        public BasicDeliver(
          string initConsumerTag,
          ulong initDeliveryTag,
          bool initRedelivered,
          string initExchange,
          string initRoutingKey)
        {
            m_consumerTag = initConsumerTag;
            m_deliveryTag = initDeliveryTag;
            m_redelivered = initRedelivered;
            m_exchange = initExchange;
            m_routingKey = initRoutingKey;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 60; } }
        public override string ProtocolMethodName { get { return "basic.deliver"; } }
        public override bool HasContent { get { return true; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_consumerTag = reader.ReadShortstr();
            m_deliveryTag = reader.ReadLonglong();
            m_redelivered = reader.ReadBit();
            m_exchange = reader.ReadShortstr();
            m_routingKey = reader.ReadShortstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShortstr(m_consumerTag);
            writer.WriteLonglong(m_deliveryTag);
            writer.WriteBit(m_redelivered);
            writer.WriteShortstr(m_exchange);
            writer.WriteShortstr(m_routingKey);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_consumerTag); sb.Append(",");
            sb.Append(m_deliveryTag); sb.Append(",");
            sb.Append(m_redelivered); sb.Append(",");
            sb.Append(m_exchange); sb.Append(",");
            sb.Append(m_routingKey);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicGet : Client.Impl.MethodBase, IBasicGet
    {
        public const int ClassId = 60;
        public const int MethodId = 70;

        public ushort m_reserved1;
        public string m_queue;
        public bool m_noAck;

        ushort IBasicGet.Reserved1 { get { return m_reserved1; } }
        string IBasicGet.Queue { get { return m_queue; } }
        bool IBasicGet.NoAck { get { return m_noAck; } }

        public BasicGet() { }
        public BasicGet(
          ushort initReserved1,
          string initQueue,
          bool initNoAck)
        {
            m_reserved1 = initReserved1;
            m_queue = initQueue;
            m_noAck = initNoAck;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 70; } }
        public override string ProtocolMethodName { get { return "basic.get"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShort();
            m_queue = reader.ReadShortstr();
            m_noAck = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShort(m_reserved1);
            writer.WriteShortstr(m_queue);
            writer.WriteBit(m_noAck);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1); sb.Append(",");
            sb.Append(m_queue); sb.Append(",");
            sb.Append(m_noAck);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicGetOk : Client.Impl.MethodBase, IBasicGetOk
    {
        public const int ClassId = 60;
        public const int MethodId = 71;

        public ulong m_deliveryTag;
        public bool m_redelivered;
        public string m_exchange;
        public string m_routingKey;
        public uint m_messageCount;

        ulong IBasicGetOk.DeliveryTag { get { return m_deliveryTag; } }
        bool IBasicGetOk.Redelivered { get { return m_redelivered; } }
        string IBasicGetOk.Exchange { get { return m_exchange; } }
        string IBasicGetOk.RoutingKey { get { return m_routingKey; } }
        uint IBasicGetOk.MessageCount { get { return m_messageCount; } }

        public BasicGetOk() { }
        public BasicGetOk(
          ulong initDeliveryTag,
          bool initRedelivered,
          string initExchange,
          string initRoutingKey,
          uint initMessageCount)
        {
            m_deliveryTag = initDeliveryTag;
            m_redelivered = initRedelivered;
            m_exchange = initExchange;
            m_routingKey = initRoutingKey;
            m_messageCount = initMessageCount;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 71; } }
        public override string ProtocolMethodName { get { return "basic.get-ok"; } }
        public override bool HasContent { get { return true; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_deliveryTag = reader.ReadLonglong();
            m_redelivered = reader.ReadBit();
            m_exchange = reader.ReadShortstr();
            m_routingKey = reader.ReadShortstr();
            m_messageCount = reader.ReadLong();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteLonglong(m_deliveryTag);
            writer.WriteBit(m_redelivered);
            writer.WriteShortstr(m_exchange);
            writer.WriteShortstr(m_routingKey);
            writer.WriteLong(m_messageCount);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_deliveryTag); sb.Append(",");
            sb.Append(m_redelivered); sb.Append(",");
            sb.Append(m_exchange); sb.Append(",");
            sb.Append(m_routingKey); sb.Append(",");
            sb.Append(m_messageCount);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicGetEmpty : Client.Impl.MethodBase, IBasicGetEmpty
    {
        public const int ClassId = 60;
        public const int MethodId = 72;

        public string m_reserved1;

        string IBasicGetEmpty.Reserved1 { get { return m_reserved1; } }

        public BasicGetEmpty() { }
        public BasicGetEmpty(
          string initReserved1)
        {
            m_reserved1 = initReserved1;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 72; } }
        public override string ProtocolMethodName { get { return "basic.get-empty"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_reserved1 = reader.ReadShortstr();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteShortstr(m_reserved1);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_reserved1);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicAck : Client.Impl.MethodBase, IBasicAck
    {
        public const int ClassId = 60;
        public const int MethodId = 80;

        public ulong m_deliveryTag;
        public bool m_multiple;

        ulong IBasicAck.DeliveryTag { get { return m_deliveryTag; } }
        bool IBasicAck.Multiple { get { return m_multiple; } }

        public BasicAck() { }
        public BasicAck(
          ulong initDeliveryTag,
          bool initMultiple)
        {
            m_deliveryTag = initDeliveryTag;
            m_multiple = initMultiple;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 80; } }
        public override string ProtocolMethodName { get { return "basic.ack"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_deliveryTag = reader.ReadLonglong();
            m_multiple = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteLonglong(m_deliveryTag);
            writer.WriteBit(m_multiple);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_deliveryTag); sb.Append(",");
            sb.Append(m_multiple);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicReject : Client.Impl.MethodBase, IBasicReject
    {
        public const int ClassId = 60;
        public const int MethodId = 90;

        public ulong m_deliveryTag;
        public bool m_requeue;

        ulong IBasicReject.DeliveryTag { get { return m_deliveryTag; } }
        bool IBasicReject.Requeue { get { return m_requeue; } }

        public BasicReject() { }
        public BasicReject(
          ulong initDeliveryTag,
          bool initRequeue)
        {
            m_deliveryTag = initDeliveryTag;
            m_requeue = initRequeue;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 90; } }
        public override string ProtocolMethodName { get { return "basic.reject"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_deliveryTag = reader.ReadLonglong();
            m_requeue = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteLonglong(m_deliveryTag);
            writer.WriteBit(m_requeue);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_deliveryTag); sb.Append(",");
            sb.Append(m_requeue);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicRecoverAsync : Client.Impl.MethodBase, IBasicRecoverAsync
    {
        public const int ClassId = 60;
        public const int MethodId = 100;

        public bool m_requeue;

        bool IBasicRecoverAsync.Requeue { get { return m_requeue; } }

        public BasicRecoverAsync() { }
        public BasicRecoverAsync(
          bool initRequeue)
        {
            m_requeue = initRequeue;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 100; } }
        public override string ProtocolMethodName { get { return "basic.recover-async"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_requeue = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteBit(m_requeue);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_requeue);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicRecover : Client.Impl.MethodBase, IBasicRecover
    {
        public const int ClassId = 60;
        public const int MethodId = 110;

        public bool m_requeue;

        bool IBasicRecover.Requeue { get { return m_requeue; } }

        public BasicRecover() { }
        public BasicRecover(
          bool initRequeue)
        {
            m_requeue = initRequeue;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 110; } }
        public override string ProtocolMethodName { get { return "basic.recover"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_requeue = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteBit(m_requeue);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_requeue);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicRecoverOk : Client.Impl.MethodBase, IBasicRecoverOk
    {
        public const int ClassId = 60;
        public const int MethodId = 111;



        public BasicRecoverOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 111; } }
        public override string ProtocolMethodName { get { return "basic.recover-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class BasicNack : Client.Impl.MethodBase, IBasicNack
    {
        public const int ClassId = 60;
        public const int MethodId = 120;

        public ulong m_deliveryTag;
        public bool m_multiple;
        public bool m_requeue;

        ulong IBasicNack.DeliveryTag { get { return m_deliveryTag; } }
        bool IBasicNack.Multiple { get { return m_multiple; } }
        bool IBasicNack.Requeue { get { return m_requeue; } }

        public BasicNack() { }
        public BasicNack(
          ulong initDeliveryTag,
          bool initMultiple,
          bool initRequeue)
        {
            m_deliveryTag = initDeliveryTag;
            m_multiple = initMultiple;
            m_requeue = initRequeue;
        }

        public override int ProtocolClassId { get { return 60; } }
        public override int ProtocolMethodId { get { return 120; } }
        public override string ProtocolMethodName { get { return "basic.nack"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_deliveryTag = reader.ReadLonglong();
            m_multiple = reader.ReadBit();
            m_requeue = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteLonglong(m_deliveryTag);
            writer.WriteBit(m_multiple);
            writer.WriteBit(m_requeue);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_deliveryTag); sb.Append(",");
            sb.Append(m_multiple); sb.Append(",");
            sb.Append(m_requeue);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class TxSelect : Client.Impl.MethodBase, ITxSelect
    {
        public const int ClassId = 90;
        public const int MethodId = 10;



        public TxSelect(
    )
        {
        }

        public override int ProtocolClassId { get { return 90; } }
        public override int ProtocolMethodId { get { return 10; } }
        public override string ProtocolMethodName { get { return "tx.select"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class TxSelectOk : Client.Impl.MethodBase, ITxSelectOk
    {
        public const int ClassId = 90;
        public const int MethodId = 11;



        public TxSelectOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 90; } }
        public override int ProtocolMethodId { get { return 11; } }
        public override string ProtocolMethodName { get { return "tx.select-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class TxCommit : Client.Impl.MethodBase, ITxCommit
    {
        public const int ClassId = 90;
        public const int MethodId = 20;



        public TxCommit(
    )
        {
        }

        public override int ProtocolClassId { get { return 90; } }
        public override int ProtocolMethodId { get { return 20; } }
        public override string ProtocolMethodName { get { return "tx.commit"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class TxCommitOk : Client.Impl.MethodBase, ITxCommitOk
    {
        public const int ClassId = 90;
        public const int MethodId = 21;



        public TxCommitOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 90; } }
        public override int ProtocolMethodId { get { return 21; } }
        public override string ProtocolMethodName { get { return "tx.commit-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class TxRollback : Client.Impl.MethodBase, ITxRollback
    {
        public const int ClassId = 90;
        public const int MethodId = 30;



        public TxRollback(
    )
        {
        }

        public override int ProtocolClassId { get { return 90; } }
        public override int ProtocolMethodId { get { return 30; } }
        public override string ProtocolMethodName { get { return "tx.rollback"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class TxRollbackOk : Client.Impl.MethodBase, ITxRollbackOk
    {
        public const int ClassId = 90;
        public const int MethodId = 31;



        public TxRollbackOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 90; } }
        public override int ProtocolMethodId { get { return 31; } }
        public override string ProtocolMethodName { get { return "tx.rollback-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConfirmSelect : Client.Impl.MethodBase, IConfirmSelect
    {
        public const int ClassId = 85;
        public const int MethodId = 10;

        public bool m_nowait;

        bool IConfirmSelect.Nowait { get { return m_nowait; } }

        public ConfirmSelect() { }
        public ConfirmSelect(
          bool initNowait)
        {
            m_nowait = initNowait;
        }

        public override int ProtocolClassId { get { return 85; } }
        public override int ProtocolMethodId { get { return 10; } }
        public override string ProtocolMethodName { get { return "confirm.select"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
            m_nowait = reader.ReadBit();
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
            writer.WriteBit(m_nowait);
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(m_nowait);
            sb.Append(")");
        }
    }
    /// <summary>Autogenerated type. Private implementation class - do not use directly.</summary>
    public class ConfirmSelectOk : Client.Impl.MethodBase, IConfirmSelectOk
    {
        public const int ClassId = 85;
        public const int MethodId = 11;



        public ConfirmSelectOk(
    )
        {
        }

        public override int ProtocolClassId { get { return 85; } }
        public override int ProtocolMethodId { get { return 11; } }
        public override string ProtocolMethodName { get { return "confirm.select-ok"; } }
        public override bool HasContent { get { return false; } }

        public override void ReadArgumentsFrom(Client.Impl.MethodArgumentReader reader)
        {
        }

        public override void WriteArgumentsTo(Client.Impl.MethodArgumentWriter writer)
        {
        }

        public override void AppendArgumentDebugStringTo(System.Text.StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(")");
        }
    }

    public class Model : Client.Impl.ModelBase
    {
        public Model(Client.Impl.ISession session) : base(session) { }
        public Model(Client.Impl.ISession session, ConsumerWorkService workService) : base(session, workService) { }
        public override void ConnectionTuneOk(
          ushort @channelMax,
          uint @frameMax,
          ushort @heartbeat)
        {
            ConnectionTuneOk __req = new ConnectionTuneOk
            {
                m_channelMax = @channelMax,
                m_frameMax = @frameMax,
                m_heartbeat = @heartbeat
            };
            ModelSend(__req, null, null);
        }
        public override void _Private_BasicCancel(
          string @consumerTag,
          bool @nowait)
        {
            BasicCancel __req = new BasicCancel
            {
                m_consumerTag = @consumerTag,
                m_nowait = @nowait
            };
            ModelSend(__req, null, null);
        }
        public override void _Private_BasicConsume(
          string @queue,
          string @consumerTag,
          bool @noLocal,
          bool @autoAck,
          bool @exclusive,
          bool @nowait,
          System.Collections.Generic.IDictionary<string, object> @arguments)
        {
            BasicConsume __req = new BasicConsume
            {
                m_queue = @queue,
                m_consumerTag = @consumerTag,
                m_noLocal = @noLocal,
                m_noAck = @autoAck,
                m_exclusive = @exclusive,
                m_nowait = @nowait,
                m_arguments = @arguments
            };
            ModelSend(__req, null, null);
        }
        public override void _Private_BasicGet(
          string @queue,
          bool @autoAck)
        {
            BasicGet __req = new BasicGet
            {
                m_queue = @queue,
                m_noAck = @autoAck
            };
            ModelSend(__req, null, null);
        }
        public override void _Private_BasicPublish(
          string @exchange,
          string @routingKey,
          bool @mandatory,
          IBasicProperties @basicProperties,
          byte[] @body)
        {
            BasicPublish __req = new BasicPublish
            {
                m_exchange = @exchange,
                m_routingKey = @routingKey,
                m_mandatory = @mandatory
            };
            ModelSend(__req, (BasicProperties)basicProperties, body);
        }
        public override void _Private_BasicRecover(
          bool @requeue)
        {
            BasicRecover __req = new BasicRecover
            {
                m_requeue = @requeue
            };
            ModelSend(__req, null, null);
        }
        public override void _Private_ChannelClose(
          ushort @replyCode,
          string @replyText,
          ushort @classId,
          ushort @methodId)
        {
            ChannelClose __req = new ChannelClose
            {
                m_replyCode = @replyCode,
                m_replyText = @replyText,
                m_classId = @classId,
                m_methodId = @methodId
            };
            ModelSend(__req, null, null);
        }
        public override void _Private_ChannelCloseOk()
        {
            ChannelCloseOk __req = new ChannelCloseOk();
            ModelSend(__req, null, null);
        }
        public override void _Private_ChannelFlowOk(
          bool @active)
        {
            ChannelFlowOk __req = new ChannelFlowOk
            {
                m_active = @active
            };
            ModelSend(__req, null, null);
        }
        public override void _Private_ChannelOpen(
          string @outOfBand)
        {
            ChannelOpen __req = new ChannelOpen
            {
                m_reserved1 = @outOfBand
            };
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is ChannelOpenOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void _Private_ConfirmSelect(
          bool @nowait)
        {
            ConfirmSelect __req = new ConfirmSelect
            {
                m_nowait = @nowait
            };
            if (nowait)
            {
                ModelSend(__req, null, null);
                return;
            }
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is ConfirmSelectOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void _Private_ConnectionClose(
          ushort @replyCode,
          string @replyText,
          ushort @classId,
          ushort @methodId)
        {
            ConnectionClose __req = new ConnectionClose
            {
                m_replyCode = @replyCode,
                m_replyText = @replyText,
                m_classId = @classId,
                m_methodId = @methodId
            };
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is ConnectionCloseOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void _Private_ConnectionCloseOk()
        {
            ConnectionCloseOk __req = new ConnectionCloseOk();
            ModelSend(__req, null, null);
        }
        public override void _Private_ConnectionOpen(
          string @virtualHost,
          string @capabilities,
          bool @insist)
        {
            ConnectionOpen __req = new ConnectionOpen
            {
                m_virtualHost = @virtualHost,
                m_reserved1 = @capabilities,
                m_reserved2 = @insist
            };
            ModelSend(__req, null, null);
        }
        public override void _Private_ConnectionSecureOk(
          byte[] @response)
        {
            ConnectionSecureOk __req = new ConnectionSecureOk
            {
                m_response = @response
            };
            ModelSend(__req, null, null);
        }
        public override void _Private_ConnectionStartOk(
          System.Collections.Generic.IDictionary<string, object> @clientProperties,
          string @mechanism,
          byte[] @response,
          string @locale)
        {
            ConnectionStartOk __req = new ConnectionStartOk
            {
                m_clientProperties = @clientProperties,
                m_mechanism = @mechanism,
                m_response = @response,
                m_locale = @locale
            };
            ModelSend(__req, null, null);
        }
        public override void _Private_ExchangeBind(
          string @destination,
          string @source,
          string @routingKey,
          bool @nowait,
          System.Collections.Generic.IDictionary<string, object> @arguments)
        {
            ExchangeBind __req = new ExchangeBind
            {
                m_destination = @destination,
                m_source = @source,
                m_routingKey = @routingKey,
                m_nowait = @nowait,
                m_arguments = @arguments
            };
            if (nowait)
            {
                ModelSend(__req, null, null);
                return;
            }
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is ExchangeBindOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void _Private_ExchangeDeclare(
          string @exchange,
          string @type,
          bool @passive,
          bool @durable,
          bool @autoDelete,
          bool @internal,
          bool @nowait,
          System.Collections.Generic.IDictionary<string, object> @arguments)
        {
            ExchangeDeclare __req = new ExchangeDeclare
            {
                m_exchange = @exchange,
                m_type = @type,
                m_passive = @passive,
                m_durable = @durable,
                m_autoDelete = @autoDelete,
                m_internal = @internal,
                m_nowait = @nowait,
                m_arguments = @arguments
            };
            if (nowait)
            {
                ModelSend(__req, null, null);
                return;
            }
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is ExchangeDeclareOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void _Private_ExchangeDelete(
          string @exchange,
          bool @ifUnused,
          bool @nowait)
        {
            ExchangeDelete __req = new ExchangeDelete
            {
                m_exchange = @exchange,
                m_ifUnused = @ifUnused,
                m_nowait = @nowait
            };
            if (nowait)
            {
                ModelSend(__req, null, null);
                return;
            }
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is ExchangeDeleteOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void _Private_ExchangeUnbind(
          string @destination,
          string @source,
          string @routingKey,
          bool @nowait,
          System.Collections.Generic.IDictionary<string, object> @arguments)
        {
            ExchangeUnbind __req = new ExchangeUnbind
            {
                m_destination = @destination,
                m_source = @source,
                m_routingKey = @routingKey,
                m_nowait = @nowait,
                m_arguments = @arguments
            };
            if (nowait)
            {
                ModelSend(__req, null, null);
                return;
            }
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is ExchangeUnbindOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void _Private_QueueBind(
          string @queue,
          string @exchange,
          string @routingKey,
          bool @nowait,
          System.Collections.Generic.IDictionary<string, object> @arguments)
        {
            QueueBind __req = new QueueBind
            {
                m_queue = @queue,
                m_exchange = @exchange,
                m_routingKey = @routingKey,
                m_nowait = @nowait,
                m_arguments = @arguments
            };
            if (nowait)
            {
                ModelSend(__req, null, null);
                return;
            }
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is QueueBindOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void _Private_QueueDeclare(
          string @queue,
          bool @passive,
          bool @durable,
          bool @exclusive,
          bool @autoDelete,
          bool @nowait,
          System.Collections.Generic.IDictionary<string, object> @arguments)
        {
            QueueDeclare __req = new QueueDeclare
            {
                m_queue = @queue,
                m_passive = @passive,
                m_durable = @durable,
                m_exclusive = @exclusive,
                m_autoDelete = @autoDelete,
                m_nowait = @nowait,
                m_arguments = @arguments
            };
            if (nowait)
            {
                ModelSend(__req, null, null);
                return;
            }
            ModelSend(__req, null, null);
        }
        public override uint _Private_QueueDelete(
          string @queue,
          bool @ifUnused,
          bool @ifEmpty,
          bool @nowait)
        {
            QueueDelete __req = new QueueDelete
            {
                m_queue = @queue,
                m_ifUnused = @ifUnused,
                m_ifEmpty = @ifEmpty,
                m_nowait = @nowait
            };
            if (nowait)
            {
                ModelSend(__req, null, null);
                return 0xFFFFFFFF;
            }
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is QueueDeleteOk __rep)) throw new UnexpectedMethodException(__repBase);
            return __rep.m_messageCount;
        }
        public override uint _Private_QueuePurge(
          string @queue,
          bool @nowait)
        {
            QueuePurge __req = new QueuePurge
            {
                m_queue = @queue,
                m_nowait = @nowait
            };
            if (nowait)
            {
                ModelSend(__req, null, null);
                return 0xFFFFFFFF;
            }
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is QueuePurgeOk __rep)) throw new UnexpectedMethodException(__repBase);
            return __rep.m_messageCount;
        }
        public override void BasicAck(
          ulong @deliveryTag,
          bool @multiple)
        {
            BasicAck __req = new BasicAck
            {
                m_deliveryTag = @deliveryTag,
                m_multiple = @multiple
            };
            ModelSend(__req, null, null);
        }
        public override void BasicNack(
          ulong @deliveryTag,
          bool @multiple,
          bool @requeue)
        {
            BasicNack __req = new BasicNack
            {
                m_deliveryTag = @deliveryTag,
                m_multiple = @multiple,
                m_requeue = @requeue
            };
            ModelSend(__req, null, null);
        }
        public override void BasicQos(
          uint @prefetchSize,
          ushort @prefetchCount,
          bool @global)
        {
            BasicQos __req = new BasicQos
            {
                m_prefetchSize = @prefetchSize,
                m_prefetchCount = @prefetchCount,
                m_global = @global
            };
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is BasicQosOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void BasicRecoverAsync(
          bool @requeue)
        {
            BasicRecoverAsync __req = new BasicRecoverAsync
            {
                m_requeue = @requeue
            };
            ModelSend(__req, null, null);
        }
        public override void BasicReject(
          ulong @deliveryTag,
          bool @requeue)
        {
            BasicReject __req = new BasicReject
            {
                m_deliveryTag = @deliveryTag,
                m_requeue = @requeue
            };
            ModelSend(__req, null, null);
        }
        public override IBasicProperties CreateBasicProperties()
        {
            return new BasicProperties();
        }
        public override void QueueUnbind(
          string @queue,
          string @exchange,
          string @routingKey,
          System.Collections.Generic.IDictionary<string, object> @arguments)
        {
            QueueUnbind __req = new QueueUnbind
            {
                m_queue = @queue,
                m_exchange = @exchange,
                m_routingKey = @routingKey,
                m_arguments = @arguments
            };
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is QueueUnbindOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void TxCommit()
        {
            TxCommit __req = new TxCommit();
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is TxCommitOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void TxRollback()
        {
            TxRollback __req = new TxRollback();
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is TxRollbackOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override void TxSelect()
        {
            TxSelect __req = new TxSelect();
            Client.Impl.MethodBase __repBase = ModelRpc(__req, null, null);
            if (!(__repBase is TxSelectOk __rep)) throw new UnexpectedMethodException(__repBase);
        }
        public override bool DispatchAsynchronous(Client.Impl.Command cmd)
        {
            Client.Impl.MethodBase __method = cmd.Method;
            switch ((__method.ProtocolClassId << 16) | __method.ProtocolMethodId)
            {
                case 3932240:
                    {
                        BasicAck __impl = (BasicAck)__method;
                        HandleBasicAck(
                          __impl.m_deliveryTag,
                          __impl.m_multiple);
                        return true;
                    }
                case 3932190:
                    {
                        BasicCancel __impl = (BasicCancel)__method;
                        HandleBasicCancel(
                          __impl.m_consumerTag,
                          __impl.m_nowait);
                        return true;
                    }
                case 3932191:
                    {
                        BasicCancelOk __impl = (BasicCancelOk)__method;
                        HandleBasicCancelOk(
                          __impl.m_consumerTag);
                        return true;
                    }
                case 3932181:
                    {
                        BasicConsumeOk __impl = (BasicConsumeOk)__method;
                        HandleBasicConsumeOk(
                          __impl.m_consumerTag);
                        return true;
                    }
                case 3932220:
                    {
                        BasicDeliver __impl = (BasicDeliver)__method;
                        HandleBasicDeliver(
                          __impl.m_consumerTag,
                          __impl.m_deliveryTag,
                          __impl.m_redelivered,
                          __impl.m_exchange,
                          __impl.m_routingKey,
                          (IBasicProperties)cmd.Header,
                          cmd.Body);
                        return true;
                    }
                case 3932232:
                    {
                        HandleBasicGetEmpty();
                        return true;
                    }
                case 3932231:
                    {
                        BasicGetOk __impl = (BasicGetOk)__method;
                        HandleBasicGetOk(
                          __impl.m_deliveryTag,
                          __impl.m_redelivered,
                          __impl.m_exchange,
                          __impl.m_routingKey,
                          __impl.m_messageCount,
                          (IBasicProperties)cmd.Header,
                          cmd.Body);
                        return true;
                    }
                case 3932280:
                    {
                        BasicNack __impl = (BasicNack)__method;
                        HandleBasicNack(
                          __impl.m_deliveryTag,
                          __impl.m_multiple,
                          __impl.m_requeue);
                        return true;
                    }
                case 3932271:
                    {
                        HandleBasicRecoverOk();
                        return true;
                    }
                case 3932210:
                    {
                        BasicReturn __impl = (BasicReturn)__method;
                        HandleBasicReturn(
                          __impl.m_replyCode,
                          __impl.m_replyText,
                          __impl.m_exchange,
                          __impl.m_routingKey,
                          (IBasicProperties)cmd.Header,
                          cmd.Body);
                        return true;
                    }
                case 1310760:
                    {
                        ChannelClose __impl = (ChannelClose)__method;
                        HandleChannelClose(
                          __impl.m_replyCode,
                          __impl.m_replyText,
                          __impl.m_classId,
                          __impl.m_methodId);
                        return true;
                    }
                case 1310761:
                    {
                        HandleChannelCloseOk();
                        return true;
                    }
                case 1310740:
                    {
                        ChannelFlow __impl = (ChannelFlow)__method;
                        HandleChannelFlow(
                          __impl.m_active);
                        return true;
                    }
                case 655420:
                    {
                        ConnectionBlocked __impl = (ConnectionBlocked)__method;
                        HandleConnectionBlocked(
                          __impl.m_reason);
                        return true;
                    }
                case 655410:
                    {
                        ConnectionClose __impl = (ConnectionClose)__method;
                        HandleConnectionClose(
                          __impl.m_replyCode,
                          __impl.m_replyText,
                          __impl.m_classId,
                          __impl.m_methodId);
                        return true;
                    }
                case 655401:
                    {
                        ConnectionOpenOk __impl = (ConnectionOpenOk)__method;
                        HandleConnectionOpenOk(
                          __impl.m_reserved1);
                        return true;
                    }
                case 655380:
                    {
                        ConnectionSecure __impl = (ConnectionSecure)__method;
                        HandleConnectionSecure(
                          __impl.m_challenge);
                        return true;
                    }
                case 655370:
                    {
                        ConnectionStart __impl = (ConnectionStart)__method;
                        HandleConnectionStart(
                          __impl.m_versionMajor,
                          __impl.m_versionMinor,
                          __impl.m_serverProperties,
                          __impl.m_mechanisms,
                          __impl.m_locales);
                        return true;
                    }
                case 655390:
                    {
                        ConnectionTune __impl = (ConnectionTune)__method;
                        HandleConnectionTune(
                          __impl.m_channelMax,
                          __impl.m_frameMax,
                          __impl.m_heartbeat);
                        return true;
                    }
                case 655421:
                    {
                        HandleConnectionUnblocked();
                        return true;
                    }
                case 3276811:
                    {
                        QueueDeclareOk __impl = (QueueDeclareOk)__method;
                        HandleQueueDeclareOk(
                          __impl.m_queue,
                          __impl.m_messageCount,
                          __impl.m_consumerCount);
                        return true;
                    }
                default: return false;
            }
        }
    }
}
