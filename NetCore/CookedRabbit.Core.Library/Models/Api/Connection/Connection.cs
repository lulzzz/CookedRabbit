using System.Runtime.Serialization;

namespace CookedRabbit.Core.Library.Models
{
    /// <summary>
    /// CookedRabbit model for mapping to Rabbit API JSON for Connection information.
    /// </summary>
    public class Connection
    {
        // Admin Data
        /// <summary>
        /// User
        /// </summary>
        [DataMember(Name = "user")]
        public string User { get; set; }
        /// <summary>
        /// User who performed action.
        /// </summary>
        [DataMember(Name = "user_who_performed_action")]
        public string ActionPerformedBy { get; set; }
        /// <summary>
        /// Node
        /// </summary>
        [DataMember(Name = "node")]
        public string Node { get; set; }
        /// <summary>
        /// Vhost
        /// </summary>
        [DataMember(Name = "vhost")]
        public string Vhost { get; set; }

        // Client Properties
        /// <summary>
        /// ClientProperties
        /// </summary>
        [DataMember(Name = "client_properties")]
        public ClientProperties ClientProperties { get; set; }

        // Timestamps
        /// <summary>
        /// ConnectedAt
        /// </summary>
        [DataMember(Name = "connected_at")]
        public long ConnectedAt { get; set; }

        // Connection Properties
        /// <summary>
        /// Connection System Name
        /// </summary>
        [DataMember(Name = "name")]
        public string ConnectionSystemName { get; set; }
        /// <summary>
        /// Connection Name Set By User
        /// </summary>
        [DataMember(Name = "user_provided_name")]
        public string ConnectionName { get; set; }
        /// <summary>
        /// Channels
        /// </summary>
        [DataMember(Name = "channels")]
        public long Channels { get; set; }
        /// <summary>
        /// ChannelMax
        /// </summary>
        [DataMember(Name = "channel_max")]
        public long ChannelMax { get; set; }
        /// <summary>
        /// FrameMax
        /// </summary>
        [DataMember(Name = "frame_max")]
        public long FrameMax { get; set; }
        /// <summary>
        /// Protocol
        /// </summary>
        [DataMember(Name = "protocol")]
        public string Protocol { get; set; }

        // Connection Network Properties
        /// <summary>
        /// Host
        /// </summary>
        [DataMember(Name = "host")]
        public string Host { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        [DataMember(Name = "port")]
        public int Port { get; set; }
        /// <summary>
        /// Peer Host
        /// </summary>
        [DataMember(Name = "peer_host")]
        public string PeerHost { get; set; }
        /// <summary>
        /// Peer Port
        /// </summary>
        [DataMember(Name = "peer_port")]
        public int PeerPort { get; set; }
        /// <summary>
        /// Timeout
        /// </summary>
        [DataMember(Name = "timeout")]
        public long Timeout { get; set; }
        /// <summary>
        /// Auth Mechanism
        /// </summary>
        [DataMember(Name = "auth_mechanism")]
        public string AuthMechanism { get; set; }
        /// <summary>
        /// Ssl Enabled
        /// </summary>
        [DataMember(Name = "ssl")]
        public bool SslEnabled { get; set; }
        /// <summary>
        /// Ssl Type
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }
        /// <summary>
        /// Ssl Hash
        /// </summary>
        [DataMember(Name = "ssl_hash")]
        public string SslHash { get; set; }
        /// <summary>
        /// Ssl Cipher
        /// </summary>
        [DataMember(Name = "ssl_cipher")]
        public string SslCipher { get; set; }
        /// <summary>
        /// Ssl Key Exchange
        /// </summary>
        [DataMember(Name = "ssl_key_exchange")]
        public string SslKeyExchange { get; set; }
        /// <summary>
        /// Ssl Protocol
        /// </summary>
        [DataMember(Name = "ssl_protocol")]
        public string SslProtocol { get; set; }
        /// <summary>
        /// Peer Certificate Validity
        /// </summary>
        [DataMember(Name = "peer_cert_validity")]
        public string PeerCertValidity { get; set; }
        /// <summary>
        /// Peer Certificate Issuer
        /// </summary>
        [DataMember(Name = "peer_cert_issuer")]
        public string PeerCertIssuer { get; set; }
        /// <summary>
        /// Peer Certificate Subject
        /// </summary>
        [DataMember(Name = "peer_cert_subject")]
        public string PeerCertSubject { get; set; }

        // Connection Stats
        /// <summary>
        /// Connection State
        /// </summary>
        [DataMember(Name = "state")]
        public string ConnectionState { get; set; }
        /// <summary>
        /// Reductions
        /// </summary>
        [DataMember(Name = "reductions")]
        public long Reductions { get; set; }
        /// <summary>
        /// Send Pending
        /// </summary>
        [DataMember(Name = "send_pend")]
        public long SendPending { get; set; }
        /// <summary>
        /// Send Count
        /// </summary>
        [DataMember(Name = "send_cnt")]
        public long SendCount { get; set; }
        /// <summary>
        /// Send Oct
        /// </summary>
        [DataMember(Name = "send_oct")]
        public long SendOct { get; set; }
        /// <summary>
        /// Receive Count
        /// </summary>
        [DataMember(Name = "recv_cnt")]
        public long ReceiveCount { get; set; }
        /// <summary>
        /// Receive Oct
        /// </summary>
        [DataMember(Name = "recv_oct")]
        public long ReceiveOct { get; set; }
    }
}
