using System.Runtime.Serialization;

namespace CookedRabbit.Library.Models
{
    public class Connection
    {
        // Admin Data
        [DataMember(Name = "user")]
        public string User { get; set; }
        [DataMember(Name = "user_who_performed_action")]
        public string ActionPerformedBy { get; set; }
        [DataMember(Name = "node")]
        public string Node { get; set; }
        [DataMember(Name = "vhost")]
        public string Vhost { get; set; }

        // Client Properties
        [DataMember(Name = "client_properties")]
        public ClientProperties ClientProperties { get; set; }

        // Timestamps
        [DataMember(Name = "connected_at")]
        public long ConnectedAt { get; set; }

        // Connection Properties
        [DataMember(Name = "name")]
        public string ConnectionSystemName { get; set; }
        [DataMember(Name = "user_provided_name")]
        public string ConnectionName { get; set; }
        [DataMember(Name = "channels")]
        public long Channels { get; set; }
        [DataMember(Name = "channel_max")]
        public long ChannelMax { get; set; }
        [DataMember(Name = "frame_max")]
        public long FrameMax { get; set; }
        [DataMember(Name = "protocol")]
        public string Protocol { get; set; }

        // Connection Network Properties
        [DataMember(Name = "host")]
        public string Host { get; set; }
        [DataMember(Name = "port")]
        public int Port { get; set; }
        [DataMember(Name = "peer_host")]
        public string PeerHost { get; set; }
        [DataMember(Name = "peer_port")]
        public int PeerPort { get; set; }
        [DataMember(Name = "timeout")]
        public long Timeout { get; set; }
        [DataMember(Name = "auth_mechanism")]
        public string AuthMechanism { get; set; }
        [DataMember(Name = "ssl")]
        public bool SslEnabled { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "ssl_hash")]
        public string SslHash { get; set; }
        [DataMember(Name = "ssl_cipher")]
        public string SslCipher { get; set; }
        [DataMember(Name = "ssl_key_exchange")]
        public string SslKeyExchange { get; set; }
        [DataMember(Name = "ssl_protocol")]
        public string SslProtocol { get; set; }
        [DataMember(Name = "peer_cert_validity")]
        public string PeerCertValidity { get; set; }
        [DataMember(Name = "peer_cert_issuer")]
        public string PeerCertIssuer { get; set; }
        [DataMember(Name = "peer_cert_subject")]
        public string PeerCertSubject { get; set; }

        // Connection Stats
        [DataMember(Name = "state")]
        public string ConnectionState { get; set; }
        [DataMember(Name = "reductions")]
        public long Reductions { get; set; }
        [DataMember(Name = "send_pend")]
        public long SendPending { get; set; }
        [DataMember(Name = "send_cnt")]
        public long SendCount { get; set; }
        [DataMember(Name = "send_oct")]
        public long SendOct { get; set; }
        [DataMember(Name = "recv_cnt")]
        public long ReceiveCount { get; set; }
        [DataMember(Name = "recv_oct")]
        public long ReceiveOct { get; set; }
    }
}
