using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound_UseBounceCardEffect : Singleton<ServerStateRound_UseBounceCardEffect>, IStateBase {

	List<PlayerRoundData> targetList;
	CARD_EFFECT cardEffect;
	int effectValue;
	PlayerRoundData user;
	public void Enter(GameStateBase owner)
	{
		targetList = ServerRoundData.TargetList;
		cardEffect = ServerRoundData.UseCardEffect;
		effectValue = ServerRoundData.CardEffectValue;
		user = ServerRoundData.UserRoundData;
		//更新玩家buff数据
		user.AddBuff(BUFF_EFFECT.BUFF_BOUNCE_CARD, effectValue);
		ServerRoundData.ServerUpdateBuffDataNtf (user.player_id, user.buff);
		//
		TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_NextStep.Instance);
	}
	public void Execute(GameStateBase owner)
	{

	}
	public void Exit(GameStateBase owner)
	{
	}
	public void Message(string message, object[] parameters)
	{

	}
}
