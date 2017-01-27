using UnityEngine;
using System.Collections;

public interface IStateBase {
	void Message(string message, object[] parameters);

	void Enter(GameStateBase owner);
	void Execute(GameStateBase owner);
	void Exit(GameStateBase owner);
}
