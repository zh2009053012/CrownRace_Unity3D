//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: proto/packet.proto
namespace com.crownrace.msg
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"buff_data")]
  public partial class buff_data : global::ProtoBuf.IExtensible
  {
    public buff_data() {}
    
    private com.crownrace.msg.BUFF_EFFECT _effect_type;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"effect_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public com.crownrace.msg.BUFF_EFFECT effect_type
    {
      get { return _effect_type; }
      set { _effect_type = value; }
    }
    private int _keep_round;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"keep_round", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int keep_round
    {
      get { return _keep_round; }
      set { _keep_round = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"packet")]
  public partial class packet : global::ProtoBuf.IExtensible
  {
    public packet() {}
    
    private com.crownrace.msg.NET_CMD _cmd;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"cmd", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public com.crownrace.msg.NET_CMD cmd
    {
      get { return _cmd; }
      set { _cmd = value; }
    }
    private byte[] _payload;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"payload", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] payload
    {
      get { return _payload; }
      set { _payload = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"NET_CMD")]
    public enum NET_CMD
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_REQ_CMD", Value=1)]
      LOGIN_REQ_CMD = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_ACK_CMD", Value=2)]
      LOGIN_ACK_CMD = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"HEARTBEAT_REQ_CMD", Value=3)]
      HEARTBEAT_REQ_CMD = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"HEARTBEAT_ACK_CMD", Value=4)]
      HEARTBEAT_ACK_CMD = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LEAVE_GAME_NTF_CMD", Value=5)]
      LEAVE_GAME_NTF_CMD = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ALL_PLAYER_DATA_NTF_CMD", Value=6)]
      ALL_PLAYER_DATA_NTF_CMD = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"PLAYER_ROUND_END_REQ_CMD", Value=7)]
      PLAYER_ROUND_END_REQ_CMD = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"PLAYER_ROLL_DICE_NTF_CMD", Value=8)]
      PLAYER_ROLL_DICE_NTF_CMD = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DICE_SYNC_NTF_CMD", Value=9)]
      DICE_SYNC_NTF_CMD = 9,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ROLL_DICE_OVER_NTF_CMD", Value=10)]
      ROLL_DICE_OVER_NTF_CMD = 10,
            
      [global::ProtoBuf.ProtoEnum(Name=@"PLAYER_MOVE_OVER_NTF_CMD", Value=11)]
      PLAYER_MOVE_OVER_NTF_CMD = 11,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CELL_EFFECT_REQ_CMD", Value=12)]
      CELL_EFFECT_REQ_CMD = 12,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CELL_EFFECT_ACK_CMD", Value=13)]
      CELL_EFFECT_ACK_CMD = 13,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CELL_EFFECT_NTF_CMD", Value=14)]
      CELL_EFFECT_NTF_CMD = 14,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOVE_TO_END_NTF_CMD", Value=15)]
      MOVE_TO_END_NTF_CMD = 15,
            
      [global::ProtoBuf.ProtoEnum(Name=@"PLAYER_PAUSE_NTF_CMD", Value=16)]
      PLAYER_PAUSE_NTF_CMD = 16,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ROLL_CARD_REQ_CMD", Value=17)]
      ROLL_CARD_REQ_CMD = 17,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ROLL_CARD_NTF_CMD", Value=18)]
      ROLL_CARD_NTF_CMD = 18,
            
      [global::ProtoBuf.ProtoEnum(Name=@"USE_CARD_NTF_CMD", Value=19)]
      USE_CARD_NTF_CMD = 19,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MESSAGE_NTF_CMD", Value=20)]
      MESSAGE_NTF_CMD = 20,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SET_DICE_BTN_STATE_NTF_CMD", Value=21)]
      SET_DICE_BTN_STATE_NTF_CMD = 21,
            
      [global::ProtoBuf.ProtoEnum(Name=@"USE_CARD_REQ_CMD", Value=22)]
      USE_CARD_REQ_CMD = 22,
            
      [global::ProtoBuf.ProtoEnum(Name=@"REMOVE_PLAYER_CARD_NTF_CMD", Value=23)]
      REMOVE_PLAYER_CARD_NTF_CMD = 23,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ADD_PLAYER_CARD_NTF_CMD", Value=24)]
      ADD_PLAYER_CARD_NTF_CMD = 24,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOVE_PLAYER_NTF_CMD", Value=25)]
      MOVE_PLAYER_NTF_CMD = 25,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SYNC_DICE_NTF_CMD", Value=26)]
      SYNC_DICE_NTF_CMD = 26,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SET_PLAYER_STATE_NTF_CMD", Value=27)]
      SET_PLAYER_STATE_NTF_CMD = 27,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ROLL_DICE_REQ_CMD", Value=28)]
      ROLL_DICE_REQ_CMD = 28,
            
      [global::ProtoBuf.ProtoEnum(Name=@"END_ROUND_REQ_CMD", Value=29)]
      END_ROUND_REQ_CMD = 29,
            
      [global::ProtoBuf.ProtoEnum(Name=@"GAME_LOAD_OVER_REQ_CMD", Value=30)]
      GAME_LOAD_OVER_REQ_CMD = 30,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SET_USE_CARD_STATE_NTF_CMD", Value=31)]
      SET_USE_CARD_STATE_NTF_CMD = 31,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SET_END_ROUND_BTN_STATE_NTF_CMD", Value=32)]
      SET_END_ROUND_BTN_STATE_NTF_CMD = 32,
            
      [global::ProtoBuf.ProtoEnum(Name=@"USE_CARD_ACK_CMD", Value=33)]
      USE_CARD_ACK_CMD = 33,
            
      [global::ProtoBuf.ProtoEnum(Name=@"UPDATE_BUFF_DATA_NTF_CMD", Value=34)]
      UPDATE_BUFF_DATA_NTF_CMD = 34,
            
      [global::ProtoBuf.ProtoEnum(Name=@"VECTORY_NTF_CMD", Value=35)]
      VECTORY_NTF_CMD = 35
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"BUFF_EFFECT")]
    public enum BUFF_EFFECT
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"BUFF_BOUNCE_CARD", Value=1)]
      BUFF_BOUNCE_CARD = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"BUFF_BLOCK_CARD", Value=2)]
      BUFF_BLOCK_CARD = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"BUFF_BLOCK_GRID", Value=3)]
      BUFF_BLOCK_GRID = 3
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"CELL_EFFECT")]
    public enum CELL_EFFECT
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"NONE", Value=1)]
      NONE = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FORWARD", Value=2)]
      FORWARD = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"BACK", Value=3)]
      BACK = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"PAUSE", Value=4)]
      PAUSE = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ROLL_CARD", Value=5)]
      ROLL_CARD = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ROLL_DICE", Value=6)]
      ROLL_DICE = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"START", Value=7)]
      START = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"END", Value=8)]
      END = 8
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"PLAYER_STATE")]
    public enum PLAYER_STATE
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"PLAYER_PAUSE", Value=1)]
      PLAYER_PAUSE = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"BOUNCE_CARD_EFFECT", Value=2)]
      BOUNCE_CARD_EFFECT = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"STOP_CARD_EFFECT", Value=3)]
      STOP_CARD_EFFECT = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"STOP_CELL_EFFECT", Value=4)]
      STOP_CELL_EFFECT = 4
    }
  
}