using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MultiplayerBattleManager : MonoBehaviour
{
    public static bool multiplayerActive = false;
    public static UIMultiplayerLobbyList.Settings[] players;

    public static List<Character> GetAllyCharacters()
    {
        return players.Select(x => (Character)x.character).ToList();
    }
}
